# G14-Helper

A tiny system tray utility that allows you set performance and GPU profiles for your laptop. Same as ASUS Armory Crate does but without it completely!. 

Designed for Asus Zephyrus G14 2022 (with AMD Radeon iGPU and dGPU). But could and should potentially work for G14 of 2021 and 2020, G15, X FLOW, and other ROG models.

![Screenshot](https://github.com/seerge/g14-helper/blob/main/screenshot.png)

## Performance Profile switching 

Profiles are **same** as in Armory Crate, including default fan curves

1. Silent (minimal or no fans, 70W PPT total, up to 45W PPT to CPU)
2. Balanced (balanced fans, 100W PPT total, up to 45W PPT to CPU)
3. Turbo (intense fans, 125W PPT total, up to 80W PPT to CPU) 

## GPU Mode switching

1. Eco mode : only low power iGPU (Radeon 680u) enabled, iGPU drives built in display
2. Standard mode (Windows Hybrid) : iGPU and dGPU (Radeon 6700s/6800s) enabled, iGPU drives built in display
3. Ultimate mode: iGPU and dGPU enabled, but dGPU drives built in display

## Extras

1. Keyboard backlight control (basic aura modes and colors)
2. **Maximum battery charge rate** limit (60% / 80% / 100%) to preserve your battery
3. CPU and GPU relative fan speed monitoring 
4. Automatic switching of Standard/Eco GPU modes when laptop is plugged / unplugged!
5. FN+F5 an M4 (Rog) keys cycle through Performance modes
6. Screen resolution and display overdrive switching
7. CPU turbo boost switching

## Things still missing

1. Custom fan profiles
2. Anime matrix control

## How to install

1. Download latest release from https://github.com/seerge/g14-helper/releases
2. Unzip to a folder of your choice
3. Run **GHelper.exe**

Note: Uses low level ASUS WMI commands to do switching and doens't require Armory Crate to be isntalled at all. 
Therefore requires Administrator priveledges on Windows to run.

I don`t have Microsoft certificate to sign app yet, so if you set a warning from Windows Defender on launch (Windows Protected your PC), click More Info -> Run anyway.

Alternatively you can comile and run project by yourself :)

Settings file is storer at %AppData%\GHelper

P.S.: It's not recommended to use app in combination with Armory Crate, cause they adjust same settings.
Please keep in mind, that if you also run MyASUS app periodically it will also try to adjust same battery charge settings
