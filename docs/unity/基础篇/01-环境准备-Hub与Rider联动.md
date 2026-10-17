# 01　环境准备：Hub、安装版本、Rider 联动

这一章全是"手上操作"，概念很少，但没做对后面每一步都会别扭。**建议照着做一遍，不要跳过。**

## 1.1 Hub 和 Editor 是两个东西

| | Unity Hub | Unity Editor |
| --- | --- | --- |
| 是什么 | 启动器 / 版本管理器 | 真正干活的编辑器 |
| 位置（你机器） | 已装 3.22.3 | `D:\APPS\Unity\2022.3.62f1\Editor\Unity.exe` |
| 干什么 | 管理版本、创建/打开项目、装模块 | 编辑场景、写代码、运行、打包 |

日常流程：**打开 Hub → 在列表里点项目 → Hub 用对应的 Editor 版本把项目打开**。你完全可以绕开 Hub 直接双击项目里的 `Assets` 文件夹？不行——**必须通过 Hub 或 Editor 打开**，双击 `.unity` 文件通常打不开（会变成用文本编辑器打开 YAML）。

## 1.2 你机器上的现状（我实际查到的）

```
Unity Hub                    3.22.3，界面语言：中文
Unity Editor                 2022.3.62f1（LTS 最终版，通过 Hub 安装）
   位置                      D:\APPS\Unity\2022.3.62f1\Editor\Unity.exe
Rider                       2026.2.0.2（另有 2024.3.2、2025.1.2 两个旧版在盘上）
你已有的 Unity 项目           F:\unity\{Stopwatch, UnityTest, ChatBox, UnoCards v0.1}
```

## 1.3 Hub 界面导览

左侧四个页面：

| 页面 | 中文名 | 用途 |
| --- | --- | --- |
| **Projects** | 项目 | 项目列表。左上 `新建项目`，右上 `打开`（打开已有项目） |
| **Installs** | 安装 | 已装的 Editor 版本 + `安装编辑器` 按钮 |
| **Learn** | 学习 | 官方教程项目（可以无视） |
| **Community** | 社区 | 论坛入口 |

右上角齿轮 → `设置`：里面能改编辑器安装路径、许可证、语言。

## 1.4 装/加 Editor 版本的两种方式

### 方式 A：Hub 下载安装（新版本推荐这样）

Installs → `安装编辑器` → 选版本 → 勾模块 → 装。

**选哪个版本**：选**最新的 LTS**（长期支持版）。2022.3 就是 LTS，你现在装的也是它，够用。不要追最新科技版（`Unity 6.x` 之类的新命名），教程/文档/插件生态的适配通常滞后。

**模块怎么勾**（关键只看两个）：

| 模块 | 要不要 |
| --- | --- |
| `Windows Build Support (IL2CPP)` | **要**，不然打不出 exe |
| `Documentation` | 可选，离线文档 |
| `Android / iOS / WebGL / Linux / Mac` | 用不到就别勾，每个几百 MB |
| `Microsoft Visual Studio Community` | **不要**，你用 Rider |

### ⚠️ 你这里有个坑

我查到你 Hub 里配置了"第二个安装路径"，但它现在指向的是 **`D:\APPS\JetBrains Rider 2024.3.2`**（一个 Rider 目录）：

```json
// C:\Users\james\AppData\Roaming\UnityHub\secondaryInstallPath.json
"D:\\APPS\\JetBrains Rider 2024.3.2"
```

如果你直接点"安装编辑器"，Unity 可能会试图装进这个 Rider 目录里，或者因为路径被占而报错。**建议先改掉**：Hub → 设置 → `安装` 标签页 → 把"编辑器安装位置"改成 `D:\APPS\Unity`（或你自己建的目录），再装新版本。

> **手动添加 vs Hub 安装**：手动添加 = 把已存在的 Editor "登记"进 Hub（不改动磁盘文件）；Hub 安装 = Hub 自己下载、安装、登记，之后可以在 Hub 里直接管理模块和升级。两者日常用法一样，只是 Hub 安装的版本管理起来更方便。

### 方式 B：手动添加（已经有 Editor 时用）

Installs → `添加` → 选到 `…\Editor\Unity.exe`（注意是 **Editor 子目录下的 Unity.exe**，不是根目录）。你机器上的完整路径：

```
D:\APPS\Unity\2022.3.62f1\Editor\Unity.exe
```

> 忘了路径怎么办：Editor 安装目录的特征是"里面有 `Editor\Unity.exe` 和 `modules.json`"。在 To Do：`Get-ChildItem -Recurse -Filter Unity.exe` 或者直接用 Everything 搜 `Unity.exe`。

## 1.5 新建项目（2048 就在这一步建）

