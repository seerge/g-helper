# G-Helper (GHelper)

[![Github all releases](https://img.shields.io/github/downloads/seerge/g-helper/total.svg)](https://GitHub.com/seerge/g-helper/releases/) [![GitHub release](https://img.shields.io/github/release/seerge/g-helper.svg)](https://GitHub.com/seerge/g-helper/releases/) [![GitHub stars](https://img.shields.io/github/stars/seerge/g-helper.svg?style=social&label=Star)](https://GitHub.com/seerge/g-helper/stargazers/)

语言: [English](https://github.com/seerge/g-helper#readme) | 中文

## 为ASUS笔记本打造的、Armoury Crate(奥创控制中心)的轻量化替代品

这是一个ROG 幻14、幻15，幻13、幻16，飞行堡垒/天选系列，ROG 枪神/魔霸系列或其他ASUS笔记本的控制工具。可以做到几乎所有Armoury Crate(奥创控制中心)能做的事情，而且不需要任何多余的功能或安装不必要的系统服务。


## :gift: 主要优点 

1. 自动且无缝的独立显卡模式切换（不需要关闭所有应用或执行其他操作）
2. 可以手动调整所有的性能模式（包括自定义风扇曲线以及PPTs(Platform Power Threshold,即CPU总功耗,下文简称PPTs--译注))
3. 极致的轻量化，几乎不消耗任何资源，也不需要安装系统服务。只需要下载一个exe文件就可以运行。
4. 简洁的界面设计，可以轻松调整所有设置。
5. 运行不需要管理员权限！

## [:floppy_disk: 下载应用](https://github.com/seerge/g-helper/releases/latest/download/GHelper.zip)

如果你喜欢这个应用，请[给这个项目:star:](https://github.com/seerge/g-helper) 或者向别人推荐它！
### :euro: [(通过paypal)支持和捐赠G-Helper](https://www.paypal.com/donate/?hosted_button_id=4HMSHS4EBQWTA)

_如果你在别的地方提到这个软件-请记得加上这个项目的网址。十分感谢。_

![Screenshot 2023-04-11 221528](https://user-images.githubusercontent.com/5920850/231278828-9bb7f5c3-4ce6-4825-b06d-572f39d3ede8.png)

### :zap: 主要功能

1. **性能模式**: 静音模式 - 平衡模式 - 增强模式 （笔电bios内置的性能模式，附带默认的风扇曲线）
2. **显卡模式**: 集显模式 - 标准模式 - 独显直连 - 自动切换
3. 笔记本屏幕刷新率 60hz 或 120hz 自动调节(或者 144hz 等刷新率)，包括屏幕Overdrive(OD,即ROG系列的屏幕快速响应/快显功能--译注)功能
4. 可调节的风扇曲线、电源功耗设置(PPTs)(_注:请谨慎调节!_)和CPU超频选项
5. ROG Anime matrix 光显矩阵屏控制， 感谢项目 [Starlight](https://github.com/vddCore/Starlight) + 应用侧的一些调整，包括动画GIF图片
6. 键盘灯光效果和颜色设置 (包括睡眠模式下的灯光效果和对飞行堡垒/天选系列的键盘支持)
7. 对 M3, M4 按键的自定义设置和 FN+F5 快捷键(性能模式切换) FN+F4 快捷键(键盘灯光效果切换)
8. CPU/GPU温度、风扇转速和电池充放电功率显示
9. 电池充电上限设置，保护电池健康度

### :gear: 当使用电池供电或插上电源时，自动切换：
- 性能模式 (软件会记住上一次使用电池或插上电源时的电源模式)
- 自动切换独立显卡模式 - 使用电池时停用独显，并在插上电源时重新启用
- 自动切换屏幕刷新率 (使用电池时切换为 60hz,插入电源后切换为 120+ hz)
- 使用电池时键盘背光可自动关闭

为了保证自动切换功能和按键绑定功能的正常工作，软件需要在后台运行并显示一个托盘图标。这并不会消耗其他资源。

### :rocket: 性能模式

性能模式与 Armoury Crate(奥创控制中心)中的 **保持一致**，因为这些设置与其对应的风扇曲线都被保存在bios中

1. 静音 (风扇转速最小或完全停转, 70W 总PPT, 其中 CPU 最高 45W PPT) + windows电源模式设置为最长续航/省电模式
2. 平衡/性能 (平衡模式的风扇曲线设置, 100W 总PPT, 其中 CPU 最高 45W PPT) + windows电源模式设置为平衡
3. 增强 (激进的风扇曲线设置, 125W 总PPT, 其中 CPU 最高 80W PPT) + windows电源模式设置为高性能/最佳性能

_PPTs 默认在 幻14 2022版上显示, 对于其他型号 PPTs 的显示将会有所变化，因为它们在bios中的设置不同。_

![Screenshot 2023-04-06 142234](https://user-images.githubusercontent.com/5920850/230377635-7032a480-3a94-4e35-9468-d8911e3e55ec.png)

### :video_game: 显卡模式

1. 集显模式 : 只启用低功耗的内置显卡, 核显连接笔电内置屏幕
2. 标准模式 (MS Hybrid) : 同时启用核显与独显, 核显连接笔电内置屏幕
3. 独显直连: 同时启用核显与独显, 但独显直连笔电屏幕 (仅在幻14 2022版等机型上支持)
4. 自动切换: 使用电池时关闭独显(集显模式)，并在插上电源后重新启用独显(混合输出)

## :question: 常见问题解答(FAQ)

#### 当我按下 M4 / Rog键的时候总是弹出安装Armoury Crate的弹窗提示，我该如何关闭它?
删除下列文件或者把它移动(注:剪切+粘贴)到别的目录 ``C:\Windows\System32\ASUSACCI\ArmouryCrateKeyControl.exe``.

如果还是出现弹窗 - 进入 BIOS (开机时按住 F2), 按照屏幕下方的快捷键提示进入 Advanced Settings,然后关闭 "Armoury Crate Control Interface" (注:把它设置为disabled)。

#### 为什么我的笔记本不支持独显输出?
独显输出只在幻14 2022版上支持(也可能支持2022年以来的其他机型)。

#### 我无法在幻14 2020版上设置集显模式(关闭独显)
很不幸，2020版的机型在硬件上不支持这个设置。

#### 我是否应该自行调整功耗设置(PPTs)和风扇曲线?
你可以不那么做，这些操作是可选的。按照经验(bios)内置的性能模式工作的很好。请只在遇到问题的时候限制功耗或者手动设置风扇模式。当你在风扇与功率设置中按下“应用”时，bios将会认为风扇配置文件为“自定义”! (无论你是否真的修改了风扇曲线)

#### G-helper是如何控制我的风扇转速的?
软件并不会修改风扇转速。这个设置实际上由bios控制(与Armoury Crate的情况相同)。G-helper 所能做的 - 就是将一个自定义的风扇配置(可选地)应用到正在使用的性能模式，其中包括8组温度+风扇速度的百分比数值%。软件通过与Armoury Crate看起来相同的WMI endpoint来应用这一设置，

#### 我如何把风扇转速显示从百分比%切换到RPM(每分钟转速)?
点击一下百分比即可切换。

#### 我没有在G-helper中看到独显温度
很可能要么你正在使用核显模式/自动切换模式，这种情况下独显只是关闭了；或者你的windows系统为独显设置了睡眠模式(为了省电)。这种情况下，G-helper无法识别到独显，也不能获得温度读数。

#### 弹窗提示"G-Helper已经在运行"
请在系统托盘里找到 (G) 图标。windows会默认隐藏所有图标，所以你可能需要点击 ^ 来看见它们。建议任务栏右键进入任务栏设置 -> 其他系统图标 -> 将 G-Helper 设置为始终在任务栏上显示。

#### 应用不能启动或者崩溃了，我该怎么做?
从开始菜单里打开“事件管理器”， 进入 Windows日志 -> 应用，然后在最近的错误日志里寻找包含G-helper的日志。如果你找到一个 - 请提出一个 [新的 issue](https://github.com/seerge/g-helper/issues) ，其中包含来自这次错误的所有的详细信息。

#### 我可以在用G-Helper的时候同时用myASUS吗?
当然可以! 唯一的问题是myASUS可能会重置你之前在g-helper里设置的电池充电上限。在这种情况下，建议你在这两个应用(myASUS和g-helper)里设置相同的充电上限(60%/80%/100%)以避免冲突。

#### 我如何将M3键设置为将麦克风静音?
这个功能由 Asus Optimization Service 管理(所以 G-helper 没有这项设置，也不管理这个功能)。请确认这个系统服务是否正常运行

#### 我如何设置不同的视觉风格(即 Armoury Crate 内的gamevisual功能)?
因为这个功能让颜色显示不正确，我个人(注:作者 :) )并不喜欢这些。但是如果你需要这个功能 - 你可以使用 Nvidia 控制面板/GefoeceExperirence滤镜或者AMD控制面板(Radeon Software/AMD Software: Adrenalin Edition)来修改显示颜色(因显示器和配置而异)。如果你真的非常需要这个功能，你也可以使用 [微软商店中ASUS自己的工具](https://apps.microsoft.com/store/detail/gamevisual/9P4K1LFTXSH8?hl=nl-nl&gl=nl&rtc=1)

#### 我能超频 Nvidia GPU 核心或显存吗?
我(注:作者 :) )现在并没有Nvidia的显卡，所以我不能保证这个功能加入之后是安全的。但是你可以用微星小飞机(msi afterburner) 去超频GPU，毕竟它是为GPU超频专门设计的 :)

#### 我该如何卸载G-helper?
G-helper是一个单文件的exe文件, 而且它不会向系统中安装任何东西。如果要“卸载”它 - 你可以直接删除exe文件 :) 如果你已经设置了自定义的风扇配置或者功耗设置(PPTs) - 在删除软件之前建议你选择你最喜欢的配置模式(比如"平衡")，然后在“风扇与电源设置”里点击“恢复默认设置”。

#### G-helper 是什么?
这是一个ASUS笔记本的控制工具。可以做到几乎所有Armoury Crate(奥创控制中心)能做的事情，而且不需要任何多余的功能或安装不必要的系统服务。


----------------------------

## :euro: [支持这个项目](https://www.paypal.com/donate/?hosted_button_id=4HMSHS4EBQWTA)
#### 如果你喜欢这个项目你可以[通过Paypal捐赠](https://www.paypal.com/donate/?hosted_button_id=4HMSHS4EBQWTA)
[![二维码](https://user-images.githubusercontent.com/5920850/233658717-0441494d-fede-4a2c-b4f2-4b16a184a69a.png)](https://www.paypal.com/donate/?hosted_button_id=4HMSHS4EBQWTA)

----------------

### 安装指南

1. 从 [**Releases Page**](https://github.com/seerge/g-helper/releases) 下载最新版本
2. 解压到你选择的文件夹
3. 运行 **GHelper.exe**

### 运行要求（必须）

- Microsoft [.NET7](https://dotnet.microsoft.com/en-us/download)。 你可能已经安装了。 如果没有的话你可以从官方网站 [立即下载](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-7.0.202-windows-x64-installer)。

- [Asus System Control Interface](https://dlcdnets.asus.com/pub/ASUS/nb/Image/CustomComponent/ASUSSystemControlInterfaceV3/ASUSSystemControlInterfaceV3.exe)。 如果你安装了或者安装过myASUS,  那么这个"驱动"应当已经安装(即使myASUS已经卸载)。 或者你可以手动下载安装。

### 推荐配置（可选）

- 推荐保持 "Asus Optimization Service" 这个windows服务的运行, 它保证基本的键盘快捷键(比如屏幕或键盘亮度)能够使用。

- 可选选项(!) 你可以通过在管理员模式下运行 [这个用于精简的.bat文件](https://raw.githubusercontent.com/seerge/g-helper/main/debloat.bat)来禁用/移除不必要的服务。如果要恢复这些服务，运行 [这个.bat文件](https://raw.githubusercontent.com/seerge/g-helper/main/bloat.bat)。

-这个应用不建议与Armoury Crate(及其服务)同时运行, 因为它们会调整相同的设置。你可以[使用ASUS官方提供的卸载工具卸载](https://dlcdnets.asus.com/pub/ASUS/mb/14Utilities/Armoury_Crate_Uninstall_Tool.zip?model=armoury%20crate)Armoury Crate。以防万一，你总是可以之后再安装回来。

-------------------------------

为Asus ROG 幻14 2022 (配置了AMD核显和独显)设计和开发。但应当可能在幻14 2021和2020款, 幻15, X FLOW, 以及其他的ROG机型上使用相关且支持的功能。

我并没有microsoft证书来为这个应用签名，所以如果你在启动时看到windows defender的警告(windows 保护了你的电脑),点击“更多详情” -> 继续运行(不推荐)。作为可选选项，你也可以使用 visual studio自行编译然后运行这个项目 :)

设置文件保存在 ``%AppData%\GHelper``

------------------

精简你的windows可以帮助延长电池的使用时间，同时让笔电的温度更低一些

![Helps to save your battery](https://raw.githubusercontent.com/seerge/g-helper/main/docs/screenshots/screen-5w.png)

---------

## 高级用户设置

### 为每一个模式自定义用户计划

在 config.json (位于 %appdata/GHelper) 中你可以手动添加自定义电源设置的GUID (它既可以是"真正的"可被选择的电源计划，也可以是"覆盖式(overlay)"的电源计划，就像g-helper默认设置的那样)

格式如下 : "scheme_<mode>" : "GUID" 

Where ``mode = 0 (balanced), 1 (turbo), 2 (silent)``

```
"scheme_0": "2ac1d0e0-17a7-44ed-8091-d88ef75a4eb0",
"scheme_1": "381b4222-f694-41f0-9685-ff5bb260df2e"
```

确保修改时保证json文件的结构不被打乱 (例如不要增减或缺失逗号、括号等操作) - 否则应用会读取失败，并将重新创建一个新的配置文件来替代它。

### 自定义热键行为

软件支持为 M3, M4 和 FN+F4 热键自定义配置。如要设置，在按键旁的选项框中选择"自定义设置"，然后执行下面的操作（任选其一）:

1. 要想运行任意应用 - 向 "action" 文本框中粘贴应用文件exe的完整路径，例如:
``C:\Program Files\EA Games\Battlefield 2042\BF2042.exe``

2. 要想模拟任意windows按键 - 向"action"文本框中粘贴相对应的 keycode，例如 ``0x2C`` 为屏幕截图键。
Keycodes的完整列表: https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes

![Screenshot 2023-04-13 172537](https://user-images.githubusercontent.com/5920850/231837470-df913847-cf8a-43e1-80b6-5eb6e9eaee4e.png)

------------

_注:请务必参考下方 **免责声明** 原文，以避免或减小错误或不恰当之翻译引起的负面影响。翻译仅为便于阅读之目的，不具有法律效力，亦不作为发生争端时处理之依据。_

**Disclaimers**
"ROG", "TUF", and "Armoury Crate" are trademarked by and belong to AsusTek Computer, Inc. I make no claims to these or any assets belonging to AsusTek Computer and use them purely for informational purposes only.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. MISUSE OF THIS SOFTWARE COULD CAUSE SYSTEM INSTABILITY OR MALFUNCTION.
