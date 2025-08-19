using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDGame.Role;

namespace TDGame.Stage
{
    /// <summary>
    /// 天气效果类型
    /// </summary>
    public enum WeatherEffectType
    {
        None,           // 无效果
        Damage,         // 伤害
        Slow,           // 减速
        Visibility,     // 视野
        Stealth         // 隐匿
    }
    
    /// <summary>
    /// 天气效果配置
    /// </summary>
    [System.Serializable]
    public class WeatherEffect
    {
        public WeatherEffectType effectType;
        public float value;
        public float interval;
        public string description;
    }
    
    /// <summary>
    /// 天气配置
    /// </summary>
    [System.Serializable]
    public class WeatherConfig
    {
        public WeatherType weatherType;
        public string weatherName;
        public string description;
        public List<WeatherEffect> effects;
        public float duration; // -1表示永久
        public Color weatherColor;
        public GameObject weatherParticlePrefab;
    }
    
    /// <summary>
    /// 天气管理器类
    /// </summary>
    public class WeatherManager : MonoBehaviour
    {
        [Header("天气设置")]
        public List<WeatherConfig> weatherConfigs;
        public WeatherType currentWeather;
        public float weatherStartTime;
        public float weatherDuration;
        
        [Header("天气效果")]
        public bool isWeatherActive;
        public List<WeatherEffect> activeEffects;
        
        [Header("视觉效果")]
        public Camera mainCamera;
        public Material weatherMaterial;
        public ParticleSystem weatherParticles;
        
        [Header("运行时状态")]
        public float lastEffectTime;
        public Dictionary<WeatherEffectType, float> effectTimers;
        
        // 事件
        public System.Action<WeatherType> OnWeatherChanged;
        public System.Action<WeatherEffect> OnWeatherEffectApplied;
        
        private Dictionary<WeatherType, WeatherConfig> weatherConfigLookup;
        private Coroutine weatherCoroutine;
        
        private void Awake()
        {
            InitializeWeatherManager();
        }
        
        private void Start()
        {
            // 设置默认天气
            SetWeather(WeatherType.Clear);
        }
        
        private void Update()
        {
            UpdateWeatherEffects();
        }
        
        /// <summary>
        /// 初始化天气管理器
        /// </summary>
        private void InitializeWeatherManager()
        {
            weatherConfigLookup = new Dictionary<WeatherType, WeatherConfig>();
            activeEffects = new List<WeatherEffect>();
            effectTimers = new Dictionary<WeatherEffectType, float>();
            
            // 初始化查找表
            foreach (var config in weatherConfigs)
            {
                weatherConfigLookup[config.weatherType] = config;
            }
            
            // 获取主摄像机
            if (mainCamera == null)
                mainCamera = Camera.main;
        }
        
        /// <summary>
        /// 设置天气
        /// </summary>
        public void SetWeather(WeatherType weatherType)
        {
            if (!weatherConfigLookup.ContainsKey(weatherType))
            {
                Debug.LogError($"天气配置不存在: {weatherType}");
                return;
            }
            
            // 停止当前天气
            StopCurrentWeather();
            
            // 设置新天气
            currentWeather = weatherType;
            weatherStartTime = Time.time;
            
            var config = weatherConfigLookup[weatherType];
            weatherDuration = config.duration;
            
            // 应用天气效果
            ApplyWeatherEffects(config);
            
            // 应用视觉效果
            ApplyVisualEffects(config);
            
            // 启动天气协程
            if (weatherDuration > 0)
            {
                weatherCoroutine = StartCoroutine(WeatherDurationCoroutine());
            }
            
            isWeatherActive = true;
            OnWeatherChanged?.Invoke(weatherType);
            
            Debug.Log($"天气变化: {config.weatherName}");
        }
        
        /// <summary>
        /// 停止当前天气
        /// </summary>
        private void StopCurrentWeather()
        {
            if (weatherCoroutine != null)
            {
                StopCoroutine(weatherCoroutine);
                weatherCoroutine = null;
            }
            
            // 清除天气效果
            ClearWeatherEffects();
            
            // 清除视觉效果
            ClearVisualEffects();
            
            isWeatherActive = false;
        }
        
