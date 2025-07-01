using System;
using UnityEngine;

namespace TDGame.Role
{
    /// <summary>
    /// 天赋类型枚举
    /// </summary>
    public enum TalentType
    {
        Self,       // 自身增益
        Team,       // 团队增益
        Enemy,      // 敌人减益
        Special     // 特殊效果
    }
    
    /// <summary>
    /// 天赋效果结构
    /// </summary>
    [Serializable]
    public struct TalentEffect
    {
        public string effectName;           // 效果名称
        public float value;                 // 效果数值
        public string targetType;           // 目标类型
        public string condition;            // 触发条件
        public string description;          // 效果描述
    }
    
    /// <summary>
    /// 天赋类
    /// </summary>
    [CreateAssetMenu(fileName = "NewTalent", menuName = "TDGame/Talent")]
    public class Talent : ScriptableObject
    {
        [Header("基础信息")]
        public string talentName;           // 天赋名称
        public string talentId;             // 天赋ID
        public TalentType talentType;       // 天赋类型
        public string description;          // 天赋描述
        
        [Header("天赋效果")]
        public TalentEffect[] effects;      // 天赋效果列表
        
        [Header("触发条件")]
        public bool isAlwaysActive;         // 是否始终激活
        public string activationCondition;  // 激活条件
        public float activationChance;      // 激活概率
        
        [Header("解锁条件")]
        public int requiredEliteLevel;      // 需要的精英化等级
        public int requiredLevel;           // 需要的等级
        
        // 运行时状态
        private bool isActive;
        private Operator owner;
        
        /// <summary>
        /// 初始化天赋
        /// </summary>
        public void Initialize(Operator operatorOwner)
        {
            owner = operatorOwner;
            isActive = false;
            
            // 检查解锁条件
            if (CheckUnlockConditions())
            {
                Activate();
            }
        }
        
        /// <summary>
        /// 检查解锁条件
        /// </summary>
        private bool CheckUnlockConditions()
        {
            if (owner.eliteLevel < requiredEliteLevel) return false;
            if (owner.level < requiredLevel) return false;
            
            return true;
        }
        
        /// <summary>
        /// 激活天赋
        /// </summary>
        public void Activate()
        {
            if (isActive) return;
            
            isActive = true;
            ApplyEffects();
        }
        
        /// <summary>
        /// 停用天赋
        /// </summary>
        public void Deactivate()
        {
            if (!isActive) return;
            
            isActive = false;
            RemoveEffects();
        }
        
        /// <summary>
        /// 应用天赋效果
        /// </summary>
        private void ApplyEffects()
        {
            foreach (var effect in effects)
            {
                ApplyEffect(effect);
            }
        }
        
        /// <summary>
        /// 移除天赋效果
        /// </summary>
        private void RemoveEffects()
        {
            // 移除所有应用的效果
            // 这里需要实现具体的移除逻辑
        }
        
        /// <summary>
        /// 应用单个天赋效果
        /// </summary>
        private void ApplyEffect(TalentEffect effect)
        {
            switch (talentType)
            {
                case TalentType.Self:
                    ApplySelfEffect(effect);
                    break;
                    
                case TalentType.Team:
                    ApplyTeamEffect(effect);
                    break;
                    
                case TalentType.Enemy:
                    ApplyEnemyEffect(effect);
                    break;
                    
                case TalentType.Special:
                    ApplySpecialEffect(effect);
                    break;
            }
        }
        
        /// <summary>
        /// 应用自身效果
        /// </summary>
        private void ApplySelfEffect(TalentEffect effect)
        {
            // 根据效果名称应用不同的自身增益
            switch (effect.effectName.ToLower())
            {
                case "attack":
                    owner.currentStats.attack += Mathf.RoundToInt(effect.value);
                    break;
                    
                case "defense":
                    owner.currentStats.defense += Mathf.RoundToInt(effect.value);
                    break;
                    
                case "maxhp":
                    owner.currentStats.maxHP += Mathf.RoundToInt(effect.value);
                    owner.currentStats.currentHP += Mathf.RoundToInt(effect.value);
                    break;
                    
                case "magicresistance":
                    owner.currentStats.magicResistance += Mathf.RoundToInt(effect.value);
                    break;
                    
                case "attackspeed":
                    owner.currentStats.attackInterval *= (1f - effect.value / 100f);
                    break;
                    
                case "deploycost":
                    owner.currentStats.deployCost = Mathf.Max(0, owner.currentStats.deployCost - Mathf.RoundToInt(effect.value));
                    break;
                    
                case "blockcount":
                    owner.currentStats.blockCount += Mathf.RoundToInt(effect.value);
                    break;
            }
        }
        
        /// <summary>
        /// 应用团队效果
        /// </summary>
        private void ApplyTeamEffect(TalentEffect effect)
        {
            // 获取所有友军干员并应用效果
            // 这里需要实现团队效果的应用逻辑
        }
        
        /// <summary>
        /// 应用敌人效果
        /// </summary>
        private void ApplyEnemyEffect(TalentEffect effect)
        {
            // 对敌人应用减益效果
            // 这里需要实现敌人效果的应用逻辑
        }
        
        /// <summary>
        /// 应用特殊效果
        /// </summary>
        private void ApplySpecialEffect(TalentEffect effect)
        {
            // 应用特殊效果
            // 比如召唤物、特殊机制等
        }
        
        /// <summary>
        /// 检查是否满足激活条件
        /// </summary>
        public bool CheckActivationCondition()
        {
            if (isAlwaysActive) return true;
            
            // 检查激活条件
            if (!string.IsNullOrEmpty(activationCondition))
            {
                // 这里应该解析并检查具体的激活条件
                // 比如血量低于50%、敌人数量大于3等
            }
            
            // 检查激活概率
            if (activationChance > 0 && activationChance < 1)
            {
                return UnityEngine.Random.Range(0f, 1f) <= activationChance;
            }
            
            return true;
        }
        
        /// <summary>
        /// 更新天赋状态
        /// </summary>
        public void Update()
        {
            if (!isActive) return;
            
            // 检查是否需要重新激活
            if (CheckActivationCondition())
            {
                if (!isActive)
                {
                    Activate();
                }
            }
            else
            {
                if (isActive && !isAlwaysActive)
                {
                    Deactivate();
                }
            }
        }
        
        /// <summary>
        /// 获取天赋状态描述
        /// </summary>
        public string GetStatusDescription()
        {
            if (!CheckUnlockConditions())
                return $"需要精英化{requiredEliteLevel}级，等级{requiredLevel}";
                
            if (isActive)
                return "激活中";
                
            return "未激活";
        }
        
        /// <summary>
        /// 获取完整的天赋描述
        /// </summary>
        public string GetFullDescription()
        {
            string fullDesc = description + "\n\n";
            
            foreach (var effect in effects)
            {
                fullDesc += $"• {effect.description}\n";
            }
            
            if (!CheckUnlockConditions())
            {
                fullDesc += $"\n解锁条件：精英化{requiredEliteLevel}级，等级{requiredLevel}";
            }
            
            return fullDesc;
        }
    }
} 