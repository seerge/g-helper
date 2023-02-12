# G14-helper

Designed for Asus Zephyrus G14 2022 (with AMD Radeon iGPU and dGPU). But could potentially work for G14 of 2021 and 2020, G15, X FLOW, and other ROG models.

A small windows system tray utility that allows you to switch between 3 main GPU modes (mirroring ones from ASUS Armory Crate). Uses  ASUS WMI commands to do switching and doens't require Armory Crate to be isntalled at all. Requires Administrator priveledges on Windows to run.

![Screenshot](https://github.com/seerge/g14-helper/blob/main/g14-helper.png)

1. Eco mode : only low power iGPU (Radeon 680u) enabled, iGPU drives built in display
2. Standard mode (Windows Hybrid) : iGPU and dGPU (Radeon 6700s/6800s) enabled, iGPU drives built in display
3. Ultimate mode: iGPU and dGPU enabled, but dGPU drives built in display

UPDATE: February 12, 2023

Swithing of performance profiles (including default fan curves) is also available! Swhiching happens via same ASUS WMI low leven commands. Profiles are same as in Armory Crate.

![Ultimate Mode](https://github.com/seerge/g14-helper/blob/main/ultimate.png)
