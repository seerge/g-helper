# G-Helper - 华硕笔记本的轻量级控制工具

小巧轻量的 Armoury Crate（奥创控制中心）替代品，为华硕笔记本提供几乎相同的功能，却拥有小得多的资源占用。兼容所有主流型号，例如 ROG 幻14 / 幻15 / 幻16、幻X（Flow 翻转系列）、幻16 Duo 双屏、天选（飞行堡垒）系列、枪神 / 魔霸系列、ProArt 创、无畏（Vivobook）、灵耀（Zenbook）、ExpertBook、ROG Ally 与 Ally X 等众多机型！

[![GitHub release](https://g-helper.com/badge/release.svg)](https://GitHub.com/seerge/g-helper/releases/)
[![Downloads](https://g-helper.com/badge/downloads.svg)](https://GitHub.com/seerge/g-helper/releases/)
[![SLSA3](https://g-helper.com/badge/slsa.svg)](https://github.com/seerge/g-helper/attestations)
[![English](https://g-helper.com/badge/lang-en.svg)](https://github.com/seerge/g-helper#readme)
[![日本語](https://g-helper.com/badge/lang-ja.svg)](https://github.com/seerge/g-helper/blob/main/docs/README.ja-JP.md)
[![Čeština](https://g-helper.com/badge/lang-cs.svg)](https://github.com/seerge/g-helper/blob/main/docs/README.cs-CZ.md)
[![Español](https://g-helper.com/badge/lang-es.svg)](https://github.com/seerge/g-helper/blob/main/docs/README.es-ES.md)
[![한국어](https://g-helper.com/badge/lang-ko.svg)](https://github.com/seerge/g-helper/blob/main/docs/README.ko-KR.md)
<a href="https://github.com/seerge/g-helper/releases/latest/download/GHelper.exe" alt="G-Helper - 华硕笔记本的轻量级控制工具"><img width="1280" alt="G-Helper - 华硕笔记本的轻量级控制工具" src="https://g-helper.com/hero/zh.png" /></a>


<a href="https://github.com/seerge/g-helper/releases/latest/download/GHelper.exe"><img width="250" alt="下载" src="https://g-helper.com/button/download.svg?lang=zh" /></a> &nbsp; <a href="https://g-helper.com/support"><img width="250" alt="支持项目" src="https://g-helper.com/button/donate.svg?lang=zh" /></a>

**⭐ 喜欢这个应用？欢迎帮忙宣传推广。**

- [常见问题（FAQ）](https://g-helper.com/zh/faq/)
- [安装与要求](https://g-helper.com/zh/requirements/)
- [故障排除](https://g-helper.com/zh/troubleshooting/)
- [高级用户设置](https://g-helper.com/zh/power-user-settings/)


[![United24](https://github.com/user-attachments/assets/aa9444e3-9daa-4b88-a473-7a7f855e3a07)](https://u24.gov.ua/)

## :loudspeaker: 视频测评与指南
| [![测无止境](https://g-helper.com/images/reviews/bili-BV1cQbBzPE6x.jpg)](https://www.bilibili.com/video/BV1cQbBzPE6x/) | [![叠甲者抗揍也](https://g-helper.com/images/reviews/bili-BV15h4y127ub.jpg)](https://www.bilibili.com/video/BV15h4y127ub/) | [![方氺](https://g-helper.com/images/reviews/bili-BV1uFHpeZEY4.jpg)](https://www.bilibili.com/video/BV1uFHpeZEY4/) |
| ----------------- | ---------------- | ---------------- | 
| [测无止境](https://www.bilibili.com/video/BV1cQbBzPE6x/) | [叠甲者抗揍也](https://www.bilibili.com/video/BV15h4y127ub/) | [方氺](https://www.bilibili.com/video/BV1uFHpeZEY4/) | 

## 📰 媒体报道
1. [Digital Trends](https://www.digitaltrends.com/computing/g-helper-armoury-crate-alternative/)
2. [Notebookcheck](https://www.notebookcheck.net/Unbloated-G-Helper-The-best-open-source-alternative-to-Asus-Armoury-Crate-Part-2.1213486.0.html)
3. [知乎](https://zhuanlan.zhihu.com/p/663917638)
4. [Chiphell](https://www.chiphell.com/thread-2719738-1-1.html)

## :gift: 主要优点 

1. 无缝且自动的 GPU 切换
2. 所有性能模式均可通过功耗限制和风扇曲线完全自定义
3. 轻量化。只需运行单个 exe 文件，不会在系统中安装任何东西。
4. 简洁清爽的原生界面，可轻松访问所有设置
5. FN 锁定和自定义热键

<img width="1960" alt="G-Helper Screenshot" src="https://github.com/user-attachments/assets/9376fe90-fbb6-420a-abbd-0186189665e1" />

### :zap: 功能

1. 性能模式：静音 - 平衡 - 增强（内置，附带默认风扇曲线）
2. 显卡模式：集显 - 标准 - 独显直连 - 自动优化
3. 屏幕刷新率控制，支持显示 Overdrive（屏幕过驱）
4. 为每个性能模式提供自定义风扇曲线编辑器、功耗限制和睿频（Turbo Boost）选择
5. Anime Matrix 光显矩阵或 Slash Lighting 光效控制，包括动画 GIF、时钟和音频可视化
6. RGB 背光动画模式与颜色
7. 热键处理
8. 监控 CPU 和 GPU 温度、风扇转速及电池状态
9. 电池充电上限设置，保护电池健康
10. NVIDIA GPU 超频与降压
11. XG Mobile 控制
12. AMD CPU 降压与温度限制
13. BIOS 与驱动更新
14. 华硕鼠标控制
15. Mini-LED 多区域调光开关
16. 无闪烁调光与显示风格（Visual Modes）

### :gear: 自动化
- 使用电池或接入电源时自动切换性能模式
- 自动优化 GPU 模式 - 电池供电时禁用独显，接入电源时启用
- 自动屏幕刷新率（电池供电时 60Hz，接入电源时为最高刷新率）
- 电池供电或接入电源时的键盘背光超时

### :rocket: 性能模式
>[!NOTE]
>所有模式连同默认风扇曲线和功耗限制都**内置于 BIOS** 中，并且与 Armoury Crate 中的**完全相同**。

每个默认 BIOS 模式都与对应的 [Windows 电源模式](https://support.microsoft.com/en-us/windows/change-the-power-mode-for-your-windows-pc-c2aff038-22c9-f46d-5ca0-78696fdf2de8) 配对。你可以在 ``风扇 + 电源`` 中调整该设置。

1. BIOS 中的 `静音` + `最佳能效` 电源模式
2. BIOS 中的 `平衡`（接入电源时为 Performance）+ `平衡` 电源模式
3. BIOS 中的 `增强` + `最佳性能` 电源模式

<img width="2330" alt="Performance Modes" src="https://github.com/user-attachments/assets/254d33eb-2af1-4715-a097-ed8678c7e9db" />

### :video_game: GPU 调校

在每个模式下，都可以根据具体需求调校独立显卡，包括核心和显存频率偏移、GPU 功耗、Dynamic Boost、温度限制等
<img width="2354" alt="GPU Tweaking" src="https://github.com/user-attachments/assets/48ce19a6-d149-40ed-9145-23f974c513f7" />

### 💻 显卡模式

1. **集显（Eco）**：仅启用低功耗的集成显卡，由核显驱动内置屏幕
2. **标准（Standard，MS Hybrid）**：同时启用核显和独显，由核显驱动内置屏幕
3. **独显直连（Ultimate）**：同时启用核显和独显，但由独显驱动内置屏幕（2022 年及以后的机型支持）
4. **自动优化（Optimized）**：电池供电时禁用独显（集显），接入电源时启用（标准）

<img width="1134" alt="显卡模式" src="https://raw.githubusercontent.com/seerge/g-helper/main/docs/gpu-modes-zh.svg" />

### 💿 驱动更新
本应用内置自动 BIOS 和驱动更新检查器，直接从华硕官网针对你的具体型号拉取更新，并在有新下载可用时予以提示。`更新` 部分中的所有链接均指向华硕官方下载。
<img width="2302" alt="Driver Updates" src="https://github.com/user-attachments/assets/303dfce9-fbbd-4d15-b6d7-f21e7c2c59a4" />

### 🎮 性能指标叠加层（Overlay）
内置的游戏内叠加层（OSD），可在游戏画面上直接显示实时 **FPS、CPU / GPU 温度、占用率和功耗** —— 无需额外工具。支持 DX10+ 游戏。在使用独占全屏模式的旧游戏上可能不显示，请将游戏设置为窗口化 / 无边框全屏模式。

- ``Ctrl + Shift + Alt + O`` - 开启 / 关闭游戏内叠加层
- ``Ctrl + Shift + Alt + 拖动鼠标`` - 移动叠加层
- ``Ctrl + Shift + Alt + 单击鼠标`` - 切换模式（精简 / 默认 / 完整）
- ``Ctrl + Shift + Alt + 滚轮`` - 调整叠加层大小
- ``Ctrl + Shift + Alt + 滚轮单击`` - 重置叠加层大小

<img width="1690" alt="性能指标叠加层" src="https://github.com/user-attachments/assets/0752a704-e9a7-4e27-8587-39967f625fae" />
<img width="3840" alt="游戏中的性能指标叠加层" src="https://github.com/user-attachments/assets/8b063f80-d508-41e0-9978-154bc936d451" />

### :mouse: 华硕鼠标及其他外设支持

<details>
<summary><a href="https://github.com/seerge/g-helper/discussions/900">目前支持的型号</a>（点击展开）</summary>

- ROG Chakram X
- ROG Chakram Core
- ROG Gladius II and Gladius II Origin
- ROG Gladius II Wireless
- ROG Gladius III
- ROG Gladius III Wireless
- ROG Harpe Ace Extreme
- ROG Harpe Ace Aim Lab Edition
- ROG Harpe Ace Mini
- ROG Harpe II Ace
- ROG Keris Wireless
- ROG Keris II Ace
- ROG Keris Wireless Aimpoint
- ROG Strix Carry
- ROG Strix III Gladius III Aimpoint Wireless
- ROG Strix Impact III
- ROG Strix Impact III Wireless
- ROG Spatha X
- ROG Strix Impact II Wireless
- ROG Pugio
- ROG Pugio II
- TUF Gaming M4 Wireless
- TUF Gaming M3
- TUF Gaming M3 Gen II
- TUF Gaming M4 AIR
- TUF Gaming M5
- TX Gaming Mini

</details>

特别感谢 [@IceStormNG](https://github.com/IceStormNG) 👑 的贡献与研究（！）。

<img width="2448" alt="Mouse and other peripherals" src="https://github.com/user-attachments/assets/fe2a766b-f514-42e9-8dff-4bcc915364d4" />

### ⌨️ 按键绑定

- ``Fn + F5 / Fn + Shift + F5`` - 向前 / 向后切换性能模式
- ``Ctrl + Shift + F5 / Ctrl + Shift + Alt + F5`` - 向前 / 向后切换性能模式
- ``Ctrl + Shift + F12`` - 打开 G-Helper 窗口
- ``Ctrl + M1 / M2`` - 屏幕亮度 调低 / 调高
- ``Shift + M1 / M2`` - 背光亮度 调低 / 调高
- ``Fn + V`` - 显示风格（Visual Modes）
- ``Fn + C`` / ``Fn + Esc`` - Fn 锁定
- ``Fn + Ctrl +  F7 / F8`` / ``Ctrl + Shift + Alt +  F7 / F8`` - 无闪烁调光 调低 / 调高
- ``Fn + Shift + F7 / F8`` - 光显矩阵 / Slash 光效亮度 调低 / 调高
- ``Fn + Shift + F7 / F8`` / ``Ctrl + Shift + Alt +  F7 / F8`` - 触控屏（Screenpad）亮度 调低 / 调高
- ``Ctrl + Shift + F20`` - 麦克风静音
- ``Ctrl + Shift + Alt + F13`` - 切换屏幕刷新率
- ``Ctrl + Shift + Alt + F14`` - 集显模式
- ``Ctrl + Shift + Alt + F15`` - 标准模式
- ``Ctrl + Shift + Alt + F16`` - 静音
- ``Ctrl + Shift + Alt + F17`` - 平衡
- ``Ctrl + Shift + Alt + F18`` - 增强
- ``Ctrl + Shift + Alt + F19`` - 自定义 1（如果存在）
- ``Ctrl + Shift + Alt + F20`` - 自定义 2（如果存在）
- [自定义键位 / 热键](https://g-helper.com/zh/power-user-settings/)

### 🎮 ROG Ally 按键
- ``M + DPad Left / Right`` - 显示亮度
- ``M + DPad Up`` - 触摸键盘
- ``M + DPad Down`` - 显示桌面
- ``M + Y`` - 切换 AMD 叠加层
- ``M + X`` - 截屏
- ``M + Right Stick Click`` - 手柄模式

------------------

> [!NOTE]
> ### 🔖 重要提示
> G-Helper **不是**操作系统、固件或驱动程序。它**不会**以任何方式实时“运行”你的硬件。
> 
> 它只是一个应用程序，让你选择制造商预先创建（并存储在 BIOS 中）的某个运行模式，并可选地（！）设置一些你设备上本就存在的设置，这与 Armoury Crate 的做法相同。它通过使用 Armoury Crate 所用的华硕系统控制接口（Asus System Control Interface）“驱动”来实现这一切。
> 
> 如果你使用与 Armoury Crate 相同的模式 / 设置 —— 你设备的性能或行为不会有任何差异。
> 
> G-Helper 对你笔记本的作用，类似于遥控器对你电视的作用。

### 使用的库与项目
- [Linux Kernel](https://github.com/torvalds/linux/blob/master/include/linux/platform_data/x86/asus-wmi.h) 用于华硕 ACPI/WMI 接口中的一些基础端点
- [NvAPIWrapper](https://github.com/falahati/NvAPIWrapper) 用于访问 Nvidia API
- [Starlight](https://github.com/vddCore/Starlight) 用于光显矩阵通信协议
- [UXTU](https://github.com/JamesCJ60/Universal-x86-Tuning-Utility) 用于使用 Ryzen SMU 进行降压的端点
- [PawnIO](https://github.com/namazso/PawnIO) 用于访问 RyzenSMU
- [AsusCtl](https://gitlab.com/asus-linux/asusctl) 提供灵感与部分逆向工程

### 代码签名政策
免费的代码签名由 [SignPath.io](https://about.signpath.io/) 提供，证书由 [SignPath Foundation](https://signpath.org/) 提供

### 隐私政策
本程序不会向任何其他联网系统传输任何信息

### 免责声明
“Asus”、“ROG”、“TUF” 和 “Armoury Crate” 是 AsusTek Computer, Inc. 的注册商标，归其所有。我对这些以及任何属于 AsusTek Computer 的资产不主张任何权利，仅出于信息传递目的使用它们。

本软件按“现状”提供，不附带任何形式的明示或暗示保证，包括但不限于对适销性、特定用途适用性和非侵权的保证。滥用本软件可能导致系统不稳定或故障。

_注：请务必以下方 **Disclaimers** 英文原文为准，以避免或减小因错误或不恰当之翻译引起的负面影响。翻译仅为便于阅读之目的，并非专业翻译，可能存在错误，亦可能与最新版本有所差异。本文不具有法律效力，亦不作为发生争端时处理之依据。_

**Disclaimers**
"Asus", "ROG", "TUF", and "Armoury Crate" are trademarked by and belong to AsusTek Computer, Inc. I make no claims to these or any assets belonging to AsusTek Computer and use them purely for informational purposes only.

THE SOFTWARE IS PROVIDED “AS IS” AND WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. MISUSE OF THIS SOFTWARE COULD CAUSE SYSTEM INSTABILITY OR MALFUNCTION.
