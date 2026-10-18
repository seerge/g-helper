# G-Helper Custom Build Rules

## Overview
Custom build of G-Helper with CPU Temperature keyboard color control modifications.

## Build Rules

### Output
- **Type:** Framework-dependent single-file exe
- **Target:** ~5MB
- **Platform:** win-x64
- **Command:**
  ```powershell
  dotnet restore app\GHelper.csproj
  dotnet publish app\GHelper.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:DebugType=None -o app\bin\publishN
  ```

### Output Location
- **EXE Destination:** `C:\Users\OVER\Downloads\Programs\UTILITY\G-Helper\GHelper.exe`
- After build, copy exe to destination and restart app:
  ```powershell
  Copy-Item -Path "app\bin\publishN\GHelper.exe" -Destination "C:\Users\OVER\Downloads\Programs\UTILITY\G-Helper\GHelper.exe" -Force
  Start-Process "C:\Users\OVER\Downloads\Programs\UTILITY\G-Helper\GHelper.exe"
  ```

### Backup
- **Patch file:** `backup/cputemp-speed.patch`
- **Generated via:** `git diff app\USB\Aura.cs > backup/cputemp-speed.patch`
- **Restore after upstream sync:** `git apply backup/cputemp-speed.patch`

## CPU Temperature Mode Modifications

### Thresholds (Aura.cs)
| Level | Default | Custom |
|-------|---------|--------|
| freeze | 45°C | 45°C |
| cold | 50°C | **55°C** |
| warm | 60°C | **70°C** |
| hot | 80°C | **85°C** |

### Speed/Timing (Aura.cs)
| Speed | Timer Interval | Smoothing |
|-------|---------------|-----------|
| Slow | 2000ms | 0.3 |
| Normal | 1500ms | 0.5 |
| Fast | 500ms | 1.0 |

### Code Changes Summary
1. **CPUTEMP timer section** (~line 880): Add smoothing and interval based on Speed
2. **Threshold defaults** (~line 1080): 55/70/85
3. **tempSmoothing field** (~line 1175): `const double` → `public static double = 0.5`

## Agent Rules

### Before Any Modification
1. Read this file first
2. Check `backup/cputemp-speed.patch` for current mod state
3. Pull latest upstream before making changes

### After Modifying Aura.cs
1. Run build command
2. Copy exe to destination
3. Run `git diff app\USB\Aura.cs > backup/cputemp-speed.patch` to update backup
4. Verify build size ~5MB

### Git Workflow
- Fork remote: `origin` → `https://github.com/overdevz/g-helper`
- Never commit to main without explicit request
- Always backup changes via patch before pulling upstream
