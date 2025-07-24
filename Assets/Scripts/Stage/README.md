# 关卡系统使用说明

## 概述

本关卡系统参考明日方舟的设计，实现了完整的关卡管理、敌人系统、地形系统、天气系统等功能。系统采用模块化设计，支持多种关卡类型和难度设置。

## 核心组件

### 1. Stage.cs - 关卡基础类
关卡的核心类，包含所有基础规则和核心机制。

**主要功能：**
- 关卡规则管理（生命点数、部署费用等）
- 敌潮波次管理
- 关卡状态控制（开始、暂停、完成、失败）
- 费用回复机制
- 关卡进度跟踪

**关键属性：**
```csharp
public StageConfig config;              // 关卡配置
public int currentLifePoints;           // 当前生命点数
public int currentDeployCost;           // 当前部署费用
public float stageTime;                 // 关卡时间
public int currentWave;                 // 当前波次
public bool isStageComplete;            // 关卡是否完成
public bool isStageFailed;              // 关卡是否失败
```

### 2. StageManager.cs - 关卡管理器
管理所有关卡实例和关卡切换。

**主要功能：**
- 关卡加载和切换
- 关卡状态管理
- 关卡配置验证
- 关卡进度统计

**关键方法：**
```csharp
public bool LoadStage(string stageId)   // 加载关卡
public void StartStage()                // 开始关卡
public void PauseStage()                // 暂停关卡
public void RestartStage()              // 重启关卡
public void ExitStage()                 // 退出关卡
```

### 3. EnemyManager.cs - 敌人管理器
管理敌人的生成、移动和战斗。

**主要功能：**
- 敌人生成和销毁
- 敌人移动路径管理
- 敌人战斗逻辑
- 敌人状态统计

**敌人类型：**
- `Normal`: 普通敌人
- `Elite`: 精英敌人
- `Boss`: BOSS敌人
- `Flying`: 飞行敌人
- `Stealth`: 隐匿敌人

### 4. TerrainManager.cs - 地形管理器
管理地图地形和环境机关。

**地形类型：**
- `Deployable`: 可部署区域
- `UnDeployable`: 不可部署区域
- `HighGround`: 高台（仅远程）
- `Pit`: 坑洞
- `EnergyRune`: 能量符文
- `IceCrystal`: 冰冻源石

**主要功能：**
- 地形生成和管理
- 环境机关效果
- 部署位置验证
- 地形信息查询

### 5. WeatherManager.cs - 天气管理器
管理天气效果和环境影响。

**天气类型：**
- `Clear`: 晴朗
- `Sandstorm`: 沙暴
- `Fog`: 迷雾

**天气效果：**
- `Damage`: 伤害效果
- `Slow`: 减速效果
- `Visibility`: 视野效果
- `Stealth`: 隐匿效果

### 6. StageUI.cs - 关卡UI
显示关卡信息和状态。

**UI组件：**
- 关卡信息显示
- 状态面板（生命、费用、进度）
- 天气信息
- 敌人统计
- 控制按钮

## 关卡类型

### 1. 主线关卡 (MainStory)
- 推进剧情，解锁新系统
- 固定难度设置
- 通常包含多个章节

### 2. 资源收集 (Resource)
- 反复刷取材料/龙门币
- 如：CE-5、LS-5
- 优化收益的关卡

### 3. 剿灭作战 (Annihilation)
- 限时击杀400名敌人
- 每周积分奖励
- 高难度挑战

### 4. 危机合约 (CrisisContract)
- 自选词条叠加难度
- 冲击高层级排名
- 高度自定义

### 5. 集成战略 (IntegratedStrategy)
- 随机地图+干员招募
- Roguelike模式
- 每次体验不同

## 关卡难度

### 1. 普通 (Normal)
- 基础难度设置
- 标准敌人属性
- 正常费用回复

### 2. 突袭 (Challenge)
- 增加限制条件
- 敌人强化/费用削减
- 更高挑战性

### 3. 合约 (Contract)
- 自选难度词条
- 高度自定义
- 极限挑战

## 使用示例

### 1. 创建关卡配置

```csharp
StageConfig config = new StageConfig
{
    stageId = "1-1",
    stageName = "第一章 第一关",
    stageType = StageType.MainStory,
    difficulty = StageDifficulty.Normal,
    initialLifePoints = 3,
    initialDeployCost = 10,
    costRecoveryRate = 1f,
    costRecoveryInterval = 1f,
    mapSize = new Vector2Int(8, 6),
    weather = WeatherType.Clear
};

// 设置地形
config.terrainMap = new TerrainType[8, 6];
// ... 地形设置

// 设置敌潮波次
config.enemyWaves = new List<EnemyWave>
{
    new EnemyWave
    {
        waveNumber = 1,
        startTime = 5f,
        enemies = new List<EnemySpawn>
        {
            new EnemySpawn
            {
                enemyId = "normal_enemy",
                spawnPosition = new Vector3(-8, 0, 0),
                count = 3
            }
        }
    }
};
```

### 2. 创建敌人配置

```csharp
EnemyConfig enemyConfig = new EnemyConfig
{
    enemyId = "normal_enemy",
    enemyName = "普通敌人",
    enemyType = EnemyType.Normal,
    maxHP = 500,
    attack = 100,
    defense = 50,
    moveSpeed = 2f,
    blockCount = 1,
    pathPoints = new List<Vector3>
    {
        new Vector3(-8, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(8, 0, 0)
    }
};
```