        /// <summary>
        /// 应用天气效果
        /// </summary>
        private void ApplyWeatherEffects(WeatherConfig config)
        {
            activeEffects.Clear();
            effectTimers.Clear();
            
            if (config.effects != null)
            {
                foreach (var effect in config.effects)
                {
                    activeEffects.Add(effect);
                    effectTimers[effect.effectType] = 0f;
                }
            }
        }
        
        /// <summary>
        /// 应用视觉效果
        /// </summary>
        private void ApplyVisualEffects(WeatherConfig config)
        {
            // 设置摄像机后处理
            if (mainCamera != null && weatherMaterial != null)
            {
                // 这里应该设置后处理效果
                Debug.Log($"应用天气视觉效果: {config.weatherName}");
            }
            
            // 设置粒子效果
            if (config.weatherParticlePrefab != null)
            {
                if (weatherParticles != null)
                {
                    Destroy(weatherParticles.gameObject);
                }
                
                GameObject particleObj = Instantiate(config.weatherParticlePrefab, transform);
                weatherParticles = particleObj.GetComponent<ParticleSystem>();
            }
        }
        
        /// <summary>
        /// 清除天气效果
        /// </summary>
        private void ClearWeatherEffects()
        {
            activeEffects.Clear();
            effectTimers.Clear();
        }
        
        /// <summary>
        /// 清除视觉效果
        /// </summary>
        private void ClearVisualEffects()
        {
            if (weatherParticles != null)
            {
                Destroy(weatherParticles.gameObject);
                weatherParticles = null;
            }
        }
        
        /// <summary>
        /// 更新天气效果
        /// </summary>
        private void UpdateWeatherEffects()
        {
            if (!isWeatherActive || activeEffects.Count == 0) return;
            
            foreach (var effect in activeEffects)
            {
                effectTimers[effect.effectType] += Time.deltaTime;
                
                if (effectTimers[effect.effectType] >= effect.interval)
                {
                    ApplyWeatherEffect(effect);
                    effectTimers[effect.effectType] = 0f;
                }
            }
        }
        
        /// <summary>
        /// 应用天气效果
        /// </summary>
        private void ApplyWeatherEffect(WeatherEffect effect)
        {
            switch (effect.effectType)
            {
                case WeatherEffectType.Damage:
                    ApplyDamageEffect(effect);
                    break;
                case WeatherEffectType.Slow:
                    ApplySlowEffect(effect);
                    break;
                case WeatherEffectType.Visibility:
                    ApplyVisibilityEffect(effect);
                    break;
                case WeatherEffectType.Stealth:
                    ApplyStealthEffect(effect);
                    break;
            }
            
            OnWeatherEffectApplied?.Invoke(effect);
        }
        
        /// <summary>
        /// 应用伤害效果
        /// </summary>
        private void ApplyDamageEffect(WeatherEffect effect)
        {
            var operatorManager = FindObjectOfType<OperatorManager>();
            if (operatorManager != null)
            {
                var deployedOperators = operatorManager.GetDeployedOperators();
                
                foreach (var op in deployedOperators)
                {
                    op.TakeDamage(Mathf.RoundToInt(effect.value));
                }
            }
            
            Debug.Log($"天气伤害效果: 对所有干员造成 {effect.value} 点伤害");
        }
        
        /// <summary>
        /// 应用减速效果
        /// </summary>
        private void ApplySlowEffect(WeatherEffect effect)
        {
            var enemyManager = FindObjectOfType<EnemyManager>();
            if (enemyManager != null)
            {
                var activeEnemies = enemyManager.activeEnemies;
                
                foreach (var enemy in activeEnemies)
                {
                    enemy.ApplySlowEffect(effect.value / 100f, effect.interval);
                }
            }
            
            Debug.Log($"天气减速效果: 所有敌人减速 {effect.value}%");
        }
        
        /// <summary>
        /// 应用视野效果
        /// </summary>
        private void ApplyVisibilityEffect(WeatherEffect effect)
        {
            // 这里应该影响干员的攻击范围或视野
            Debug.Log($"天气视野效果: 视野范围减少 {effect.value}%");
        }
        
