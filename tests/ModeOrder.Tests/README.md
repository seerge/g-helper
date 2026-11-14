# Profile ordering regression checks

Run on Windows with the .NET 10 SDK from the repository root.

```powershell
dotnet run --project tests/ModeOrder.Tests -c Release -p:GITHUB_ACTIONS=true
```

The checks call the compiled application's `Modes` methods and real `AppConfig` reader and writer. Reflection accesses the internal class and redirects configuration and log output to the test executable's directory. A child process checks persisted order after restart. No hardware controller or application main entry point is started.

`GITHUB_ACTIONS=true` skips the application's build target that closes a running G-Helper instance. It does not upload anything.

An existing configuration can also be checked read-only. The test copies it into the test output directory before loading it.
The copy is written and reloaded to verify round-trip preservation. Supplying the test fixture itself as input is rejected before modification.

```powershell
dotnet run --project tests/ModeOrder.Tests -c Release -p:GITHUB_ACTIONS=true -- --check-config C:\path\to\config.json
```
