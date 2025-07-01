using System.Collections.Generic;
using UnityEngine;

namespace TDGame.Stage
{
    /// <summary>
    /// 敌人类型枚举
    /// </summary>
    public enum EnemyType
    {
        Normal,     // 普通敌人
        Elite,      // 精英敌人
        Boss,       // BOSS敌人
        Flying,     // 飞行敌人
        Stealth     // 隐匿敌人
    }
    
    /// <summary>
    /// 敌人配置
    /// </summary>
    [System.Serializable]
    public class EnemyConfig
    {
        [Header("基础信息")]
        public string enemyId;
        public string enemyName;
        public EnemyType enemyType;
        public int rarity;
        
        [Header("基础属性")]
        public int maxHP;
        public int attack;
        public int defense;
        public int magicResistance;
        public float moveSpeed;
        public int blockCount;
        
        [Header("特殊属性")]
        public bool isFlying;           // 是否为飞行单位
        public bool isStealth;          // 是否为隐匿单位
        public bool canDestroyTerrain;  // 是否可以破坏地形
        public float stealthRange;      // 隐匿范围
        
        [Header("路径设置")]
        public List<Vector3> pathPoints; // 路径点
        public int currentPathIndex;     // 当前路径点索引
    }
    
    /// <summary>
    /// 敌人类
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        [Header("敌人配置")]
        public EnemyConfig config;
        
        [Header("运行时状态")]
        public int currentHP;
        public Vector3 targetPosition;
        public bool isMoving;
        public bool isAttacking;
        public bool isDead;
        public bool isLeaked;
        
        [Header("战斗状态")]
        public float lastAttackTime;
        public float attackInterval = 1.5f;
        public List<GameObject> blockedOperators;
        
        // 事件
        public System.Action<Enemy> OnEnemyDeath;
        public System.Action<Enemy> OnEnemyLeaked;
        public System.Action<Enemy> OnEnemyReachedTarget;
        
        private Rigidbody2D rb;
        private SpriteRenderer sr;
        private Animator animator;
        
        private void Awake()
        {
            InitializeEnemy();
        }
        
        private void Start()
        {
            if (config.pathPoints.Count > 0)
            {
                targetPosition = config.pathPoints[0];
                isMoving = true;
            }
        }
        
        private void Update()
        {
            if (isDead) return;
            
            UpdateMovement();
            UpdateCombat();
            UpdateStealth();
        }
        
        /// <summary>
        /// 初始化敌人
        /// </summary>
        private void InitializeEnemy()
        {
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            
            currentHP = config.maxHP;
            blockedOperators = new List<GameObject>();
            
            // 设置刚体属性
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
        
        /// <summary>
        /// 更新移动
        /// </summary>
        private void UpdateMovement()
        {
            if (!isMoving || isAttacking) return;
            
            Vector3 direction = (targetPosition - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetPosition);
            
            if (distance > 0.1f)
            {
                // 移动敌人
                transform.position += direction * config.moveSpeed * Time.deltaTime;
                
                // 更新朝向
                if (direction.x != 0)
                {
                    transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
                }
            }
            else
            {
                // 到达目标点
                transform.position = targetPosition;
                MoveToNextPathPoint();
            }
        }
        
        /// <summary>
        /// 移动到下一个路径点
        /// </summary>
        private void MoveToNextPathPoint()
        {
            config.currentPathIndex++;
            
            if (config.currentPathIndex < config.pathPoints.Count)
            {
                targetPosition = config.pathPoints[config.currentPathIndex];
            }
            else
            {
                // 到达终点，敌人漏过
                Leak();
            }
        }
        
        /// <summary>
        /// 更新战斗
        /// </summary>
        private void UpdateCombat()
        {
            if (blockedOperators.Count > 0 && Time.time - lastAttackTime >= attackInterval)
            {
                Attack();
            }
        }
        
        /// <summary>
        /// 更新隐匿状态
        /// </summary>
        private void UpdateStealth()
        {
            if (!config.isStealth) return;
            
            bool shouldBeStealth = true;
            
            // 检查是否有干员在隐匿范围内
            if (blockedOperators.Count > 0)
            {
                shouldBeStealth = false;
            }
            
            // 更新隐匿效果
            if (sr != null)
            {
                Color color = sr.color;
                color.a = shouldBeStealth ? 0.3f : 1f;
                sr.color = color;
            }
        }
        
        /// <summary>
        /// 受到伤害
        /// </summary>
        public void TakeDamage(int damage, bool isMagicDamage = false)
        {
            if (isDead) return;
            
            int finalDamage = CalculateDamage(damage, isMagicDamage);
            currentHP = Mathf.Max(0, currentHP - finalDamage);
            
            // 显示伤害数字
            ShowDamageNumber(finalDamage);
            
            if (currentHP <= 0)
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
                // 法术伤害计算
                float resistance = config.magicResistance / 100f;
                return Mathf.RoundToInt(damage * (1f - resistance));
            }
            else
            {
                // 物理伤害计算
                int finalDamage = damage - config.defense;
                return Mathf.Max(1, finalDamage);
            }
        }
        
