# UI管理系统

这是一个完整的Unity UI管理系统，提供了UI栈管理、层级控制、预制件加载/卸载、生命周期管理、缓存池、事件通信等功能。

## 系统特性

### 1. UI栈管理
- 支持多层UI栈管理
- 自动处理UI的打开/关闭顺序
- 防止UI重复打开

### 2. 层级控制
- **Background (0)**: 背景层
- **Normal (1)**: 普通层
- **Popup (2)**: 弹窗层
- **Top (3)**: 顶层

### 3. 预制件加载/卸载
- 自动从Resources/UI/目录加载预制件
- 支持UI缓存池，提高性能
- 自动管理UI的生命周期

### 4. 生命周期方法
- `Init(data)`: 初始化UI
- `Open()`: 打开UI
- `Close()`: 关闭UI
- `Destroy()`: 销毁UI

### 5. 单例管理器
- 全局单例访问
- 自动初始化
- 场景切换时保持

### 6. UI缓存池
- 自动缓存已关闭的UI
- 减少实例化开销
- 支持手动清空缓存

### 7. 简单事件通信
- 支持UI间事件通信
- 类型安全的事件系统
- 自动清理事件监听

### 8. 输入阻断
- 弹窗层及以上自动阻断下层输入
- 可配置的输入控制

### 9. 基础Canvas适配
- 自动设置Canvas适配
- 支持安全区域适配
- 可配置的参考分辨率

### 10. UI查找工具
- 封装的GetComponent方法
- 组件缓存机制
- 路径查找支持

### 11. 主窗口系统 (WndBase)
- 管理页签和子窗口
- 页签切换自动显示对应的子窗口内容
- 自动管理页签按钮状态
- 主窗口关闭时自动关闭子窗口

### 12. 子窗口系统 (SubViewBase)
- 被主窗口管理的子窗口
- 自动查找父窗口引用
- 支持向父窗口发送事件
- 独立的生命周期管理

### 13. UI项系统 (UIItemBase)
- 用于列表项、单个UI元素等
- 支持对象池管理
- 数据绑定和更新
- 自动回收机制

## 使用方法

### 1. 创建UI类

```csharp
public class MyUI : UIBase
{
    protected override void OnInit(object data)
    {
        // 初始化逻辑
        if (data is MyUIData uiData)
        {
            SetText("TitleText", uiData.title);
        }
        
        // 设置按钮事件
        SetButtonClick("CloseButton", OnCloseClick);
    }
    
    protected override void OnOpen()
    {
        // 打开时的逻辑
    }
    
    protected override void OnClose()
    {
        // 关闭时的逻辑
    }
    
    private void OnCloseClick()
    {
        UIManager.Instance.CloseUI(uiName);
    }
}
```

### 2. 打开UI

```csharp
// 打开UI
MyUI ui = UIManager.Instance.OpenUI<MyUI>("MyUI", UILayer.Normal, data);

// 获取已打开的UI
MyUI existingUI = UIManager.Instance.GetUI<MyUI>("MyUI");
```

### 3. 关闭UI

```csharp
// 关闭指定UI
UIManager.Instance.CloseUI("MyUI");

// 关闭指定层级的所有UI
UIManager.Instance.CloseLayer(UILayer.Popup);

// 关闭所有UI
UIManager.Instance.CloseAllUI();
```

### 4. 事件通信

```csharp
// 注册事件
RegisterEvent<string>("OnDataChanged", OnDataChanged);

// 发送事件
SendEvent("OnDataChanged", "新数据");

// 注销事件
UnregisterEvent<string>("OnDataChanged", OnDataChanged);
```

### 5. 组件查找

```csharp
// 获取组件（带缓存）
Button button = GetComponent<Button>("CloseButton");
Text text = GetComponent<Text>("TitleText");
Image image = GetComponent<Image>("IconImage");

// 设置组件
SetText("TitleText", "新标题");
SetImage("IconImage", sprite);
SetButtonClick("ConfirmButton", OnConfirmClick);
```

### 6. 主窗口和子窗口使用

```csharp
// 创建主窗口
public class MyWindow : WndBase
{
    protected override void OnInit(object data)
    {
        // 设置页签文本
        SetTabText("CharacterTab", "角色");
        SetTabText("EquipmentTab", "装备");
        SetTabText("SkillTab", "技能");
    }
    
    protected override void OnTabSwitched(string tabName)
    {
        // 页签切换时的逻辑
        Debug.Log($"切换到页签: {tabName}");
    }
    
    protected override void OnSubViewSwitched(string subViewName, object data)
    {
        // 子窗口切换时的逻辑
        Debug.Log($"切换到子窗口: {subViewName}");
    }
}

// 创建子窗口
public class CharacterSubView : SubViewBase
{
    protected override void OnInit(object data)
    {
        // 初始化角色子窗口
    }
    
    protected override void OnOpen()
    {
        // 子窗口打开时的逻辑
    }
}

// 打开主窗口并切换到指定页签
UIManager.Instance.OpenWindowWithTab<MyWindow>("MyWindow", "CharacterTab", UILayer.Normal, data);

// 手动切换页签
MyWindow window = UIManager.Instance.GetWindow<MyWindow>("MyWindow");
window.SwitchTab("EquipmentTab");

// 页签操作
window.SetTabText("CharacterTab", "新文本");
window.SetTabEnabled("SkillTab", false);
window.SetTabVisible("LevelTab", false);
```

