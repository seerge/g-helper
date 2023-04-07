# G-Helper (GHelper)

[![Github all releases](https://img.shields.io/github/downloads/seerge/g-helper/total.svg)](https://GitHub.com/seerge/g-helper/releases/) [![GitHub release](https://img.shields.io/github/release/seerge/g-helper.svg)](https://GitHub.com/seerge/g-helper/releases/) [![GitHub stars](https://img.shields.io/github/stars/seerge/g-helper.svg?style=social&label=Star)](https://GitHub.com/seerge/g-helper/stargazers/)

## Lightweight Armoury Crate alternative for Asus laptops
### Control tool for ROG Zephyrus G14, G15, Flow X13, Flow X16, TUF, Strix, Scar and other models

A small utility that allows you to do almost everything you could do with Armoury Crate but without extra bloat and unnecessary services.

### :gift: Main advantages 

1. Seamless and automatic GPU switching (without asking you to close all apps, etc)
2. All performance modes can be fully customized (with fan curves and PPTs)
3. Very lightweight and consumes almost no resources, doesn't install any services. Just a single exe to run
4. Simple and clean native UI with easy access to all settings
5. Doesn't need administrator privileges to run!

### [:floppy_disk: Download latest release](https://github.com/seerge/g-helper/releases/latest/download/GHelper.zip)

If you like this app, please [star :star: it on Github](https://github.com/seerge/g-helper) and spread a word about it!

_If you post about the app - please include a link. Thanks._

![Screenshot 2023-03-30 115149](https://user-images.githubusercontent.com/5920850/228799078-bc93148f-6580-4319-b9e6-fbde4d246cd2.png)

### :zap: Main features

1. **Performance modes**: Silent - Balanced - Turbo (built-in, with default fan curves)
2. **GPU modes**: Eco - Standard - Ultimate - Optimized
3. Laptop screen refresh rate 60hz or 120hz (144hz, etc) with display overdrive (OD) and miniled multizone switch
4. Custom fan curve editor, power limits (PPT) and turbo boost selection for every performance mode
5. Anime matrix control thanks to [Starlight](https://github.com/vddCore/Starlight) + some tweaks from my side including animated GIFs
6. Keyboard backlit animation and colors (including sleep animation and support for TUF models)
7. Custom bindings for M3, M4 keys and FN+F5 (performance mode) FN+F4 (keyboard animation modes)
8. Monitor CPU / GPU temperature, fan speeds and battery discharge rate
9. Battery charge limit to preserve battery health

### :gear: Automatic switching when on battery or plugged in
- Performance modes (app remembers last mode used on battery or when plugged)
- Optimized GPU mode - disables dGPU on battery and enables when plugged
- Auto Screen refresh rate (60hz on battery, 120+ hz when plugged)
- Keyboard backlight can be turned off on battery

To keep auto switching and hotkeys working the app needs to stay running in the tray. It doesn't consume any resources. 

### :rocket: Performance Modes

Modes are **same** as in Armoury Crate as they are stored in bios including default fan curves

1. Silent (minimal or no fans, 70W PPT total, up to 45W PPT to CPU) + Best power efficiency setting in windows
2. Balanced (balanced fans, 100W PPT total, up to 45W PPT to CPU) + Balanced setting in windows
3. Turbo (intense fans, 125W PPT total, up to 80W PPT to CPU) + Best performance setting in windows

_PPTs are shown for G14 2022, for other models PPTs will be different as they are set in bios._

![Screenshot 2023-04-06 142234](https://user-images.githubusercontent.com/5920850/230377635-7032a480-3a94-4e35-9468-d8911e3e55ec.png)

### :video_game: GPU Modes

1. Eco mode : only low power integrated GPU enabled, iGPU drives built in display
2. Standard mode (MS Hybrid) : iGPU and dGPU enabled, iGPU drives built in display
3. Ultimate mode: iGPU and dGPU enabled, but dGPU drives built in display (supported only on G14 2022 model)
4. Optimized: disables dGPU on battery (Eco) and enables when plugged (Standard)

## :question: FAQ

### How do I stop the Armory Crate install popup appearing every time I press the M4 / Rog key?
Go to BIOS (F2 on boot), open Advanced Settings and disable "Armory Control Interface". 
If it still appears - delete or move somewhere following file ``C:\Windows\System32\ASUSACCI\ArmouryCrateKeyControl.exe``

### Why is Ultimate GPU mode not available on my laptop?
Ultimate mode is supported (by hardware) only on G14 2022 (and possibly other models from 2022+)

### I can't set Eco mode (disable dGPU) on my G14 2020
Unfortunately 2020 model doesn't support that on hardware level

### Should I apply custom PPTs and fan profiles?
You don't have to, it's purely optional. From my experience built in (in bios) performance modes work well. Limit your power or apply custom fan curves only if you have issues. As soon as you click Apply in the fan + power section bios will be considering the fan profile as "custom"! (no matter if you modified it or not)

### How does G-helper control my fan speeds?
It doesn't. Your bios does (same as in case with armoury). What G-helper can do - is (optionally) set a custom fan profile to current performance mode consisting of 8 pairs of temperature + fan speed % via same endpoint armoury seem to use.

### How do I change fan % to fan RPM?
Click on them

### I don't see a GPU temperature in G-helper
Most probably either you are using Eco / Optimized mode and your dGPU is simply off, or your windows has put the dGPU into sleep (to preserve power). In this situations G-helper won't be able to reach your GPU and get readings

### It says, that app is already running
Please check system tray for a (G) icon. By default windows is keen to hide all icons, so you may need to click ^ to see them all. I would advise to right click on Task Bar select Task Bar Settings -> Other System Tray icons -> Mark G-Helper to be always ON.

### App doesn't start / or crashes, what should I do ?
Open "Event Viewer" from start menu, go to Windows Logs -> Application and check for recent Errors mentioning G-Helper. If you see one - please post a [new issue](https://github.com/seerge/g-helper/issues) with all details from this error.

### Can I use MyASUS app along with G-Helper?
Sure, you can! The only problem is that MyASUS may override the battery charge limit that you set before. My advice in such a situation would be to set the same limit (i.e. 80%) in both MyASUS and G-Helper.

### How do I uninstall G-helper?
G-helper is a single exe, and it doesn't install anything in the system. To remove it - you can simply delete exe :) If you have applied any custom fan profiles or PPTs - before removing I would recommend selecting your favorite performance mode (for example balanced) and clicking "Factory defaults" under Fans + Power.

----------------------------

### How to install

1. Download latest release from [**Releases Page**](https://github.com/seerge/g-helper/releases)
2. Unzip to a folder of your choice
3. Run **GHelper.exe**

### Dependencies & Requirements

- Microsoft [.NET7](https://dotnet.microsoft.com/en-us/download). Most probably you already have it. Otherwise you can [download it](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-7.0.202-windows-x64-installer) from the official website.

- [Asus System Control Interface](https://dlcdnets.asus.com/pub/ASUS/nb/Image/CustomComponent/ASUSSystemControlInterfaceV3/ASUSSystemControlInterfaceV3.exe). If you have or had MyASUS app installed this "driver" probably still in place (even after MyASUS uninstalls). Alternatively - you can download and install it

- Optionally(!) you can disable / remove unnecessary services by running [this debloat bat file](https://raw.githubusercontent.com/seerge/g-helper/main/debloat.bat) as admin. To restore services - run [this bloat bat file](https://raw.githubusercontent.com/seerge/g-helper/main/bloat.bat) instead.

- I recommend keeping "Asus Optimization Service" running, as it keeps basic laptop hotkeys such as screen or keyboard brightness adjustment working.

- It's not recommended to use an app in combination with Armoury Crate services, because they adjust the same settings. You can [uninstall it using it's own uninstall tool](https://dlcdnets.asus.com/pub/ASUS/mb/14Utilities/Armoury_Crate_Uninstall_Tool.zip?model=armoury%20crate). Just in case, you can always install it back later.

-------------------------------

Designed and developed for Asus Zephyrus G14 2022 (with AMD Radeon iGPU and dGPU). But could and should potentially work for G14 of 2021 and 2020, G15, X FLOW, and other ROG models for relevant and supported features.

I don't have a Microsoft certificate to sign the app yet, so if you get a warning from Windows Defender on launch (Windows Protected your PC), click More Info -> Run anyway. Alternatively you can compile and run project by yourself using Visual Studio :)

Settings file is stored at ``%AppData%\GHelper``

------------------

Debloating helps to save your battery and keep laptop a bit cooler

![Helps to save your battery](https://raw.githubusercontent.com/seerge/g-helper/main/docs/screenshots/screen-5w.png)

---------

**Disclaimers**
"ROG", "TUF", and "Armoury Crate" are trademarked by and belong to AsusTek Computer, Inc. I make no claims to these or any assets belonging to AsusTek Computer and use them purely for informational purposes only.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. MISUSE OF THIS SOFTWARE COULD CAUSE SYSTEM INSTABILITY OR MALFUNCTION.
