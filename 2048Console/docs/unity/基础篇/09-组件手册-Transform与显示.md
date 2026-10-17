# 09　组件手册（一）：Transform、SpriteRenderer、Camera

从这一章开始是**组件手册**。每个组件的讲法固定为五段：**它是什么 → 挂在谁身上 → 关键属性 → 代码怎么用 → 什么时候用 / 坑**。这套结构你以后查任何新组件都可以套用。

> **怎么读组件手册**：不用背。先通读一遍建立"有哪些积木"的全局印象，做项目时遇到需求再回来查具体属性。

## 9.1 Transform：每个对象都有的"位置"

### 它是什么

每个 GameObject **必定有且只有一个** Transform，表示"这个对象在世界里的位置、朝向、大小"。**它不能删除**。

### 关键属性

| 属性 | 含义 |
| --- | --- |
| `position` | **世界坐标**（相对于场景原点） |
| `localPosition` | **相对父对象的坐标**（有父对象时和 `position` 不同） |
| `rotation` / `localRotation` | 旋转（四元数） |
| `eulerAngles` / `localEulerAngles` | 旋转的"角度"表示（2D 里通常只用 Z） |
| `lossyScale` | 世界缩放（考虑所有父级后的结果） |
| `localScale` | 相对父级的缩放 |
| `parent` | 父对象（可读写，赋 null 就脱离父级） |
| `childCount` | 子对象数量 |
| `root` | 最顶层的祖先（场景根） |
| `right` / `up` | 本地坐标轴方向 |

### 代码怎么用

```csharp
// 移动（推荐：用 deltaTime 保证帧率无关）
transform.position += Vector3.right * speed * Time.deltaTime;

// 相对自己"前方"移动（对象旋转过时很有用）
transform.Translate(Vector3.forward * speed * Time.deltaTime);   // 默认是本地坐标

// 旋转（2D 里绕 Z 轴）
transform.Rotate(0f, 0f, 90f * Time.deltaTime);

// 父子关系
transform.SetParent(parentTransform);            // 换父对象（世界位置保持不变）
transform.SetParent(parentTransform, false);     // 换父对象，并保持 localPosition 不变

// 遍历子对象
for (int i = 0; i < transform.childCount; i++)
{
    Transform child = transform.GetChild(i);
    child.gameObject.SetActive(false);
}
```

### 坐标转换（很常用）

```csharp
Vector3 local = transform.InverseTransformPoint(worldPos);   // 世界 → 本地
Vector3 world = transform.TransformPoint(localPos);          // 本地 → 世界
Vector3 localDir = transform.InverseTransformDirection(worldDir);   // 只转方向
```

### 坑

- **有父对象时 `position` 是推算出来的**：给子对象设 `position` 会反过来改它的 `localPosition`。想"相对父对象移动"就改 `localPosition`。
- **父对象的 Scale 会传染给子对象**：父级 Scale 为 2，子级的视觉大小也翻倍。所以别用 Scale 缩放整棵 UI 树（用 Canvas Scaler）。
- **2D 里 Z 决定渲染先后**（Z 越小越靠前），但更规范的做法是用 SpriteRenderer 的 Sorting Order。
- 在 `Update` 里疯狂改几百个 Transform 会触发物理重建，性能有代价。

## 9.2 RectTransform：UI 专用版 Transform

### 它是什么

UI 元素（Canvas 下的对象）用的是 `RectTransform`，它**继承自 Transform**，额外多了"矩形尺寸"和"锚点"概念。

### 五个关键概念

| 概念 | 含义 | 类比 |
| --- | --- | --- |
| `anchorMin` / `anchorMax` | 锚点（相对父容器 0~1 的两个点）。两点相同 = 固定定位；两点分开 = 随父级拉伸 | 图钉钉在父容器的哪个位置 |
| `pivot` | 轴心（自身 0~1 的点，旋转/缩放围绕它） | 手柄的位置 |
| `anchoredPosition` | 相对锚点的位置 | 离图钉多远 |
| `sizeDelta` | 尺寸（锚点不拉伸时 = 实际宽高；拉伸时 = 相对父级的增减量） | 宽高 |
| `offsetMin` / `offsetMax` | 四条边到锚点的距离（拉伸模式下的精确控制） | 边距 |

### 常见用法

```csharp
RectTransform rt = GetComponent<RectTransform>();

// 固定尺寸 + 右上角定位
rt.anchorMin = Vector2.one;      // (1,1) 右上
rt.anchorMax = Vector2.one;
rt.pivot = Vector2.one;
rt.anchoredPosition = new Vector2(-20, -20);
rt.sizeDelta = new Vector2(200, 80);

// 全屏拉伸（比如游戏结束遮罩）
rt.anchorMin = Vector2.zero;
rt.anchorMax = Vector2.one;
rt.offsetMin = Vector2.zero;     // 左下边距 0
rt.offsetMax = Vector2.zero;     // 右上边距 0
```

### 坑

- **锚点设错是"UI 在不同分辨率下跑飞"的头号原因**。在 Game 视图上方切换分辨率预览就能发现。
- `sizeDelta` 在拉伸锚点下**不是宽高**，是"比锚点范围大/小多少"。想要绝对宽高就先把 `anchorMin == anchorMax`。
- UI 的 `localScale` 一般保持 1，缩放交给 Canvas Scaler。

## 9.3 SpriteRenderer：把图"画"出来

### 它是什么

2D 里负责绘制的组件。它引用一个 `Sprite` 资源，把它画在对象位置上。

