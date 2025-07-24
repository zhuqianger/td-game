using System.Collections.Generic;
using UnityEngine;
using TDGame.Role;

namespace TDGame.Stage
{
    /// <summary>
    /// 关卡管理器类
    /// </summary>
    public class StageManager : MonoBehaviour
    {
        [Header("关卡设置")]
        public List<StageConfig> stageConfigs;
        public Stage currentStage;
        public StageConfig currentStageConfig;
        
        [Header("组件引用")]
        public OperatorManager operatorManager;
        public TerrainManager terrainManager;
        public EnemyManager enemyManager;
        public WeatherManager weatherManager;
        public StageUI stageUI;
        
        [Header("关卡状态")]
        public bool isStageActive;
        public StageType currentStageType;
        public StageDifficulty currentDifficulty;
        
        // 事件
        public System.Action<Stage> OnStageLoaded;
        public System.Action<Stage> OnStageStarted;
        public System.Action<Stage> OnStageCompleted;
        public System.Action<Stage> OnStageFailed;
        
        private Dictionary<string, StageConfig> stageConfigLookup;
        private List<Stage> completedStages;
        
        private void Awake()
        {
            InitializeStageManager();
        }
        
        /// <summary>
        /// 初始化关卡管理器
        /// </summary>
        private void InitializeStageManager()
        {
            stageConfigLookup = new Dictionary<string, StageConfig>();
            completedStages = new List<Stage>();
            
            // 初始化查找表
            foreach (var config in stageConfigs)
            {
                stageConfigLookup[config.stageId] = config;
            }
            
            // 获取组件引用
            if (operatorManager == null)
                operatorManager = FindObjectOfType<OperatorManager>();
                
            if (terrainManager == null)
                terrainManager = FindObjectOfType<TerrainManager>();
                
            if (enemyManager == null)
                enemyManager = FindObjectOfType<EnemyManager>();
                
            if (weatherManager == null)
                weatherManager = FindObjectOfType<WeatherManager>();
                
            if (stageUI == null)
                stageUI = FindObjectOfType<StageUI>();
        }
        
        /// <summary>
        /// 加载关卡
        /// </summary>
        public bool LoadStage(string stageId)
        {
            if (!stageConfigLookup.ContainsKey(stageId))
            {
                Debug.LogError($"关卡配置不存在: {stageId}");
                return false;
            }
            
            // 清理当前关卡
            if (currentStage != null)
            {
                Destroy(currentStage.gameObject);
            }
            
            // 创建新关卡
            currentStageConfig = stageConfigLookup[stageId];
            GameObject stageObj = new GameObject($"Stage_{stageId}");
            currentStage = stageObj.AddComponent<Stage>();
            
            // 配置关卡
            currentStage.config = currentStageConfig;
            currentStage.stageManager = this;
            currentStage.terrainManager = terrainManager;
            currentStage.enemyManager = enemyManager;
            currentStage.weatherManager = weatherManager;
            
            // 订阅事件
            currentStage.OnStageStart += OnStageStart;
            currentStage.OnStageComplete += OnStageComplete;
            currentStage.OnStageFailed += OnStageFailed;
            currentStage.OnLifePointsChanged += OnLifePointsChanged;
            currentStage.OnDeployCostChanged += OnDeployCostChanged;
            currentStage.OnWaveChanged += OnWaveChanged;
            
            // 更新状态
            currentStageType = currentStageConfig.stageType;
            currentDifficulty = currentStageConfig.difficulty;
            isStageActive = false;
            
            // 通知UI
            if (stageUI != null)
            {
                stageUI.SetStage(currentStage);
            }
            
            OnStageLoaded?.Invoke(currentStage);
            Debug.Log($"加载关卡: {currentStageConfig.stageName}");
            
            return true;
        }
        
        /// <summary>
        /// 开始关卡
        /// </summary>
        public void StartStage()
        {
            if (currentStage == null)
            {
                Debug.LogError("没有加载的关卡");
                return;
            }
            
            isStageActive = true;
            currentStage.StartStage();
            OnStageStarted?.Invoke(currentStage);
        }
        
        /// <summary>
        /// 暂停关卡
        /// </summary>
        public void PauseStage()
        {
            if (currentStage != null)
            {
                currentStage.PauseStage();
            }
        }
        
        /// <summary>
        /// 恢复关卡
        /// </summary>
        public void ResumeStage()
        {
            if (currentStage != null)
            {
                currentStage.ResumeStage();
            }
        }
        
        /// <summary>
        /// 重启关卡
        /// </summary>
        public void RestartStage()
        {
            if (currentStage != null)
            {
                currentStage.RestartStage();
                StartStage();
            }
        }
        
