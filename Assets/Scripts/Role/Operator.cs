using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDGame.Role
{
    /// <summary>
    /// 干员职业枚举
    /// </summary>
    public enum OperatorProfession
    {
        Vanguard,   // 先锋
        Guard,      // 近卫
        Defender,   // 重装
        Sniper,     // 狙击
        Caster,     // 术师
        Medic,      // 医疗
        Supporter,  // 辅助
        Specialist  // 特种
    }

    /// <summary>
    /// 干员基础属性结构
    /// </summary>
    [Serializable]
    public struct OperatorStats
    {
        public int maxHP;           // 最大生命值
        public int currentHP;       // 当前生命值
        public int attack;          // 攻击力
        public int defense;         // 防御力
        public int magicResistance; // 法术抗性
        public int deployCost;      // 部署费用
        public int blockCount;      // 阻挡数
        public float attackInterval; // 攻击间隔
        public float redeployTime;  // 再部署时间
    }

    /// <summary>
    /// 干员类 - 游戏核心单位
    /// </summary>
    public class Operator : MonoBehaviour
    {
        [Header("基础信息")]
        public string operatorName;           // 干员名称
        public string operatorId;             // 干员ID
        public OperatorProfession profession; // 职业
        public int rarity;                    // 稀有度 (1-6星)
        
        [Header("基础属性")]
        public OperatorStats baseStats;       // 基础属性
        public OperatorStats currentStats;    // 当前属性（包含加成）
        
        [Header("养成属性")]
        public int eliteLevel;                // 精英化等级 (0-2)
        public int level;                     // 等级 (1-90)
        public int skillLevel;                // 技能等级 (1-7)
        public int skillMastery;              // 技能专精 (0-3)
        
        [Header("战斗特性")]
        public Vector2Int attackRange;        // 攻击范围
        public List<Skill> skills;            // 技能列表
        public List<Talent> talents;          // 天赋列表
        
        [Header("状态")]
        public bool isDeployed;               // 是否已部署
        public bool isAlive;                  // 是否存活
        public float lastAttackTime;          // 上次攻击时间
        
        // 事件
        public event Action<Operator> OnDeath;
        public event Action<Operator> OnDeploy;
        public event Action<Operator> OnRetreat;
        
        private void Awake()
        {
            InitializeOperator();
        }
        
        /// <summary>
        /// 初始化干员
        /// </summary>
        private void InitializeOperator()
        {
            skills = new List<Skill>();
            talents = new List<Talent>();
            isAlive = true;
            currentStats = baseStats;
            currentStats.currentHP = currentStats.maxHP;
        }
        
        /// <summary>
        /// 部署干员
        /// </summary>
        public bool Deploy(Vector3 position)
        {
            if (isDeployed) return false;
            
            transform.position = position;
            isDeployed = true;
            OnDeploy?.Invoke(this);
            return true;
        }
        
        /// <summary>
        /// 撤退干员
        /// </summary>
        public void Retreat()
        {
            if (!isDeployed) return;
            
            isDeployed = false;
            OnRetreat?.Invoke(this);
        }
        
        /// <summary>
        /// 受到伤害
        /// </summary>
        public void TakeDamage(int damage, bool isMagicDamage = false)
        {
            if (!isAlive) return;
            
            int finalDamage = CalculateDamage(damage, isMagicDamage);
            currentStats.currentHP = Mathf.Max(0, currentStats.currentHP - finalDamage);
            
            if (currentStats.currentHP <= 0)
            {
                Die();
            }
        }
        
        /// <summary>
        /// 计算伤害
        /// </summary>
        private int CalculateDamage(int damage, bool isMagicDamage)
        {
            if (isMagicDamage)
            {
                // 法术伤害计算：最终伤害 = 敌方ATK × (1 - 我方RES/100)
                float resistance = currentStats.magicResistance / 100f;
                return Mathf.RoundToInt(damage * (1f - resistance));
            }
            else
            {
                // 物理伤害计算：最终伤害 = 敌方ATK - 我方DEF（若DEF≥敌方ATK，则伤害=1）
                int finalDamage = damage - currentStats.defense;
                return Mathf.Max(1, finalDamage);
            }
        }
        
        /// <summary>
        /// 治疗
        /// </summary>
        public void Heal(int healAmount)
        {
            if (!isAlive) return;
            
            currentStats.currentHP = Mathf.Min(currentStats.maxHP, currentStats.currentHP + healAmount);
        }
        
        /// <summary>
        /// 死亡
        /// </summary>
        private void Die()
        {
            isAlive = false;
            isDeployed = false;
            OnDeath?.Invoke(this);
        }
        
        /// <summary>
        /// 升级
        /// </summary>
        public void LevelUp()
        {
            if (level >= 90) return;
            
            level++;
            UpdateStats();
        }
        
        /// <summary>
        /// 精英化
        /// </summary>
        public bool Elite()
        {
            if (eliteLevel >= 2) return false;
            
            eliteLevel++;
            UpdateStats();
            return true;
        }
        
        /// <summary>
        /// 更新属性
        /// </summary>
        private void UpdateStats()
        {
            // 这里应该根据等级和精英化等级计算实际属性
            // 简化实现，实际应该有详细的计算公式
            currentStats = baseStats;
            
            // 应用等级加成
            float levelMultiplier = 1f + (level - 1) * 0.02f;
            currentStats.maxHP = Mathf.RoundToInt(baseStats.maxHP * levelMultiplier);
            currentStats.attack = Mathf.RoundToInt(baseStats.attack * levelMultiplier);
            currentStats.defense = Mathf.RoundToInt(baseStats.defense * levelMultiplier);
            
            // 应用精英化加成
            float eliteMultiplier = 1f + eliteLevel * 0.1f;
            currentStats.maxHP = Mathf.RoundToInt(currentStats.maxHP * eliteMultiplier);
            currentStats.attack = Mathf.RoundToInt(currentStats.attack * eliteMultiplier);
            currentStats.defense = Mathf.RoundToInt(currentStats.defense * eliteMultiplier);
            
            // 确保当前血量不超过最大血量
            currentStats.currentHP = Mathf.Min(currentStats.currentHP, currentStats.maxHP);
        }
        
        /// <summary>
        /// 添加技能
        /// </summary>
        public void AddSkill(Skill skill)
        {
            if (!skills.Contains(skill))
            {
                skills.Add(skill);
            }
        }
        
        /// <summary>
        /// 添加天赋
        /// </summary>
        public void AddTalent(Talent talent)
        {
            if (!talents.Contains(talent))
            {
                talents.Add(talent);
            }
        }
        
        /// <summary>
        /// 获取当前血量百分比
        /// </summary>
        public float GetHealthPercentage()
        {
            return (float)currentStats.currentHP / currentStats.maxHP;
        }
        
        /// <summary>
        /// 是否可以攻击
        /// </summary>
        public bool CanAttack()
        {
            return isAlive && isDeployed && 
                   Time.time - lastAttackTime >= currentStats.attackInterval;
        }
        
        /// <summary>
        /// 执行攻击
        /// </summary>
        public void Attack()
        {
            if (!CanAttack()) return;
            
            lastAttackTime = Time.time;
            // 这里应该实现具体的攻击逻辑
        }
    }
} 