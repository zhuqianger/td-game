# 塔防游戏配置文件说明文档

本文档详细说明了塔防游戏后端系统中各个JSON配置文件的字段含义和作用。

## 📁 配置文件概览

系统包含以下配置文件：
- `professions.json` - 职业配置
- `operators.json` - 干员基础信息配置
- `operator_base_attributes.json` - 干员基础属性配置
- `operator_growths.json` - 干员属性成长配置
- `operator_levels.json` - 干员等级经验配置
- `operator_attributes.json` - 干员属性修正配置
- `skills.json` - 技能系统配置
- `talents.json` - 天赋系统配置
- `items.json` - 道具物品配置
- `backpack_types.json` - 背包类型配置
- `stage_config.json` - 关卡基础配置
- `enemy_config.json` - 敌人配置
- `stage_enemy_config.json` - 关卡敌人配置

---

## 🎯 干员系统配置

### 1. professions.json - 职业配置

| 字段名 | 类型 | 说明 | 示例值 |
|--------|------|------|--------|
| `id` | Integer | 职业唯一ID | 1 |
| `name` | String | 职业名称 | "近卫" |
| `description` | String | 职业描述 | "近战物理输出职业..." |
| `attackInterval` | Float | 攻击间隔(秒) | 1.2 |
| `redeployTime` | Integer | 再部署时间(秒) | 70 |
| `sortOrder` | Integer | 排序顺序 | 1 |

**职业ID对应关系：**
- 1 - 近卫
- 2 - 狙击
- 3 - 术师
- 4 - 重装
- 5 - 医疗
- 6 - 先锋
- 7 - 辅助
- 8 - 特种

### 2. operators.json - 干员基础信息

| 字段名 | 类型 | 说明 | 示例值 |
|--------|------|------|--------|
| `id` | Integer | 干员唯一标识ID | 1001 |
| `operatorName` | String | 干员名称 | "银灰" |
| `professionId` | Integer | 职业ID | 1 |
| `rarity` | Integer | 稀有度(1-6星) | 6 |
| `skillIds` | Array<Integer> | 技能ID列表 | [1, 2, 3] |
| `talentIds` | Array<Integer> | 天赋ID列表 | [1, 2] |

**职业ID对应关系：**
- 1 - 近卫
- 2 - 狙击
- 3 - 术师
- 4 - 重装
- 5 - 医疗
- 6 - 先锋
- 7 - 辅助
- 8 - 特种



### 2. operators.json - 干员基础信息

| 字段名 | 类型 | 说明 | 示例值 |
|--------|------|------|--------|
| `id` | Integer | 干员唯一标识ID | 1001 |
| `operatorName` | String | 干员名称 | "银灰" |
| `professionId` | Integer | 职业ID | 1 |
| `rarity` | Integer | 稀有度(1-6星) | 6 |
| `skillIds` | Array<Integer> | 技能ID列表 | [1, 2, 3] |
| `talentIds` | Array<Integer> | 天赋ID列表 | [1, 2] |

### 3. operator_base_attributes.json - 干员基础属性

| 字段名 | 类型 | 说明 | 示例值 |
|--------|------|------|--------|
| `id` | Integer | 配置唯一ID，通过公式 id = operatorId * 10 + eliteLevel 计算 | 10010 |
| `operatorId` | Integer | 干员ID | 1001 |
| `eliteLevel` | Integer | 精英化等级(0-2) | 0 |
| `baseHP` | Integer | 基础生命值 | 800 |
| `baseAttack` | Integer | 基础攻击力 | 150 |
| `baseDefense` | Integer | 基础防御力 | 80 |
| `baseMagicResistance` | Integer | 基础法术抗性 | 0 |
| `attackRange` | Array<Array<Integer>> | 攻击范围，相对于干员位置的坐标列表 [[x_offset, y_offset], ...] | [[0, 0], [1, 0]] |
| `deployCost` | Integer | 部署费用 | 18 |
| `blockCount` | Integer | 阻挡数量 | 1 |

### 4. operator_growths.json - 干员属性成长

