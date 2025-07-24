using UnityEngine;
using System.Collections.Generic;

namespace TDGame.Role
{
    /// <summary>
    /// 示例干员配置
    /// 这个类展示了如何配置不同类型的干员
    /// </summary>
    public class ExampleOperators : MonoBehaviour
    {
        [Header("示例干员配置")]
        public List<OperatorConfig> exampleConfigs;
        
        private void Awake()
        {
            CreateExampleConfigs();
        }
        
        /// <summary>
        /// 创建示例干员配置
        /// </summary>
        private void CreateExampleConfigs()
        {
            exampleConfigs = new List<OperatorConfig>();
            
            // 示例1：先锋干员 - 银灰
            OperatorConfig silverAsh = new OperatorConfig
            {
                operatorId = "silver_ash",
                operatorName = "银灰",
                profession = OperatorProfession.Vanguard,
                rarity = 6,
                baseStats = new OperatorStats
                {
                    maxHP = 2000,
                    currentHP = 2000,
                    attack = 350,
                    defense = 150,
                    magicResistance = 0,
                    deployCost = 12,
                    blockCount = 2,
                    attackInterval = 1.3f,
                    redeployTime = 70f
                },
                attackRange = new Vector2Int(2, 2),
                skillIds = new string[] { "skill_vanguard_charge", "skill_leader" },
                talentIds = new string[] { "talent_leader", "talent_elite" }
            };
            exampleConfigs.Add(silverAsh);
            
            // 示例2：重装干员 - 星熊
            OperatorConfig hoshiguma = new OperatorConfig
            {
                operatorId = "hoshiguma",
                operatorName = "星熊",
                profession = OperatorProfession.Defender,
                rarity = 5,
                baseStats = new OperatorStats
                {
                    maxHP = 3500,
                    currentHP = 3500,
                    attack = 280,
                    defense = 400,
                    magicResistance = 10,
                    deployCost = 18,
                    blockCount = 3,
                    attackInterval = 1.5f,
                    redeployTime = 80f
                },
                attackRange = new Vector2Int(1, 1),
                skillIds = new string[] { "skill_defender_taunt", "skill_counter" },
                talentIds = new string[] { "talent_iron_will", "talent_guardian" }
            };
            exampleConfigs.Add(hoshiguma);
            
            // 示例3：医疗干员 - 白面鸮
            OperatorConfig ptilopsis = new OperatorConfig
            {
                operatorId = "ptilopsis",
                operatorName = "白面鸮",
                profession = OperatorProfession.Medic,
                rarity = 5,
                baseStats = new OperatorStats
                {
                    maxHP = 1200,
                    currentHP = 1200,
                    attack = 180,
                    defense = 80,
                    magicResistance = 0,
                    deployCost = 14,
                    blockCount = 1,
                    attackInterval = 2.8f,
                    redeployTime = 60f
                },
                attackRange = new Vector2Int(3, 3),
                skillIds = new string[] { "skill_heal_boost", "skill_emergency_heal" },
                talentIds = new string[] { "talent_healing_mastery", "talent_sp_recovery" }
            };
            exampleConfigs.Add(ptilopsis);
            
            // 示例4：狙击干员 - 能天使
            OperatorConfig exusiai = new OperatorConfig
            {
                operatorId = "exusiai",
                operatorName = "能天使",
                profession = OperatorProfession.Sniper,
                rarity = 6,
                baseStats = new OperatorStats
                {
                    maxHP = 1400,
                    currentHP = 1400,
                    attack = 420,
                    defense = 60,
                    magicResistance = 0,
                    deployCost = 11,
                    blockCount = 1,
                    attackInterval = 1.0f,
                    redeployTime = 65f
                },
                attackRange = new Vector2Int(3, 3),
                skillIds = new string[] { "skill_rapid_fire", "skill_overdrive" },
                talentIds = new string[] { "talent_precision", "talent_ace" }
            };
            exampleConfigs.Add(exusiai);
            
            // 示例5：术师干员 - 艾雅法拉
            OperatorConfig eyjafjalla = new OperatorConfig
            {
                operatorId = "eyjafjalla",
                operatorName = "艾雅法拉",
                profession = OperatorProfession.Caster,
                rarity = 6,
                baseStats = new OperatorStats
                {
                    maxHP = 1300,
                    currentHP = 1300,
                    attack = 380,
                    defense = 50,
                    magicResistance = 0,
                    deployCost = 19,
                    blockCount = 1,
                    attackInterval = 1.6f,
                    redeployTime = 75f
                },
                attackRange = new Vector2Int(3, 3),
                skillIds = new string[] { "skill_volcanic_burst", "skill_ignition" },
                talentIds = new string[] { "talent_volcanic_heart", "talent_mastery" }
            };
            exampleConfigs.Add(eyjafjalla);
        }
        