        /// <summary>
        /// 显示伤害数字
        /// </summary>
        private void ShowDamageNumber(int damage)
        {
            // 这里应该实例化伤害数字UI
            Debug.Log($"敌人 {config.enemyName} 受到 {damage} 点伤害");
        }
        
        /// <summary>
        /// 攻击
        /// </summary>
        private void Attack()
        {
            if (blockedOperators.Count == 0) return;
            
            lastAttackTime = Time.time;
            isAttacking = true;
            
            // 攻击第一个阻挡的干员
            GameObject target = blockedOperators[0];
            var operatorComponent = target.GetComponent<Role.Operator>();
            
            if (operatorComponent != null)
            {
                operatorComponent.TakeDamage(config.attack);
            }
            
            // 播放攻击动画
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            
            // 延迟重置攻击状态
            Invoke(nameof(ResetAttackState), 0.5f);
        }
        
        /// <summary>
        /// 重置攻击状态
        /// </summary>
        private void ResetAttackState()
        {
            isAttacking = false;
        }
        
        /// <summary>
        /// 死亡
        /// </summary>
        private void Die()
        {
            isDead = true;
            isMoving = false;
            isAttacking = false;
            
            // 播放死亡动画
            if (animator != null)
            {
                animator.SetTrigger("Die");
            }
            
            // 延迟销毁
            Invoke(nameof(DestroyEnemy), 1f);
            
            OnEnemyDeath?.Invoke(this);
        }
        
        /// <summary>
        /// 漏过
        /// </summary>
        private void Leak()
        {
            isLeaked = true;
            isMoving = false;
            
            OnEnemyLeaked?.Invoke(this);
            
            // 立即销毁
            DestroyEnemy();
        }
        
        /// <summary>
        /// 销毁敌人
        /// </summary>
        private void DestroyEnemy()
        {
            Destroy(gameObject);
        }
        
        /// <summary>
        /// 添加阻挡的干员
        /// </summary>
        public void AddBlockedOperator(GameObject operatorObj)
        {
            if (!blockedOperators.Contains(operatorObj))
            {
                blockedOperators.Add(operatorObj);
            }
        }
        
        /// <summary>
        /// 移除阻挡的干员
        /// </summary>
        public void RemoveBlockedOperator(GameObject operatorObj)
        {
            blockedOperators.Remove(operatorObj);
        }
        
        /// <summary>
        /// 检查是否被阻挡
        /// </summary>
        public bool IsBlocked()
        {
            return blockedOperators.Count >= config.blockCount;
        }
        
        /// <summary>
        /// 获取血量百分比
        /// </summary>
        public float GetHealthPercentage()
        {
            return (float)currentHP / config.maxHP;
        }
        
