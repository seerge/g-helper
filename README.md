# G-Helper / GHelper 

## Open source Armory Crate alternative for Asus ROG Zephyrus G14, G15, Flow X13, Flow X16, and other models

A small utility that allows you to do almost everything you could do with Armory Crate but without extra bloat and unnecessary services.

## Advantages 

1. Seamless and automatic GPU switching (without asking you to close all apps, etc)
2. All performance modes can be fully customized (with fan curves and PPTs)
3. Very lightweight and consumes almost no resources

## [Download latest release](https://github.com/seerge/g-helper/releases)

![Screenshot](https://raw.githubusercontent.com/seerge/g-helper/main/screenshot.png)

## NEW (and experimental) features

1. Set Power limits (PPT) for Total (APU + dGPU) and CPU. 
2. Anime matrix control thanks to [Starlight](https://github.com/vddCore/Starlight) + some tweaks from my side

## Main features

1. Switch between built-in system **Performance modes** Silent / Balanced / Turbo and apply default fan curves
2. Switch between Eco / Standard or Ultimate **GPU modes**
3. Change laptop screen refresh rate - 60hz or your maximum (120hz, 144hz, etc depending on the model) with display overdrive (OD)
4. View default fan profiles for every mode and **auto apply** custom ones 
5. Control keyboard backlit animation and colors
6. Set battery charge limit to preserve battery health
7. Monitor CPU temperature, fan speeds and battery discharge rate
8. **Automatically switch to Eco(iGPU)/60hz on battery** and back to Standard(GPU)/120hz modes when plugged
9. Support for FN+F5 to cycle through performance modes (with OSD notification) and FN+F4 to cycle through keyboard animation modes
10. Basic keybindings for M3 and M4 keys
11. Turn cpu turbo boost on/off with one checkbox to keep temps cooler

Designed and developed for Asus Zephyrus G14 2022 (with AMD Radeon iGPU and dGPU). But could and should potentially work for G14 of 2021 and 2020, G15, X FLOW, and other ROG models for relevant and supported features.

To keep auto switching and hotkeys working the app needs to stay in running in the tray. It doesn't consume any resources. 

## Performance Profile switching 

Profiles are **same** as in Armory Crate (as they are stored in bios), including default fan curves

1. Silent (minimal or no fans, 70W PPT total, up to 45W PPT to CPU) + Best power efficiency setting in windows
2. Balanced (balanced fans, 100W PPT total, up to 45W PPT to CPU) + Balanced setting in windows
3. Turbo (intense fans, 125W PPT total, up to 80W PPT to CPU) + Best performance setting in windows

PPTs are shown for G14 2022, for other models PPTs will be different as they are set in bios.

## GPU Modes

1. Eco mode : only low power integrated GPU enabled, iGPU drives built in display
2. Standard mode (Windows Hybrid) : iGPU and dGPU enabled, iGPU drives built in display
3. Ultimate mode: iGPU and dGPU enabled, but dGPU drives built in display (supported only on G14 2022 model)

## How to install

1. Download latest release from [**Releases Page**](https://github.com/seerge/g-helper/releases)
2. Unzip to a folder of your choice
3. Run **GHelper.exe**

App requires [.NET7](https://dotnet.microsoft.com/en-us/download) to be installed. Most probably you already have it. Otherwise you can [download it](https://dotnet.microsoft.com/en-us/download).

I recommend to keep "Asus Optimization Service" running, as it keeps basic laptop hotkeys such as screen or keyboard brightness adjustment working. If you have (or had) MyASUS app installed, that service is most probably still up an running even after MyASUS uninstall. It's part of [Asus System Control Interface](https://www.asus.com/support/FAQ/1047338/). You can install it, and later disable / remove unnecessarily services by running [this bat file](https://raw.githubusercontent.com/seerge/g-helper/main/stop-asus-sv.bat) as admin.

Note: Uses low level ASUS ACPI commands and doesn't require Armory Crate to be installed at all!  Doesn't need administrator privileges to run!

I don't have a Microsoft certificate to sign the app yet, so if you get a warning from Windows Defender on launch (Windows Protected your PC), click More Info -> Run anyway. Alternatively you can compile and run project by yourself using Visual Studio :)

Settings file is stored at %AppData%\GHelper

P.S.: It's not recommended to use an app in combination with Armory Crate, cause they adjust the same settings.

------------------

Debloating helps to save your battery and keep laptop a bit cooler

![Helps to save your battery](https://raw.githubusercontent.com/seerge/g-helper/main/screenshots/screen-5w.png)

---------

**Disclaimers**
"ROG", "TUF", and "Armoury Crate" are trademarked by and belong to AsusTek Computer, Inc. I make no claims to these or any assets belonging to AsusTek Computer and use them purely for informational purposes only.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. MISUSE OF THIS SOFTWARE COULD CAUSE SYSTEM INSTABILITY OR MALFUNCTION.
