using System.Collections.Generic;
using UnityEngine;

namespace TDGame.Role
{
    /// <summary>
    /// 干员管理器类
    /// </summary>
    public class OperatorManager : MonoBehaviour
    {
        [Header("干员工厂")]
        public OperatorFactory operatorFactory;
        
        [Header("部署设置")]
        public LayerMask deployableLayer;   // 可部署层
        public float deployRange = 5f;      // 部署范围
        
        // 干员列表
        private List<Operator> allOperators = new List<Operator>();
        private List<Operator> deployedOperators = new List<Operator>();
        private List<Operator> availableOperators = new List<Operator>();
        
        // 事件
        public System.Action<Operator> OnOperatorDeployed;
        public System.Action<Operator> OnOperatorRetreated;
        public System.Action<Operator> OnOperatorDied;
        
        private void Awake()
        {
            if (operatorFactory == null)
            {
                operatorFactory = FindObjectOfType<OperatorFactory>();
            }
        }
        
        private void Update()
        {
            UpdateOperators();
        }
        
        /// <summary>
        /// 更新所有干员状态
        /// </summary>
        private void UpdateOperators()
        {
            // 更新已部署干员
            for (int i = deployedOperators.Count - 1; i >= 0; i--)
            {
                var op = deployedOperators[i];
                
                if (op == null)
                {
                    deployedOperators.RemoveAt(i);
                    continue;
                }
                
                // 更新技能
                foreach (var skill in op.skills)
                {
                    skill.Update(Time.deltaTime);
                }
                
                // 更新天赋
                foreach (var talent in op.talents)
                {
                    talent.Update();
                }
                
                // 检查攻击
                if (op.CanAttack())
                {
                    op.Attack();
                }
            }
        }
        
