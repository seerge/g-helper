using GHelper.Helpers;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace GHelper.Mcp
{
    /// <summary>
    /// A minimal, self-contained Model Context Protocol (MCP) server.
    ///
    /// It exposes G-Helper's telemetry and controls as MCP "tools" so that AI assistants
    /// (Claude Desktop / Claude Code / any MCP client) can read sensors and change laptop
    /// settings. The implementation speaks the MCP "Streamable HTTP" transport directly on
    /// top of <see cref="HttpListener"/> so it adds no extra NuGet dependencies.
    ///
    /// Security model (see <see cref="HandleRequest"/>):
    ///  - Disabled by default; opt-in via the "mcp_enabled" config flag / tray menu.
    ///  - Bound to 127.0.0.1 only (never all interfaces).
    ///  - Requires a bearer token (auto-generated, stored in config) on every request.
    ///  - Validates the Origin header to block DNS-rebinding attacks from browsers.
    /// </summary>
    public static class McpServer
    {
        public const int DefaultPort = 8787;

        // Protocol versions we understand. We answer "initialize" with the client's
        // requested version when we support it, otherwise with our preferred one.
        public const string PreferredProtocolVersion = "2025-06-18";
        private static readonly HashSet<string> SupportedProtocolVersions = new()
        {
            "2025-06-18", "2025-03-26", "2024-11-05"
        };

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        private static readonly object _lock = new();
        private static HttpListener? _listener;
        private static CancellationTokenSource? _cts;

        // Active session ids handed out at initialize time. Kept so we can validate /
        // expire them, but we stay lenient about clients that omit the header.
        private static readonly HashSet<string> _sessions = new();

        public static bool IsRunning
        {
            get { lock (_lock) return _listener is not null && _listener.IsListening; }
        }

        #region Config accessors

        public static bool Enabled
        {
            get => AppConfig.Is("mcp_enabled");
            set => AppConfig.Set("mcp_enabled", value ? 1 : 0);
        }

        public static int Port
        {
            get
            {
                int port = AppConfig.Get("mcp_port", DefaultPort);
                return (port > 0 && port <= 65535) ? port : DefaultPort;
            }
        }

        /// <summary>Bearer token required on every request. Generated once and persisted.</summary>
        public static string Token
        {
            get
            {
                string? token = AppConfig.GetString("mcp_token");
                if (string.IsNullOrEmpty(token))
                {
                    token = Convert.ToHexString(RandomNumberGenerator.GetBytes(24)).ToLowerInvariant();
                    AppConfig.Set("mcp_token", token);
                }
                return token;
            }
        }

        public static string Url => $"http://127.0.0.1:{Port}/mcp";

        #endregion

        /// <summary>Starts or stops the server to match the current "mcp_enabled" flag.</summary>
        public static void ApplyState()
        {
            if (Enabled) Start();
            else Stop();
        }

        public static void Start()
        {
            lock (_lock)
            {
                if (_listener is not null && _listener.IsListening) return;

                try
                {
                    var listener = new HttpListener();
                    // Bind to loopback only. Match both "/mcp" and "/mcp/..." by listening at root.
                    listener.Prefixes.Add($"http://127.0.0.1:{Port}/");
                    listener.Start();

                    _listener = listener;
                    _cts = new CancellationTokenSource();

                    // Touch the token so it is generated/persisted before first use.
                    _ = Token;

                    Task.Run(() => AcceptLoop(listener, _cts.Token));
                    Logger.WriteLine($"MCP server listening on {Url}");
                }
                catch (HttpListenerException ex)
                {
                    // Access-denied (5) typically means the URL ACL is missing and G-Helper
                    // is not elevated. Hardware control already needs admin, so this is rare.
                    _listener = null;
                    Logger.WriteLine($"MCP server failed to start on port {Port}: {ex.Message} " +
                        "(run G-Helper as administrator, or reserve the URL with: " +
                        $"netsh http add urlacl url=http://127.0.0.1:{Port}/ user=Everyone)");
                }
                catch (Exception ex)
                {
                    _listener = null;
                    Logger.WriteLine($"MCP server failed to start: {ex.Message}");
                }
            }
        }

        public static void Stop()
        {
            lock (_lock)
            {
                if (_listener is null) return;
                try
                {
                    _cts?.Cancel();
                    _listener.Stop();
                    _listener.Close();
                    Logger.WriteLine("MCP server stopped");
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"MCP server stop error: {ex.Message}");
                }
                finally
                {
                    _listener = null;
                    _cts = null;
                    _sessions.Clear();
                }
            }
        }

        private static async Task AcceptLoop(HttpListener listener, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                HttpListenerContext context;
                try
                {
                    context = await listener.GetContextAsync();
                }
                catch (Exception)
                {
                    // Listener stopped/disposed - exit the loop quietly.
                    break;
                }

                // Handle each request independently; never let one failure kill the loop.
                _ = Task.Run(() =>
                {
                    try { HandleRequest(context); }
                    catch (Exception ex) { Logger.WriteLine($"MCP request error: {ex.Message}"); }
                });
            }
        }

        private static void HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            try
            {
                // --- DNS-rebinding protection: reject cross-origin browser requests. ---
                string? origin = request.Headers["Origin"];
                if (!string.IsNullOrEmpty(origin) && !IsLocalOrigin(origin))
                {
                    WriteStatus(response, 403, "Forbidden origin");
                    return;
                }

                // --- Authentication: bearer token on every request. ---
                if (!IsAuthorized(request))
                {
                    response.AddHeader("WWW-Authenticate", "Bearer");
                    WriteStatus(response, 401, "Unauthorized");
                    return;
                }

                // --- Routing: only the MCP endpoint is served. ---
                string path = request.Url?.AbsolutePath.TrimEnd('/') ?? "";
                if (path != "/mcp" && path != "")
                {
                    WriteStatus(response, 404, "Not found");
                    return;
                }

                switch (request.HttpMethod)
                {
                    case "POST":
                        HandlePost(request, response);
                        break;

                    case "DELETE":
                        // Client asked to terminate its session.
                        string? sid = request.Headers["Mcp-Session-Id"];
                        if (!string.IsNullOrEmpty(sid)) lock (_lock) _sessions.Remove(sid);
                        WriteStatus(response, 200, "OK");
                        break;

                    case "GET":
                        // We do not offer a server-initiated SSE stream.
                        WriteStatus(response, 405, "Method Not Allowed");
                        break;

                    default:
                        WriteStatus(response, 405, "Method Not Allowed");
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"MCP handler error: {ex.Message}");
                try { WriteStatus(response, 500, "Internal error"); } catch { }
            }
            finally
            {
                try { response.OutputStream.Close(); } catch { }
            }
        }

        private static void HandlePost(HttpListenerRequest request, HttpListenerResponse response)
        {
            string body;
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding ?? Encoding.UTF8))
                body = reader.ReadToEnd();

            JsonNode? message;
            try
            {
                message = JsonNode.Parse(body);
            }
            catch (Exception)
            {
                WriteJson(response, 200, McpProtocol.ParseError());
                return;
            }

            if (message is not JsonObject obj)
            {
                WriteJson(response, 200, McpProtocol.ParseError());
                return;
            }

            // A JSON-RPC notification or response carries no "id" that needs answering
            // (notifications have no id; method-less messages are responses). Ack with 202.
            bool hasId = obj.ContainsKey("id") && obj["id"] is not null;
            bool hasMethod = obj.ContainsKey("method");
            if (!hasMethod || !hasId)
            {
                WriteStatus(response, 202, "Accepted");
                return;
            }

            JsonNode? id = obj["id"]?.DeepClone();
            string method = obj["method"]!.GetValue<string>();
            JsonObject? parameters = obj["params"] as JsonObject;

            JsonObject result = McpProtocol.Dispatch(method, parameters, id, out string? newSessionId);

            if (newSessionId is not null)
            {
                lock (_lock) _sessions.Add(newSessionId);
                response.AddHeader("Mcp-Session-Id", newSessionId);
            }

            WriteJson(response, 200, result);
        }

        #region Security helpers

        private static bool IsAuthorized(HttpListenerRequest request)
        {
            string? auth = request.Headers["Authorization"];
            if (string.IsNullOrEmpty(auth) || !auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return false;

            string provided = auth.Substring("Bearer ".Length).Trim();
            return FixedTimeEquals(provided, Token);
        }

        private static bool FixedTimeEquals(string a, string b)
        {
            byte[] ba = Encoding.UTF8.GetBytes(a);
            byte[] bb = Encoding.UTF8.GetBytes(b);
            return CryptographicOperations.FixedTimeEquals(ba, bb);
        }

        private static bool IsLocalOrigin(string origin)
        {
            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri)) return false;
            string host = uri.Host;
            return host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase)
                || host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
                || host.Equals("::1", StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Response writers

        public static string NewSessionId() =>
            Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();

        private static void WriteJson(HttpListenerResponse response, int status, JsonObject payload)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(payload.ToJsonString(JsonOptions));
            response.StatusCode = status;
            response.ContentType = "application/json";
            response.ContentLength64 = bytes.Length;
            response.OutputStream.Write(bytes, 0, bytes.Length);
        }

        private static void WriteStatus(HttpListenerResponse response, int status, string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            response.StatusCode = status;
            response.ContentType = "text/plain";
            response.ContentLength64 = bytes.Length;
            response.OutputStream.Write(bytes, 0, bytes.Length);
        }

        #endregion

        public static bool IsProtocolSupported(string version) => SupportedProtocolVersions.Contains(version);
    }
}
