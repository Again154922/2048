# 2048

一个 2048 游戏的两个实现（控制台版 + Unity 版），外加一套从零开始的 Unity 教程。

## 仓库结构

| 路径 | 说明 |
| --- | --- |
| `2048Console/` | 控制台版（C# / .NET 10），包含 v1.0 与 v2.0 两个实现 |
| `2048Unity v1.0/` | Unity 版（Unity 2022.3 LTS），开发中 |
| `2048Console/docs/unity/` | **Unity 教程**（21 篇，从零基础做到 2048） |

## 控制台版

### 两个版本

| 版本 | 说明 |
| --- | --- |
| `2048Console v1.0` | 最初的实现，移动逻辑用"逐种情况分类讨论"的写法，已冻结不再改动 |
| `2048Console v2.0` | 当前版本：移动逻辑重写为"按线处理"，并补齐了存档、历史最高等功能 |

### 玩法

| 按键 | 作用 |
| --- | --- |
| 方向键 / `W` `A` `S` `D` | 移动方块 |
| `R` | 重新开始（本局最大值会记入历史最高） |
| `Esc` | 退出，并询问是否保存当前进度 |

棋盘下方一行显示"历史最高 / 当前最高"。记录文件 `2048-best.txt` 和 `2048-save.txt` 生成在 exe 同目录。

### 构建与运行

```powershell
dotnet build "2048Console/2048Console.sln"
dotnet run --project "2048Console/2048Console v2.0/2048Console v2.0.csproj"
```

### 发布单文件

```powershell
dotnet publish "2048Console/2048Console v2.0/2048Console v2.0.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false
```

产物是一个约 70 MB 的单文件 exe（自包含、不含 pdb）。

## Unity 版

| 项 | 值 |
| --- | --- |
| 引擎 | **Unity 2022.3.62f3（LTS）** |
| 目录 | `2048Unity v1.0/` |
| 当前进度 | 主菜单场景与方块图片已就位，游戏逻辑搭建中 |
| 方块图片 | `Assets/Images/2048/`：`square_base.png` + `tile_0.png` ~ `tile_32768.png`（256×256、透明圆角、数字已烘焙进图） |

用 Unity Hub 的 `Add` 选中 `2048Unity v1.0` 目录即可打开（不需要重新安装引擎）。

## Unity 教程

`2048Console/docs/unity/` 下是一套约 3600 行的 Unity 教程，按"从零 → 能自己做游戏"组织：

| 部分 | 内容 |
| --- | --- |
| **基础篇 01–08** | 环境准备、编辑器、对象模型、脚本生命周期、2D 与输入、调试、综合练习（完整接球小游戏） |
| **组件手册 09–12** | Transform/SpriteRenderer/Camera、2D 物理、UI 系统、动画特效与声音 |
| **类盘点 13–14** | Vector2/3、Quaternion、Color、Mathf、Time、Application、PlayerPrefs、Input、Physics2D、JsonUtility 等 |
| **2048 实战** | 搭棋盘 → 移植算法 → 打磨表现 → 存档与发布 |
| **附录** | 术语表、常见坑清单 |

入口：[`2048Console/docs/unity/README.md`](2048Console/docs/unity/README.md)

## 开发环境

| 项 | 版本 |
| --- | --- |
| .NET SDK | 10.0 |
| Unity | 2022.3.62f3（LTS） |
| IDE | Rider 2026.2 |
