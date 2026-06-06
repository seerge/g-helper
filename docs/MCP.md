# G-Helper MCP Server

G-Helper can expose its laptop controls and telemetry to AI assistants through a built-in
[Model Context Protocol](https://modelcontextprotocol.io) (MCP) server. Once enabled, an
MCP client (Claude Desktop, Claude Code, or any other) can read sensors and change settings
on your Asus laptop.

The server is **disabled by default**. It runs inside the G-Helper process, listens on
**localhost only**, and requires a **bearer token** on every request.

## Enabling the server

1. Right-click the G-Helper tray icon.
2. Click **MCP Server** to toggle it on.
3. A dialog shows the connection **URL** and **token** — copy them. The token is also stored
   in `config.json` under `mcp_token`.

> Because the server controls hardware (performance mode, GPU mode, battery limit, etc.),
> binding to `http://127.0.0.1` requires permission to reserve the URL. G-Helper already runs
> elevated for most hardware features, in which case it works out of the box. If the server
> fails to start (see `log.txt`) while running unelevated, reserve the URL once as admin:
>
> ```powershell
> netsh http add urlacl url=http://127.0.0.1:8787/ user=Everyone
> ```

### Configuration keys (`config.json`)

| Key           | Default | Meaning                                      |
| ------------- | ------- | -------------------------------------------- |
| `mcp_enabled` | `0`     | `1` to start the server on launch.           |
| `mcp_port`    | `8787`  | TCP port (loopback only).                    |
| `mcp_token`   | (auto)  | Bearer token, auto-generated on first start. |

## Transport

The server implements the MCP **Streamable HTTP** transport at:

```
http://127.0.0.1:8787/mcp
```

Every request must include:

- `Authorization: Bearer <mcp_token>`
- `Accept: application/json, text/event-stream`

The server returns single `application/json` JSON-RPC responses (it does not open
server-initiated SSE streams). Cross-origin browser requests are rejected to prevent
DNS-rebinding attacks.

## Connecting from Claude Code

```bash
claude mcp add --transport http g-helper http://127.0.0.1:8787/mcp \
  --header "Authorization: Bearer <mcp_token>"
```

## Connecting from Claude Desktop

Add to your MCP config (replace the token):

```json
{
  "mcpServers": {
    "g-helper": {
      "type": "http",
      "url": "http://127.0.0.1:8787/mcp",
      "headers": { "Authorization": "Bearer <mcp_token>" }
    }
  }
}
```

## Available tools

| Tool                     | Type    | Description                                                                  |
| ------------------------ | ------- | ---------------------------------------------------------------------------- |
| `get_status`             | read    | Model, performance mode, GPU mode, power source, battery, display, backlight |
| `get_sensors`            | read    | CPU/GPU temperature, fan RPMs, CPU/GPU power draw                            |
| `list_performance_modes` | read    | Built-in and custom performance modes, plus the current one                  |
| `set_performance_mode`   | control | `silent` \| `balanced` \| `turbo`                                            |
| `set_gpu_mode`           | control | `eco` \| `standard` (Ultimate is excluded — it requires a reboot)            |
| `set_battery_limit`      | control | Charge limit percentage (40–100)                                             |
| `set_keyboard_backlight` | control | Backlight level 0–3                                                          |
| `set_refresh_rate`       | control | Refresh rate in Hz (`0` = minimum, `1000` = maximum)                         |

## Security notes

- Anyone with the token can read sensors and change laptop settings. Treat it like a password.
- The server only accepts connections from the local machine (`127.0.0.1`).
- Toggling the tray menu item off stops the server immediately.
- To rotate the token, delete `mcp_token` from `config.json` and re-enable the server.

## Implementation

The server lives in [`app/Mcp/`](../app/Mcp):

- `McpServer.cs` — `HttpListener`-based transport, auth, Origin validation, session handling.
- `McpProtocol.cs` — JSON-RPC 2.0 framing and `initialize` / `tools/list` / `tools/call` dispatch.
- `McpTools.cs` — the tool definitions wrapping G-Helper's existing controllers.

It adds **no new NuGet dependencies**.