        /// <summary>
        /// 获取示例配置
        /// </summary>
        public List<OperatorConfig> GetExampleConfigs()
        {
            return exampleConfigs;
        }
        
        /// <summary>
        /// 根据ID获取示例配置
        /// </summary>
        public OperatorConfig GetExampleConfig(string operatorId)
        {
            return exampleConfigs.Find(config => config.operatorId == operatorId);
        }
        
        /// <summary>
        /// 打印所有示例配置信息
        /// </summary>
        public void PrintAllConfigs()
        {
            foreach (var config in exampleConfigs)
            {
                Debug.Log($"干员: {config.operatorName} ({config.operatorId})");
                Debug.Log($"  职业: {config.profession}, 稀有度: {config.rarity}星");
                Debug.Log($"  血量: {config.baseStats.maxHP}, 攻击: {config.baseStats.attack}, 防御: {config.baseStats.defense}");
                Debug.Log($"  部署费用: {config.baseStats.deployCost}, 阻挡数: {config.baseStats.blockCount}");
                Debug.Log($"  攻击间隔: {config.baseStats.attackInterval}s, 再部署时间: {config.baseStats.redeployTime}s");
                Debug.Log($"  攻击范围: {config.attackRange.x}x{config.attackRange.y}");
                Debug.Log("---");
            }
        }
    }
    
    /// <summary>
    /// 示例技能配置
    /// </summary>
    public class ExampleSkills : MonoBehaviour
    {
        [Header("示例技能")]
        public List<Skill> exampleSkills;
        
        private void Awake()
        {
            CreateExampleSkills();
        }
        
        /// <summary>
        /// 创建示例技能
        /// </summary>
        private void CreateExampleSkills()
        {
            exampleSkills = new List<Skill>();
            
            // 先锋技能：冲锋
            Skill vanguardCharge = ScriptableObject.CreateInstance<Skill>();
            vanguardCharge.skillName = "冲锋";
            vanguardCharge.skillId = "skill_vanguard_charge";
            vanguardCharge.skillType = SkillType.Active;
            vanguardCharge.description = "立即回复2点部署费用，攻击力+50%，持续10秒";
            vanguardCharge.cooldown = 30f;
            vanguardCharge.duration = 10f;
            vanguardCharge.spCost = 20;
            vanguardCharge.spRecovery = 5f;
            vanguardCharge.effects = new SkillEffect[]
            {
                new SkillEffect
                {
                    effectType = SkillEffectType.Buff,
                    value = 50f,
                    duration = 10f,
                    description = "攻击力+50%"
                }
            };
            exampleSkills.Add(vanguardCharge);
            
            // 领袖技能
            Skill leader = ScriptableObject.CreateInstance<Skill>();
            leader.skillName = "领袖";
            leader.skillId = "skill_leader";
            leader.skillType = SkillType.Passive;
            leader.description = "所有友军攻击力+10%";
            leader.effects = new SkillEffect[]
            {
                new SkillEffect
                {
                    effectType = SkillEffectType.Buff,
                    value = 10f,
                    duration = -1f, // 永久
                    description = "团队攻击力+10%"
                }
            };
            exampleSkills.Add(leader);
            
            // 重装技能：嘲讽
            Skill defenderTaunt = ScriptableObject.CreateInstance<Skill>();
            defenderTaunt.skillName = "嘲讽";
            defenderTaunt.skillId = "skill_defender_taunt";
            defenderTaunt.skillType = SkillType.Active;
            defenderTaunt.description = "防御力+100%，吸引所有敌人攻击，持续8秒";
            defenderTaunt.cooldown = 25f;
            defenderTaunt.duration = 8f;
            defenderTaunt.spCost = 15;
            defenderTaunt.spRecovery = 3f;
            defenderTaunt.effects = new SkillEffect[]
            {
                new SkillEffect
                {
                    effectType = SkillEffectType.Buff,
                    value = 100f,
                    duration = 8f,
                    description = "防御力+100%"
                }
            };
            exampleSkills.Add(defenderTaunt);
            
            // 医疗技能：治疗强化
            Skill healBoost = ScriptableObject.CreateInstance<Skill>();
            healBoost.skillName = "治疗强化";
            healBoost.skillId = "skill_heal_boost";
            healBoost.skillType = SkillType.Active;
            healBoost.description = "治疗量+80%，治疗范围扩大，持续12秒";
            healBoost.cooldown = 35f;
            healBoost.duration = 12f;
            healBoost.spCost = 25;
            healBoost.spRecovery = 4f;
            healBoost.effects = new SkillEffect[]
            {
                new SkillEffect
                {
                    effectType = SkillEffectType.Buff,
                    value = 80f,
                    duration = 12f,
                    description = "治疗量+80%"
                }
            };
            exampleSkills.Add(healBoost);
            
            // 狙击技能：连射
            Skill rapidFire = ScriptableObject.CreateInstance<Skill>();
            rapidFire.skillName = "连射";
            rapidFire.skillId = "skill_rapid_fire";
            rapidFire.skillType = SkillType.Active;
            rapidFire.description = "攻击间隔-50%，攻击力+30%，持续15秒";
            rapidFire.cooldown = 40f;
            rapidFire.duration = 15f;
            rapidFire.spCost = 30;
            rapidFire.spRecovery = 6f;
            rapidFire.effects = new SkillEffect[]
            {
                new SkillEffect
                {
                    effectType = SkillEffectType.Buff,
                    value = 30f,
                    duration = 15f,
                    description = "攻击力+30%"
                }
            };
            exampleSkills.Add(rapidFire);
        }
    }
    