1. Hub → Projects → `新建项目`
2. 顶部选 Editor 版本：`2022.3.62f1`
3. 模板列表里选 **2D (Core)**（别选 3D，别选 URP/HDRP 模板）
4. 右下：项目名 `2048`，位置 `F:\unity`，取消勾选"使用 Unity 云"之类的版本控制选项
5. `创建项目` → 第一次会编译一会儿，然后 Editor 打开

**创建完成后目录长这样**（关键只要认三个）：

```
F:\unity\2048\
├── Assets\           ← 你的所有东西
├── Packages\         ← 依赖清单
├── ProjectSettings\  ← 项目设置
├── Library\          ← 缓存（很大，别提交、别删着玩）
└── ...
```

## 1.6 和 Rider 联动（重点，装一次用一辈子）

### 第 1 步：Unity 端告诉它"我用 Rider"

Editor 里：`Edit → Preferences`（Windows 上是 Edit 菜单，不是 Unity 菜单）→ 左侧 `External Tools` → `External Script Editor` → 选 **Rider 2026.2**。

> 如果下拉里只有 `Open by file extension` 和一堆 Visual Studio，说明 Unity 没检测到 Rider：选 `Browse...` 手动指到
> `D:\APPS\JetBrains Rider 2026.2.0.2\bin\rider64.exe`。
> **注意选最新那个版本**——你盘上有三个 Rider，自动检测有可能挑到旧的 2024.3.2。

这个设置是**全局**的（不是每个项目设一次）。

### 第 2 步：确认项目里装了 Rider 集成包

Unity 项目自带 `com.unity.ide.rider` 包（你的 Stopwatch 项目里是 3.0.31），它负责生成 `.csproj` 让 Rider 认识项目结构。检查方式：

`Window → Package Manager` → 左上方下拉切到 `In Project` → 看有没有 `Rider Editor`。

有了就行，不用管。**没有的话**（老项目可能出现）：`+` → `Add package by name` → 输入 `com.unity.ide.rider` → 添加。

### 第 3 步：验证"双击脚本能在 Rider 里打开"

Project 窗口里双击任意 `.cs` → 应该在 Rider 里打开，并且 Rider 底部状态栏会显示 Unity 相关图标。

如果打开的是别的编辑器 / 记事本：回第 1 步重新设。

### 第 4 步：断点调试（这一步最值钱）

这是 Rider + Unity 相比"用 VS Code 瞎写"最大的优势——**你可以在 Unity 运行时打断点、单步、看变量**，就像调试控制台程序一样。

1. Rider 右上角运行配置下拉框 → 选 `Attach to Unity Editor`（如果列表里没有，Rider 会在检测到 Unity 项目时自动创建）
2. 先点 Unity 的 ▶ 进入 Play 模式
3. 再点 Rider 的 🐞（Debug）→ 选 `Attach to Unity Editor & Play`
4. 在代码里点行号左边打红点（断点）
5. 触发那段代码时，Unity 会卡住，Rider 的调试器窗口就能看局部变量、调用栈、单步执行（F8 单步、F9 步进、F5 继续）

**这个能力对你做 2048 特别有用**：棋盘数据算错的时候，直接在 `MergeLine` 里下断点，看 `line` 数组和 `result` 每一步的值，比 `Debug.Log` 打印强十倍。

### 第 5 步（可选但推荐）：把 Unity 日志接进 Rider

Rider 底部一般会有 `Unity` 工具窗口（没看到的话：`View → Tool Windows → Unity`），里面能直接看 Unity 的 Console 输出，不用在 Unity 和 Rider 之间来回切。

### Rider 联动常见问题

| 现象 | 解决 |
| --- | --- |
| Unity 下拉列表里没有 Rider | 手动 Browse 到 `rider64.exe`（见第 1 步） |
| 双击脚本打开了错误的 Rider 版本 | 重新在第 1 步指定具体版本，或者把旧版 Rider 卸载 |
| Rider 里代码一片红（报找不到 `UnityEngine`） | Rider 没识别成 Unity 项目：等 Unity 重新生成 `.sln`（`Assets → Open C# Project`），或在 Rider 里 `File → Open` 直接选项目根目录 |
| 断点打不上（灰色空心圈） | 没 Attach；或者代码没被编译（Unity 有编译错误时不会加载新代码）；或者 Attach 的是旧版 Editor |
| Attach 后 Unity 卡顿 | 正常，调试模式会慢一些；记得调试完停掉 |

## 1.7 做完这一章的验收清单

- [x] Hub 的 Installs 里能看到 `2022.3.62f1`
- [x] 新建了 `F:\unity\2048` 项目，能正常打开
- [x] 如果能装新版本：先确认编辑器安装路径不是那个 Rider 目录
- [x] Unity 的 External Script Editor 指向 **Rider 2026.2**
- [x] 双击 `Assets` 里新建的脚本，能在 Rider 里打开
- [x] 能在 Rider 里 Attach to Unity Editor，并在 `Update` 里断住

**下一章**（01）：认识编辑器五大窗口，搞清项目目录里哪些文件该进 git。
