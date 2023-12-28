# Power user settings

GENERAL NOTE: "Power user" settings require editing config located at ``%AppData%\GHelper\config.json``. 

- Quit G-Helper
- Make your changes / additions co ``config.json``
- Start G-Helper again

_Make sure to keep json structure (i.e. not to break it with extra or missing commas, etc.) or the app will fail to read it and will just recreate an empty config instead._

### Manual app language setting

By default the app will use your windows language setting. But you can set language manually (if it supported of course)

```
"language" : "en",
``` 
(by replacing "en" with language of your choice)

### Custom windows power plans with each mode

You can manually assign a custom power plan GUID to each mode. 

Format is following : ``"scheme_<mode>" : "GUID" ``
Where ``mode = 0 (balanced), 1 (turbo), 2 (silent)``

Example (for default windows "balanced" power plan):
```
"scheme_0": "381b4222-f694-41f0-9685-ff5bb260df2e",
"scheme_1": "381b4222-f694-41f0-9685-ff5bb260df2e",
"scheme_2": "381b4222-f694-41f0-9685-ff5bb260df2e",
```

### Alternative Activation for XG Mobile 6850XT
If you experience a situation when your XG Mobile doesn't work on full power when Activated. It's possible it needs an "alternative" command to get activated. 
To turn it on, add following line to config :
```
"xgm_special" : 1,
```

### Override UI theme

By default the app would set the UI theme from the "app" theme in Windows setting. You can override it to specific theme, or general windows theme

```
"ui_mode" : "dark",
"ui_mode" : "light",
"ui_mode" : "windows",
```

### Skip keyboard Aura initialisation on startup
By default the app would set the last remembered RGB mode for the keyboard on each launch. To disable it completely 

```
"skip_aura" : 1,
````

### Disable OSD
Disable app's OSD (for performance modes, keyboard backlight, etc.) 
```
"disable_osd": 1,
```

### Disable "Tablet mode" on X13/X16
To disable automatic touchpad toggling when laptop enters / leaves tablet mode
```
"disable_tablet": 1,
```

### Extra Keybindings 
- ``Ctrl + Shift + F5`` / ``Ctrl + Shift + Alt + F5``  - Toggle Performance Modes
- ``Ctrl + Shift + F12`` - Open G-Helper window 
- ``Ctrl + M1 / M2`` - Screen brightness Down / Up
- ``Shift + M1 / M2`` - Backlight brightness Down / Up

If you don't want this bindings to work you can add 
```
"skip_hotkeys":1,
``` 

### Toggle Performance Mode or Toggle App Window key binding

To change binding for Toggle Performance Modes forward / backward to ``Ctrl + Shift + KEY`` / ``Ctrl + Shift + Alt + KEY``
```
"keybind_profile": 116,
``` 

To change binding for Toggle App Window  to ``Ctrl + Shift + KEY``
```
"keybind_app": 123,
``` 

Where 116 is [numerical code for desired key](https://www.oreilly.com/library/view/javascript-dhtml/9780596514082/apb.html). Put 0 to completely disable this binding.


### Higher Maximum GPU Clock / Memory Offsets

By default under the GPU section you can set up to +250/+250 for Core and Memory Clock Offset. To increase this value:
```
  "max_gpu_core": 300,
  "max_gpu_memory": 1500,
```

### Custom hotkey actions

Select ``Custom`` next to appropriate hotkey under ``Extra`` settings and do one of the following:

1. To run any custom application - put a full path to exe into "action" text field, for example:
``C:\Program Files\EA Games\Battlefield 2042\BF2042.exe``

2. To simulate any windows key or key-combination - put appropriate keycode(s) into the "action" field separated by space.
For example ``0x2C`` for ``Print Screen`` or ``0x11 0xA0 0x31`` for ``Ctrl+Shift+1``

Full list of keycodes https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes

![Screenshot 2023-07-17 192155](https://github.com/seerge/g-helper/assets/5920850/e450e124-1589-4787-bce8-7c37ffe72fbd)