| 字段名 | 类型 | 说明 | 示例值 |
|--------|------|------|--------|
| `id` | Integer | 配置唯一ID，通过公式 id = operatorId * 10 + eliteLevel 计算 | 10010 |
| `operatorId` | Integer | 干员ID | 1001 |
| `eliteLevel` | Integer | 精英化等级 | 0 |
| `hpGrowthPerLevel` | Integer | 每级生命值成长 | 100 |
| `attackGrowthPerLevel` | Integer | 每级攻击力成长 | 20 |
| `defenseGrowthPerLevel` | Integer | 每级防御力成长 | 10 |
| `magicResistanceGrowthPerLevel` | Integer | 每级法术抗性成长 | 5 |

### 5. operator_levels.json - 干员等级经验

| 字段名 | 类型 | 说明 | 示例值 |
|--------|------|------|--------|
| `id` | Integer | 配置唯一ID | 1001 |
| `rarity` | Integer | 稀有度 | 1 |
| `eliteLevel` | Integer | 精英化等级 | 0 |
| `level` | Integer | 等级 | 1 |
| `needExp` | Integer | 升级所需经验 | 0 |

### 6. operator_attributes.json - 干员属性修正

| 字段名 | 类型 | 说明 | 示例值 |
|--------|------|------|--------|
| `id` | Integer | 配置唯一ID | 1 |
| `name` | String | 干员名称 | "阿米娅" |
| `rarity` | Integer | 稀有度 | 5 |
| `professionId` | Integer | 职业ID | 3 |
| `hpModifier` | Float | 生命值修正系数 | 1.0 |
| `attackModifier` | Float | 攻击力修正系数 | 1.2 |
| `defenseModifier` | Float | 防御力修正系数 | 0.8 |
| `magicResistanceModifier` | Float | 法术抗性修正系数 | 1.5 |
| `description` | String | 干员描述 | "罗德岛领袖..." |

---

## ⚡ 技能系统配置

### skills.json - 技能配置

| 字段名 | 类型 | 说明 | 示例值 |
|--------|------|------|--------|
| `id` | Integer | 技能唯一ID | 1 |
| `skillName` | String | 技能名称 | "强力击" |
| `skillType` | String | 技能类型 | "PASSIVE" |
| `description` | String | 技能描述 | "攻击力提升" |
| `spCost` | Integer | SP消耗 | 0 |
| `duration` | Integer | 持续时间(秒) | 0 |
| `cooldown` | Float | 冷却时间(秒) | 0.0 |
| `triggerChance` | Float | 触发概率 | 1.0 |
| `spRecovery` | Integer | SP回复量 | 0 |
| `autoTrigger` | Boolean | 是否自动触发 | true |
| `triggerCondition` | String | 触发条件 | "always" |
| `effects` | Array | 技能效果列表 | [] |

**技能类型说明：**
- `PASSIVE` - 被动技能
- `ACTIVE` - 主动技能

**触发条件说明：**
- `always` - 始终生效
- `manual` - 手动触发
- `attack` - 攻击时触发
- `defend` - 防御时触发

---

## 🌟 天赋系统配置

### talents.json - 天赋配置

| 字段名 | 类型 | 说明 | 示例值 |
|--------|------|------|--------|
| `id` | Integer | 天赋唯一ID | 1 |
| `talentName` | String | 天赋名称 | "领袖气质" |
| `talentType` | String | 天赋类型 | "PASSIVE" |
| `description` | String | 天赋描述 | "部署时，所有友方干员攻击力+10%" |
| `effects` | Array | 天赋效果列表 | [] |
| `isAlwaysActive` | Boolean | 是否始终生效 | true |
| `activationCondition` | String | 激活条件 | "deploy" |
| `activationChance` | Float | 激活概率 | 1.0 |
| `requiredEliteLevel` | Integer | 所需精英化等级 | 0 |
| `requiredLevel` | Integer | 所需等级 | 1 |

**激活条件说明：**
- `deploy` - 部署时
- `attack` - 攻击时
- `defend` - 防御时
- `always` - 始终生效

---

## 🎒 背包系统配置

### 1. backpack_types.json - 背包类型

| 字段名 | 类型 | 说明 | 示例值 |
|--------|------|------|--------|
| `id` | Integer | 类型唯一ID | 1 |
| `name` | String | 类型名称 | "材料背包" |
| `description` | String | 类型描述 | "存放各种材料道具的背包" |
| `sortOrder` | Integer | 排序顺序 | 1 |

