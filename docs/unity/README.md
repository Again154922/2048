# 从零开始学 Unity（并且能做出自己的游戏）

## 这份教程的前提

- **C# 你没问题**：刚写完控制台版 2048，会写 TCP 服务端，`class`、泛型、`async`、LINQ 这些不用我教
- **Unity 当零基础处理**：你很久没写了，所以这份教程**不假设你记得任何 Unity 细节**——每个窗口怎么点、每个术语什么意思、每段代码为什么这么写，都会讲
- **目标是"能自己做游戏"**，不只是把 2048 搬过去。所以基础篇会尽量把通用能力（对象模型、输入、调试、UI、存档、发布）讲扎实

## 学习路线

```
基础篇（学 Unity 本身）
   ↓  边学边做，每章都有动手清单
练手小游戏（把基础篇用一遍，脱离 2048）
   ↓
2048 实战（有现成算法，专注"把游戏做完整"）
   ↓
自己做别的游戏（附录给了通用套路和坑清单）
```

**为什么中间要插一个"练手小游戏"**：2048 的难点在于算法你已经有现成的，容易变成"抄代码"；先用一个小游戏把"从零搭一个能玩的东西"这件事走通一遍，再回头做 2048 会轻松很多。

## 目录

### 基础篇（按顺序读，每章都动手）

| 章节 | 内容 |
| --- | --- |
| [01 环境准备：Hub 与 Rider 联动](基础篇/01-环境准备-Hub与Rider联动.md) | Hub 操作、装/加 Editor 版本、让 Unity 和 Rider 联动、断点调试 |
| [02 编辑器入门：项目结构与五大窗口](基础篇/02-编辑器入门-项目结构与五大窗口.md) | 项目目录哪些进 git、五大窗口各管什么、Play 模式的三个陷阱 |
| [03 对象与资源模型](基础篇/03-对象与资源模型.md) | GameObject / Component / Prefab / 序列化字段——**最核心的一章** |
| [04 编辑器操作与快捷键](基础篇/04-编辑器操作与快捷键.md) | 鼠标怎么转视图、五个工具、常用快捷键、导入图片音效 |
| [05 脚本与生命周期](基础篇/05-脚本与生命周期.md) | `Update` 就是你的游戏循环、`Time`、协程；纠正后台线程写法 |
| [06 2D 世界与输入](基础篇/06-2D世界与输入.md) | Sprite、摄像机、世界单位、键盘鼠标输入 |
| [07 调试与排错](基础篇/07-调试与排错.md) | Console、读报错、断点调试、Gizmos 可视化、"引用空了"的排查流程 |
| [08 综合练习：小游戏](基础篇/08-综合练习-小游戏.md) | **做一个完整的"接球"游戏**：2D 物理触发、协程生成器、UI、结算、存档 |

### 组件手册（可以当参考书查，不必一次读完）

| 章节 | 覆盖的组件 |
| --- | --- |
| [09 Transform 与显示](基础篇/09-组件手册-Transform与显示.md) | `Transform`、`RectTransform`、`SpriteRenderer`、`SortingGroup`、`Camera` |
| [10 2D 物理](基础篇/10-组件手册-2D物理.md) | `Rigidbody2D`、六种 `Collider2D`、`PhysicsMaterial2D`、碰撞/触发回调、Layer 矩阵、`Physics2D.Raycast` |
| [11 UI 系统](基础篇/11-组件手册-UI.md) | `Canvas`、`CanvasScaler`、`GraphicRaycaster`、`Image`、`TextMeshProUGUI`、`Button`、`Toggle`、`Slider`、`TMP_InputField`、`TMP_Dropdown`、`ScrollRect`、布局组 |
| [12 动画、特效与声音](基础篇/12-组件手册-动画与特效.md) | `Animator` / 状态机、`ParticleSystem`、`LineRenderer`、`TrailRenderer`、`SpriteMask`、`AudioSource`、`VideoPlayer` |

每个组件的讲法都是固定的五段：**它是什么 → 挂在谁身上 → 关键属性 → 代码怎么用 → 什么时候用 / 坑**。遇到没见过的组件，也可以按这个结构自己补一条进笔记。

### 类盘点（Unity 的"数据类型"手册）

| 章节 | 覆盖的类 |
| --- | --- |
| [13 数值与数学](基础篇/13-类盘点-数值与数学.md) | **`Vector2` / `Vector3` / `Vector2Int`**、`Quaternion`、`Color` / `Color32`、`Rect` / `Bounds`、`Mathf`、`Random`、`Time` |
| [14 引擎入口与工具](基础篇/14-类盘点-引擎入口与工具.md) | `Application`、`Screen`、`SceneManager`、`PlayerPrefs`、`Debug` / `Gizmos`、`Cursor`、`Input`、`Physics2D` / `LayerMask`、协程相关类、`Object` 体系、`JsonUtility`、`Resources` |

