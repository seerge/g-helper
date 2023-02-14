# G14-Helper

Designed for Asus Zephyrus G14 2022 (with AMD Radeon iGPU and dGPU). But could and should potentially work for G14 of 2021 and 2020, G15, X FLOW, and other ROG models.

A tiny system tray utility that allows you set performance and GPU profiles for your laptop. Same as ASUS Armory Crate does but without it completely!. 

## Performance Profile switching 

Profiles are **same** as in Armory Crate, including default fan curves

1. Silent (minimal or no fans, 45W PPT to CPU)
2. Balanced (balanced fans, up to 45W PPT to CPU)
3. Turbo (intense fans, 125W PPT total, up to 80W PPT to CPU) 

## GPU Mode switching

1. Eco mode : only low power iGPU (Radeon 680u) enabled, iGPU drives built in display
2. Standard mode (Windows Hybrid) : iGPU and dGPU (Radeon 6700s/6800s) enabled, iGPU drives built in display
3. Ultimate mode: iGPU and dGPU enabled, but dGPU drives built in display

## Extras

1. **Maximum battery charge rate** limit (60% / 80% / 100%) to preserve your battery
2. CPU and GPU relative fan speed monitoring 
3. Laptop screen refresh adjustments for power saving (60hz) and gaming (120hz)
4. Laptop screen panel overdrive switch
5. Start with windows (optional)


## How to install

1. Download latest release from https://github.com/seerge/g14-helper/releases
2. Unzip to a folder of your choice
3. Run **g14-helper.exe**

_App is written in Powershell and wrapped into executable with https://github.com/MScholtes/PS2EXE . 
If you don't trust exe - you can always run ghelper.ps1 script by yourself directly._


Note: Uses low level ASUS WMI commands to do switching and doens't require Armory Crate to be isntalled at all. 
Therefore requires Administrator priveledges on Windows to run.

![Screenshot](https://github.com/seerge/g14-helper/blob/main/g14-helper.png)



Settings and basic log are located in APPDATA\LOCAL

P.S.: It's not recommended to use app in combination with Armory Crate, cause they adjust same settings.
Please keep in mind, that if you also run MyASUS app periodically it will also try to adjust same battery charge settings


--------
![Ultimate Mode](https://github.com/seerge/g14-helper/blob/main/ultimate.png)
