using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDGame.Stage
{
    /// <summary>
    /// 关卡类型枚举
    /// </summary>
    public enum StageType
    {
        MainStory,      // 主线关卡
        Resource,       // 资源收集
        Annihilation,   // 剿灭作战
        CrisisContract, // 危机合约
        IntegratedStrategy // 集成战略
    }
    
    /// <summary>
    /// 关卡难度枚举
    /// </summary>
    public enum StageDifficulty
    {
        Normal,         // 普通
        Challenge,      // 突袭
        Contract        // 合约
    }
    
    /// <summary>
    /// 地形类型枚举
    /// </summary>
    public enum TerrainType
    {
        Deployable,     // 可部署
        UnDeployable,   // 不可部署
        HighGround,     // 高台（仅远程）
        Pit,           // 坑洞
        EnergyRune,    // 能量符文
        IceCrystal     // 冰冻源石
    }
    
    /// <summary>
    /// 天气类型枚举
    /// </summary>
    public enum WeatherType
    {
        Clear,         // 晴朗
        Sandstorm,     // 沙暴
        Fog            // 迷雾
    }
    
    /// <summary>
    /// 关卡配置数据
    /// </summary>
    [Serializable]
    public class StageConfig
    {
        [Header("基础信息")]
        public string stageId;              // 关卡ID
        public string stageName;            // 关卡名称
        public StageType stageType;         // 关卡类型
        public StageDifficulty difficulty;  // 关卡难度
        
        [Header("关卡规则")]
        public int initialLifePoints;       // 初始生命点数
        public int initialDeployCost;       // 初始部署费用
        public float costRecoveryRate;      // 费用回复速度
        public float costRecoveryInterval;  // 费用回复间隔
        
        [Header("敌人设置")]
        public List<EnemyWave> enemyWaves;  // 敌潮波次
        public float waveInterval;          // 波次间隔
        
        [Header("地形设置")]
        public Vector2Int mapSize;          // 地图大小
        public TerrainType[,] terrainMap;   // 地形地图
        
        [Header("环境设置")]
        public WeatherType weather;         // 天气类型
        public List<EnvironmentObject> environmentObjects; // 环境机关
        
        [Header("特殊条件")]
        public bool enemyAttackBoost;       // 敌方攻击加成
        public float enemyAttackMultiplier; // 敌方攻击倍数
        public bool costRecoveryReduction;  // 费用回复削减
        public float costRecoveryMultiplier; // 费用回复倍数
    }
    
    /// <summary>
    /// 敌潮波次
    /// </summary>
    [Serializable]
    public class EnemyWave
    {
        public int waveNumber;              // 波次编号
        public float startTime;             // 开始时间
        public List<EnemySpawn> enemies;    // 敌人生成列表
        public bool isBossWave;             // 是否为BOSS波次
    }
    
    /// <summary>
    /// 敌人生成
    /// </summary>
    [Serializable]
    public class EnemySpawn
    {
        public string enemyId;              // 敌人ID
        public Vector3 spawnPosition;       // 生成位置
        public float spawnDelay;            // 生成延迟
        public int count;                   // 生成数量
    }
    
    /// <summary>
    /// 环境机关
    /// </summary>
    [Serializable]
    public class EnvironmentObject
    {
        public string objectId;             // 机关ID
        public Vector3 position;            // 位置
        public float effectRadius;          // 影响半径
        public float effectInterval;        // 效果间隔
        public string effectType;           // 效果类型
    }
    
    /// <summary>
    /// 关卡类
    /// </summary>
    public class Stage : MonoBehaviour
    {
        [Header("关卡配置")]
        public StageConfig config;
        
        [Header("运行时状态")]
        public int currentLifePoints;       // 当前生命点数
        public int currentDeployCost;       // 当前部署费用
        public float stageTime;             // 关卡时间
        public int currentWave;             // 当前波次
        public bool isStageComplete;        // 关卡是否完成
        public bool isStageFailed;          // 关卡是否失败
        
        [Header("组件引用")]
        public StageManager stageManager;
        public TerrainManager terrainManager;
        public EnemyManager enemyManager;
        public WeatherManager weatherManager;
        
        // 事件
        public event Action<Stage> OnStageStart;
        public event Action<Stage> OnStageComplete;
        public event Action<Stage> OnStageFailed;
        public event Action<int> OnLifePointsChanged;
        public event Action<int> OnDeployCostChanged;
        public event Action<int> OnWaveChanged;
        
        private float lastCostRecoveryTime;
        private List<EnemyWave> remainingWaves;
        
        private void Awake()
        {
            InitializeStage();
        }
        
        private void Start()
        {
            StartStage();
        }
        
        private void Update()
        {
            UpdateStage();
        }
        
        /// <summary>
        /// 初始化关卡
        /// </summary>
        private void InitializeStage()
        {
            if (config == null)
            {
                Debug.LogError("关卡配置为空！");
                return;
            }
            
            // 初始化状态
            currentLifePoints = config.initialLifePoints;
            currentDeployCost = config.initialDeployCost;
            stageTime = 0f;
            currentWave = 0;
            isStageComplete = false;
            isStageFailed = false;
            
            // 复制敌潮波次
            remainingWaves = new List<EnemyWave>(config.enemyWaves);
            
            // 初始化地形
            if (terrainManager != null)
            {
                terrainManager.InitializeTerrain(config.terrainMap, config.mapSize);
            }
            
            // 初始化天气
            if (weatherManager != null)
            {
                weatherManager.SetWeather(config.weather);
            }
            
            // 初始化环境机关
            InitializeEnvironmentObjects();
        }
        
        /// <summary>
        /// 开始关卡
        /// </summary>
        public void StartStage()
        {
            Debug.Log($"开始关卡: {config.stageName}");
            OnStageStart?.Invoke(this);
        }
        
        /// <summary>
        /// 更新关卡状态
        /// </summary>
        private void UpdateStage()
        {
            if (isStageComplete || isStageFailed) return;
            
            stageTime += Time.deltaTime;
            
            // 更新费用回复
            UpdateCostRecovery();
            
            // 更新敌潮波次
            UpdateEnemyWaves();
            
            // 检查关卡状态
            CheckStageStatus();
        }
        
        /// <summary>
        /// 更新费用回复
        /// </summary>
        private void UpdateCostRecovery()
        {
            if (Time.time - lastCostRecoveryTime >= config.costRecoveryInterval)
            {
                float recoveryAmount = config.costRecoveryRate;
                
                // 应用难度修正
                if (config.costRecoveryReduction)
                {
                    recoveryAmount *= config.costRecoveryMultiplier;
                }
                
                AddDeployCost(Mathf.RoundToInt(recoveryAmount));
                lastCostRecoveryTime = Time.time;
            }
        }
        
        /// <summary>
        /// 更新敌潮波次
        /// </summary>
        private void UpdateEnemyWaves()
        {
            for (int i = remainingWaves.Count - 1; i >= 0; i--)
            {
                var wave = remainingWaves[i];
                
                if (stageTime >= wave.startTime)
                {
                    // 生成敌人
                    if (enemyManager != null)
                    {
                        enemyManager.SpawnWave(wave);
                    }
                    
                    currentWave = wave.waveNumber;
                    OnWaveChanged?.Invoke(currentWave);
                    
                    remainingWaves.RemoveAt(i);
                    
                    Debug.Log($"开始第 {wave.waveNumber} 波敌人");
                }
            }
        }
        
        /// <summary>
        /// 检查关卡状态
        /// </summary>
        private void CheckStageStatus()
        {
            // 检查失败条件
            if (currentLifePoints <= 0)
            {
                FailStage();
                return;
            }
            
            // 检查胜利条件
            if (remainingWaves.Count == 0 && enemyManager != null && enemyManager.GetActiveEnemyCount() == 0)
            {
                CompleteStage();
                return;
            }
        }
        
        /// <summary>
        /// 完成关卡
        /// </summary>
        public void CompleteStage()
        {
            if (isStageComplete || isStageFailed) return;
            
            isStageComplete = true;
            Debug.Log($"关卡完成: {config.stageName}");
            OnStageComplete?.Invoke(this);
        }
        
        /// <summary>
        /// 失败关卡
        /// </summary>
        public void FailStage()
        {
            if (isStageComplete || isStageFailed) return;
            
            isStageFailed = true;
            Debug.Log($"关卡失败: {config.stageName}");
            OnStageFailed?.Invoke(this);
        }
        
        /// <summary>
        /// 敌人漏过
        /// </summary>
        public void EnemyLeaked()
        {
            currentLifePoints = Mathf.Max(0, currentLifePoints - 1);
            OnLifePointsChanged?.Invoke(currentLifePoints);
            
            Debug.Log($"敌人漏过！剩余生命: {currentLifePoints}");
        }
        
        /// <summary>
        /// 添加部署费用
        /// </summary>
        public void AddDeployCost(int amount)
        {
            currentDeployCost += amount;
            OnDeployCostChanged?.Invoke(currentDeployCost);
        }
        
        /// <summary>
        /// 消耗部署费用
        /// </summary>
        public bool ConsumeDeployCost(int amount)
        {
            if (currentDeployCost >= amount)
            {
                currentDeployCost -= amount;
                OnDeployCostChanged?.Invoke(currentDeployCost);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 获取当前部署费用
        /// </summary>
        public int GetCurrentDeployCost()
        {
            return currentDeployCost;
        }
        
        /// <summary>
        /// 获取剩余生命点数
        /// </summary>
        public int GetRemainingLifePoints()
        {
            return currentLifePoints;
        }
        
        /// <summary>
        /// 获取关卡进度
        /// </summary>
        public float GetStageProgress()
        {
            if (config.enemyWaves.Count == 0) return 1f;
            
            int completedWaves = config.enemyWaves.Count - remainingWaves.Count;
            return (float)completedWaves / config.enemyWaves.Count;
        }
        
        /// <summary>
        /// 初始化环境机关
        /// </summary>
        private void InitializeEnvironmentObjects()
        {
            if (config.environmentObjects == null) return;
            
            foreach (var envObj in config.environmentObjects)
            {
                // 这里应该创建环境机关实例
                // 比如能量符文、冰冻源石等
                Debug.Log($"初始化环境机关: {envObj.objectId} at {envObj.position}");
            }
        }
        
        /// <summary>
        /// 获取地形类型
        /// </summary>
        public TerrainType GetTerrainType(Vector2Int position)
        {
            if (config.terrainMap == null || 
                position.x < 0 || position.x >= config.mapSize.x ||
                position.y < 0 || position.y >= config.mapSize.y)
            {
                return TerrainType.UnDeployable;
            }
            
            return config.terrainMap[position.x, position.y];
        }
        
        /// <summary>
        /// 检查位置是否可部署
        /// </summary>
        public bool IsPositionDeployable(Vector2Int position)
        {
            TerrainType terrain = GetTerrainType(position);
            return terrain == TerrainType.Deployable || terrain == TerrainType.HighGround;
        }
        
        /// <summary>
        /// 检查是否为高台位置
        /// </summary>
        public bool IsHighGround(Vector2Int position)
        {
            return GetTerrainType(position) == TerrainType.HighGround;
        }
        
        /// <summary>
        /// 暂停关卡
        /// </summary>
        public void PauseStage()
        {
            Time.timeScale = 0f;
        }
        
        /// <summary>
        /// 恢复关卡
        /// </summary>
        public void ResumeStage()
        {
            Time.timeScale = 1f;
        }
        
        /// <summary>
        /// 重启关卡
        /// </summary>
        public void RestartStage()
        {
            // 重置状态
            currentLifePoints = config.initialLifePoints;
            currentDeployCost = config.initialDeployCost;
            stageTime = 0f;
            currentWave = 0;
            isStageComplete = false;
            isStageFailed = false;
            
            // 重新初始化
            remainingWaves = new List<EnemyWave>(config.enemyWaves);
            lastCostRecoveryTime = 0f;
            
            // 清理敌人
            if (enemyManager != null)
            {
                enemyManager.ClearAllEnemies();
            }
            
            // 恢复时间缩放
            Time.timeScale = 1f;
            
            Debug.Log($"重启关卡: {config.stageName}");
        }
    }
} 