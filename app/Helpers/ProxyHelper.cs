using System.Net;
using System.Net.Http;

namespace GHelper.Helpers;

public enum ProxyType
{
    None = 0,
    Http = 1,
    Socks5 = 2
}

public static class ProxyHelper
{
    public static ProxyType Type => (ProxyType)AppConfig.Get("proxy_type", 0);
    public static string Host => AppConfig.GetString("proxy_host", "");
    public static int Port => AppConfig.Get("proxy_port", 0);
    public static string Username => AppConfig.GetString("proxy_username", "");
    public static string Password => AppConfig.GetString("proxy_password", "");

    public static bool IsConfigured() => Type != ProxyType.None && !string.IsNullOrEmpty(Host) && Port > 0;

    public static void Save(ProxyType type, string host, int port, string username = "", string password = "")
    {
        AppConfig.Set("proxy_type", (int)type);
        AppConfig.Set("proxy_host", host);
        AppConfig.Set("proxy_port", port);
        AppConfig.Set("proxy_username", username);
        AppConfig.Set("proxy_password", password);
    }

    public static HttpClient CreateHttpClient(string? userAgent = null, bool autoDecompression = false)
    {
        HttpMessageHandler handler;

        switch (Type)
        {
            case ProxyType.Http:
                var socketsHandler = new SocketsHttpHandler
                {
                    Proxy = new WebProxy($"http://{Host}:{Port}"),
                    UseProxy = true
                };
                if (autoDecompression)
                    socketsHandler.AutomaticDecompression = DecompressionMethods.All;
                handler = socketsHandler;
                break;

            case ProxyType.Socks5:
                handler = new Socks5HttpMessageHandler(Host, Port,
                    string.IsNullOrEmpty(Username) ? null : Username,
                    string.IsNullOrEmpty(Password) ? null : Password);
                break;

            default:
                var defaultHandler = new SocketsHttpHandler();
                if (autoDecompression)
                    defaultHandler.AutomaticDecompression = DecompressionMethods.All;
                handler = defaultHandler;
                break;
        }

        var client = new HttpClient(handler);

        if (!string.IsNullOrEmpty(userAgent))
        {
            client.DefaultRequestHeaders.Add("User-Agent", userAgent);
        }

        if (autoDecompression && Type != ProxyType.Socks5)
        {
            client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
        }

        return client;
    }

    public static string GetDisplayText()
    {
        if (!IsConfigured()) return "";

        string typeStr = Type == ProxyType.Http ? "HTTP" : "SOCKS5";
        string authStr = string.IsNullOrEmpty(Username) ? "" : "(认证)";
        return $"{typeStr} {Host}:{Port}{authStr}";
    }
}