        /// <summary>
        /// 部署干员
        /// </summary>
        public bool DeployOperator(string operatorId, Vector3 position)
        {
            // 检查位置是否可部署
            if (!IsPositionDeployable(position))
            {
                Debug.LogWarning("位置不可部署");
                return false;
            }
            
            // 检查是否有可用干员
            var availableOp = GetAvailableOperator(operatorId);
            if (availableOp == null)
            {
                Debug.LogWarning($"没有可用的干员: {operatorId}");
                return false;
            }
            
            // 部署干员
            if (availableOp.Deploy(position))
            {
                deployedOperators.Add(availableOp);
                availableOperators.Remove(availableOp);
                
                // 订阅事件
                availableOp.OnDeath += OnOperatorDeath;
                availableOp.OnRetreat += OnOperatorRetreat;
                
                OnOperatorDeployed?.Invoke(availableOp);
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// 撤退干员
        /// </summary>
        public void RetreatOperator(Operator op)
        {
            if (!deployedOperators.Contains(op)) return;
            
            op.Retreat();
            deployedOperators.Remove(op);
            availableOperators.Add(op);
            
            // 取消订阅事件
            op.OnDeath -= OnOperatorDeath;
            op.OnRetreat -= OnOperatorRetreat;
            
            OnOperatorRetreated?.Invoke(op);
        }
        
        /// <summary>
        /// 撤退所有干员
        /// </summary>
        public void RetreatAllOperators()
        {
            var operatorsToRetreat = new List<Operator>(deployedOperators);
            
            foreach (var op in operatorsToRetreat)
            {
                RetreatOperator(op);
            }
        }
        
        /// <summary>
        /// 添加可用干员
        /// </summary>
        public void AddAvailableOperator(string operatorId)
        {
            var op = operatorFactory.CreateOperator(operatorId, Vector3.zero);
            if (op != null)
            {
                availableOperators.Add(op);
                allOperators.Add(op);
            }
        }
        
        /// <summary>
        /// 移除干员
        /// </summary>
        public void RemoveOperator(Operator op)
        {
            if (deployedOperators.Contains(op))
            {
                RetreatOperator(op);
            }
            
            if (availableOperators.Contains(op))
            {
                availableOperators.Remove(op);
            }
            
            if (allOperators.Contains(op))
            {
                allOperators.Remove(op);
            }
            
            if (op != null)
            {
                Destroy(op.gameObject);
            }
        }
        
        /// <summary>
        /// 获取可用干员
        /// </summary>
        private Operator GetAvailableOperator(string operatorId)
        {
            return availableOperators.Find(op => op.operatorId == operatorId);
        }
        
        /// <summary>
        /// 检查位置是否可部署
        /// </summary>
        private bool IsPositionDeployable(Vector3 position)
        {
            // 检查是否有其他干员在该位置
            foreach (var op in deployedOperators)
            {
                if (Vector3.Distance(op.transform.position, position) < 1f)
                {
                    return false;
                }
            }
            
            // 检查地形是否可部署
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.5f, deployableLayer);
            return colliders.Length > 0;
        }
        
        /// <summary>
        /// 干员死亡事件处理
        /// </summary>
        private void OnOperatorDeath(Operator op)
        {
            deployedOperators.Remove(op);
            OnOperatorDied?.Invoke(op);
        }
        
        /// <summary>
        /// 干员撤退事件处理
        /// </summary>
        private void OnOperatorRetreat(Operator op)
        {
            // 这个事件在RetreatOperator中已经处理了
        }
        
        /// <summary>
        /// 获取所有干员
        /// </summary>
        public List<Operator> GetAllOperators()
        {
            return new List<Operator>(allOperators);
        }
        
        /// <summary>
        /// 获取已部署干员
        /// </summary>
        public List<Operator> GetDeployedOperators()
        {
            return new List<Operator>(deployedOperators);
        }
        
        /// <summary>
        /// 获取可用干员
        /// </summary>
        public List<Operator> GetAvailableOperators()
        {
            return new List<Operator>(availableOperators);
        }
        
        /// <summary>
        /// 根据职业获取已部署干员
        /// </summary>
        public List<Operator> GetDeployedOperatorsByProfession(OperatorProfession profession)
        {
            List<Operator> result = new List<Operator>();
            
            foreach (var op in deployedOperators)
            {
                if (op.profession == profession)
                {
                    result.Add(op);
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// 获取指定范围内的干员
        /// </summary>
        public List<Operator> GetOperatorsInRange(Vector3 center, float range)
        {
            List<Operator> result = new List<Operator>();
            
            foreach (var op in deployedOperators)
            {
                if (Vector3.Distance(op.transform.position, center) <= range)
                {
                    result.Add(op);
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// 获取血量最低的干员
        /// </summary>
        public Operator GetLowestHealthOperator()
        {
            Operator lowest = null;
            float lowestHealth = float.MaxValue;
            
            foreach (var op in deployedOperators)
            {
                if (op.GetHealthPercentage() < lowestHealth)
                {
                    lowestHealth = op.GetHealthPercentage();
                    lowest = op;
                }
            }
            
            return lowest;
        }
        
        /// <summary>
        /// 获取攻击力最高的干员
        /// </summary>
        public Operator GetHighestAttackOperator()
        {
            Operator highest = null;
            int highestAttack = 0;
            
            foreach (var op in deployedOperators)
            {
                if (op.currentStats.attack > highestAttack)
                {
                    highestAttack = op.currentStats.attack;
                    highest = op;
                }
            }
            
            return highest;
        }
        
        /// <summary>
        /// 治疗所有干员
        /// </summary>
        public void HealAllOperators(int healAmount)
        {
            foreach (var op in deployedOperators)
            {
                op.Heal(healAmount);
            }
        }
        
        /// <summary>
        /// 治疗指定职业的干员
        /// </summary>
        public void HealOperatorsByProfession(OperatorProfession profession, int healAmount)
        {
            foreach (var op in deployedOperators)
            {
                if (op.profession == profession)
                {
                    op.Heal(healAmount);
                }
            }
        }
        
        /// <summary>
        /// 获取团队总血量
        /// </summary>
        public int GetTeamTotalHealth()
        {
            int total = 0;
            foreach (var op in deployedOperators)
            {
                total += op.currentStats.currentHP;
            }
            return total;
        }
        
        /// <summary>
        /// 获取团队最大血量
        /// </summary>
        public int GetTeamMaxHealth()
        {
            int total = 0;
            foreach (var op in deployedOperators)
            {
                total += op.currentStats.maxHP;
            }
            return total;
        }
        
        /// <summary>
        /// 获取团队血量百分比
        /// </summary>
        public float GetTeamHealthPercentage()
        {
            int current = GetTeamTotalHealth();
            int max = GetTeamMaxHealth();
            
            return max > 0 ? (float)current / max : 0f;
        }
    }
} 