        /// <summary>
        /// 应用隐匿效果
        /// </summary>
        private void ApplyStealthEffect(WeatherEffect effect)
        {
            var enemyManager = FindObjectOfType<EnemyManager>();
            if (enemyManager != null)
            {
                var activeEnemies = enemyManager.activeEnemies;
                
                foreach (var enemy in activeEnemies)
                {
                    if (enemy.config.isStealth)
                    {
                        // 增强隐匿效果
                        Debug.Log($"天气隐匿效果: 隐匿敌人更难被发现");
                    }
                }
            }
        }
        
        /// <summary>
        /// 天气持续时间协程
        /// </summary>
        private IEnumerator WeatherDurationCoroutine()
        {
            yield return new WaitForSeconds(weatherDuration);
            
            // 天气结束，恢复到晴朗
            SetWeather(WeatherType.Clear);
        }
        
        /// <summary>
        /// 获取当前天气配置
        /// </summary>
        public WeatherConfig GetCurrentWeatherConfig()
        {
            return weatherConfigLookup.ContainsKey(currentWeather) ? weatherConfigLookup[currentWeather] : null;
        }
        
        /// <summary>
        /// 获取天气剩余时间
        /// </summary>
        public float GetWeatherRemainingTime()
        {
            if (weatherDuration <= 0) return -1f;
            
            float elapsed = Time.time - weatherStartTime;
            return Mathf.Max(0f, weatherDuration - elapsed);
        }
        
        /// <summary>
        /// 获取天气进度
        /// </summary>
        public float GetWeatherProgress()
        {
            if (weatherDuration <= 0) return 1f;
            
            float elapsed = Time.time - weatherStartTime;
            return Mathf.Clamp01(elapsed / weatherDuration);
        }
        
        /// <summary>
        /// 检查天气是否会影响干员
        /// </summary>
        public bool IsWeatherAffectingOperators()
        {
            foreach (var effect in activeEffects)
            {
                if (effect.effectType == WeatherEffectType.Damage)
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 检查天气是否会影响敌人
        /// </summary>
        public bool IsWeatherAffectingEnemies()
        {
            foreach (var effect in activeEffects)
            {
                if (effect.effectType == WeatherEffectType.Slow || 
                    effect.effectType == WeatherEffectType.Stealth)
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 获取天气信息
        /// </summary>
        public string GetWeatherInfo()
        {
            var config = GetCurrentWeatherConfig();
            if (config == null) return "无天气";
            
            string info = $"当前天气: {config.weatherName}\n";
            info += $"描述: {config.description}\n";
            
            if (weatherDuration > 0)
            {
                float remaining = GetWeatherRemainingTime();
                info += $"剩余时间: {remaining:F1}秒\n";
            }
            
            info += $"活跃效果: {activeEffects.Count}个";
            
            return info;
        }
        
        /// <summary>
        /// 随机切换天气
        /// </summary>
        public void RandomizeWeather()
        {
            if (weatherConfigs.Count == 0) return;
            
            int randomIndex = Random.Range(0, weatherConfigs.Count);
            WeatherType randomWeather = weatherConfigs[randomIndex].weatherType;
            
            SetWeather(randomWeather);
        }
        
        /// <summary>
        /// 设置天气持续时间
        /// </summary>
        public void SetWeatherDuration(float duration)
        {
            weatherDuration = duration;
            
            if (weatherCoroutine != null)
            {
                StopCoroutine(weatherCoroutine);
                weatherCoroutine = StartCoroutine(WeatherDurationCoroutine());
            }
        }
        
        /// <summary>
        /// 暂停天气效果
        /// </summary>
        public void PauseWeather()
        {
            if (weatherParticles != null)
            {
                weatherParticles.Pause();
            }
        }
        
        /// <summary>
        /// 恢复天气效果
        /// </summary>
        public void ResumeWeather()
        {
            if (weatherParticles != null)
            {
                weatherParticles.Play();
            }
        }
    }
} 