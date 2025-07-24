using UnityEngine;
using System.Collections.Generic;

namespace TDGame.Role
{
    /// <summary>
    /// 干员系统测试脚本
    /// 演示如何使用干员系统的各个组件
    /// </summary>
    public class OperatorSystemTest : MonoBehaviour
    {
        [Header("测试组件")]
        public OperatorManager operatorManager;
        public OperatorFactory operatorFactory;
        public OperatorUI operatorUI;
        
        [Header("测试设置")]
        public string testOperatorId = "silver_ash";
        public Vector3 deployPosition = new Vector3(0, 0, 0);
        
        private Operator testOperator;
        
        private void Start()
        {
            InitializeTest();
        }
        
        /// <summary>
        /// 初始化测试
        /// </summary>
        private void InitializeTest()
        {
            // 获取组件引用
            if (operatorManager == null)
                operatorManager = FindObjectOfType<OperatorManager>();
                
            if (operatorFactory == null)
                operatorFactory = FindObjectOfType<OperatorFactory>();
                
            if (operatorUI == null)
                operatorUI = FindObjectOfType<OperatorUI>();
            
            // 创建测试干员
            CreateTestOperator();
            
            // 设置UI
            if (operatorUI != null && testOperator != null)
            {
                operatorUI.SetTargetOperator(testOperator);
            }
        }
        
        /// <summary>
        /// 创建测试干员
        /// </summary>
        private void CreateTestOperator()
        {
            if (operatorFactory == null) return;
            
            // 创建干员实例
            testOperator = operatorFactory.CreateOperator(testOperatorId, Vector3.zero);
            
            if (testOperator != null)
            {
                Debug.Log($"成功创建干员: {testOperator.operatorName}");
                
                // 添加到可用干员列表
                if (operatorManager != null)
                {
                    operatorManager.AddAvailableOperator(testOperatorId);
                }
            }
            else
            {
                Debug.LogError($"创建干员失败: {testOperatorId}");
            }
        }
        
        /// <summary>
        /// 测试部署干员
        /// </summary>
        [ContextMenu("测试部署干员")]
        public void TestDeployOperator()
        {
            if (operatorManager == null) return;
            
            bool success = operatorManager.DeployOperator(testOperatorId, deployPosition);
            
            if (success)
            {
                Debug.Log($"成功部署干员: {testOperatorId}");
            }
            else
            {
                Debug.LogWarning($"部署干员失败: {testOperatorId}");
            }
        }
        
        /// <summary>
        /// 测试撤退干员
        /// </summary>
        [ContextMenu("测试撤退干员")]
        public void TestRetreatOperator()
        {
            if (operatorManager == null) return;
            
            var deployed = operatorManager.GetDeployedOperators();
            if (deployed.Count > 0)
            {
                operatorManager.RetreatOperator(deployed[0]);
                Debug.Log("成功撤退干员");
            }
            else
            {
                Debug.LogWarning("没有已部署的干员");
            }
        }
        
        /// <summary>
        /// 测试干员升级
        /// </summary>
        [ContextMenu("测试干员升级")]
        public void TestLevelUp()
        {
            if (testOperator == null) return;
            
            int oldLevel = testOperator.level;
            testOperator.LevelUp();
            
            Debug.Log($"干员升级: {oldLevel} -> {testOperator.level}");
            
            // 更新UI
            if (operatorUI != null)
            {
                operatorUI.UpdateUI();
            }
        }
        
        /// <summary>
        /// 测试干员精英化
        /// </summary>
        [ContextMenu("测试干员精英化")]
        public void TestElite()
        {
            if (testOperator == null) return;
            
            int oldElite = testOperator.eliteLevel;
            bool success = testOperator.Elite();
            
            if (success)
            {
                Debug.Log($"干员精英化: {oldElite} -> {testOperator.eliteLevel}");
            }
            else
            {
                Debug.LogWarning("精英化失败，可能已达到最高等级");
            }
            
            // 更新UI
            if (operatorUI != null)
            {
                operatorUI.UpdateUI();
            }
        }
        
