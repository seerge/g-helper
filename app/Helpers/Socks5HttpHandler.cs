using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;

namespace GHelper.Helpers;

public class Socks5HttpMessageHandler : HttpMessageHandler
{
    private readonly string _proxyHost;
    private readonly int _proxyPort;
    private readonly string? _username;
    private readonly string? _password;

    public Socks5HttpMessageHandler(string proxyHost, int proxyPort, string? username = null, string? password = null)
    {
        _proxyHost = proxyHost;
        _proxyPort = proxyPort;
        _username = username;
        _password = password;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var proxyEndPoint = new DnsEndPoint(_proxyHost, _proxyPort);
        using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        await socket.ConnectAsync(proxyEndPoint, cancellationToken);
        using var stream = new NetworkStream(socket, ownsSocket: true);

        await DoSocks5HandshakeAsync(stream, request, cancellationToken);

        return await SendHttpRequestAsync(stream, request, cancellationToken);
    }

    private async Task DoSocks5HandshakeAsync(
        Stream stream, HttpRequestMessage request, CancellationToken cancellationToken)
    {
        byte[] greeting;
        bool hasAuth = !string.IsNullOrEmpty(_username);

        if (hasAuth)
            greeting = [0x05, 0x02, 0x00, 0x02];
        else
            greeting = [0x05, 0x01, 0x00];

        await stream.WriteAsync(greeting, cancellationToken);

        var response = new byte[2];
        await ReadExactAsync(stream, response, cancellationToken);

        if (response[0] != 0x05)
            throw new InvalidOperationException($"SOCKS5: unexpected version {response[0]}");

        byte selectedMethod = response[1];
        if (selectedMethod == 0xFF)
            throw new InvalidOperationException("SOCKS5: no acceptable authentication method");

        if (selectedMethod == 0x02)
        {
            if (!hasAuth)
                throw new InvalidOperationException("SOCKS5: authentication required");

            byte[] authRequest = BuildAuthRequest(_username!, _password!);
            await stream.WriteAsync(authRequest, cancellationToken);

            var authResponse = new byte[2];
            await ReadExactAsync(stream, authResponse, cancellationToken);

            if (authResponse[1] != 0x00)
                throw new InvalidOperationException("SOCKS5: authentication failed");
        }
        else if (selectedMethod != 0x00)
        {
            throw new InvalidOperationException($"SOCKS5: unsupported auth method {selectedMethod}");
        }

        string targetHost = request.RequestUri?.Host ?? "";
        int targetPort = request.RequestUri?.Port ?? 80;
        byte[] connectRequest = BuildConnectRequest(targetHost, targetPort);
        await stream.WriteAsync(connectRequest, cancellationToken);

        var connectResponseHeader = new byte[4];
        await ReadExactAsync(stream, connectResponseHeader, cancellationToken);

        if (connectResponseHeader[1] != 0x00)
            throw new InvalidOperationException($"SOCKS5: connect failed (status {connectResponseHeader[1]})");

        byte atyp = connectResponseHeader[3];
        int addrLength = atyp switch
        {
            0x01 => 4,
            0x03 => await ReadVarIntAsync(stream, cancellationToken),
            0x04 => 16,
            _ => throw new InvalidOperationException($"SOCKS5: unsupported address type {atyp}")
        };

        if (atyp == 0x03)
        {
            var domainBytes = new byte[addrLength];
            await ReadExactAsync(stream, domainBytes, cancellationToken);
        }
        else
        {
            var addrBytes = new byte[addrLength];
            await ReadExactAsync(stream, addrBytes, cancellationToken);
        }

        var portBytes = new byte[2];
        await ReadExactAsync(stream, portBytes, cancellationToken);
    }

    private async Task<HttpResponseMessage> SendHttpRequestAsync(
        Stream stream, HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Write request line
        var uri = request.RequestUri;
        string pathAndQuery = uri?.PathAndQuery ?? "/";
        if (string.IsNullOrEmpty(pathAndQuery)) pathAndQuery = "/";

        var requestLine = $"{request.Method.Method} {pathAndQuery} HTTP/{request.Version}";
        byte[] requestLineBytes = Encoding.ASCII.GetBytes(requestLine + "\r\n");
        await stream.WriteAsync(requestLineBytes, cancellationToken);

        // Write headers
        foreach (var header in request.Headers)
        {
            var headerLine = $"{header.Key}: {string.Join(", ", header.Value)}\r\n";
            byte[] headerBytes = Encoding.ASCII.GetBytes(headerLine);
            await stream.WriteAsync(headerBytes, cancellationToken);
        }

        if (request.Content != null)
        {
            foreach (var header in request.Content.Headers)
            {
                var headerLine = $"{header.Key}: {string.Join(", ", header.Value)}\r\n";
                byte[] headerBytes = Encoding.ASCII.GetBytes(headerLine);
                await stream.WriteAsync(headerBytes, cancellationToken);
            }
        }

        // End of headers
        await stream.WriteAsync(new byte[] { 0x0D, 0x0A }, cancellationToken);

        // Write body
        if (request.Content != null)
        {
            await request.Content.CopyToAsync(stream, cancellationToken);
        }

        // Read response
        return await ReadHttpResponseAsync(stream, cancellationToken);
    }