> **`Vector3` 为什么看不懂**：它不是什么"高级类型"，就是**装着三个 `float` 的 struct**，既可以表示"位置"，也可以表示"方向"或"位移"。关键是记住它是**值类型**——`transform.position.x = 5` 编译不过就是因为它返回的是副本。13 章有完整拆解。

### 2048 实战

| 章节 | 内容 |
| --- | --- |
| [01 搭出棋盘](2048实战/01-搭出棋盘.md) | 方块 Prefab、用 `Instantiate` 生成 4×4 棋盘 |
| [02 移植核心算法](2048实战/02-移植核心算法.md) | 逻辑/表现/控制三层、把控制台版算法搬进 Unity |
| [03 打磨表现](2048实战/03-打磨表现.md) | 协程动画、输入锁、分数 UI、锚点、游戏结束面板、音效 |
| [04 存档与发布](2048实战/04-存档与发布.md) | `PlayerPrefs`、JSON 存档、为什么不能在退出时询问、打包 exe |

### 附录

| 文档 | 内容 |
| --- | --- |
| [术语表](术语表.md) | Unity 常见术语的中文解释 + 最容易搞混的六组概念。**遇到不懂的词先查这里** |
| [常见坑清单](附录/常见坑清单.md) | 40 条踩坑速查 + 从控制台思维迁移的对照表 |

## 怎么用

1. **每章都动手，别只读**。Unity 是"看会了"和"做出来"差距最大的东西之一：Inspector 里少拖一个引用，代码再对也白搭
2. 每章末尾都有动手清单，做完再往下走
3. 遇到不认识的英文术语 → 翻 [术语表](术语表.md)
4. 卡住时先看 [常见坑清单](附录/常见坑清单.md)，八成能在里面找到
5. 这些文件可以直接用 **Obsidian** 打开这个目录（`docs/unity`），双链和搜索都很方便

## 环境（已确认可用）

| 项 | 值 |
| --- | --- |
| Unity Editor | `2022.3.62f1`（LTS 最终版），路径 `D:\APPS\Unity\2022.3.62f1\Editor` |
| Unity Hub | `3.22.3`，路径 `D:\APPS\Unity Hub` |
| IDE | Rider `2026.2.0.2` |
| 新建项目建议 | 模板选 **2D (Built-In Render Pipeline)**，位置 `F:\unity\2048` |

## 版本策略：统一使用 2022.3 LTS

**这份教程和你的项目都只按 Unity 2022.3 LTS 写**（你装的是 `2022.3.62f1`）。所有示例、路径、API 都以它为准，不再兼顾 Unity 6。

教程里用到的都是 2022.3 的标准写法：

| 场景 | 2022.3 的标准写法 |
| --- | --- |
| 刚体速度 | `rb.velocity` |
| 找多个对象 | `FindObjectsByType<T>(FindObjectsSortMode.None)`（2022.2 起支持） |
| 输入 | 旧输入系统的 `Input.GetKeyDown` / `Input.GetAxisRaw` |
| 文本 | `TextMeshProUGUI`（UI）/ `TextMeshPro`（世界坐标） |
| 动画 | 协程为主，`Animator` 按需 |

### 万一以后要升级到 Unity 6

现在不用管，真到那天只需处理四处：`rb.velocity` 改名 `rb.linearVelocity`；新项目可能默认启用新输入系统（要改 `Active Input Handling` 为 `Both`）；2D 模板可能变成 URP（要加 `Global Light 2D`）；`FindObjectsOfType` 彻底过时（教程已用新的 `FindObjectsByType`）。

### 装 2022.3 时的两条注意

1. **只勾需要的模块**：Android（含 SDK & NDK Tools、OpenJDK）、Windows Build Support (IL2CPP) 按需勾，iOS/WebGL/Linux/Mac 先别勾
2. **Hub 的"编辑器安装位置"设成 `D:\APPS\Unity`**，别留着指向其他目录的旧配置

## 关于"以后想写别的游戏"

做完 2048 之后，你手上会有这些可复用的东西：

- 一套**对象模型的心智**（对象/组件/Prefab/实例化）
- **输入、UI、动画、存档、发布**的完整流程各走了一遍
- 一个**纯逻辑 + 表现分离**的代码结构（`BoardModel` 这种写法能直接搬到别的棋类/回合制游戏）

再往后想扩展，按性价比排序学：协程吃透 → `ScriptableObject`（配置数据）→ 事件/委托（模块解耦）→ 状态机（游戏流程）→ 对象池 → 新输入系统 → 联机。附录的坑清单最后也列了这一串。