        /// <summary>
        /// 测试干员受到伤害
        /// </summary>
        [ContextMenu("测试受到伤害")]
        public void TestTakeDamage()
        {
            if (testOperator == null) return;
            
            int oldHP = testOperator.currentStats.currentHP;
            int damage = 100;
            
            testOperator.TakeDamage(damage);
            
            Debug.Log($"干员受到伤害: {oldHP} -> {testOperator.currentStats.currentHP} (伤害: {damage})");
            
            // 更新UI
            if (operatorUI != null)
            {
                operatorUI.UpdateUI();
            }
        }
        
        /// <summary>
        /// 测试干员治疗
        /// </summary>
        [ContextMenu("测试干员治疗")]
        public void TestHeal()
        {
            if (testOperator == null) return;
            
            int oldHP = testOperator.currentStats.currentHP;
            int healAmount = 50;
            
            testOperator.Heal(healAmount);
            
            Debug.Log($"干员治疗: {oldHP} -> {testOperator.currentStats.currentHP} (治疗: {healAmount})");
            
            // 更新UI
            if (operatorUI != null)
            {
                operatorUI.UpdateUI();
            }
        }
        
        /// <summary>
        /// 测试技能使用
        /// </summary>
        [ContextMenu("测试技能使用")]
        public void TestSkillUse()
        {
            if (testOperator == null || testOperator.skills.Count == 0) return;
            
            var skill = testOperator.skills[0];
            bool success = skill.TryUse(testOperator);
            
            if (success)
            {
                Debug.Log($"成功使用技能: {skill.skillName}");
            }
            else
            {
                Debug.LogWarning($"使用技能失败: {skill.skillName}");
            }
        }
        
        /// <summary>
        /// 测试团队状态
        /// </summary>
        [ContextMenu("测试团队状态")]
        public void TestTeamStatus()
        {
            if (operatorManager == null) return;
            
            var deployed = operatorManager.GetDeployedOperators();
            Debug.Log($"已部署干员数量: {deployed.Count}");
            
            if (deployed.Count > 0)
            {
                float teamHealth = operatorManager.GetTeamHealthPercentage();
                Debug.Log($"团队血量百分比: {teamHealth:P1}");
                
                var lowestHealth = operatorManager.GetLowestHealthOperator();
                if (lowestHealth != null)
                {
                    Debug.Log($"血量最低干员: {lowestHealth.operatorName} ({lowestHealth.GetHealthPercentage():P1})");
                }
                
                var highestAttack = operatorManager.GetHighestAttackOperator();
                if (highestAttack != null)
                {
                    Debug.Log($"攻击力最高干员: {highestAttack.operatorName} (攻击: {highestAttack.currentStats.attack})");
                }
            }
        }
        
        /// <summary>
        /// 测试职业分类
        /// </summary>
        [ContextMenu("测试职业分类")]
        public void TestProfessionFilter()
        {
            if (operatorManager == null) return;
            
            var vanguards = operatorManager.GetDeployedOperatorsByProfession(OperatorProfession.Vanguard);
            var defenders = operatorManager.GetDeployedOperatorsByProfession(OperatorProfession.Defender);
            var medics = operatorManager.GetDeployedOperatorsByProfession(OperatorProfession.Medic);
            
            Debug.Log($"先锋: {vanguards.Count}, 重装: {defenders.Count}, 医疗: {medics.Count}");
        }
        
        /// <summary>
        /// 测试范围查询
        /// </summary>
        [ContextMenu("测试范围查询")]
        public void TestRangeQuery()
        {
            if (operatorManager == null) return;
            
            Vector3 center = deployPosition;
            float range = 5f;
            
            var operatorsInRange = operatorManager.GetOperatorsInRange(center, range);
            Debug.Log($"范围内干员数量: {operatorsInRange.Count} (范围: {range})");
            
            foreach (var op in operatorsInRange)
            {
                float distance = Vector3.Distance(op.transform.position, center);
                Debug.Log($"  {op.operatorName}: {distance:F1} 单位");
            }
        }
        
