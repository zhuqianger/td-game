# 任务系统使用说明

## 概述

任务系统是塔防游戏的核心系统之一，负责管理玩家的任务进度、奖励发放和游戏引导。系统支持多种任务类型，包括日常任务、周常任务、主线任务、活动任务和见习任务。

## 系统架构

### 核心组件

1. **Task.cs** - 任务基础类
   - 管理单个任务的状态和进度
   - 处理任务目标完成和奖励发放
   - 提供任务信息查询接口

2. **TaskManager.cs** - 任务管理器
   - 管理所有任务实例
   - 处理任务刷新和重置
   - 提供任务查询和管理接口

3. **TaskUI.cs** - 任务界面
   - 显示任务列表和详情
   - 处理任务筛选和操作
   - 提供用户交互接口

4. **ExampleTasks.cs** - 示例任务配置
   - 提供各种类型的任务示例
   - 便于快速配置和测试

5. **TaskSystemTest.cs** - 系统测试
   - 提供完整的测试功能
   - 支持自动测试和手动测试

## 任务类型

### 1. 日常任务 (Daily)
- **刷新时间**: 每日04:00
- **特点**: 基础资源获取，每日重置
- **示例**: 完成3次作战关卡、制造1份赤金、消耗100理智

### 2. 周常任务 (Weekly)
- **刷新时间**: 每周一04:00
- **特点**: 稀有资源获取，每周重置
- **示例**: 累计完成20次作战、剿灭作战累计400杀、精英化1名干员

### 3. 主线任务 (MainStory)
- **特点**: 剧情推进，一次性完成
- **示例**: 通关TR-15、精英化干员、解锁基建系统

### 4. 活动任务 (Activity)
- **特点**: 限时活动，活动结束后关闭
- **示例**: 活动关卡累计胜利10次、收集活动代币500个

### 5. 见习任务 (Tutorial)
- **特点**: 新手引导，一次性完成
- **示例**: 完成新手教程、招募1名干员

## 任务目标类型

### 基础目标类型
- `StageComplete` - 完成关卡
- `StageCompleteCount` - 完成关卡次数
- `OperatorRecruit` - 招募干员
- `OperatorElite` - 精英化干员
- `ItemCraft` - 制造物品
- `ItemCollect` - 收集物品
- `ResourceSpend` - 消耗资源
- `EnemyKill` - 击杀敌人
- `EnemyKillCount` - 击杀敌人数
- `Custom` - 自定义目标

## 使用方法

### 1. 基本设置

```csharp
// 获取任务管理器
TaskManager taskManager = FindObjectOfType<TaskManager>();

// 获取任务UI
TaskUI taskUI = FindObjectOfType<TaskUI>();
```

### 2. 创建任务配置

```csharp
TaskConfig taskConfig = new TaskConfig
{
    taskId = "daily_001",
    taskName = "日常作战",
    taskType = TaskType.Daily,
    description = "完成3次作战关卡",
    targets = new List<TaskTarget>
    {
        new TaskTarget
        {
            targetType = TaskTargetType.StageCompleteCount,
            targetId = "any_stage",
            targetName = "完成作战关卡",
            requiredAmount = 3,
            description = "完成任意作战关卡3次"
        }
    },
    rewards = new List<TaskReward>
    {
        new TaskReward
        {
            rewardId = "lmd",
            rewardName = "龙门币",
            rewardType = "currency",
            amount = 2000
        }
    },
    isTimeLimited = true,
    refreshInterval = 24
};
```

### 3. 添加任务

```csharp
Task task = taskManager.AddTask(taskConfig);
```

### 4. 更新任务进度

```csharp
// 更新特定任务进度
taskManager.UpdateTaskProgress(TaskTargetType.StageCompleteCount, "any_stage", 1);

// 更新共享进度的任务
taskManager.UpdateSharedTaskProgress(TaskTargetType.StageCompleteCount, "any_stage", 1);
```

### 5. 领取奖励

```csharp
// 领取单个任务奖励
if (task.status == TaskStatus.Completed)
{
    task.ClaimReward();
}

// 领取所有可领取的奖励
int claimedCount = taskManager.ClaimAllRewards();
```

### 6. 任务刷新

```csharp
// 刷新日常任务
taskManager.RefreshDailyTasks();

// 刷新周常任务
taskManager.RefreshWeeklyTasks();

// 刷新活动任务
taskManager.RefreshActivityTasks();
```

## 任务链系统

### 创建任务链

```csharp
TaskConfig task1 = new TaskConfig
{
    taskId = "chain_001",
    taskName = "任务链1",
    isChainTask = true,
    nextTaskId = "chain_002"
};

TaskConfig task2 = new TaskConfig
{
    taskId = "chain_002",
    taskName = "任务链2",
    prerequisiteTaskId = "chain_001"
};
```

### 任务链激活

当前置任务完成并领取奖励后，后续任务会自动激活。

## 特殊机制

### 1. 共享进度