        /// <summary>
        /// 获取当前路径进度
        /// </summary>
        public float GetPathProgress()
        {
            if (config.pathPoints.Count <= 1) return 1f;
            return (float)config.currentPathIndex / (config.pathPoints.Count - 1);
        }
        
        /// <summary>
        /// 设置路径点
        /// </summary>
        public void SetPathPoints(List<Vector3> points)
        {
            config.pathPoints = new List<Vector3>(points);
            config.currentPathIndex = 0;
            
            if (config.pathPoints.Count > 0)
            {
                targetPosition = config.pathPoints[0];
                isMoving = true;
            }
        }
        
        /// <summary>
        /// 减速效果
        /// </summary>
        public void ApplySlowEffect(float slowFactor, float duration)
        {
            float originalSpeed = config.moveSpeed;
            config.moveSpeed *= (1f - slowFactor);
            
            Invoke(nameof(ResetSpeed), duration);
        }
        
        /// <summary>
        /// 重置速度
        /// </summary>
        private void ResetSpeed()
        {
            // 这里应该恢复到原始速度
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 检查是否与干员碰撞
            if (other.CompareTag("Operator"))
            {
                AddBlockedOperator(other.gameObject);
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            // 检查是否离开干员
            if (other.CompareTag("Operator"))
            {
                RemoveBlockedOperator(other.gameObject);
            }
        }
    }
    
    /// <summary>
    /// 敌人管理器类
    /// </summary>
    public class EnemyManager : MonoBehaviour
    {
        [Header("敌人设置")]
        public GameObject enemyPrefab;
        public List<EnemyConfig> enemyConfigs;
        
        [Header("生成设置")]
        public Transform enemyContainer;
        public List<Vector3> spawnPoints;
        
        [Header("运行时状态")]
        public List<Enemy> activeEnemies;
        public int totalEnemiesSpawned;
        public int totalEnemiesKilled;
        public int totalEnemiesLeaked;
        
        // 事件
        public System.Action<Enemy> OnEnemySpawned;
        public System.Action<Enemy> OnEnemyKilled;
        public System.Action<Enemy> OnEnemyLeaked;
        
        private Dictionary<string, EnemyConfig> enemyConfigLookup;
        private Queue<EnemySpawn> spawnQueue;
        
        private void Awake()
        {
            InitializeEnemyManager();
        }
        
        /// <summary>
        /// 初始化敌人管理器
        /// </summary>
        private void InitializeEnemyManager()
        {
            activeEnemies = new List<Enemy>();
            spawnQueue = new Queue<EnemySpawn>();
            enemyConfigLookup = new Dictionary<string, EnemyConfig>();
            
            // 初始化查找表
            foreach (var config in enemyConfigs)
            {
                enemyConfigLookup[config.enemyId] = config;
            }
            
            if (enemyContainer == null)
            {
                enemyContainer = transform;
            }
        }
        
        /// <summary>
        /// 生成敌潮波次
        /// </summary>
        public void SpawnWave(EnemyWave wave)
        {
            if (wave.enemies == null) return;
            
            foreach (var enemySpawn in wave.enemies)
            {
                for (int i = 0; i < enemySpawn.count; i++)
                {
                    // 添加到生成队列
                    spawnQueue.Enqueue(enemySpawn);
                    
                    // 延迟生成
                    Invoke(nameof(SpawnNextEnemy), enemySpawn.spawnDelay + i * 0.5f);
                }
            }
        }
        
        /// <summary>
        /// 生成下一个敌人
        /// </summary>
        private void SpawnNextEnemy()
        {
            if (spawnQueue.Count == 0) return;
            
            var enemySpawn = spawnQueue.Dequeue();
            SpawnEnemy(enemySpawn);
        }
        
