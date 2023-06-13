# G-Helper (GHelper)
[![Github all releases](https://img.shields.io/github/downloads/seerge/g-helper/total.svg)](https://GitHub.com/seerge/g-helper/releases/) [![GitHub release](https://img.shields.io/github/release/seerge/g-helper.svg)](https://GitHub.com/seerge/g-helper/releases/) [![GitHub stars](https://img.shields.io/github/stars/seerge/g-helper.svg?style=social&label=Star)](https://GitHub.com/seerge/g-helper/stargazers/)

[![United24](https://raw.githubusercontent.com/seerge/g-helper/main/docs/ua.png)](https://u24.gov.ua/)

Language: English | [中文](https://github.com/seerge/g-helper/blob/main/docs/README.zh-CN.md)

## Control tool for Asus laptops

Lightweight Armoury Crate alternative for Asus laptops. A small utility that allows you to do almost everything you could do with Armoury Crate but without extra bloat and unnecessary services. Works on all popular models, such as ROG Zephyrus G14, G15, G16, M16, Flow X13, Flow X16, TUF, Strix, Scar, ProArt and many more! Feel free to try :)

## :gift: Main advantages 

1. Seamless and automatic GPU switching (without asking you to close all apps, etc)
2. All performance modes can be fully customized (with fan curves and PPTs)
3. Very lightweight and consumes almost no resources, doesn't install any services. Just a single exe to run
4. Simple and clean native UI with easy access to all settings
5. FN-Lock
6. Doesn't need administrator privileges to run (*)

## [:floppy_disk: Download App](https://github.com/seerge/g-helper/releases/latest/download/GHelper.zip)

If you like this app, please [star :star: it on Github](https://github.com/seerge/g-helper) and spread a word about it!
### [:euro:  Donate EUR](https://www.paypal.com/donate/?hosted_button_id=4HMSHS4EBQWTA) | [💵 Donate USD](https://www.paypal.com/donate/?hosted_button_id=SRM6QUX6ACXDY) | [:credit_card: Donate via Stripe](https://buy.stripe.com/00gaFJ9Lf79v7WobII)

_If you post about the app - please include a link. Thanks._

![Gihhub](https://github.com/seerge/g-helper/assets/5920850/4d98465a-63a5-4498-ae14-afb3e67e7e82)

### :zap: Main features

1. **Performance modes**: Silent - Balanced - Turbo (built-in, with default fan curves)
2. **GPU modes**: Eco - Standard - Ultimate - Optimized
3. Laptop screen refresh rate 60hz or 120hz (144hz, etc) with display overdrive (OD) and miniled multizone switch
4. Custom fan curve editor, power limits (PPT) and turbo boost selection for every performance mode
5. Anime matrix control thanks to [Starlight](https://github.com/vddCore/Starlight) + some tweaks from my side including animated GIFs, clock and audio visualizer
6. Keyboard backlit animation and colors (including sleep animation and support for TUF models)
7. All basic and custom Keyboard hotkeys (M-keys, FN+X keys)
8. Monitor CPU / GPU temperature, fan speeds and battery discharge rate
9. Battery charge limit to preserve battery health
10. NVidia GPU overclocking
11. XG Mobile Control

![Screenshot 2023-04-13 190951](https://user-images.githubusercontent.com/5920850/231859391-c4963af4-491c-4523-95d4-0bdcfd7cfd6f.png)

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

### :video_game: GPU Modes

1. Eco mode : only low power integrated GPU enabled, iGPU drives built in display
2. Standard mode (MS Hybrid) : iGPU and dGPU enabled, iGPU drives built in display
3. Ultimate mode: iGPU and dGPU enabled, but dGPU drives built in display (supported only on G14 2022 model)
4. Optimized: disables dGPU on battery (Eco) and enables when plugged (Standard)

![Screenshot 2023-05-07 182519](https://user-images.githubusercontent.com/5920850/236697890-26938ac4-8840-4fed-a7b1-9a7b839fb865.png)

## :question: FAQ

#### How do I stop the Armory Crate install popup appearing every time I press the M4 / Rog key?
Delete or move somewhere following file ``C:\Windows\System32\ASUSACCI\ArmouryCrateKeyControl.exe``.
If it still appears - Go to BIOS (F2 on boot), open Advanced Settings and disable "Armory Control Interface". 

#### Why is Ultimate GPU mode not available on my laptop?
Ultimate mode is supported (by hardware) only on 2022+ models

####  I don't see GPU modes section 
Some older models (for example G14 2020) don't support disabling GPU on hardware level, therefore GPU section makes no sense for them and will be hidden

#### Should I apply custom power limits (PPT) and fan profiles?
You don't have to, it's purely optional. From my experience built in (in bios) performance modes work well. Limit your power or apply custom fan curves only if you have issues. As soon as you click Apply in the fan + power section bios will be considering the fan profile as "custom"! (no matter if you modified it or not)

#### How does G-helper control my fan speeds?
It doesn't. Your bios does (same as in case with armoury). What G-helper can do - is (optionally) set a custom fan profile to current performance mode consisting of 8 pairs of temperature + fan speed % via same endpoint armoury seem to use.

#### How do I change fan % to fan RPM?
Click on them

#### I don't see a GPU temperature in G-helper
Most probably either you are using Eco / Optimized mode and your dGPU is simply off, or your windows has put the dGPU into sleep (to preserve power). In this situations G-helper won't be able to reach your GPU and get readings

#### It says, that app is already running
Please check system tray for a (G) icon. By default windows is keen to hide all icons, so you may need to click ^ to see them all. I would advise to right click on Task Bar select Task Bar Settings -> Other System Tray icons -> Mark G-Helper to be always ON.

#### App crash or doesn't work properly what should I do ?
Open "Event Viewer" from start menu, go to Windows Logs -> Application and check for recent Errors mentioning G-Helper. If you see one - please post a [new issue](https://github.com/seerge/g-helper/issues) with all details from this error.

#### Battery charge limiter is not working
Open application log.text from ``%AppData%\GHelper``. If you see something like ``BatteryLimit = 60 : OK`` there with your selected limit - App has done everything it could to set a limit. It could be that MyASUS or other Asus services are overwriting this limit after. You may want to stop them by clicking "Stop" in Asus Services section (under Extra).

#### Can I use MyASUS app along with G-Helper?
You can, the only problem is that MyASUS may override the battery charge limit that you set before. My advice in such a situation would be to set the same limit (i.e. 80%) in both MyASUS and G-Helper.

#### How do I set Mute Microphone to M3?
If you have Asus Optimization Service running, it's controlled by that service (therefore G-helper doesn't interfere and doesn't touch this function). Alternatively you can stop that service - and you can bind M3 to anything you want.

#### How do I set different "Visual styles"?
Personally, i'm not a big fan of them, as they make colors very inaccurate. But if you want so - you can adjust display colors using either Nvidia Control panel or AMD Adrenaline (appropriate display sections). If you really want you can also use [own ASUS utility from MS Store](https://apps.microsoft.com/store/detail/gamevisual/9P4K1LFTXSH8?hl=nl-nl&gl=nl&rtc=1)

#### Can I overclock Nvidia GPU core / memory? 
Make sure that your dGPU is enabled (i.e. it's not in Eco mode). Open Fans + Power section and adjust core / memory clock offsets. They work same as in armoury's manual mode. Please keep in mind that (unfortunately) you need admin permissions for that, and app will ask you for them. (*)

#### Windows defender marks app download as malware / virus
False positives from Windows Defender (or any other similar system that uses machine learning for detection) is possible as application is not digitally signed with a certificate. You can always download a version below or compile app by yourself. All application sources are open and can be monitored from A to Z :)

#### Where can I find app settings or logs ?
You can find them under ``%AppData%\GHelper`` folder. Please include them when posting a new bug-report or issue.

#### How do I uninstall G-helper?
G-helper is a single exe, and it doesn't install anything in the system. To remove it - you can simply delete exe :) If you have applied any custom fan profiles or PPTs - before removing I would recommend selecting your favorite performance mode (for example balanced) and clicking "Factory defaults" under Fans + Power.

#### What is G-helper ?
G-Helper is a lightweight Armoury Crate alternative for Asus laptops. A small utility that allows you to do almost everything you could do with Armoury Crate but without extra bloat and unnecessary services.

-----------------------------

## :euro: [Support Project](https://www.paypal.com/donate/?hosted_button_id=4HMSHS4EBQWTA)
#### If you like the app you can make a Donation 

| [Paypal in EUR](https://www.paypal.com/donate/?hosted_button_id=4HMSHS4EBQWTA) | [Paypal in USD](https://www.paypal.com/donate/?hosted_button_id=SRM6QUX6ACXDY) |
| ------------------------------------------ | ----------------------------------------------- |
| [![QR Code](https://user-images.githubusercontent.com/5920850/233658717-0441494d-fede-4a2c-b4f2-4b16a184a69a.png)](https://www.paypal.com/donate/?hosted_button_id=4HMSHS4EBQWTA) | [![QR Code](https://github-production-user-asset-6210df.s3.amazonaws.com/5920850/239492811-b487e89a-3df6-42ea-bdb8-24c455ab2310.png)](https://www.paypal.com/donate/?hosted_button_id=SRM6QUX6ACXDY) |

----------------

### How to install

1. Download latest release from [**Releases Page**](https://github.com/seerge/g-helper/releases)
2. Unzip to a folder of your choice _(don't run exe from zip directly, as windows will put it into temp folder and delete after)_
3. Run **GHelper.exe**

### Requirements (mandatory)

- [Microsoft .NET7](https://dotnet.microsoft.com/en-us/download). Most probably you already have it. Otherwise [download it](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-7.0.202-windows-x64-installer) from the official website.

- [Asus System Control Interface v3+](https://dlcdnets.asus.com/pub/ASUS/nb/Image/CustomComponent/ASUSSystemControlInterfaceV3/ASUSSystemControlInterfaceV3.exe). This "driver" from asus should be installed automatically by windows update or along other asus apps. If it's not the case by some reason - you can download and install it manually.

### Recommendations (optional)

- It's **not recommended** to use an app in combination with Armoury Crate services, because they adjust the same settings. You can [uninstall it using AC own uninstall tool](https://dlcdnets.asus.com/pub/ASUS/mb/14Utilities/Armoury_Crate_Uninstall_Tool.zip?model=armoury%20crate). Just in case, you can always install it back later.

- It's **not recommended** to have "ASUS Smart Display Control" app running, as it will try to change refresh rates and fight with g-helper for the same function. You can safely uninstall it.

- You can stop / disable unnecessary services: Go to **Extra** in the app, and press "Stop" in Asus Services section (former **[debloat.bat](https://raw.githubusercontent.com/seerge/g-helper/main/debloat.bat)**). To start / enable services back - click "Start" instead (former **[bloat.bat](https://raw.githubusercontent.com/seerge/g-helper/main/bloat.bat)**)

- It is **strongly recommended** to run app with windows default "balanced" power plan
![Screenshot 2023-06-09 153453](https://github.com/seerge/g-helper/assets/5920850/d1d05c53-a0bd-4207-b23a-244653f3e7df)

-------------------------------

_Designed and developed for Asus Zephyrus G14 2022 (with AMD Radeon iGPU and dGPU). But could and should potentially work for G14 of 2021 and 2020, G15, X FLOW, and other ROG models for relevant and supported features._

I don't have a Microsoft certificate to sign the app yet, so if you get a warning from Windows Defender on launch (Windows Protected your PC), click More Info -> Run anyway. Alternatively you can compile and run project by yourself using Visual Studio :)

------------------

## Power user settings

### Manual app language setting

By default app will use your windows language setting. But you can set language manually (if it supported of course)

Add following line to ``%AppData%\GHelper\config.json`` : ``"language" : "en"`` (by replacing "en" with language of your choice)

### Custom windows power plans with each mode

In ``%AppData%\GHelper\config.json`` you can manually add custom power plan (or power mode) GUID. It can be either "real" power plan that can be switched or "overlay" power plan like the ones g-helper sets by default.

Format is following : ``"scheme_<mode>" : "GUID" ``
Where ``mode = 0 (balanced), 1 (turbo), 2 (silent)``

Default behaviour is :
```
"scheme_0": "00000000-0000-0000-0000-000000000000",
"scheme_1": "ded574b5-45a0-4f42-8737-46345c09c238",
"scheme_2": "961cc777-2547-4f9d-8174-7d86181b8a7a",
```

Make sure to keep json structure (i.e. not to break it with extra or missing comas, etc) - or app will fail to read it and will just recreate a new config instead.

### Custom hotkey actions

App supports custom actions for M3, M4 and FN+F4 hotkeys. To set them select "Custom" next to appropriate hotkey and do one of the following:

1. To run any custom application - put a full path to exe into "action" text field, for example:
``C:\Program Files\EA Games\Battlefield 2042\BF2042.exe``

2. To simulate any windows key - put appropriate keycode into "action" field, for example ``0x2C`` for Print screen. 
Full list of keycodes https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes

![Screenshot 2023-06-04 232848](https://github.com/seerge/g-helper/assets/5920850/3db6f058-af2b-4220-9ac9-3b812ab09903)

### Workaround for [bugged bios on G15](https://github.com/seerge/g-helper/issues/253) when external monitor is connected

Some people reported that on G15 model bios can be bugged and either not turn on or not turn off fans, when you set custom fan curve (in Armoury or g-helper, or any other app)
I have added a very optional workaround as it was asked, on launch app will try to disable and re-enable dGPU when external monitor is connected, that seems to wake up fans :)

To enable this custom workaround you need to add an extra line in config.json (under ``%AppData%\GHelper``) 
``gpu_reenable : 1,``

### Keybinding to toggle performance modes (on external keyboards)

By default app will toggle performance modes with Ctr+Shift+F5. You can change this binding by adding ``"keybind_profile": 116`` in config.json (under ``%AppData%\GHelper``), where 116 is [numerical code for desired key](https://www.oreilly.com/library/view/javascript-dhtml/9780596514082/apb.html). Put 0 to completely disable this binding.

### Keybinding to open G-helper from external keyboards
Ctrl + Shift + F12

------------

**Disclaimers**
"ROG", "TUF", and "Armoury Crate" are trademarked by and belong to AsusTek Computer, Inc. I make no claims to these or any assets belonging to AsusTek Computer and use them purely for informational purposes only.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. MISUSE OF THIS SOFTWARE COULD CAUSE SYSTEM INSTABILITY OR MALFUNCTION.
