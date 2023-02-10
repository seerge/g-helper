# G14-helper

Designed for Asus Zephyrus G14 2022 (with AMD Radeon iGPU and dGPU). But could potentially work for G14 of 2021 and 2020.

A small windows system tray utility that allows you to switch between 3 main GPU modes (mirroring ones from ASUS Armory Crate). Uses proprietary ASUS WMI commands to do switching. Therefore requires Administrator priveledges on Windows to run.

![Screenshot](https://github.com/seerge/g14-helper/blob/main/g14-gpu.png)

1. Eco mode : only low power iGPU (Radeon 680u) enabled, iGPU drives built in display
2. Standard mode (Windows Hybrid) : iGPU and dGPU (Radeon 6700s/6800s) enabled, iGPU drives built in display
3. Ultimate mode: iGPU and dGPU enabled, but dGPU drives built in display

