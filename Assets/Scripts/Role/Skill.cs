using System;
using UnityEngine;

namespace TDGame.Role
{
    /// <summary>
    /// 技能类型枚举
    /// </summary>
    public enum SkillType
    {
        Passive,    // 被动技能
        Active,     // 主动技能
        Auto        // 自动技能
    }
    
    /// <summary>
    /// 技能效果类型枚举
    /// </summary>
    public enum SkillEffectType
    {
        Damage,         // 伤害
        Heal,          // 治疗
        Buff,          // 增益
        Debuff,        // 减益
        Summon,        // 召唤
        Movement,      // 移动
        Special        // 特殊效果
    }
    
    /// <summary>
    /// 技能效果结构
    /// </summary>
    [Serializable]
    public struct SkillEffect
    {
        public SkillEffectType effectType;
        public float value;
        public float duration;
        public string description;
    }
    
    /// <summary>
    /// 技能类
    /// </summary>
    [CreateAssetMenu(fileName = "NewSkill", menuName = "TDGame/Skill")]
    public class Skill : ScriptableObject
    {
        [Header("基础信息")]
        public string skillName;           // 技能名称
        public string skillId;             // 技能ID
        public SkillType skillType;        // 技能类型
        public string description;         // 技能描述
        
        [Header("技能属性")]
        public float cooldown;             // 冷却时间
        public float duration;             // 持续时间
        public int spCost;                 // 技力消耗
        public int spRecovery;             // 技力回复
        
        [Header("技能效果")]
        public SkillEffect[] effects;      // 技能效果列表
        
        [Header("触发条件")]
        public bool autoTrigger;           // 自动触发
        public float triggerChance;        // 触发概率
        public string triggerCondition;    // 触发条件
        
        // 运行时状态
        private float lastUseTime;
        private float currentSp;
        private bool isActive;
        private float activeStartTime;
        
        /// <summary>
        /// 初始化技能
        /// </summary>
        public void Initialize()
        {
            lastUseTime = -cooldown;
            currentSp = 0;
            isActive = false;
        }
        
        /// <summary>
        /// 更新技能状态
        /// </summary>
        public void Update(float deltaTime)
        {
            // 更新技力
            if (isActive)
            {
                currentSp += spRecovery * deltaTime;
            }
            
            // 检查持续时间
            if (isActive && Time.time - activeStartTime >= duration)
            {
                Deactivate();
            }
        }
        
        /// <summary>
        /// 尝试使用技能
        /// </summary>
        public bool TryUse(Operator caster)
        {
            if (!CanUse()) return false;
            
            Use(caster);
            return true;
        }
        
        /// <summary>
        /// 检查是否可以使用技能
        /// </summary>
        public bool CanUse()
        {
            if (skillType == SkillType.Passive) return false;
            
            bool cooldownReady = Time.time - lastUseTime >= cooldown;
            bool spReady = currentSp >= spCost;
            
            return cooldownReady && spReady;
        }
        
        /// <summary>
        /// 使用技能
        /// </summary>
        private void Use(Operator caster)
        {
            lastUseTime = Time.time;
            currentSp -= spCost;
            
            if (skillType == SkillType.Active)
            {
                Activate(caster);
            }
            
            // 应用技能效果
            ApplyEffects(caster);
        }
        
        /// <summary>
        /// 激活技能
        /// </summary>
        private void Activate(Operator caster)
        {
            isActive = true;
            activeStartTime = Time.time;
        }
        
        /// <summary>
        /// 停用技能
        /// </summary>
        private void Deactivate()
        {
            isActive = false;
        }
        
        /// <summary>
        /// 应用技能效果
        /// </summary>
        private void ApplyEffects(Operator caster)
        {
            foreach (var effect in effects)
            {
                ApplyEffect(caster, effect);
            }
        }
        
        /// <summary>
        /// 应用单个效果
        /// </summary>
        private void ApplyEffect(Operator caster, SkillEffect effect)
        {
            switch (effect.effectType)
            {
                case SkillEffectType.Damage:
                    // 对敌人造成伤害
                    break;
                    
                case SkillEffectType.Heal:
                    // 治疗友军
                    caster.Heal(Mathf.RoundToInt(effect.value));
                    break;
                    
                case SkillEffectType.Buff:
                    // 应用增益效果
                    ApplyBuff(caster, effect);
                    break;
                    
                case SkillEffectType.Debuff:
                    // 应用减益效果
                    break;
                    
                case SkillEffectType.Summon:
                    // 召唤单位
                    break;
                    
                case SkillEffectType.Movement:
                    // 移动效果
                    break;
                    
                case SkillEffectType.Special:
                    // 特殊效果
                    break;
            }
        }
        
        /// <summary>
        /// 应用增益效果
        /// </summary>
        private void ApplyBuff(Operator target, SkillEffect effect)
        {
            // 这里应该实现具体的增益效果
            // 比如增加攻击力、防御力等
        }
        
        /// <summary>
        /// 获取技能冷却进度
        /// </summary>
        public float GetCooldownProgress()
        {
            if (skillType == SkillType.Passive) return 1f;
            
            float elapsed = Time.time - lastUseTime;
            return Mathf.Clamp01(elapsed / cooldown);
        }
        
        /// <summary>
        /// 获取技力进度
        /// </summary>
        public float GetSpProgress()
        {
            return Mathf.Clamp01(currentSp / spCost);
        }
        
        /// <summary>
        /// 获取技能状态描述
        /// </summary>
        public string GetStatusDescription()
        {
            if (skillType == SkillType.Passive)
                return "被动技能";
                
            if (isActive)
                return "激活中";
                
            if (CanUse())
                return "可用";
                
            return "冷却中";
        }
    }
} 