        /// <summary>
        /// 打印干员详细信息
        /// </summary>
        [ContextMenu("打印干员信息")]
        public void PrintOperatorInfo()
        {
            if (testOperator == null) return;
            
            Debug.Log("=== 干员详细信息 ===");
            Debug.Log($"名称: {testOperator.operatorName}");
            Debug.Log($"ID: {testOperator.operatorId}");
            Debug.Log($"职业: {testOperator.profession}");
            Debug.Log($"稀有度: {testOperator.rarity}星");
            Debug.Log($"等级: {testOperator.level}");
            Debug.Log($"精英化: {testOperator.eliteLevel}");
            Debug.Log($"血量: {testOperator.currentStats.currentHP}/{testOperator.currentStats.maxHP}");
            Debug.Log($"攻击: {testOperator.currentStats.attack}");
            Debug.Log($"防御: {testOperator.currentStats.defense}");
            Debug.Log($"法抗: {testOperator.currentStats.magicResistance}%");
            Debug.Log($"部署费用: {testOperator.currentStats.deployCost}");
            Debug.Log($"阻挡数: {testOperator.currentStats.blockCount}");
            Debug.Log($"攻击间隔: {testOperator.currentStats.attackInterval}s");
            Debug.Log($"技能数量: {testOperator.skills.Count}");
            Debug.Log($"天赋数量: {testOperator.talents.Count}");
            Debug.Log($"状态: {(testOperator.isAlive ? "存活" : "死亡")} {(testOperator.isDeployed ? "已部署" : "未部署")}");
            Debug.Log("==================");
        }
        
        /// <summary>
        /// 运行完整测试流程
        /// </summary>
        [ContextMenu("运行完整测试")]
        public void RunFullTest()
        {
            Debug.Log("开始运行完整测试流程...");
            
            // 1. 创建干员
            CreateTestOperator();
            
            // 2. 升级和精英化
            TestLevelUp();
            TestElite();
            
            // 3. 部署干员
            TestDeployOperator();
            
            // 4. 测试伤害和治疗
            TestTakeDamage();
            TestHeal();
            
            // 5. 测试技能
            TestSkillUse();
            
            // 6. 测试团队功能
            TestTeamStatus();
            TestProfessionFilter();
            TestRangeQuery();
            
            // 7. 打印信息
            PrintOperatorInfo();
            
            Debug.Log("完整测试流程完成！");
        }
        
        private void OnGUI()
        {
            if (!Application.isPlaying) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 400));
            GUILayout.Label("干员系统测试", GUI.skin.box);
            
            if (GUILayout.Button("部署干员"))
                TestDeployOperator();
                
            if (GUILayout.Button("撤退干员"))
                TestRetreatOperator();
                
            if (GUILayout.Button("升级干员"))
                TestLevelUp();
                
            if (GUILayout.Button("精英化干员"))
                TestElite();
                
            if (GUILayout.Button("受到伤害"))
                TestTakeDamage();
                
            if (GUILayout.Button("治疗干员"))
                TestHeal();
                
            if (GUILayout.Button("使用技能"))
                TestSkillUse();
                
            if (GUILayout.Button("团队状态"))
                TestTeamStatus();
                
            if (GUILayout.Button("职业分类"))
                TestProfessionFilter();
                
            if (GUILayout.Button("范围查询"))
                TestRangeQuery();
                
            if (GUILayout.Button("打印信息"))
                PrintOperatorInfo();
                
            if (GUILayout.Button("完整测试"))
                RunFullTest();
                
            GUILayout.EndArea();
        }
    }
} 