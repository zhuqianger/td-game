using System.Collections.Generic;
using UnityEngine;

namespace TDGame.Role
{
    /// <summary>
    /// 干员配置数据
    /// </summary>
    [System.Serializable]
    public class OperatorConfig
    {
        public string operatorId;
        public string operatorName;
        public OperatorProfession profession;
        public int rarity;
        public OperatorStats baseStats;
        public Vector2Int attackRange;
        public string[] skillIds;
        public string[] talentIds;
    }
    
    /// <summary>
    /// 干员工厂类
    /// </summary>
    public class OperatorFactory : MonoBehaviour
    {
        [Header("干员预制体")]
        public GameObject operatorPrefab;
        
        [Header("干员配置")]
        public List<OperatorConfig> operatorConfigs;
        
        [Header("技能数据库")]
        public List<Skill> skillDatabase;
        
        [Header("天赋数据库")]
        public List<Talent> talentDatabase;
        
        private Dictionary<string, OperatorConfig> configLookup;
        private Dictionary<string, Skill> skillLookup;
        private Dictionary<string, Talent> talentLookup;
        
        private void Awake()
        {
            InitializeLookups();
        }
        
        /// <summary>
        /// 初始化查找表
        /// </summary>
        private void InitializeLookups()
        {
            configLookup = new Dictionary<string, OperatorConfig>();
            skillLookup = new Dictionary<string, Skill>();
            talentLookup = new Dictionary<string, Talent>();
            
            // 初始化配置查找表
            foreach (var config in operatorConfigs)
            {
                configLookup[config.operatorId] = config;
            }
            
            // 初始化技能查找表
            foreach (var skill in skillDatabase)
            {
                skillLookup[skill.skillId] = skill;
            }
            
            // 初始化天赋查找表
            foreach (var talent in talentDatabase)
            {
                talentLookup[talent.talentId] = talent;
            }
        }
        
        /// <summary>
        /// 创建干员实例
        /// </summary>
        public Operator CreateOperator(string operatorId, Vector3 position)
        {
            if (!configLookup.ContainsKey(operatorId))
            {
                Debug.LogError($"干员配置不存在: {operatorId}");
                return null;
            }
            
            var config = configLookup[operatorId];
            
            // 实例化干员预制体
            GameObject operatorObj = Instantiate(operatorPrefab, position, Quaternion.identity);
            Operator operatorComponent = operatorObj.GetComponent<Operator>();
            
            if (operatorComponent == null)
            {
                Debug.LogError("干员预制体缺少Operator组件");
                Destroy(operatorObj);
                return null;
            }
            
            // 配置干员属性
            ConfigureOperator(operatorComponent, config);
            
            return operatorComponent;
        }
        
        /// <summary>
        /// 配置干员属性
        /// </summary>
        private void ConfigureOperator(Operator operatorComponent, OperatorConfig config)
        {
            // 设置基础信息
            operatorComponent.operatorId = config.operatorId;
            operatorComponent.operatorName = config.operatorName;
            operatorComponent.profession = config.profession;
            operatorComponent.rarity = config.rarity;
            
            // 设置基础属性
            operatorComponent.baseStats = config.baseStats;
            operatorComponent.currentStats = config.baseStats;
            operatorComponent.currentStats.currentHP = config.baseStats.maxHP;
            
            // 设置攻击范围
            operatorComponent.attackRange = config.attackRange;
            
            // 添加技能
            if (config.skillIds != null)
            {
                foreach (string skillId in config.skillIds)
                {
                    if (skillLookup.ContainsKey(skillId))
                    {
                        Skill skill = skillLookup[skillId];
                        operatorComponent.AddSkill(skill);
                        skill.Initialize();
                    }
                    else
                    {
                        Debug.LogWarning($"技能不存在: {skillId}");
                    }
                }
            }
            
            // 添加天赋
            if (config.talentIds != null)
            {
                foreach (string talentId in config.talentIds)
                {
                    if (talentLookup.ContainsKey(talentId))
                    {
                        Talent talent = talentLookup[talentId];
                        operatorComponent.AddTalent(talent);
                        talent.Initialize(operatorComponent);
                    }
                    else
                    {
                        Debug.LogWarning($"天赋不存在: {talentId}");
                    }
                }
            }
        }
        
        /// <summary>
        /// 获取干员配置
        /// </summary>
        public OperatorConfig GetOperatorConfig(string operatorId)
        {
            return configLookup.ContainsKey(operatorId) ? configLookup[operatorId] : null;
        }
        
        /// <summary>
        /// 获取所有干员配置
        /// </summary>
        public List<OperatorConfig> GetAllOperatorConfigs()
        {
            return new List<OperatorConfig>(operatorConfigs);
        }
        
        /// <summary>
        /// 根据职业获取干员配置
        /// </summary>
        public List<OperatorConfig> GetOperatorConfigsByProfession(OperatorProfession profession)
        {
            List<OperatorConfig> result = new List<OperatorConfig>();
            
            foreach (var config in operatorConfigs)
            {
                if (config.profession == profession)
                {
                    result.Add(config);
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// 根据稀有度获取干员配置
        /// </summary>
        public List<OperatorConfig> GetOperatorConfigsByRarity(int rarity)
        {
            List<OperatorConfig> result = new List<OperatorConfig>();
            
            foreach (var config in operatorConfigs)
            {
                if (config.rarity == rarity)
                {
                    result.Add(config);
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// 获取技能
        /// </summary>
        public Skill GetSkill(string skillId)
        {
            return skillLookup.ContainsKey(skillId) ? skillLookup[skillId] : null;
        }
        
        /// <summary>
        /// 获取天赋
        /// </summary>
        public Talent GetTalent(string talentId)
        {
            return talentLookup.ContainsKey(talentId) ? talentLookup[talentId] : null;
        }
        
        /// <summary>
        /// 验证干员配置
        /// </summary>
        public bool ValidateOperatorConfig(string operatorId)
        {
            if (!configLookup.ContainsKey(operatorId))
                return false;
                
            var config = configLookup[operatorId];
            
            // 检查技能是否存在
            if (config.skillIds != null)
            {
                foreach (string skillId in config.skillIds)
                {
                    if (!skillLookup.ContainsKey(skillId))
                    {
                        Debug.LogError($"干员 {operatorId} 的技能 {skillId} 不存在");
                        return false;
                    }
                }
            }
            
            // 检查天赋是否存在
            if (config.talentIds != null)
            {
                foreach (string talentId in config.talentIds)
                {
                    if (!talentLookup.ContainsKey(talentId))
                    {
                        Debug.LogError($"干员 {operatorId} 的天赋 {talentId} 不存在");
                        return false;
                    }
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 验证所有配置
        /// </summary>
        public void ValidateAllConfigs()
        {
            foreach (var config in operatorConfigs)
            {
                ValidateOperatorConfig(config.operatorId);
            }
        }
    }
} 