**背包类型ID对应关系：**
- 1 - 材料背包
- 2 - 货币背包
- 3 - 装饰背包
- 4 - 皮肤背包
- 5 - 消耗品背包
- 6 - 招募背包

### 2. items.json - 道具物品

| 字段名 | 类型 | 说明 | 示例值 |
|--------|------|------|--------|
| `id` | Integer | 道具唯一ID | 1001 |
| `name` | String | 道具名称 | "经验卡" |
| `quality` | Integer | 道具品质(1-4) | 1 |
| `backpackTypeId` | Integer | 所属背包类型ID | 1 |
| `description` | String | 道具描述 | "用于提升干员等级..." |

**道具品质说明：**
- 1 - 普通
- 2 - 优秀
- 3 - 稀有
- 4 - 史诗

---

## 🗺️ 关卡系统配置

### 9. stage_config.json - 关卡配置

| 字段名 | 类型 | 说明 | 示例值 |
|--------|------|------|--------|
| `id` | Integer | 关卡唯一ID | 101 |
| `stageName` | String | 关卡名称 | "第一章 第一关" |
| `stageType` | Integer | 关卡类型 | 1 |
| `difficulty` | Integer | 难度等级 | 1 |
| `chapter` | Integer | 章节编号 | 1 |

### 10. enemy_config.json - 敌人配置

| 字段名 | 类型 | 说明 | 示例值 |
|--------|------|------|--------|
| `enemyId` | Integer | 敌人唯一ID | 1001 |
| `enemyName` | String | 敌人名称 | "普通敌人" |
| `enemyType` | Integer | 敌人类型 | 1 |
| `hp` | Integer | 生命值 | 500 |
| `attack` | Integer | 攻击力 | 100 |
| `defense` | Integer | 防御力 | 50 |
| `magicResistance` | Integer | 魔法抗性 | 0 |
| `moveSpeed` | Float | 移动速度 | 2.0 |
| `blockCount` | Integer | 阻挡数量 | 1 |
| `attackRange` | Array<Array<Integer>> | 攻击范围，相对于敌人位置的坐标列表 [[x_offset, y_offset], ...] | [[0, 0], [1, 0]] |

### 11. stage_enemy_config.json - 关卡敌人配置

| 字段名 | 类型 | 说明 | 示例值 |
|--------|------|------|--------|
| `id` | Integer | 关卡唯一ID | 101 |
| `mapConfig` | Object | 地图配置 | - |
| `mapConfig.entryPoints` | Array<Object> | 敌人进入点列表 | [{ "id": 1, "position": [0, 0], "name": "入口1" }] |
| `mapConfig.targetPoint` | Object | 敌方目标点 | { "position": [10, 3], "name": "目标点" } |
| `spawns` | Array<Object> | 敌人生成配置 | - |
| `spawns[].enemyId` | Integer | 敌人ID | 1001 |
| `spawns[].spawnTime` | Integer | 出现时间(秒) | 0 |
| `spawns[].entryPointId` | Integer | 进入点ID | 1 |

---

## 🔧 配置使用说明

### 数据关联关系

1. **干员系统**：
   - `professions.json` 定义职业基础信息和属性（攻击间隔、再部署时间）
   - `operators.json` 定义干员基础信息
   - `operator_base_attributes.json` 提供基础属性模板（包含部署费用、阻挡数）
   - `operator_growths.json` 计算等级成长
   - `operator_levels.json` 管理升级经验
   - `operator_attributes.json` 应用个体修正

2. **技能天赋**：
   - `skills.json` 定义技能效果
   - `talents.json` 定义天赋效果
   - 通过ID关联到具体干员

3. **背包系统**：
   - `backpack_types.json` 定义背包分类
   - `items.json` 定义具体道具
   - 通过`backpackTypeId`关联

4. **关卡系统**：
   - `stage_config.json` 定义关卡基础信息
   - `enemy_config.json` 定义敌人属性
   - `stage_enemy_config.json` 配置关卡敌人

### 注意事项

- 所有ID字段必须为整数类型
- 配置的主键名必须为`id`
- 数据库只保存必要的游戏数据，不保存前端展示字段
- 属性计算：最终属性 = 基础属性 × 修正系数 + 成长值
- 技能和天赋通过概率和条件系统触发