    private async Task<HttpResponseMessage> ReadHttpResponseAsync(
        Stream stream, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage();

        // Read status line
        string statusLine = await ReadLineAsync(stream, cancellationToken);
        if (string.IsNullOrEmpty(statusLine))
            throw new InvalidOperationException("SOCKS5: empty response");

        // Parse: HTTP/1.1 200 OK
        var statusParts = statusLine.Split(' ', 3);
        if (statusParts.Length < 2 || !int.TryParse(statusParts[1], out int statusCode))
            throw new InvalidOperationException($"SOCKS5: invalid status line: {statusLine}");

        response.StatusCode = (System.Net.HttpStatusCode)statusCode;
        response.ReasonPhrase = statusParts.Length > 2 ? statusParts[2] : "";

        string httpVersion = statusParts[0].Contains("1.0") ? "1.0" : "1.1";
        response.Version = new Version(httpVersion);

        // Read headers
        var contentHeaders = new Dictionary<string, string>();
        while (true)
        {
            string headerLine = await ReadLineAsync(stream, cancellationToken);
            if (string.IsNullOrEmpty(headerLine))
                break;

            int colonIdx = headerLine.IndexOf(':');
            if (colonIdx > 0)
            {
                string key = headerLine[..colonIdx].Trim();
                string value = headerLine[(colonIdx + 1)..].Trim();

                if (!response.Headers.TryAddWithoutValidation(key, value))
                    contentHeaders[key] = value;
            }
        }

        // Set content headers
        foreach (var kvp in contentHeaders)
        {
            response.Content?.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
        }

        // Read body
        byte[] bodyBytes;
        if (contentHeaders.TryGetValue("Content-Length", out string? contentLengthStr) &&
            long.TryParse(contentLengthStr, out long contentLength))
        {
            bodyBytes = new byte[contentLength];
            await ReadExactAsync(stream, bodyBytes, cancellationToken);
        }
        else if (contentHeaders.TryGetValue("Transfer-Encoding", out string? transferEncoding) &&
                 transferEncoding.Equals("chunked", StringComparison.OrdinalIgnoreCase))
        {
            bodyBytes = await ReadChunkedBodyAsync(stream, cancellationToken);
        }
        else
        {
            // Read until connection close
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms, cancellationToken);
            bodyBytes = ms.ToArray();
        }

        response.Content = new ByteArrayContent(bodyBytes);

        // Apply content headers
        foreach (var kvp in contentHeaders)
        {
            response.Content.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
        }

        return response;
    }

    private async Task<byte[]> ReadChunkedBodyAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var ms = new MemoryStream();
        while (true)
        {
            string sizeLine = await ReadLineAsync(stream, cancellationToken);
            if (string.IsNullOrEmpty(sizeLine))
                break;

            int chunkSize = Convert.ToInt32(sizeLine.Trim(), 16);
            if (chunkSize == 0)
            {
                // Read trailing CRLF
                await ReadLineAsync(stream, cancellationToken);
                break;
            }

            var chunk = new byte[chunkSize];
            await ReadExactAsync(stream, chunk, cancellationToken);
            ms.Write(chunk, 0, chunkSize);

            // Read trailing CRLF
            await ReadLineAsync(stream, cancellationToken);
        }
        return ms.ToArray();
    }

    private static async Task<string> ReadLineAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var ms = new MemoryStream();
        int prevByte = -1;
        while (true)
        {
            int b = await ReadByteAsync(stream, cancellationToken);
            if (b == -1) break;

            if (prevByte == '\r' && b == '\n')
            {
                // Remove the \r we added
                var bytes = ms.ToArray();
                if (bytes.Length > 0 && bytes[^1] == (byte)'\r')
                    return Encoding.ASCII.GetString(bytes, 0, bytes.Length - 1);
                return Encoding.ASCII.GetString(bytes);
            }

            ms.WriteByte((byte)b);
            prevByte = b;
        }

        return Encoding.ASCII.GetString(ms.ToArray());
    }

    private static async Task<int> ReadByteAsync(Stream stream, CancellationToken cancellationToken)
    {
        var buffer = new byte[1];
        int bytesRead = await stream.ReadAsync(buffer, cancellationToken);
        return bytesRead == 0 ? -1 : buffer[0];
    }

    private static byte[] BuildAuthRequest(string username, string password)
    {
        byte[] userBytes = Encoding.UTF8.GetBytes(username);
        byte[] passBytes = Encoding.UTF8.GetBytes(password);

        var request = new byte[3 + userBytes.Length + passBytes.Length];
        request[0] = 0x01;
        request[1] = (byte)userBytes.Length;
        Buffer.BlockCopy(userBytes, 0, request, 2, userBytes.Length);
        request[2 + userBytes.Length] = (byte)passBytes.Length;
        Buffer.BlockCopy(passBytes, 0, request, 3 + userBytes.Length, passBytes.Length);

        return request;
    }

    private static byte[] BuildConnectRequest(string host, int port)
    {
        byte[] hostBytes = Encoding.UTF8.GetBytes(host);

        var request = new byte[7 + hostBytes.Length];
        request[0] = 0x05;
        request[1] = 0x01;
        request[2] = 0x00;
        request[3] = 0x03;
        request[4] = (byte)hostBytes.Length;
        Buffer.BlockCopy(hostBytes, 0, request, 5, hostBytes.Length);
        request[5 + hostBytes.Length] = (byte)(port >> 8);
        request[6 + hostBytes.Length] = (byte)(port & 0xFF);

        return request;
    }

    private static async Task ReadExactAsync(Stream stream, byte[] buffer, CancellationToken cancellationToken)
    {
        int totalRead = 0;
        while (totalRead < buffer.Length)
        {
            int bytesRead = await stream.ReadAsync(buffer.AsMemory(totalRead, buffer.Length - totalRead), cancellationToken);
            if (bytesRead == 0)
                throw new InvalidOperationException("SOCKS5: connection closed unexpectedly");
            totalRead += bytesRead;
        }
    }

    private static async Task<int> ReadVarIntAsync(Stream stream, CancellationToken cancellationToken)
    {
        var buffer = new byte[1];
        await ReadExactAsync(stream, buffer, cancellationToken);
        return buffer[0];
    }
}