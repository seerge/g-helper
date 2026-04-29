# G-Helper - Lightweight control tool for Asus laptops
[![GitHub release](https://img.shields.io/github/release/seerge/g-helper?color=%234493f8)](https://GitHub.com/seerge/g-helper/releases/)
[![Github all releases](https://img.shields.io/github/downloads/seerge/g-helper/total?logo=download&color=%234493f8)](https://GitHub.com/seerge/g-helper/releases/)
[![SLSA3](https://img.shields.io/badge/SLSA-level%203-blue?color=%234493f8)](https://github.com/seerge/g-helper/attestations)
[![GitHub stars](https://img.shields.io/github/stars/seerge/g-helper?style=flat&color=4493f8&logo=github)](https://GitHub.com/seerge/g-helper/stargazers/)
[![中文](https://img.shields.io/badge/-中文-555555)](https://github.com/seerge/g-helper/blob/main/docs/README.zh-CN.md)
[![日本語](https://img.shields.io/badge/-日本語-555555)](https://github.com/seerge/g-helper/blob/main/docs/README.ja-JP.md)
[![Čeština](https://img.shields.io/badge/-Čeština-555555)](https://github.com/seerge/g-helper/blob/main/docs/README.cs-CZ.md)
<a href="https://github.com/seerge/g-helper/releases/latest/download/GHelper.exe"><img width="1280" alt="G-Helper - Lightweight control tool for Asus laptops" src="https://github.com/user-attachments/assets/8ba47820-e0ae-4431-a50a-0ec31e07ffa7" /></a>

**The control app every laptop should come with.** G-Helper is fast, native, and straight to the point - a tool for tuning performance, fans, GPU, battery, and RGB on any Asus laptop or handheld: ROG Zephyrus, Flow, Strix, Scar, TUF, Vivobook, Zenbook, ProArt, ROG Ally, and beyond.

<a href="https://github.com/seerge/g-helper/releases/latest/download/GHelper.exe"><img width="250" alt="Download" src="https://raw.githubusercontent.com/seerge/g-helper/main/docs/Download.png" /></a> &nbsp; <a href="https://g-helper.com/support"><img width="250" alt="Support" src="https://raw.githubusercontent.com/seerge/g-helper/main/docs/Support.png" /></a>

**⭐ Like the app? Help spread the word.**

- [FAQ](https://github.com/seerge/g-helper/wiki/FAQ)
- [Setup and Requirements](https://github.com/seerge/g-helper/wiki/Requirements)
- [Troubleshooting](https://github.com/seerge/g-helper/wiki/Troubleshooting)
- [Power User Settings](https://github.com/seerge/g-helper/wiki/Power-user-settings)
-----------

[![United24](https://github.com/user-attachments/assets/aa9444e3-9daa-4b88-a473-7a7f855e3a07)](https://u24.gov.ua/)

## :loudspeaker: YouTube Reviews and Guides
| [![Youtube review Josh Cravey](https://i.ytimg.com/vi/hqe-PjuE-K8/hqdefault.jpg)](https://www.youtube.com/watch?v=hqe-PjuE-K8) | [![Youtube review Crimson Tech](https://i.ytimg.com/vi/5XUIMUzgHU0/hqdefault.jpg)](https://www.youtube.com/watch?v=5XUIMUzgHU0) | [![Youtube review cbutters Tech](https://i.ytimg.com/vi/6aVdwJKZSSc/hqdefault.jpg)](https://www.youtube.com/watch?v=6aVdwJKZSSc) |
| ----------------- | ---------------- | ---------------- | 
| [Josh Cravey](https://www.youtube.com/watch?v=hqe-PjuE-K8) | [Crimson Tech](https://www.youtube.com/watch?v=5XUIMUzgHU0) | [cbutters Tech](https://www.youtube.com/watch?v=6aVdwJKZSSc) | 

## 📰 Articles
1. [Digital Trends](https://www.digitaltrends.com/computing/g-helper-armoury-crate-alternative/)
2. [BinaryFork](https://binaryfork.com/ghelper-armoury-crate-alternative-10216/)
3. [Ultrabookreview](https://www.ultrabookreview.com/71392-optimized-quiet-gaming-5080-5090/)
4. [Notebookcheck](https://www.notebookcheck.net/Unbloated-G-Helper-The-best-open-source-alternative-to-Asus-Armoury-Crate-Part-2.1213486.0.html)
5. [Les Numériques](https://www.lesnumeriques.com/appli-logiciel/marre-des-lenteurs-d-armoury-crate-sur-votre-pc-portable-asus-decouvrez-g-helper-l-alternative-ultra-legere-n240111.html)

## :gift: Advantages 

1. Seamless and automatic GPU switching
2. All performance modes can be fully customized with power limits and fan curves
3. Lightweight. Just a single exe to run. Doesn't install anything in your system. 
4. Simple and clean native UI with easy access to all settings
5. FN-Lock and custom hotkeys

<img width="1960" height="1587" alt="G-Helper Screenshot" src="https://github.com/user-attachments/assets/9376fe90-fbb6-420a-abbd-0186189665e1" />

### :zap: Features

1. Performance modes: Silent - Balanced - Turbo (built-in, with default fan curves)
2. GPU modes: Eco - Standard - Ultimate - Optimized
3. Screen refresh rate control with display overdrive
4. Custom fan curve editor, power limits and turbo boost selection for every performance mode
5. Anime Matrix or Slash Lighting control including animated GIFs, clock and Audio visualizer
6. RGB Backlight animation modes and colors 
7. Hotkey handling
8. Monitor CPU and GPU temperature, fan speeds and battery status
9. Battery charge limit to preserve battery health
10. NVidia GPU overclocking and undervolting
11. XG Mobile Control
12. AMD CPU Undervolting and temp limits
13. BIOS and Driver Updates
14. Asus Mice controls
15. Mini-led multi-zone switch
16. Flicker-free dimming and Visual Modes

### :gear: Automation
- Performance Mode switching when on battery or plugged in
- Optimized GPU mode - disables dGPU on battery and enables when plugged in
- Auto Screen refresh rate (60Hz on battery and max Hz when plugged)
- Keyboard backlight timeout on battery or when plugged in

### :rocket: Performance Modes
>[!NOTE]
>All Modes are **baked into BIOS** along with default fan curves and power limits. And and they are the **same thing** as in the Armoury Crate.

Each default BIOS mode is paired with matching [Windows Power Mode](https://support.microsoft.com/en-us/windows/change-the-power-mode-for-your-windows-pc-c2aff038-22c9-f46d-5ca0-78696fdf2de8). You can adjust this setting under ``Fans + Power``

1. `Silent` in BIOS + `Best power efficiency` power mode
2. `Balanced` (Performance in AC) in BIOS  + `Balanced` power mode
3. `Turbo` in BIOS + `Best performance` power mode

<img width="2330" height="1524" alt="Performance Modes" src="https://github.com/user-attachments/assets/254d33eb-2af1-4715-a097-ed8678c7e9db" />

### :video_game: GPU Tweaking

In each mode dedicated GPU can be tweaked to specific needs, including Core and Memory Clock offsets, GPU power, Dynamic Boost, Temperature Limit etc
<img width="2354" height="1550" alt="GPU Tweaking" src="https://github.com/user-attachments/assets/48ce19a6-d149-40ed-9145-23f974c513f7" />

### 💻 GPU Modes

1. **Eco** : only low power integrated GPU enabled, iGPU drives built in display
2. **Standard** (MS Hybrid) : iGPU and dGPU enabled, iGPU drives built in display
3. **Ultimate**: iGPU and dGPU enabled, but dGPU drives built in display (supported on 2022+ models)
4. **Optimized**: disables dGPU on battery (Eco) and enables when plugged in (Standard)

<img width="1134" height="746" alt="GPU Modes" src="https://github.com/user-attachments/assets/c273baa1-dbe7-4361-88aa-ec670128f956" />

### 💿Driver Updates
The app includes an automatic BIOS and driver update checker that pulls directly from the official Asus website for your specific model, highlighting new downloads as they become available. All links in the `Updates` section point to official Asus downloads.
<img width="2302" height="1493" alt="Driver Updates" src="https://github.com/user-attachments/assets/303dfce9-fbbd-4d15-b6d7-f21e7c2c59a4" />

### :mouse: Asus Mouse and other peripherals support

<details>
<summary><a href="https://github.com/seerge/g-helper/discussions/900">Currently supported models</a> (click to expand)</summary>

- ROG Chakram X
- ROG Chakram Core
- ROG Gladius II and Gladius II Origin
- ROG Gladius II Wireless
- ROG Gladius III
- ROG Gladius III Wireless
- ROG Harpe Ace Extreme
- ROG Harpe Ace Aim Lab Edition
- ROG Harpe Ace Mini
- ROG Harpe II Ace
- ROG Keris Wireless
- ROG Keris II Ace
- ROG Keris Wireless Aimpoint
- ROG Strix Carry
- ROG Strix III Gladius III Aimpoint Wireless
- ROG Strix Impact III
- ROG Strix Impact III Wireless
- ROG Spatha X
- ROG Strix Impact II Wireless
- ROG Pugio
- ROG Pugio II
- TUF Gaming M4 Wireless
- TUF Gaming M3
- TUF Gaming M3 Gen II
- TUF Gaming M4 AIR
- TUF Gaming M5
- TX Gaming Mini

</details>

Huge thanks to [@IceStormNG](https://github.com/IceStormNG) 👑 for contribution and research (!).

<img width="2448" height="1653" alt="Mouse and other peripherals" src="https://github.com/user-attachments/assets/fe2a766b-f514-42e9-8dff-4bcc915364d4" />

### ⌨️ Keybindings

- ``Fn + F5 / Fn + Shift + F5`` - Toggle Performance Modes forwards / backwards
- ``Ctrl + Shift + F5 / Ctrl + Shift + Alt + F5`` - Toggle Performance Modes forwards / backwards
- ``Ctrl + Shift + F12`` - Open G-Helper window
- ``Ctrl + M1 / M2`` - Screen brightness Down / Up
- ``Shift + M1 / M2`` - Backlight brightness Down / Up
- ``Fn + V`` - Visual Modes
- ``Fn + C`` / ``Fn + Esc`` - Fn-Lock
- ``Fn + Ctrl +  F7 / F8`` / ``Ctrl + Shift + Alt +  F7 / F8`` - Flicker-free dimming Down / Up
- ``Fn + Shift + F7 / F8`` - Matrix / Slash Lighting brightness Down / Up
- ``Fn + Shift + F7 / F8`` / ``Ctrl + Shift + Alt +  F7 / F8`` - Screenpad brightness Down / Up
- ``Ctrl + Shift + F20`` - Mute Microphone
- ``Ctrl + Shift + Alt + F13`` - Toggle Display Refresh Rate
- ``Ctrl + Shift + Alt + F14`` - Eco GPU Mode
- ``Ctrl + Shift + Alt + F15`` - Standard GPU Mode
- ``Ctrl + Shift + Alt + F16`` - Silent
- ``Ctrl + Shift + Alt + F17`` - Balanced
- ``Ctrl + Shift + Alt + F18`` - Turbo
- ``Ctrl + Shift + Alt + F19`` - Custom 1 (if exists)
- ``Ctrl + Shift + Alt + F20`` - Custom 2 (if exists)
- [Custom keybindings / hotkeys](https://github.com/seerge/g-helper/wiki/Power-user-settings#custom-hotkey-actions)

### 🎮ROG Ally Bindings
- ``M + DPad Left / Right`` - Display Brightness
- ``M + DPad Up`` - Touch keyboard
- ``M + DPad Down`` - Show desktop
- ``M + Y`` - Toggle AMD overay
- ``M + X`` - Screenshot
- ``M + Right Stick Click`` - Controller Mode

------------------

> [!NOTE]
> ### 🔖 Important Notice
> G-Helper is **NOT** an operating system, firmware, or driver. It **DOES NOT** "run" your hardware in real-time anyhow.
> 
> It's an app that lets you select one of the predefined operating modes created by manufacturer (and stored in BIOS) and optionally(!) set some settings that already exist on your device same as Armoury Crate can. It does it by using the Asus System Control Interface "driver" that Armoury uses for it.
> 
> If you use equivalent mode/settings as in Armoury Crate - the performance or the behavior of your device won't be different.
> 
> The role of G-Helper for your laptop is similar to the role of a remote control for your TV.

### Libraries and projects used
- [Linux Kernel](https://github.com/torvalds/linux/blob/master/include/linux/platform_data/x86/asus-wmi.h) for some basic endpoints in ASUS ACPI/WMI interface
- [NvAPIWrapper](https://github.com/falahati/NvAPIWrapper) for accessing Nvidia API
- [Starlight](https://github.com/vddCore/Starlight) for anime matrix communication protocol
- [UXTU](https://github.com/JamesCJ60/Universal-x86-Tuning-Utility) for undervolting endpoints using Ryzen SMU
- [PawnIO](https://github.com/namazso/PawnIO) for access to RyzenSMU
- [AsusCtl](https://gitlab.com/asus-linux/asusctl) for inspiration and some reverse engineering

### Code Signing Policy
Free code signing provided by [SignPath.io](https://about.signpath.io/), certificate by [SignPath Foundation](https://signpath.org/)

### Privacy Policy
This program will not transfer any information to other networked systems

### Disclaimers
"Asus", "ROG", "TUF", and "Armoury Crate" are trademarked by and belong to AsusTek Computer, Inc. I make no claims to these or any assets belonging to AsusTek Computer and use them purely for informational purposes only.

THE SOFTWARE IS PROVIDED “AS IS” AND WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. MISUSE OF THIS SOFTWARE COULD CAUSE SYSTEM INSTABILITY OR MALFUNCTION.