```csharp
TaskConfig sharedTask = new TaskConfig
{
    shareProgress = true,
    sharedTaskIds = new string[] { "task_001", "task_002" }
};
```

### 2. 隐藏任务

```csharp
TaskConfig hiddenTask = new TaskConfig
{
    isHidden = true
};
```

### 3. 首通奖励

```csharp
TaskConfig firstTimeTask = new TaskConfig
{
    isFirstTimeReward = true
};
```

## UI使用

### 1. 显示任务界面

```csharp
taskUI.SetUIVisibility(true);
```

### 2. 刷新任务列表

```csharp
taskUI.RefreshTaskList();
```

### 3. 设置任务筛选

```csharp
taskUI.SetTaskFilter(TaskType.Daily);
taskUI.SetTaskFilter(TaskType.Weekly);
taskUI.SetTaskFilter(TaskType.MainStory);
```

### 4. 显示任务详情

```csharp
taskUI.ShowTaskDetail(task);
```

## 测试功能

### 1. 自动测试

```csharp
TaskSystemTest test = FindObjectOfType<TaskSystemTest>();
test.StartAutoTest();
```

### 2. 手动测试

```csharp
// 测试任务初始化
test.TestTaskInitialization();

// 测试任务进度更新
test.TestTaskProgressUpdate();

// 测试任务完成
test.TestTaskCompletion();
```

### 3. 测试日志

```csharp
// 清空测试日志
test.ClearTestLog();

// 保存测试日志
test.SaveTestLog();
```

## 配置示例

### 日常任务配置

```csharp
// 完成3次作战关卡
TaskConfig dailyCombat = new TaskConfig
{
    taskId = "daily_combat",
    taskName = "日常作战",
    taskType = TaskType.Daily,
    description = "完成3次作战关卡",
    targets = new List<TaskTarget>
    {
        new TaskTarget
        {
            targetType = TaskTargetType.StageCompleteCount,
            targetId = "any_stage",
            targetName = "完成作战关卡",
            requiredAmount = 3,
            description = "完成任意作战关卡3次"
        }
    },
    rewards = new List<TaskReward>
    {
        new TaskReward
        {
            rewardId = "lmd",
            rewardName = "龙门币",
            rewardType = "currency",
            amount = 2000
        },
        new TaskReward
        {
            rewardId = "originite_prime",
            rewardName = "合成玉",
            rewardType = "currency",
            amount = 100
        }
    },
    isTimeLimited = true,
    refreshInterval = 24
};
```

### 周常任务配置

```csharp
// 累计完成20次作战
TaskConfig weeklyCombat = new TaskConfig
{
    taskId = "weekly_combat",
    taskName = "作战达人",
    taskType = TaskType.Weekly,
    description = "累计完成20次作战关卡",
    targets = new List<TaskTarget>
    {
        new TaskTarget
        {
            targetType = TaskTargetType.StageCompleteCount,
            targetId = "any_stage",
            targetName = "完成作战关卡",
            requiredAmount = 20,
            description = "累计完成20次作战关卡"
        }
    },
    rewards = new List<TaskReward>
    {
        new TaskReward
        {
            rewardId = "originite",
            rewardName = "至纯源石",
            rewardType = "currency",
            amount = 1
        }
    },
    isTimeLimited = true,
    refreshInterval = 168 // 7天
};
```

## 注意事项

1. **任务ID唯一性**: 确保每个任务的ID在系统中是唯一的
2. **时间设置**: 活动任务需要正确设置开始和结束时间
3. **前置条件**: 任务链需要正确设置前置任务ID
4. **奖励发放**: 确保奖励ID与资源系统匹配
5. **进度更新**: 及时更新任务进度，避免进度丢失
6. **UI刷新**: 任务状态变化后及时刷新UI显示

## 扩展功能

### 1. 自定义目标类型

可以通过继承或扩展`TaskTargetType`枚举来添加新的目标类型。

### 2. 自定义奖励类型

可以通过扩展`TaskReward`类来支持更复杂的奖励机制。

### 3. 任务条件系统

可以添加更复杂的任务激活条件，如等级要求、干员要求等。

### 4. 任务成就系统

可以基于任务完成情况实现成就系统。

## 性能优化

1. **对象池**: 对于频繁创建销毁的UI元素使用对象池
2. **事件优化**: 合理使用事件系统，避免内存泄漏
3. **数据缓存**: 缓存常用的任务数据，减少重复计算
4. **异步加载**: 对于大量任务数据使用异步加载

## 故障排除

### 常见问题

1. **任务不激活**: 检查前置任务是否完成
2. **进度不更新**: 检查目标ID和类型是否正确
3. **奖励不发放**: 检查奖励配置和资源系统
4. **UI不刷新**: 检查事件绑定和UI更新逻辑

### 调试方法

1. 使用`TaskSystemTest`进行系统测试
2. 查看控制台日志输出
3. 检查任务状态和进度数据
4. 验证UI组件绑定状态 