### 7. UI项使用

```csharp
// 创建UI项
public class CharacterItem : UIItemBase
{
    protected override void OnDataChanged(object data, int index)
    {
        // 数据改变时更新显示
        if (data is CharacterData characterData)
        {
            SetItemText("NameText", characterData.name);
            SetItemText("LevelText", $"等级: {characterData.level}");
        }
    }
    
    protected override void OnReset()
    {
        // 重置时的逻辑
        SetItemText("NameText", "");
        SetItemText("LevelText", "");
    }
}

// 使用UI项
CharacterItem item = GetComponent<CharacterItem>();
item.SetData(characterData, index);
item.SetItemClick(() => OnCharacterSelected(characterData));
```

## 文件结构

```
Assets/Scripts/Common/UI/
├── UIManager.cs              # UI管理器主类
├── UIBase.cs                 # UI基类
├── WndBase.cs                # 主窗口基类
├── SubViewBase.cs            # 子窗口基类
├── UIItemBase.cs             # UI项基类
├── UIEventManager.cs         # 事件管理器
├── UIHelper.cs               # UI工具类
├── UIExample.cs              # 示例UI类
├── UIWithSubviewExample.cs   # 主窗口和子窗口示例类
├── UISystemTest.cs           # 基础系统测试类
├── SubviewSystemTest.cs      # 子窗口系统测试类
└── README.md                 # 说明文档
```

## 预制件要求

1. UI预制件必须放在 `Resources/UI/` 目录下
2. 预制件必须挂载继承自 `UIBase` 的脚本
3. 预制件名称要与UI类名对应

## 子窗口预制件要求

1. 子窗口必须放在主窗口的子对象中
2. 子窗口必须挂载继承自 `UIBase` 或 `SubviewBase` 的脚本
3. 子窗口名称要与脚本中的子窗口名称对应
4. 页签按钮和内容必须按照约定命名

### 主窗口和子窗口预制件结构示例

```
MainWindow (WndBase)
├── TabContainer (Transform)           # 页签按钮容器
│   ├── CharacterTab (Button)         # 角色页签按钮
│   ├── EquipmentTab (Button)         # 装备页签按钮
│   └── SkillTab (Button)             # 技能页签按钮
├── SubviewContainer (Transform)      # 子窗口容器
│   ├── CharacterTab (SubViewBase)    # 角色子窗口
│   ├── EquipmentTab (SubViewBase)    # 装备子窗口
│   └── SkillTab (SubViewBase)        # 技能子窗口
└── ContentContainer (Transform)      # 内容容器
    ├── CharacterContent (GameObject) # 角色内容
    ├── EquipmentContent (GameObject) # 装备内容
    └── SkillContent (GameObject)     # 技能内容
```

### UI项预制件结构示例

```
CharacterItem (UIItemBase)
├── IconImage (Image)                 # 角色图标
├── NameText (Text)                   # 角色名称
├── LevelText (Text)                  # 角色等级
└── SelectButton (Button)             # 选择按钮
```

## 配置说明

### Canvas适配配置
- 参考分辨率: 1920x1080
- 适配模式: ScaleWithScreenSize
- 屏幕匹配模式: MatchWidthOrHeight
- 匹配值: 0.5 (宽度和高度各占50%)

### 层级配置
- 每个层级间隔100个排序值
- Background: 0
- Normal: 100
- Popup: 200
- Top: 300

## 性能优化

1. **缓存池**: 自动缓存已关闭的UI，减少实例化开销
2. **组件缓存**: UI组件查找结果会被缓存，提高查找效率
3. **事件清理**: 自动清理事件监听，防止内存泄漏
4. **层级管理**: 合理的层级排序，减少渲染开销

## 扩展功能

### 自定义动画
重写 `PlayOpenAnimation()` 和 `PlayCloseAnimation()` 方法来自定义UI动画。

### 自定义事件
继承 `UIEventManager` 来扩展事件系统功能。

### 自定义适配
使用 `UIHelper.SetupCanvasScaler()` 来自定义Canvas适配。

## 注意事项

1. UI预制件必须正确配置Canvas组件
2. 事件监听要在UI销毁前手动注销
3. 缓存池中的UI会在场景切换时自动清理
4. 建议在UI关闭时清理临时数据

## 测试

### 基础UI系统测试
使用 `UISystemTest` 类来测试系统功能：
- F1: 测试打开UI
- F2: 测试关闭UI
- F3: 测试关闭所有UI
- F4: 测试层级管理
- F5: 测试事件系统
- F6: 测试缓存池

### 子窗口系统测试
使用 `SubviewSystemTest` 类来测试子窗口功能：
- F1: 打开带有子窗口的UI
- F2: 切换子窗口
- F3: 切换页签
- F4: 关闭UI
- F5: 测试页签操作
- F6: 测试子窗口操作 