### 3. 创建天气配置

```csharp
WeatherConfig weatherConfig = new WeatherConfig
{
    weatherType = WeatherType.Sandstorm,
    weatherName = "沙暴",
    description = "沙暴天气，所有干员持续扣血",
    duration = 60f,
    effects = new List<WeatherEffect>
    {
        new WeatherEffect
        {
            effectType = WeatherEffectType.Damage,
            value = 10f,
            interval = 5f,
            description = "每5秒对所有干员造成10点伤害"
        }
    }
};
```

### 4. 使用关卡管理器

```csharp
// 加载关卡
stageManager.LoadStage("1-1");

// 开始关卡
stageManager.StartStage();

// 暂停关卡
stageManager.PauseStage();

// 重启关卡
stageManager.RestartStage();

// 退出关卡
stageManager.ExitStage();
```

## 关卡机制

### 1. 生命点数系统
- 初始值通常为1~3点
- 每漏过1个敌人扣除1点
- 生命归零则关卡失败

### 2. 部署费用系统
- 随时间自动回复
- 击杀敌人或使用先锋技能可额外回复
- 突袭模式可能削减回复速度

### 3. 敌潮波次系统
- 分多批次出现敌人
- 波次间隔显示倒计时
- BOSS常出现在最终波次

### 4. 地形障碍系统
- 不可部署区域（黑色格子）
- 高台（仅远程干员）
- 地面坑洞（敌人坠落即死）

### 5. 环境机关系统
- 能量符文（周期性回复技力）
- 冰冻源石（周期性冻结单位）

### 6. 天气系统
- 沙暴（全场干员持续扣血）
- 迷雾（隐匿未受攻击的敌人）

## 敌人属性

### 1. 基础属性
- **生命值 (HP)**: 承受伤害上限
- **攻击力 (ATK)**: 物理/法术伤害基础值
- **防御力 (DEF)**: 减少受到的物理伤害
- **法术抗性 (RES)**: 百分比减免法术伤害
- **移动速度**: 敌人移动速度
- **阻挡数**: 同时阻挡的干员数量

### 2. 特殊属性
- **飞行单位**: 仅受对空干员攻击
- **隐匿**: 未进入攻击范围时不显示
- **地形破坏**: 可破坏地形开辟新路径

### 3. 索敌逻辑
- 默认沿最短路径冲向目标点
- 受嘲讽/束缚等效果影响
- 优先攻击最后部署的干员

## 数值计算公式

### 1. 生命点数扣除规则
```
剩余生命 = 当前生命 - 漏过敌人数量
```

### 2. 费用自动回复
```
每秒回复值 = 基础值（通常1点） × 关卡倍率（突袭模式可能为0.5）
```

### 3. 敌人移动速度
```
实际移速 = 基础移速 × (1 - 减速效果叠加)
```

### 4. 物理伤害计算
```
最终伤害 = 敌方ATK - 我方DEF
如果DEF ≥ 敌方ATK，则伤害 = 1
```

### 5. 法术伤害计算
```
最终伤害 = 敌方ATK × (1 - 我方RES/100)
```

## 扩展指南

### 1. 添加新关卡类型
1. 在`StageType`枚举中添加新类型
2. 在`StageUI.cs`的`GetStageTypeName`方法中添加类型名称
3. 根据需要调整关卡特性

### 2. 添加新敌人类型
1. 在`EnemyType`枚举中添加新类型
2. 在`Enemy.cs`中实现对应的特殊逻辑
3. 在UI中显示敌人类型信息

### 3. 添加新地形类型
1. 在`TerrainType`枚举中添加新类型
2. 在`TerrainManager.cs`中实现对应的生成逻辑
3. 添加对应的视觉效果

### 4. 添加新天气类型
1. 在`WeatherType`枚举中添加新类型
2. 在`WeatherManager.cs`中实现对应的效果逻辑
3. 添加对应的视觉效果

### 5. 添加新天气效果
1. 在`WeatherEffectType`枚举中添加新效果类型
2. 在`WeatherManager.cs`的`ApplyWeatherEffect`方法中实现效果逻辑

## 注意事项

1. 所有关卡配置都应该通过`StageManager`管理
2. 敌人和天气的ID必须唯一
3. 地形地图的大小必须与配置一致
4. 敌潮波次的时间设置要合理
5. 天气效果的持续时间要平衡

## 性能优化

1. 使用对象池管理敌人实例
2. 缓存地形查找结果
3. 批量更新减少Update调用
4. 使用事件系统减少耦合

## 调试工具

1. 使用`ExampleStages`查看示例配置
2. 使用`StageUI`查看关卡状态
3. 在`StageManager`中添加调试日志
4. 使用Unity Inspector查看实时数据

## 总结

本关卡系统提供了一个完整、可扩展的关卡管理解决方案，具有以下特点：

1. **完整性**：涵盖关卡的所有核心功能
2. **可扩展性**：支持新功能和新类型的添加
3. **性能优化**：采用多种优化策略
4. **易用性**：提供清晰的API和文档
5. **可维护性**：模块化设计，易于维护

通过这个系统，开发者可以快速构建复杂的关卡管理功能，为塔防游戏提供强大的关卡系统支持。 