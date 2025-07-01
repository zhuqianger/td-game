using UnityEngine;
using System.Collections.Generic;

namespace TDGame.Stage
{
    /// <summary>
    /// 示例关卡配置
    /// 展示如何配置不同类型的关卡
    /// </summary>
    public class ExampleStages : MonoBehaviour
    {
        [Header("示例关卡配置")]
        public List<StageConfig> exampleStageConfigs;
        
        [Header("示例敌人配置")]
        public List<EnemyConfig> exampleEnemyConfigs;
        
        [Header("示例天气配置")]
        public List<WeatherConfig> exampleWeatherConfigs;
        
        private void Awake()
        {
            CreateExampleStageConfigs();
            CreateExampleEnemyConfigs();
            CreateExampleWeatherConfigs();
        }
        
        /// <summary>
        /// 创建示例关卡配置
        /// </summary>
        private void CreateExampleStageConfigs()
        {
            exampleStageConfigs = new List<StageConfig>();
            
            // 示例1：主线关卡 - 第一章第一关
            StageConfig mainStoryStage = new StageConfig
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
                weather = WeatherType.Clear,
                enemyAttackBoost = false,
                costRecoveryReduction = false
            };
            
            // 设置地形
            mainStoryStage.terrainMap = new TerrainType[8, 6];
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    if (x == 0 || x == 7 || y == 0 || y == 5)
                    {
                        mainStoryStage.terrainMap[x, y] = TerrainType.UnDeployable;
                    }
                    else if (x == 2 && y == 2)
                    {
                        mainStoryStage.terrainMap[x, y] = TerrainType.HighGround;
                    }
                    else
                    {
                        mainStoryStage.terrainMap[x, y] = TerrainType.Deployable;
                    }
                }
            }
            
            // 设置敌潮波次
            mainStoryStage.enemyWaves = new List<EnemyWave>
            {
                new EnemyWave
                {
                    waveNumber = 1,
                    startTime = 5f,
                    isBossWave = false,
                    enemies = new List<EnemySpawn>
                    {
                        new EnemySpawn
                        {
                            enemyId = "normal_enemy",
                            spawnPosition = new Vector3(-8, 0, 0),
                            spawnDelay = 0f,
                            count = 3
                        }
                    }
                },
                new EnemyWave
                {
                    waveNumber = 2,
                    startTime = 20f,
                    isBossWave = false,
                    enemies = new List<EnemySpawn>
                    {
                        new EnemySpawn
                        {
                            enemyId = "elite_enemy",
                            spawnPosition = new Vector3(-8, 0, 0),
                            spawnDelay = 0f,
                            count = 1
                        }
                    }
                }
            };
            
            exampleStageConfigs.Add(mainStoryStage);
            
            // 示例2：资源收集关卡
            StageConfig resourceStage = new StageConfig
            {
                stageId = "CE-5",
                stageName = "CE-5 龙门币",
                stageType = StageType.Resource,
                difficulty = StageDifficulty.Normal,
                initialLifePoints = 2,
                initialDeployCost = 15,
                costRecoveryRate = 1.5f,
                costRecoveryInterval = 0.8f,
                mapSize = new Vector2Int(10, 8),
                weather = WeatherType.Clear,
                enemyAttackBoost = false,
                costRecoveryReduction = false
            };
            
            // 设置地形
            resourceStage.terrainMap = new TerrainType[10, 8];
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (x == 0 || x == 9 || y == 0 || y == 7)
                    {
                        resourceStage.terrainMap[x, y] = TerrainType.UnDeployable;
                    }
                    else if ((x == 3 || x == 6) && (y == 2 || y == 5))
                    {
                        resourceStage.terrainMap[x, y] = TerrainType.HighGround;
                    }
                    else
                    {
                        resourceStage.terrainMap[x, y] = TerrainType.Deployable;
                    }
                }
            }
            
            // 设置敌潮波次
            resourceStage.enemyWaves = new List<EnemyWave>
            {
                new EnemyWave
                {
                    waveNumber = 1,
                    startTime = 3f,
                    isBossWave = false,
                    enemies = new List<EnemySpawn>
                    {
                        new EnemySpawn
                        {
                            enemyId = "normal_enemy",
                            spawnPosition = new Vector3(-10, 0, 0),
                            spawnDelay = 0f,
                            count = 5
                        }
                    }
                },
                new EnemyWave
                {
                    waveNumber = 2,
                    startTime = 15f,
                    isBossWave = false,
                    enemies = new List<EnemySpawn>
                    {
                        new EnemySpawn
                        {
                            enemyId = "elite_enemy",
                            spawnPosition = new Vector3(-10, 0, 0),
                            spawnDelay = 0f,
                            count = 2
                        }
                    }
                }
            };
            
            exampleStageConfigs.Add(resourceStage);
            
            // 示例3：突袭关卡
            StageConfig challengeStage = new StageConfig
            {
                stageId = "1-1-challenge",
                stageName = "第一章 第一关 突袭",
                stageType = StageType.MainStory,
                difficulty = StageDifficulty.Challenge,
                initialLifePoints = 2,
                initialDeployCost = 8,
                costRecoveryRate = 0.5f,
                costRecoveryInterval = 2f,
                mapSize = new Vector2Int(8, 6),
                weather = WeatherType.Sandstorm,
                enemyAttackBoost = true,
                enemyAttackMultiplier = 1.5f,
                costRecoveryReduction = true,
                costRecoveryMultiplier = 0.5f
            };
            
            // 复制地形
            challengeStage.terrainMap = new TerrainType[8, 6];
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    challengeStage.terrainMap[x, y] = mainStoryStage.terrainMap[x, y];
                }
            }
            
            // 设置敌潮波次
            challengeStage.enemyWaves = new List<EnemyWave>
            {
                new EnemyWave
                {
                    waveNumber = 1,
                    startTime = 3f,
                    isBossWave = false,
                    enemies = new List<EnemySpawn>
                    {
                        new EnemySpawn
                        {
                            enemyId = "elite_enemy",
                            spawnPosition = new Vector3(-8, 0, 0),
                            spawnDelay = 0f,
                            count = 2
                        }
                    }
                },
                new EnemyWave
                {
                    waveNumber = 2,
                    startTime = 15f,
                    isBossWave = true,
                    enemies = new List<EnemySpawn>
                    {
                        new EnemySpawn
                        {
                            enemyId = "boss_enemy",
                            spawnPosition = new Vector3(-8, 0, 0),
                            spawnDelay = 0f,
                            count = 1
                        }
                    }
                }
            };
            
            exampleStageConfigs.Add(challengeStage);
        }
        
        /// <summary>
        /// 创建示例敌人配置
        /// </summary>
        private void CreateExampleEnemyConfigs()
        {
            exampleEnemyConfigs = new List<EnemyConfig>();
            
            // 普通敌人
            EnemyConfig normalEnemy = new EnemyConfig
            {
                enemyId = "normal_enemy",
                enemyName = "普通敌人",
                enemyType = EnemyType.Normal,
                rarity = 1,
                maxHP = 500,
                attack = 100,
                defense = 50,
                magicResistance = 0,
                moveSpeed = 2f,
                blockCount = 1,
                isFlying = false,
                isStealth = false,
                canDestroyTerrain = false,
                stealthRange = 0f,
                pathPoints = new List<Vector3>
                {
                    new Vector3(-8, 0, 0),
                    new Vector3(0, 0, 0),
                    new Vector3(8, 0, 0)
                },
                currentPathIndex = 0
            };
            exampleEnemyConfigs.Add(normalEnemy);
            
            // 精英敌人
            EnemyConfig eliteEnemy = new EnemyConfig
            {
                enemyId = "elite_enemy",
                enemyName = "精英敌人",
                enemyType = EnemyType.Elite,
                rarity = 2,
                maxHP = 1000,
                attack = 150,
                defense = 80,
                magicResistance = 10,
                moveSpeed = 1.5f,
                blockCount = 2,
                isFlying = false,
                isStealth = false,
                canDestroyTerrain = false,
                stealthRange = 0f,
                pathPoints = new List<Vector3>
                {
                    new Vector3(-8, 0, 0),
                    new Vector3(0, 0, 0),
                    new Vector3(8, 0, 0)
                },
                currentPathIndex = 0
            };
            exampleEnemyConfigs.Add(eliteEnemy);
            
            // BOSS敌人
            EnemyConfig bossEnemy = new EnemyConfig
            {
                enemyId = "boss_enemy",
                enemyName = "BOSS敌人",
                enemyType = EnemyType.Boss,
                rarity = 3,
                maxHP = 3000,
                attack = 200,
                defense = 120,
                magicResistance = 20,
                moveSpeed = 1f,
                blockCount = 3,
                isFlying = false,
                isStealth = false,
                canDestroyTerrain = true,
                stealthRange = 0f,
                pathPoints = new List<Vector3>
                {
                    new Vector3(-8, 0, 0),
                    new Vector3(0, 0, 0),
                    new Vector3(8, 0, 0)
                },
                currentPathIndex = 0
            };
            exampleEnemyConfigs.Add(bossEnemy);
            
            // 飞行敌人
            EnemyConfig flyingEnemy = new EnemyConfig
            {
                enemyId = "flying_enemy",
                enemyName = "飞行敌人",
                enemyType = EnemyType.Flying,
                rarity = 2,
                maxHP = 800,
                attack = 120,
                defense = 30,
                magicResistance = 0,
                moveSpeed = 3f,
                blockCount = 1,
                isFlying = true,
                isStealth = false,
                canDestroyTerrain = false,
                stealthRange = 0f,
                pathPoints = new List<Vector3>
                {
                    new Vector3(-8, 2, 0),
                    new Vector3(0, 2, 0),
                    new Vector3(8, 2, 0)
                },
                currentPathIndex = 0
            };
            exampleEnemyConfigs.Add(flyingEnemy);
            
            // 隐匿敌人
            EnemyConfig stealthEnemy = new EnemyConfig
            {
                enemyId = "stealth_enemy",
                enemyName = "隐匿敌人",
                enemyType = EnemyType.Stealth,
                rarity = 2,
                maxHP = 600,
                attack = 80,
                defense = 40,
                magicResistance = 0,
                moveSpeed = 2.5f,
                blockCount = 1,
                isFlying = false,
                isStealth = true,
                canDestroyTerrain = false,
                stealthRange = 3f,
                pathPoints = new List<Vector3>
                {
                    new Vector3(-8, 0, 0),
                    new Vector3(0, 0, 0),
                    new Vector3(8, 0, 0)
                },
                currentPathIndex = 0
            };
            exampleEnemyConfigs.Add(stealthEnemy);
        }
        
        /// <summary>
        /// 创建示例天气配置
        /// </summary>
        private void CreateExampleWeatherConfigs()
        {
            exampleWeatherConfigs = new List<WeatherConfig>();
            
            // 晴朗天气
            WeatherConfig clearWeather = new WeatherConfig
            {
                weatherType = WeatherType.Clear,
                weatherName = "晴朗",
                description = "天气晴朗，无特殊效果",
                duration = -1f, // 永久
                weatherColor = Color.white,
                effects = new List<WeatherEffect>()
            };
            exampleWeatherConfigs.Add(clearWeather);
            
            // 沙暴天气
            WeatherConfig sandstormWeather = new WeatherConfig
            {
                weatherType = WeatherType.Sandstorm,
                weatherName = "沙暴",
                description = "沙暴天气，所有干员持续扣血",
                duration = 60f,
                weatherColor = new Color(0.8f, 0.6f, 0.2f),
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
            exampleWeatherConfigs.Add(sandstormWeather);
            
            // 迷雾天气
            WeatherConfig fogWeather = new WeatherConfig
            {
                weatherType = WeatherType.Fog,
                weatherName = "迷雾",
                description = "迷雾天气，隐匿敌人更难被发现",
                duration = 45f,
                weatherColor = new Color(0.7f, 0.7f, 0.7f),
                effects = new List<WeatherEffect>
                {
                    new WeatherEffect
                    {
                        effectType = WeatherEffectType.Stealth,
                        value = 50f,
                        interval = 1f,
                        description = "增强隐匿敌人的隐匿效果"
                    },
                    new WeatherEffect
                    {
                        effectType = WeatherEffectType.Visibility,
                        value = 20f,
                        interval = 1f,
                        description = "减少干员视野范围20%"
                    }
                }
            };
            exampleWeatherConfigs.Add(fogWeather);
        }
        
        /// <summary>
        /// 获取示例关卡配置
        /// </summary>
        public List<StageConfig> GetExampleStageConfigs()
        {
            return exampleStageConfigs;
        }
        
        /// <summary>
        /// 获取示例敌人配置
        /// </summary>
        public List<EnemyConfig> GetExampleEnemyConfigs()
        {
            return exampleEnemyConfigs;
        }
        
        /// <summary>
        /// 获取示例天气配置
        /// </summary>
        public List<WeatherConfig> GetExampleWeatherConfigs()
        {
            return exampleWeatherConfigs;
        }
        
        /// <summary>
        /// 根据ID获取示例关卡配置
        /// </summary>
        public StageConfig GetExampleStageConfig(string stageId)
        {
            return exampleStageConfigs.Find(config => config.stageId == stageId);
        }
        
        /// <summary>
        /// 根据ID获取示例敌人配置
        /// </summary>
        public EnemyConfig GetExampleEnemyConfig(string enemyId)
        {
            return exampleEnemyConfigs.Find(config => config.enemyId == enemyId);
        }
        
        /// <summary>
        /// 根据类型获取示例天气配置
        /// </summary>
        public WeatherConfig GetExampleWeatherConfig(WeatherType weatherType)
        {
            return exampleWeatherConfigs.Find(config => config.weatherType == weatherType);
        }
        
        /// <summary>
        /// 打印所有示例配置信息
        /// </summary>
        public void PrintAllConfigs()
        {
            Debug.Log("=== 示例关卡配置 ===");
            foreach (var config in exampleStageConfigs)
            {
                Debug.Log($"关卡: {config.stageName} ({config.stageId})");
                Debug.Log($"  类型: {config.stageType}, 难度: {config.difficulty}");
                Debug.Log($"  生命: {config.initialLifePoints}, 费用: {config.initialDeployCost}");
                Debug.Log($"  地图: {config.mapSize.x}x{config.mapSize.y}");
                Debug.Log("---");
            }
            
            Debug.Log("=== 示例敌人配置 ===");
            foreach (var config in exampleEnemyConfigs)
            {
                Debug.Log($"敌人: {config.enemyName} ({config.enemyId})");
                Debug.Log($"  类型: {config.enemyType}, 稀有度: {config.rarity}");
                Debug.Log($"  血量: {config.maxHP}, 攻击: {config.attack}, 防御: {config.defense}");
                Debug.Log($"  移动: {config.moveSpeed}, 阻挡: {config.blockCount}");
                Debug.Log("---");
            }
            
            Debug.Log("=== 示例天气配置 ===");
            foreach (var config in exampleWeatherConfigs)
            {
                Debug.Log($"天气: {config.weatherName}");
                Debug.Log($"  描述: {config.description}");
                Debug.Log($"  持续时间: {(config.duration > 0 ? config.duration + "秒" : "永久")}");
                Debug.Log($"  效果数量: {config.effects.Count}");
                Debug.Log("---");
            }
        }
    }
} 