### 关键属性

| 属性 | 含义 |
| --- | --- |
| `sprite` | 显示哪张图 |
| `color` | 染色（白色 = 原色） |
| `flipX` / `flipY` | 水平/垂直翻转 |
| `sortingLayerName` | 排序层（Layer 名称） |
| `sortingOrder` | 层内顺序，**越大越靠前** |
| `material` | 材质（URP 下要注意用 Unlit 或加光源） |
| `drawMode` | `Simple` / `Sliced`（九宫格）/ `Tiled`（平铺） |
| `maskInteraction` | 与 SpriteMask 的遮罩关系 |

### 代码怎么用

```csharp
SpriteRenderer sr = GetComponent<SpriteRenderer>();

sr.sprite = someSprite;                 // 换图
sr.color = Color.red;                   // 染色
sr.flipX = true;                        // 水平翻转（敌人转身）
sr.sortingOrder = 10;                   // 画在前面
sr.enabled = false;                     // 不画（对象还在）

// 简易帧动画：按索引换 sprite
sr.sprite = frames[index];
```

### 排序规则（记这个顺序）

1. **Sorting Layer**（在 `Edit → Project Settings → Tags and Layers` 里定义）——优先级最高
2. **Order in Layer**（同一 Sorting Layer 内比较）
3. **Z 坐标**（2D 摄像机下，Z 越小越靠前；一般前两项就够了）

### 坑

- **"我的图是黑的"**：URP 模板下 Sprite 默认用受光材质，场景没光源就黑。加 `Global Light 2D`，或把 Material 换成 `Sprite-Unlit-Default`。
- **"图是粉的"**：材质/Shader 丢失（常见于把 Built-in 项目切到 URP）。重新指定材质。
- 改 `color` 会**相乘**：Sprite 本身带颜色时，结果会比预期暗。

## 9.4 Sorting Group：整组对象一起排序

### 它是什么

挂在"父对象"上，让**整棵子树作为一个整体**参与排序（子对象之间的相对顺序保留）。

### 什么时候用

- 角色由"身体 + 武器 + 血条"多个 Sprite 组成，想整体排在某个物体前/后
- 2048 的方块如果由"底板 + 数字 + 光效"三个 Sprite 组成，也能保证它们不散

```csharp
var sg = GetComponent<SortingGroup>();
sg.sortingOrder = 5;
```

### 坑

一旦挂了 Sorting Group，**子对象自己的 `sortingOrder` 不再和外部比较**，只在自己组内比较。

## 9.5 Camera：玩家看到的一切

### 它是什么

摄像机决定"屏幕上显示什么"。2D 用**正交（Orthographic）**投影。

### 关键属性

| 属性 | 含义 |
| --- | --- |
| `orthographic` | 2D 必须为 `true` |
| `orthographicSize` | **可见高度的一半**（Size=5 → 高度 10 个单位） |
| `fieldOfView` | 透视模式下的视角（2D 不用） |
| `clearFlags` | 背景怎么处理：`Skybox` / `Solid Color` / `Depth only` / `Don't Clear` |
| `backgroundColor` | `Solid Color` 时的背景色 |
| `cullingMask` | 只渲染哪些 Layer（分层渲染、小地图常用） |
| `depth` | 多个摄像机时的绘制顺序（大的后画、盖在上面） |
| `targetTexture` | 渲染到贴图（小地图、镜子、UI 相机） |

### 代码怎么用

```csharp
Camera cam = Camera.main;                 // Tag 为 MainCamera 的摄像机（场景里只该有一个）

// 视野尺寸
float height = cam.orthographicSize * 2f;
float width = height * (float)Screen.width / Screen.height;

// 屏幕坐标 ↔ 世界坐标（拖拽、点击物体必用）
Vector3 world = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                                                    Input.mousePosition.y,
                                                    -cam.transform.position.z));
Vector3 screen = cam.WorldToScreenPoint(transform.position);

// 让摄像机跟随（放在 LateUpdate 里）
transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
```

### 2D 常用配置

| 场景 | 设置 |
| --- | --- |
| 固定棋盘（2048、三消） | Size 调到刚好装下棋盘，摄像机不动 |
| 横版平台跳跃 | Size ≈ 5~6，跟随玩家（只跟 X 或 X+Y） |
| 俯视角 | Size ≈ 6~8，跟随玩家 |
| 需要背景色 | `clearFlags = Solid Color`，`backgroundColor = 深色` |

### 坑

- **`Camera.main` 每次调用有开销**：在 `Awake` 里缓存一次。
- 摄像机 Z 必须是负数（比如 -10），否则看不见 Z=0 的 2D 物体。
- 多个摄像机时用 `depth` 控制叠加顺序；UI 用 Canvas 的 Overlay 模式时不需要额外摄像机。

## 9.6 本章练习

1. **父子测试**：父对象 Scale 设 2，子对象 `localPosition` 设 `(1,0,0)`，观察子对象的世界 `position` 变成多少
2. 用代码让一个 Sprite 每秒绕自身中心转 90°（`transform.Rotate`）
3. 用 `RectTransform` 做一个"右上角固定的按钮"，把 Game 视图分辨率从 1920×1080 切到 800×600，确认它依然贴右上角
4. 用 `SpriteRenderer.sortingOrder` 让两个重叠方块的前后关系互相切换
5. 写一段代码把鼠标位置转成世界坐标，并让一个对象跟到鼠标下

**下一章**：2D 物理——Rigidbody2D、Collider2D 和那套"为什么没碰撞"的排查。
