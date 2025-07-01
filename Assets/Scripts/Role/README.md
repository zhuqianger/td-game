# 干员系统使用说明

## 概述

本干员系统参考明日方舟的设计，实现了完整的干员管理、技能系统、天赋系统等功能。系统采用模块化设计，易于扩展和维护。

## 核心组件

### 1. Operator.cs - 干员基础类
干员的核心类，包含所有基础属性和核心功能。

**主要功能：**
- 基础属性管理（生命值、攻击力、防御力等）
- 部署和撤退逻辑
- 伤害计算（物理/法术）
- 等级和精英化系统
- 技能和天赋管理

**关键属性：**
```csharp
public OperatorStats baseStats;       // 基础属性
public OperatorStats currentStats;    // 当前属性（包含加成）
public int eliteLevel;                // 精英化等级 (0-2)
public int level;                     // 等级 (1-90)
public List<Skill> skills;            // 技能列表
public List<Talent> talents;          // 天赋列表
```

### 2. Skill.cs - 技能系统
管理干员的主动、被动和自动技能。

**技能类型：**
- `Passive`: 被动技能，始终生效
- `Active`: 主动技能，需要手动触发
- `Auto`: 自动技能，满足条件自动触发

**技能效果类型：**
- `Damage`: 伤害
- `Heal`: 治疗
- `Buff`: 增益
- `Debuff`: 减益
- `Summon`: 召唤
- `Movement`: 移动
- `Special`: 特殊效果

### 3. Talent.cs - 天赋系统
管理干员的被动天赋效果。

**天赋类型：**
- `Self`: 自身增益
- `Team`: 团队增益
- `Enemy`: 敌人减益
- `Special`: 特殊效果

### 4. OperatorFactory.cs - 干员工厂
负责创建和管理干员实例。

**主要功能：**
- 根据配置创建干员
- 管理技能和天赋数据库
- 验证干员配置

### 5. OperatorManager.cs - 干员管理器
管理所有干员实例和战斗逻辑。

**主要功能：**
- 部署和撤退干员
- 更新干员状态
- 团队管理
- 战斗辅助功能

### 6. OperatorUI.cs - 干员UI
显示干员信息和状态。

**UI组件：**
- 基础信息显示
- 属性面板
- 技能和天赋槽
- 状态指示器

## 使用示例

### 1. 创建干员配置

```csharp
OperatorConfig config = new OperatorConfig
{
    operatorId = "silver_ash",
    operatorName = "银灰",
    profession = OperatorProfession.Vanguard,
    rarity = 6,
    baseStats = new OperatorStats
    {
        maxHP = 2000,
        attack = 350,
        defense = 150,
        deployCost = 12,
        blockCount = 2,
        attackInterval = 1.3f
    },
    attackRange = new Vector2Int(2, 2),
    skillIds = new string[] { "skill_vanguard_charge" },
    talentIds = new string[] { "talent_leader" }
};
```

### 2. 创建技能

```csharp
Skill skill = ScriptableObject.CreateInstance<Skill>();
skill.skillName = "冲锋";
skill.skillType = SkillType.Active;
skill.description = "立即回复2点部署费用，攻击力+50%";
skill.cooldown = 30f;
skill.duration = 10f;
skill.spCost = 20;
skill.effects = new SkillEffect[]
{
    new SkillEffect
    {
        effectType = SkillEffectType.Buff,
        value = 50f,
        duration = 10f,
        description = "攻击力+50%"
    }
};
```

### 3. 创建天赋

```csharp
Talent talent = ScriptableObject.CreateInstance<Talent>();
talent.talentName = "领袖";
talent.talentType = TalentType.Team;
talent.description = "所有友军攻击力+10%";
talent.isAlwaysActive = true;
talent.requiredEliteLevel = 1;
talent.effects = new TalentEffect[]
{
    new TalentEffect
    {
        effectName = "attack",
        value = 10f,
        targetType = "team",
        condition = "always",
        description = "团队攻击力+10%"
    }
};
```

### 4. 使用干员管理器

```csharp
// 添加可用干员
operatorManager.AddAvailableOperator("silver_ash");

// 部署干员
operatorManager.DeployOperator("silver_ash", new Vector3(0, 0, 0));

// 获取已部署干员
List<Operator> deployed = operatorManager.GetDeployedOperators();

// 撤退干员
operatorManager.RetreatOperator(deployed[0]);
```

## 职业特性

### 先锋 (Vanguard)
- 可以回复部署费用
- 通常阻挡数为2
- 攻击力中等，防御力较低

### 近卫 (Guard)
- 平衡的攻击和防御
- 阻挡数为1-2
- 适合前排输出

### 重装 (Defender)
- 高防御力，高生命值
- 阻挡数为3
- 适合承受伤害

### 狙击 (Sniper)
- 高攻击力，远程攻击
- 阻挡数为1
- 适合后排输出

### 术师 (Caster)
- 法术伤害，高攻击力
- 阻挡数为1
- 适合对付高防御敌人

### 医疗 (Medic)
- 治疗友军
- 阻挡数为1
- 不攻击敌人

### 辅助 (Supporter)
- 提供增益效果
- 阻挡数为1
- 攻击力较低

### 特种 (Specialist)
- 特殊机制
- 阻挡数可变
- 功能多样

## 属性计算

### 物理伤害计算
```
最终伤害 = 敌方ATK - 我方DEF
如果DEF ≥ 敌方ATK，则伤害 = 1
```

### 法术伤害计算
```
最终伤害 = 敌方ATK × (1 - 我方RES/100)
```

### 等级加成
```
属性 = 基础属性 × (1 + (等级 - 1) × 0.02)
```

### 精英化加成
```
属性 = 基础属性 × (1 + 精英化等级 × 0.1)
```

## 扩展指南

### 添加新职业
1. 在`OperatorProfession`枚举中添加新职业
2. 在`OperatorUI.cs`的`GetProfessionName`方法中添加职业名称
3. 根据需要调整职业特性

### 添加新技能效果
1. 在`SkillEffectType`枚举中添加新效果类型
2. 在`Skill.cs`的`ApplyEffect`方法中实现效果逻辑
3. 在`Talent.cs`中实现对应的天赋效果

### 添加新天赋类型
1. 在`TalentType`枚举中添加新类型
2. 在`Talent.cs`的`ApplyEffect`方法中实现对应逻辑

## 注意事项

1. 所有干员配置都应该通过`OperatorFactory`创建
2. 技能和天赋的ID必须唯一
3. 干员的部署位置需要检查是否可部署
4. 技能冷却和技力消耗需要正确管理
5. 天赋的激活条件需要正确实现

## 性能优化

1. 使用对象池管理干员实例
2. 缓存查找表提高查询效率
3. 批量更新减少Update调用
4. 使用事件系统减少耦合

## 调试工具

1. 使用`ExampleOperators`查看示例配置
2. 使用`OperatorUI`查看干员状态
3. 在`OperatorManager`中添加调试日志
4. 使用Unity Inspector查看实时数据 