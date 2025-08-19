using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TDGame.Stage
{
    /// <summary>
    /// 关卡UI类
    /// </summary>
    public class StageUI : MonoBehaviour
    {
        [Header("关卡信息UI")]
        public TextMeshProUGUI stageNameText;
        public TextMeshProUGUI stageTypeText;
        public TextMeshProUGUI difficultyText;
        public TextMeshProUGUI stageTimeText;
        public TextMeshProUGUI waveNumberText;
        
        [Header("状态UI")]
        public Slider lifePointsSlider;
        public TextMeshProUGUI lifePointsText;
        public Slider deployCostSlider;
        public TextMeshProUGUI deployCostText;
        public Slider progressSlider;
        public TextMeshProUGUI progressText;
        
        [Header("天气UI")]
        public Image weatherIcon;
        public TextMeshProUGUI weatherNameText;
        public TextMeshProUGUI weatherTimeText;
        public Slider weatherProgressSlider;
        
        [Header("敌人信息UI")]
        public TextMeshProUGUI enemyCountText;
        public TextMeshProUGUI enemyKilledText;
        public TextMeshProUGUI enemyLeakedText;
        
        [Header("控制按钮")]
        public Button pauseButton;
        public Button resumeButton;
        public Button restartButton;
        public Button exitButton;
        
        [Header("颜色设置")]
        public Color normalColor = Color.white;
        public Color warningColor = Color.yellow;
        public Color dangerColor = Color.red;
        public Color successColor = Color.green;
        
        private Stage currentStage;
        private StageManager stageManager;
        private WeatherManager weatherManager;
        private EnemyManager enemyManager;
        
        private void Awake()
        {
            InitializeUI();
        }
        
        private void Start()
        {
            // 获取组件引用
            stageManager = FindObjectOfType<StageManager>();
            weatherManager = FindObjectOfType<WeatherManager>();
            enemyManager = FindObjectOfType<EnemyManager>();
            
            // 设置按钮事件
            SetupButtonEvents();
        }
        
        private void Update()
        {
            UpdateUI();
        }
        
        /// <summary>
        /// 初始化UI
        /// </summary>
        private void InitializeUI()
        {
            // 初始化滑块
            if (lifePointsSlider != null)
            {
                lifePointsSlider.minValue = 0f;
                lifePointsSlider.maxValue = 1f;
            }
            
            if (deployCostSlider != null)
            {
                deployCostSlider.minValue = 0f;
                deployCostSlider.maxValue = 1f;
            }
            
            if (progressSlider != null)
            {
                progressSlider.minValue = 0f;
                progressSlider.maxValue = 1f;
            }
            
            if (weatherProgressSlider != null)
            {
                weatherProgressSlider.minValue = 0f;
                weatherProgressSlider.maxValue = 1f;
            }
        }
        
        /// <summary>
        /// 设置按钮事件
        /// </summary>
        private void SetupButtonEvents()
        {
            if (pauseButton != null)
                pauseButton.onClick.AddListener(OnPauseButtonClick);
                
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeButtonClick);
                
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartButtonClick);
                
            if (exitButton != null)
                exitButton.onClick.AddListener(OnExitButtonClick);
        }
        
        /// <summary>
        /// 设置关卡
        /// </summary>
        public void SetStage(Stage stage)
        {
            currentStage = stage;
            UpdateStageInfo();
        }
        
        /// <summary>
        /// 更新UI
        /// </summary>
        private void UpdateUI()
        {
            if (currentStage == null) return;
            
            UpdateStageInfo();
            UpdateStatusInfo();
            UpdateWeatherInfo();
            UpdateEnemyInfo();
        }
        
        /// <summary>
        /// 更新关卡信息
        /// </summary>
        private void UpdateStageInfo()
        {
            if (currentStage == null || currentStage.config == null) return;
            
            var config = currentStage.config;
            
            if (stageNameText != null)
                stageNameText.text = config.stageName;
                
            if (stageTypeText != null)
                stageTypeText.text = GetStageTypeName(config.stageType);
                
            if (difficultyText != null)
                difficultyText.text = GetDifficultyName(config.difficulty);
                
            if (stageTimeText != null)
                stageTimeText.text = $"时间: {FormatTime(currentStage.stageTime)}";
                
            if (waveNumberText != null)
                waveNumberText.text = $"波次: {currentStage.currentWave}";
        }
        
        /// <summary>
        /// 更新状态信息
        /// </summary>
        private void UpdateStatusInfo()
        {
            if (currentStage == null) return;
            
            // 生命点数
            if (lifePointsSlider != null)
            {
                float lifePercentage = (float)currentStage.currentLifePoints / currentStage.config.initialLifePoints;
                lifePointsSlider.value = lifePercentage;
                
                // 根据血量设置颜色
                Image fillImage = lifePointsSlider.fillRect.GetComponent<Image>();
                if (fillImage != null)
                {
                    if (lifePercentage > 0.5f)
                        fillImage.color = successColor;
                    else if (lifePercentage > 0.2f)
                        fillImage.color = warningColor;
                    else
                        fillImage.color = dangerColor;
                }
            }
            
            if (lifePointsText != null)
                lifePointsText.text = $"生命: {currentStage.currentLifePoints}/{currentStage.config.initialLifePoints}";
                
            // 部署费用
            if (deployCostSlider != null)
            {
                float costPercentage = (float)currentStage.currentDeployCost / currentStage.config.initialDeployCost;
                deployCostSlider.value = costPercentage;
            }
            
            if (deployCostText != null)
                deployCostText.text = $"费用: {currentStage.currentDeployCost}";
                
            // 关卡进度
            if (progressSlider != null)
            {
                float progress = currentStage.GetStageProgress();
                progressSlider.value = progress;
            }
            
            if (progressText != null)
                progressText.text = $"进度: {currentStage.GetStageProgress():P0}";
        }
        
        /// <summary>
        /// 更新天气信息
        /// </summary>
        private void UpdateWeatherInfo()
        {
            if (weatherManager == null) return;
            
            var weatherConfig = weatherManager.GetCurrentWeatherConfig();
            if (weatherConfig == null) return;
            
            if (weatherNameText != null)
                weatherNameText.text = weatherConfig.weatherName;
                
            if (weatherTimeText != null)
            {
                float remaining = weatherManager.GetWeatherRemainingTime();
                if (remaining >= 0)
                    weatherTimeText.text = $"剩余: {FormatTime(remaining)}";
                else
                    weatherTimeText.text = "永久";
            }
            
            if (weatherProgressSlider != null)
            {
                float progress = weatherManager.GetWeatherProgress();
                weatherProgressSlider.value = progress;
            }
        }
        
        /// <summary>
        /// 更新敌人信息
        /// </summary>
        private void UpdateEnemyInfo()
        {
            if (enemyManager == null) return;
            
            if (enemyCountText != null)
                enemyCountText.text = $"活跃: {enemyManager.GetActiveEnemyCount()}";
                
            if (enemyKilledText != null)
                enemyKilledText.text = $"击杀: {enemyManager.totalEnemiesKilled}";
                
            if (enemyLeakedText != null)
                enemyLeakedText.text = $"漏过: {enemyManager.totalEnemiesLeaked}";
        }
        
        /// <summary>
        /// 更新生命点数
        /// </summary>
        public void UpdateLifePoints(int lifePoints)
        {
            if (lifePointsText != null)
                lifePointsText.text = $"生命: {lifePoints}";
                
            if (lifePointsSlider != null)
            {
                float percentage = (float)lifePoints / currentStage.config.initialLifePoints;
                lifePointsSlider.value = percentage;
            }
        }
        
        /// <summary>
        /// 更新部署费用
        /// </summary>
        public void UpdateDeployCost(int deployCost)
        {
            if (deployCostText != null)
                deployCostText.text = $"费用: {deployCost}";
                
            if (deployCostSlider != null)
            {
                float percentage = (float)deployCost / currentStage.config.initialDeployCost;
                deployCostSlider.value = percentage;
            }
        }
        
        /// <summary>
        /// 更新波次编号
        /// </summary>
        public void UpdateWaveNumber(int waveNumber)
        {
            if (waveNumberText != null)
                waveNumberText.text = $"波次: {waveNumber}";
        }
        
        /// <summary>
        /// 获取关卡类型名称
        /// </summary>
        private string GetStageTypeName(StageType stageType)
        {
            switch (stageType)
            {
                case StageType.MainStory: return "主线关卡";
                case StageType.Resource: return "资源收集";
                case StageType.Annihilation: return "剿灭作战";
                case StageType.CrisisContract: return "危机合约";
                case StageType.IntegratedStrategy: return "集成战略";
                default: return "未知";
            }
        }
        
        /// <summary>
        /// 获取难度名称
        /// </summary>
        private string GetDifficultyName(StageDifficulty difficulty)
        {
            switch (difficulty)
            {
                case StageDifficulty.Normal: return "普通";
                case StageDifficulty.Challenge: return "突袭";
                case StageDifficulty.Contract: return "合约";
                default: return "未知";
            }
        }
        
        /// <summary>
        /// 格式化时间
        /// </summary>
        private string FormatTime(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        
        /// <summary>
        /// 显示关卡完成界面
        /// </summary>
        public void ShowStageComplete()
        {
            // 这里应该显示关卡完成界面
            Debug.Log("关卡完成！");
        }
        
        /// <summary>
        /// 显示关卡失败界面
        /// </summary>
        public void ShowStageFailed()
        {
            // 这里应该显示关卡失败界面
            Debug.Log("关卡失败！");
        }
        
        /// <summary>
        /// 显示警告信息
        /// </summary>
        public void ShowWarning(string message)
        {
            // 这里应该显示警告信息
            Debug.LogWarning($"警告: {message}");
        }
        
        // 按钮事件处理方法
        private void OnPauseButtonClick()
        {
            if (stageManager != null)
            {
                stageManager.PauseStage();
            }
        }
        
        private void OnResumeButtonClick()
        {
            if (stageManager != null)
            {
                stageManager.ResumeStage();
            }
        }
        
        private void OnRestartButtonClick()
        {
            if (stageManager != null)
            {
                stageManager.RestartStage();
            }
        }
        
        private void OnExitButtonClick()
        {
            if (stageManager != null)
            {
                stageManager.ExitStage();
            }
        }
        
        /// <summary>
        /// 设置UI可见性
        /// </summary>
        public void SetUIVisibility(bool visible)
        {
            gameObject.SetActive(visible);
        }
        
        /// <summary>
        /// 设置按钮交互性
        /// </summary>
        public void SetButtonInteractable(bool interactable)
        {
            if (pauseButton != null)
                pauseButton.interactable = interactable;
                
            if (resumeButton != null)
                resumeButton.interactable = interactable;
                
            if (restartButton != null)
                restartButton.interactable = interactable;
                
            if (exitButton != null)
                exitButton.interactable = interactable;
        }
        
        /// <summary>
        /// 更新按钮状态
        /// </summary>
        public void UpdateButtonStates()
        {
            bool isPaused = Time.timeScale == 0f;
            
            if (pauseButton != null)
                pauseButton.gameObject.SetActive(!isPaused);
                
            if (resumeButton != null)
                resumeButton.gameObject.SetActive(isPaused);
        }
    }
} 