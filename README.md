# G-Helper (For G14, G15, ROG FLOW, and others)

A small utility that allows you do almost everyting you could do with Armory Crate but without extra bloat and unnecessary services.

1. Switch between default **Performance modes** - Silent / Balanced / Turbo and apply default fan curves
2. Switch between Eco / Standard or Ultimate **GPU modes**
3. Change laptop screen refresh rate - 60hz or your maximum (120hh, 144hz, etc depending on the model) with display overdrive (OD)
4. View default fan profiles for every mode and apply custom ones
5. Control keyboard backlit animation and colors
6. Set battery charge limit to preserve battery
7. Monitor CPU temperature, fan speeds and battery discharge rate
8. **Automatically switch to Eco(iGPU)/60hz on battery** and back to Standard(GPU)/120hz modes when plugged
9. Support for M4 key / FN+F5 to cycle through performance modes (with OSD notification) and FN+F4 to cycle through keeyboard animation modes
10. Turn cpu turbo boost on/off with one checkbox to keep temps cooler

Designed and developer for Asus Zephyrus G14 2022 (with AMD Radeon iGPU and dGPU). But could and should potentially work for G14 of 2021 and 2020, G15, X FLOW, and other ROG models for relevant and supported features.

To keep autoswitching and hotkeys work app needs to stay in running in tray. It doesn't consume any resources. 

### [Download latest release](https://github.com/seerge/g-helper/releases)

![Screenshot](https://github.com/seerge/g-helper/blob/main/screenshot.png)

## Performance Profile switching 

Profiles are **same** as in Armory Crate, including default fan curves

1. Silent (minimal or no fans, 70W PPT total, up to 45W PPT to CPU)
2. Balanced (balanced fans, 100W PPT total, up to 45W PPT to CPU)
3. Turbo (intense fans, 125W PPT total, up to 80W PPT to CPU) 

PPTs are shown for G14 2022, for other models PPTs will be different as they are set in bios.

## GPU Modes

1. Eco mode : only low power integrated GPU enabled, iGPU drives built in display
2. Standard mode (Windows Hybrid) : iGPU and dGPU enabled, iGPU drives built in display
3. Ultimate mode: iGPU and dGPU enabled, but dGPU drives built in display (supported only on G14 2022 model)

## Things still missing

1. Anime matrix control
2. Custom bindings for M1-M3 keys

## How to install

1. Download latest release from https://github.com/seerge/g-helper/releases
2. Unzip to a folder of your choice
3. Run **GHelper.exe**

Note: Uses low level ASUS ACPI commands to do switching and doens't require Armory Crate to be isntalled at all. 
Doesn't require administrator privileges to run (anymore)!

I don`t have Microsoft certificate to sign app yet, so if you set a warning from Windows Defender on launch (Windows Protected your PC), click More Info -> Run anyway.

Alternatively you can comile and run project by yourself :)
Settings file is storer at %AppData%\GHelper

P.S.: It's not recommended to use app in combination with Armory Crate, cause they adjust same settings.
Please keep in mind, that if you also run MyASUS app periodically it will also try to adjust same battery charge settings

------------------

Debloating helps to save your battery and keep laptop a bit cooler

![Helps to save your battery](https://raw.githubusercontent.com/seerge/g-helper/main/screenshots/screen-5w.png)
