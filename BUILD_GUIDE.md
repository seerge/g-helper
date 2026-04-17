# Compilation & Deployment Guide

## Prerequisites
- .NET 8.0 SDK
- Windows OS

## Build Command (Optimized for Accessibility & Reputation)

To build a single-file executable that minimizes false positives from Smart App Control and Windows Defender:

```powershell
dotnet publish app/GHelper.csproj -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true /p:PublishReadyToRun=true /p:IncludeNativeLibrariesForSelfExtract=true
```

### Flag Explanations:
- `--self-contained false`: Relies on system .NET Runtime. Dramatically reduces file size (~5MB vs 150MB) and improves file reputation.
- `/p:PublishSingleFile=true`: Packs everything into one `GHelper.exe`.
- `/p:PublishReadyToRun=true`: Pre-compiles code to native image for faster startup and better security compliance.
- `/p:IncludeNativeLibrariesForSelfExtract=true`: Ensures necessary native DLLs (like `WinRing0x64.dll`) are correctly handled.

## Post-Build
The output executable will be located in:
`app/bin/Release/net8.0-windows/win-x64/publish/GHelper.exe`

Copy it to the project root for distribution.
