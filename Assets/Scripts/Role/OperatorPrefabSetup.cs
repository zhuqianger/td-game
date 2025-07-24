using UnityEngine;
using UnityEditor;

namespace TDGame.Role
{
    /// <summary>
    /// 干员预制体设置工具
    /// 用于在Unity编辑器中快速设置干员预制体
    /// </summary>
    public class OperatorPrefabSetup : MonoBehaviour
    {
        [Header("预制体设置")]
        public GameObject operatorPrefab;
        public OperatorConfig operatorConfig;
        
        [Header("组件设置")]
        public bool addOperatorComponent = true;
        public bool addRigidbody2D = true;
        public bool addCollider2D = true;
        public bool addSpriteRenderer = true;
        
        [Header("默认设置")]
        public Sprite defaultSprite;
        public LayerMask defaultLayer = 8; // 假设第8层是干员层
        
        /// <summary>
        /// 创建干员预制体
        /// </summary>
        [ContextMenu("创建干员预制体")]
        public void CreateOperatorPrefab()
        {
            if (operatorPrefab == null)
            {
                Debug.LogError("请先设置干员预制体");
                return;
            }
            
            // 添加必要的组件
            if (addOperatorComponent)
            {
                Operator op = operatorPrefab.GetComponent<Operator>();
                if (op == null)
                {
                    op = operatorPrefab.AddComponent<Operator>();
                }
                
                // 应用配置
                if (operatorConfig != null)
                {
                    ApplyConfigToOperator(op, operatorConfig);
                }
            }
            
            if (addRigidbody2D)
            {
                Rigidbody2D rb = operatorPrefab.GetComponent<Rigidbody2D>();
                if (rb == null)
                {
                    rb = operatorPrefab.AddComponent<Rigidbody2D>();
                }
                rb.gravityScale = 0f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            
            if (addCollider2D)
            {
                Collider2D col = operatorPrefab.GetComponent<Collider2D>();
                if (col == null)
                {
                    col = operatorPrefab.AddComponent<CircleCollider2D>();
                }
            }
            
            if (addSpriteRenderer)
            {
                SpriteRenderer sr = operatorPrefab.GetComponent<SpriteRenderer>();
                if (sr == null)
                {
                    sr = operatorPrefab.AddComponent<SpriteRenderer>();
                }
                
                if (defaultSprite != null)
                {
                    sr.sprite = defaultSprite;
                }
                
                sr.sortingOrder = 10; // 确保干员在背景之上
            }
            
            // 设置层
            operatorPrefab.layer = (int)Mathf.Log(defaultLayer.value, 2);
            
            Debug.Log($"干员预制体设置完成: {operatorPrefab.name}");
        }
        
        /// <summary>
        /// 应用配置到干员
        /// </summary>
        private void ApplyConfigToOperator(Operator op, OperatorConfig config)
        {
            op.operatorId = config.operatorId;
            op.operatorName = config.operatorName;
            op.profession = config.profession;
            op.rarity = config.rarity;
            op.baseStats = config.baseStats;
            op.currentStats = config.baseStats;
            op.currentStats.currentHP = config.baseStats.maxHP;
            op.attackRange = config.attackRange;
            
            // 设置默认等级
            op.level = 1;
            op.eliteLevel = 0;
        }
        
        /// <summary>
        /// 批量创建干员预制体
        /// </summary>
        [ContextMenu("批量创建预制体")]
        public void BatchCreatePrefabs()
        {
            if (operatorConfig == null)
            {
                Debug.LogError("请先设置干员配置");
                return;
            }
            
            // 这里可以添加批量创建逻辑
            // 比如从配置文件读取多个干员配置
            Debug.Log("批量创建功能待实现");
        }
    }
    
    #if UNITY_EDITOR
    /// <summary>
    /// 干员预制体设置编辑器
    /// </summary>
    [CustomEditor(typeof(OperatorPrefabSetup))]
    public class OperatorPrefabSetupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            OperatorPrefabSetup setup = (OperatorPrefabSetup)target;
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("创建干员预制体"))
            {
                setup.CreateOperatorPrefab();
            }
            
            if (GUILayout.Button("批量创建预制体"))
            {
                setup.BatchCreatePrefabs();
            }
            
            EditorGUILayout.Space();
            
            // 添加一些有用的提示
            EditorGUILayout.HelpBox(
                "使用说明:\n" +
                "1. 设置干员预制体\n" +
                "2. 设置干员配置\n" +
                "3. 选择要添加的组件\n" +
                "4. 点击创建按钮",
                MessageType.Info
            );
        }
    }
    #endif
} 