        /// <summary>
        /// 退出关卡
        /// </summary>
        public void ExitStage()
        {
            if (currentStage != null)
            {
                // 取消事件订阅
                currentStage.OnStageStart -= OnStageStart;
                currentStage.OnStageComplete -= OnStageComplete;
                currentStage.OnStageFailed -= OnStageFailed;
                currentStage.OnLifePointsChanged -= OnLifePointsChanged;
                currentStage.OnDeployCostChanged -= OnDeployCostChanged;
                currentStage.OnWaveChanged -= OnWaveChanged;
                
                // 清理
                Destroy(currentStage.gameObject);
                currentStage = null;
                currentStageConfig = null;
                isStageActive = false;
                
                // 清理干员
                if (operatorManager != null)
                {
                    operatorManager.RetreatAllOperators();
                }
                
                // 清理敌人
                if (enemyManager != null)
                {
                    enemyManager.ClearAllEnemies();
                }
                
                Debug.Log("退出关卡");
            }
        }
        
        /// <summary>
        /// 获取关卡配置
        /// </summary>
        public StageConfig GetStageConfig(string stageId)
        {
            return stageConfigLookup.ContainsKey(stageId) ? stageConfigLookup[stageId] : null;
        }
        
        /// <summary>
        /// 获取所有关卡配置
        /// </summary>
        public List<StageConfig> GetAllStageConfigs()
        {
            return new List<StageConfig>(stageConfigs);
        }
        
        /// <summary>
        /// 根据类型获取关卡配置
        /// </summary>
        public List<StageConfig> GetStageConfigsByType(StageType type)
        {
            List<StageConfig> result = new List<StageConfig>();
            
            foreach (var config in stageConfigs)
            {
                if (config.stageType == type)
                {
                    result.Add(config);
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// 根据难度获取关卡配置
        /// </summary>
        public List<StageConfig> GetStageConfigsByDifficulty(StageDifficulty difficulty)
        {
            List<StageConfig> result = new List<StageConfig>();
            
            foreach (var config in stageConfigs)
            {
                if (config.difficulty == difficulty)
                {
                    result.Add(config);
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// 检查关卡是否已完成
        /// </summary>
        public bool IsStageCompleted(string stageId)
        {
            return completedStages.Exists(stage => stage.config.stageId == stageId);
        }
        
        /// <summary>
        /// 获取已完成的关卡数量
        /// </summary>
        public int GetCompletedStageCount()
        {
            return completedStages.Count;
        }
        
        /// <summary>
        /// 获取关卡进度
        /// </summary>
        public float GetStageProgress()
        {
            if (stageConfigs.Count == 0) return 0f;
            return (float)completedStages.Count / stageConfigs.Count;
        }
        
        /// <summary>
        /// 验证关卡配置
        /// </summary>
        public bool ValidateStageConfig(string stageId)
        {
            if (!stageConfigLookup.ContainsKey(stageId))
                return false;
                
            var config = stageConfigLookup[stageId];
            
            // 检查基础配置
            if (string.IsNullOrEmpty(config.stageName))
            {
                Debug.LogError($"关卡 {stageId} 名称为空");
                return false;
            }
            
            if (config.initialLifePoints <= 0)
            {
                Debug.LogError($"关卡 {stageId} 初始生命点数无效");
                return false;
            }
            
            if (config.initialDeployCost < 0)
            {
                Debug.LogError($"关卡 {stageId} 初始部署费用无效");
                return false;
            }
            
            // 检查敌潮波次
            if (config.enemyWaves == null || config.enemyWaves.Count == 0)
            {
                Debug.LogWarning($"关卡 {stageId} 没有敌潮波次");
            }
            
            return true;
        }
        
        /// <summary>
        /// 验证所有关卡配置
        /// </summary>
        public void ValidateAllStageConfigs()
        {
            foreach (var config in stageConfigs)
            {
                ValidateStageConfig(config.stageId);
            }
        }
        
        // 事件处理方法
        private void OnStageStart(Stage stage)
        {
            Debug.Log($"关卡开始: {stage.config.stageName}");
        }
        
        private void OnStageComplete(Stage stage)
        {
            completedStages.Add(stage);
            isStageActive = false;
            OnStageCompleted?.Invoke(stage);
            
            Debug.Log($"关卡完成: {stage.config.stageName}");
        }
        
        private void OnStageFailed(Stage stage)
        {
            isStageActive = false;
            OnStageFailed?.Invoke(stage);
            
            Debug.Log($"关卡失败: {stage.config.stageName}");
        }
        
        private void OnLifePointsChanged(int lifePoints)
        {
            if (stageUI != null)
            {
                stageUI.UpdateLifePoints(lifePoints);
            }
        }
        
        private void OnDeployCostChanged(int deployCost)
        {
            if (stageUI != null)
            {
                stageUI.UpdateDeployCost(deployCost);
            }
        }
        
        private void OnWaveChanged(int waveNumber)
        {
            if (stageUI != null)
            {
                stageUI.UpdateWaveNumber(waveNumber);
            }
        }
        
        /// <summary>
        /// 获取当前关卡信息
        /// </summary>
        public string GetCurrentStageInfo()
        {
            if (currentStage == null) return "无";
            
            return $"{currentStage.config.stageName} ({currentStage.config.stageType})";
        }
        
        /// <summary>
        /// 获取关卡统计信息
        /// </summary>
        public string GetStageStatistics()
        {
            return $"总关卡: {stageConfigs.Count}, 已完成: {completedStages.Count}, 进度: {GetStageProgress():P1}";
        }
    }
} 