        /// <summary>
        /// 生成敌人
        /// </summary>
        public Enemy SpawnEnemy(EnemySpawn enemySpawn)
        {
            if (!enemyConfigLookup.ContainsKey(enemySpawn.enemyId))
            {
                Debug.LogError($"敌人配置不存在: {enemySpawn.enemyId}");
                return null;
            }
            
            // 实例化敌人
            GameObject enemyObj = Instantiate(enemyPrefab, enemySpawn.spawnPosition, Quaternion.identity, enemyContainer);
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            
            if (enemy == null)
            {
                Debug.LogError("敌人预制体缺少Enemy组件");
                Destroy(enemyObj);
                return null;
            }
            
            // 配置敌人
            enemy.config = enemyConfigLookup[enemySpawn.enemyId];
            enemy.currentHP = enemy.config.maxHP;
            
            // 设置路径点
            if (enemy.config.pathPoints.Count == 0)
            {
                // 使用默认路径
                enemy.SetPathPoints(GetDefaultPath());
            }
            
            // 订阅事件
            enemy.OnEnemyDeath += OnEnemyDeath;
            enemy.OnEnemyLeaked += OnEnemyLeaked;
            
            // 添加到活跃列表
            activeEnemies.Add(enemy);
            totalEnemiesSpawned++;
            
            OnEnemySpawned?.Invoke(enemy);
            
            Debug.Log($"生成敌人: {enemy.config.enemyName}");
            
            return enemy;
        }
        
        /// <summary>
        /// 获取默认路径
        /// </summary>
        private List<Vector3> GetDefaultPath()
        {
            // 这里应该根据关卡设计返回默认路径
            return new List<Vector3>
            {
                new Vector3(-10, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(10, 0, 0)
            };
        }
        
        /// <summary>
        /// 清理所有敌人
        /// </summary>
        public void ClearAllEnemies()
        {
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy.gameObject);
                }
            }
            
            activeEnemies.Clear();
            spawnQueue.Clear();
        }
        
        /// <summary>
        /// 获取活跃敌人数量
        /// </summary>
        public int GetActiveEnemyCount()
        {
            return activeEnemies.Count;
        }
        
        /// <summary>
        /// 获取敌人统计信息
        /// </summary>
        public string GetEnemyStatistics()
        {
            return $"生成: {totalEnemiesSpawned}, 击杀: {totalEnemiesKilled}, 漏过: {totalEnemiesLeaked}";
        }
        
        /// <summary>
        /// 获取敌人配置
        /// </summary>
        public EnemyConfig GetEnemyConfig(string enemyId)
        {
            return enemyConfigLookup.ContainsKey(enemyId) ? enemyConfigLookup[enemyId] : null;
        }
        
        /// <summary>
        /// 根据类型获取敌人
        /// </summary>
        public List<Enemy> GetEnemiesByType(EnemyType type)
        {
            List<Enemy> result = new List<Enemy>();
            
            foreach (var enemy in activeEnemies)
            {
                if (enemy.config.enemyType == type)
                {
                    result.Add(enemy);
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// 获取指定范围内的敌人
        /// </summary>
        public List<Enemy> GetEnemiesInRange(Vector3 center, float range)
        {
            List<Enemy> result = new List<Enemy>();
            
            foreach (var enemy in activeEnemies)
            {
                if (Vector3.Distance(enemy.transform.position, center) <= range)
                {
                    result.Add(enemy);
                }
            }
            
            return result;
        }
        
        // 事件处理方法
        private void OnEnemyDeath(Enemy enemy)
        {
            activeEnemies.Remove(enemy);
            totalEnemiesKilled++;
            OnEnemyKilled?.Invoke(enemy);
        }
        
        private void OnEnemyLeaked(Enemy enemy)
        {
            activeEnemies.Remove(enemy);
            totalEnemiesLeaked++;
            OnEnemyLeaked?.Invoke(enemy);
            
            // 通知关卡管理器
            var stage = FindObjectOfType<Stage>();
            if (stage != null)
            {
                stage.EnemyLeaked();
            }
        }
    }
} 