    /// <summary>
    /// 示例天赋配置
    /// </summary>
    public class ExampleTalents : MonoBehaviour
    {
        [Header("示例天赋")]
        public List<Talent> exampleTalents;
        
        private void Awake()
        {
            CreateExampleTalents();
        }
        
        /// <summary>
        /// 创建示例天赋
        /// </summary>
        private void CreateExampleTalents()
        {
            exampleTalents = new List<Talent>();
            
            // 领袖天赋
            Talent leader = ScriptableObject.CreateInstance<Talent>();
            leader.talentName = "领袖";
            leader.talentId = "talent_leader";
            leader.talentType = TalentType.Team;
            leader.description = "所有友军攻击力+10%";
            leader.isAlwaysActive = true;
            leader.requiredEliteLevel = 1;
            leader.requiredLevel = 1;
            leader.effects = new TalentEffect[]
            {
                new TalentEffect
                {
                    effectName = "attack",
                    value = 10f,
                    targetType = "team",
                    condition = "always",
                    description = "团队攻击力+10%"
                }
            };
            exampleTalents.Add(leader);
            
            // 精英天赋
            Talent elite = ScriptableObject.CreateInstance<Talent>();
            elite.talentName = "精英";
            elite.talentId = "talent_elite";
            elite.talentType = TalentType.Self;
            elite.description = "精英化后攻击力+15%，防御力+15%";
            elite.isAlwaysActive = true;
            elite.requiredEliteLevel = 2;
            elite.requiredLevel = 1;
            elite.effects = new TalentEffect[]
            {
                new TalentEffect
                {
                    effectName = "attack",
                    value = 15f,
                    targetType = "self",
                    condition = "elite2",
                    description = "攻击力+15%"
                },
                new TalentEffect
                {
                    effectName = "defense",
                    value = 15f,
                    targetType = "self",
                    condition = "elite2",
                    description = "防御力+15%"
                }
            };
            exampleTalents.Add(elite);
            
            // 铁壁天赋
            Talent ironWill = ScriptableObject.CreateInstance<Talent>();
            ironWill.talentName = "铁壁";
            ironWill.talentId = "talent_iron_will";
            ironWill.talentType = TalentType.Self;
            ironWill.description = "受到伤害时，有25%概率减少50%伤害";
            ironWill.isAlwaysActive = false;
            ironWill.activationChance = 0.25f;
            ironWill.requiredEliteLevel = 1;
            ironWill.requiredLevel = 1;
            ironWill.effects = new TalentEffect[]
            {
                new TalentEffect
                {
                    effectName = "damage_reduction",
                    value = 50f,
                    targetType = "self",
                    condition = "on_damage",
                    description = "伤害减免50%"
                }
            };
            exampleTalents.Add(ironWill);
            
            // 治疗精通天赋
            Talent healingMastery = ScriptableObject.CreateInstance<Talent>();
            healingMastery.talentName = "治疗精通";
            healingMastery.talentId = "talent_healing_mastery";
            healingMastery.talentType = TalentType.Self;
            healingMastery.description = "治疗量+20%";
            healingMastery.isAlwaysActive = true;
            healingMastery.requiredEliteLevel = 1;
            healingMastery.requiredLevel = 1;
            healingMastery.effects = new TalentEffect[]
            {
                new TalentEffect
                {
                    effectName = "heal_boost",
                    value = 20f,
                    targetType = "self",
                    condition = "always",
                    description = "治疗量+20%"
                }
            };
            exampleTalents.Add(healingMastery);
            
            // 精准天赋
            Talent precision = ScriptableObject.CreateInstance<Talent>();
            precision.talentName = "精准";
            precision.talentId = "talent_precision";
            precision.talentType = TalentType.Self;
            precision.description = "攻击时有15%概率造成暴击，伤害翻倍";
            precision.isAlwaysActive = false;
            precision.activationChance = 0.15f;
            precision.requiredEliteLevel = 1;
            precision.requiredLevel = 1;
            precision.effects = new TalentEffect[]
            {
                new TalentEffect
                {
                    effectName = "critical_hit",
                    value = 100f,
                    targetType = "self",
                    condition = "on_attack",
                    description = "暴击伤害+100%"
                }
            };
            exampleTalents.Add(precision);
        }
    }
} 