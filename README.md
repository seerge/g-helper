# G14-helper

Designed for Asus Zephyrus G14 2022 (with AMD Radeon iGPU and dGPU). But could potentially work for G14 of 2021 and 2020, G15, X FLOW, and other ROG models.

A small windows system tray utility that allows you to switch between 3 main GPU modes (mirroring ones from ASUS Armory Crate). Uses  ASUS WMI commands to do switching and doens't require Armory Crate to be isntalled at all. Requires Administrator priveledges on Windows to run.

![Screenshot](https://github.com/seerge/g14-helper/blob/main/g14-helper.png)

1. Eco mode : only low power iGPU (Radeon 680u) enabled, iGPU drives built in display
2. Standard mode (Windows Hybrid) : iGPU and dGPU (Radeon 6700s/6800s) enabled, iGPU drives built in display
3. Ultimate mode: iGPU and dGPU enabled, but dGPU drives built in display

UPDATE: February 12, 2023

1. Added switching of **Performance profiles (including default fan curves)** is also available! switching happens via same ASUS WMI low level commands. Profiles are same as in Armory Crate - Silent, Balanced and Turbo.

2. App now monitors (once every 3 seconds) fan speeds and shows them in the menu

3. App will save settings and write a basic log of it's actions to APPDATA\LOCAL directory

Extra: **autostart.ps1** script to schedule autostart of the app (with admin privileges) on every user logon for convenience. Later will be integrated into main app.

--------
![Ultimate Mode](https://github.com/seerge/g14-helper/blob/main/ultimate.png)
