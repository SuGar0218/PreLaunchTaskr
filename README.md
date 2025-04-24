# PreLaunchTaskr.ReadMe

PreLaunchTaskr 的 WPF 分支

## 这个程序有什么用？

在程序启动时，执行一些您想要的操作：

- 附加启动参数
- 屏蔽启动参数（支持正则表达式）
- 在程序启动前，先执行脚本或运行其他程序
- 为程序设置专有环境变量，而无需更改系统或用户的环境变量

## 为什么要用这个程序？

最常用的可能是附加启动参数，这完全可以通过快捷方式做到。但快捷方式里的启动参数不能全局生效，例如 PDF 打开方式设为 Chrome，当您打开 PDF 文件时，Chrome 并不会包含快捷方式中的那些参数。

## 这是怎么做到的？

*映像劫持*

注册表 (regedit)：HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options

修改注册表是敏感操作，设置映像劫持更是敏感操作。因此防病毒软件很可能阻止此程序执行这个操作，如果使用此软件时提示或阻止了，请允许。但不建议关闭此项防护，以防不信任的程序作恶。

## 开始使用

### 使用哪个版本

- 如果使用 Windows 10 1909 (18362) 或更高版本的 64 位系统，建议使用 WinUI 3 版本。
- 如果使用 Windows 7 或更高版本，都可以使用 WPF 版本。
- 更低版本的系统不保证能使用。

### 下载和使用

1. 直接下载 Release 中的 zip 文件，解压到你习惯放应用程序的地方，以后就不要移动了。
2. 运行里面名称含有 GUI 的程序。
3. 不要随意移动或删除，因为此程序改动了注册表，删除前先关闭此程序中已添加的程序列表中的所有开关。

### 首次使用

可能提示安装 .NET 运行环境，点击“是”，然后等待浏览器打开网页。WPF 版本需要 .NET 6 版本的运行环境。

## 目前的缺陷

- 由于没有调用 C++ API，受限于 C# 中的 Process 类提供的功能，绕开自己设置的映像劫持的方式是创建一个名称不同的符号链接。这可能对于文件名敏感和检测程序目录的软件会有影响。
- 注册表中的映像劫持如果被此程序意外的因素修改，此程序不会察觉到。会在您下一次开关程序配置时恢复到此程序预期的设置。

## 解压后有什么

- PreLaunchTaskr.GUI.WPF.exe：主要程序
- PreLaunchTaskr.Configurator.NET6.exe：主要用于被 GUI 程序以管理员权限调用，你也可以在命令行中使用，带上 -h 参数会显示帮助，大多数功能需要管理员权限。
- PreLaunchTaskr.Launcher.NET6.exe：用于完成启动前任务的启动器，映像劫持就是为了运行它。

## 疑难解答

- 如果程序出现异常，请在此程序的已添加的程序列表中关闭或移除它。
- 如果不小心移动或删除了还难以恢复，请在注册表中进入 ```HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options```，然后寻找设置过的程序的文件名，点进去，在右侧删掉 Debugger 那一项。

## 开发者

此项目分为以下模块：

- PreLaunchTaskr.Common：共用的的配置和工具类。
- PreLaunchTaskr.Core：核心逻辑，包括操作注册表、增删查改用户对程序的配置、带参数启动、实体类。
- PreLaunchTaskr.CLI.Common：命令行程序共用的工具类。
- PreLaunchTaskr.Configurator.NET6：对 Core 中配置逻辑的命令行包装，WPF 版本的 GUI 程序会以管理员权限调用它。
- PreLaunchTaskr.Launcher.NET6：对 Core 中启动逻辑包装，映像劫持指向的是该项目生成的程序。
- PreLaunchTaskr.GUI.Common：GUI 程序中基本功能所需的组件，WPF 和 WinUI 3 程序共用组件、共用抽象 ViewModel，但是抽象 ViewModel 如果只是同名在前面加一个I的接口，对程序逻辑没有影响，只是为了便于从 WinUI 3 向 WPF 移植。
- PreLaunchTaskr.GUI.WPF：WPF 应用程序

程序设计时是按照 Configurator、Launcher、GUI 程序和用于存储程序配置的 settings.config 放在同一文件夹，但是在 Visual Studio 中，对单个项目运行时，它们的输出目录并不在同一个。我目前还没能解决，每次要测试都是手动复制。
