using System.Reflection;
using System.Text.Json.Nodes;

namespace GHelper.Mcp
{
    /// <summary>
    /// JSON-RPC 2.0 message construction and MCP method dispatch
    /// (initialize / tools/list / tools/call / ping).
    /// </summary>
    internal static class McpProtocol
    {
        // JSON-RPC error codes.
        private const int ParseErrorCode = -32700;
        private const int InvalidRequestCode = -32600;
        private const int MethodNotFoundCode = -32601;
        private const int InvalidParamsCode = -32602;
        private const int InternalErrorCode = -32603;

        /// <summary>
        /// Handles a single JSON-RPC request and returns the response object to send back.
        /// When the method is "initialize", <paramref name="newSessionId"/> is set so the
        /// transport can emit the Mcp-Session-Id header.
        /// </summary>
        public static JsonObject Dispatch(string method, JsonObject? parameters, JsonNode? id, out string? newSessionId)
        {
            newSessionId = null;

            try
            {
                switch (method)
                {
                    case "initialize":
                        newSessionId = McpServer.NewSessionId();
                        return Result(id, Initialize(parameters));

                    case "notifications/initialized":
                        // Should arrive as a notification (no id); answered here defensively.
                        return Result(id, new JsonObject());

                    case "ping":
                        return Result(id, new JsonObject());

                    case "tools/list":
                        return Result(id, ToolsList());

                    case "tools/call":
                        return Result(id, ToolsCall(parameters));

                    default:
                        return Error(id, MethodNotFoundCode, $"Method not found: {method}");
                }
            }
            catch (McpToolException ex)
            {
                return Error(id, InvalidParamsCode, ex.Message);
            }
            catch (Exception ex)
            {
                return Error(id, InternalErrorCode, ex.Message);
            }
        }

        private static JsonObject Initialize(JsonObject? parameters)
        {
            // Echo back the client's protocol version when we support it, else our preferred.
            string requested = parameters?["protocolVersion"]?.GetValue<string>() ?? McpServer.PreferredProtocolVersion;
            string version = McpServer.IsProtocolSupported(requested) ? requested : McpServer.PreferredProtocolVersion;

            string appVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0";

            return new JsonObject
            {
                ["protocolVersion"] = version,
                ["capabilities"] = new JsonObject
                {
                    ["tools"] = new JsonObject { ["listChanged"] = false }
                },
                ["serverInfo"] = new JsonObject
                {
                    ["name"] = "G-Helper",
                    ["version"] = appVersion
                },
                ["instructions"] = "Controls and reads telemetry from an Asus laptop running G-Helper. " +
                    "Use get_status / get_sensors to read state, and the set_* tools to change performance mode, " +
                    "GPU mode, battery charge limit, keyboard backlight and screen refresh rate."
            };
        }

        private static JsonObject ToolsList()
        {
            var tools = new JsonArray();
            foreach (var tool in McpTools.All)
            {
                tools.Add(new JsonObject
                {
                    ["name"] = tool.Name,
                    ["description"] = tool.Description,
                    ["inputSchema"] = tool.InputSchema.DeepClone()
                });
            }
            return new JsonObject { ["tools"] = tools };
        }

        private static JsonObject ToolsCall(JsonObject? parameters)
        {
            string? name = parameters?["name"]?.GetValue<string>();
            if (string.IsNullOrEmpty(name))
                throw new McpToolException("Missing tool name");

            var tool = McpTools.Find(name);
            if (tool is null)
                throw new McpToolException($"Unknown tool: {name}");

            JsonObject args = parameters?["arguments"] as JsonObject ?? new JsonObject();

            try
            {
                string text = McpTools.Invoke(tool, args);
                return ToolText(text, isError: false);
            }
            catch (McpToolException ex)
            {
                // Tool-level failures are reported via isError content, not a protocol error,
                // so the model can read and react to the message.
                return ToolText(ex.Message, isError: true);
            }
            catch (Exception ex)
            {
                return ToolText($"Error: {ex.Message}", isError: true);
            }
        }

        private static JsonObject ToolText(string text, bool isError)
        {
            return new JsonObject
            {
                ["content"] = new JsonArray
                {
                    new JsonObject { ["type"] = "text", ["text"] = text }
                },
                ["isError"] = isError
            };
        }

        #region JSON-RPC envelope builders

        private static JsonObject Result(JsonNode? id, JsonObject result) => new()
        {
            ["jsonrpc"] = "2.0",
            ["id"] = id?.DeepClone(),
            ["result"] = result
        };

        private static JsonObject Error(JsonNode? id, int code, string message) => new()
        {
            ["jsonrpc"] = "2.0",
            ["id"] = id?.DeepClone(),
            ["error"] = new JsonObject { ["code"] = code, ["message"] = message }
        };

        public static JsonObject ParseError() => new()
        {
            ["jsonrpc"] = "2.0",
            ["id"] = null,
            ["error"] = new JsonObject { ["code"] = ParseErrorCode, ["message"] = "Parse error" }
        };

        #endregion
    }

    /// <summary>Thrown by tool handlers for user-facing failures (bad args, unsupported hardware).</summary>
    internal class McpToolException : Exception
    {
        public McpToolException(string message) : base(message) { }
    }
}
