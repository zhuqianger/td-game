# ConfigManager 使用说明

## 功能特性

ConfigManager 现在支持将数组格式的JSON配置文件自动转换为以id为键的Map格式，提高配置查询效率。

### 主要改进

1. **自动字段名统一**: 自动将配置文件中的第一个字段（如 `enemyId`、`operatorId` 等）统一改为 `id`
2. **Map格式缓存**: 在内存中维护以id为键的配置字典，提高查询效率
3. **向后兼容**: 保持原有的API接口不变，新增Map相关功能

## 使用方法

### 基本用法

```csharp
// 获取原始配置JSON
string enemyConfig = ConfigManager.GetConfig("enemy_config");

// 获取特定ID的配置
var enemy = ConfigManager.GetConfigById<EnemyConfig>("enemy_config", 1001);

// 检查配置是否存在
bool exists = ConfigManager.HasConfig("enemy_config", 1001);

// 获取所有配置ID
List<int> allIds = ConfigManager.GetAllConfigIds("enemy_config");

// 获取配置总数
int count = ConfigManager.GetConfigCount("enemy_config");
```

### 配置类定义

```csharp
[Serializable]
public class EnemyConfig
{
    public int id;           // 统一使用id字段
    public string enemyName;
    public int hp;
    public int attack;
    // ... 其他字段
}
```

### 辅助类使用

```csharp
// 使用预定义的辅助类
EnemyConfig enemy = EnemyConfigHelper.GetEnemy(1001);
EnemyConfig[] allEnemies = EnemyConfigHelper.GetAllEnemies();
bool exists = EnemyConfigHelper.IsEnemyExists(1001);
```

## 性能优势

- **O(1) 查询**: 通过id直接查询配置，无需遍历数组
- **内存缓存**: 配置数据在内存中以Map形式缓存
- **自动转换**: 启动时自动处理所有配置文件

## 注意事项

1. 配置文件必须包含 `id` 字段（或自动转换后的字段）
2. 建议使用强类型的配置类而不是 `object`
3. 配置文件的修改需要重新启动游戏才能生效

## 支持的配置文件

- `enemy_config.json` - 敌人配置
- `operators_config.json` - 干员配置  
- `skills_config.json` - 技能配置
- `talents_config.json` - 天赋配置
- 其他符合格式的配置文件
