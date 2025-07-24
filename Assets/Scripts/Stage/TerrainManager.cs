using System.Collections.Generic;
using UnityEngine;

namespace TDGame.Stage
{
    /// <summary>
    /// 地形管理器类
    /// </summary>
    public class TerrainManager : MonoBehaviour
    {
        [Header("地形设置")]
        public GameObject terrainTilePrefab;
        public GameObject highGroundPrefab;
        public GameObject pitPrefab;
        public GameObject energyRunePrefab;
        public GameObject iceCrystalPrefab;
        
        [Header("地形颜色")]
        public Color deployableColor = Color.blue;
        public Color unDeployableColor = Color.black;
        public Color highGroundColor = Color.cyan;
        public Color pitColor = Color.red;
        public Color energyRuneColor = Color.yellow;
        public Color iceCrystalColor = Color.white;
        
        [Header("运行时状态")]
        public TerrainType[,] terrainMap;
        public Vector2Int mapSize;
        public List<EnvironmentObject> activeEnvironmentObjects;
        
        private Dictionary<Vector2Int, GameObject> terrainTiles;
        private Dictionary<Vector2Int, EnvironmentObject> environmentObjects;
        
        private void Awake()
        {
            InitializeTerrainManager();
        }
        
        /// <summary>
        /// 初始化地形管理器
        /// </summary>
        private void InitializeTerrainManager()
        {
            terrainTiles = new Dictionary<Vector2Int, GameObject>();
            environmentObjects = new Dictionary<Vector2Int, EnvironmentObject>();
            activeEnvironmentObjects = new List<EnvironmentObject>();
        }
        
        /// <summary>
        /// 初始化地形
        /// </summary>
        public void InitializeTerrain(TerrainType[,] terrainData, Vector2Int size)
        {
            // 清理现有地形
            ClearTerrain();
            
            terrainMap = terrainData;
            mapSize = size;
            
            // 生成地形
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    TerrainType terrainType = terrainData[x, y];
                    
                    CreateTerrainTile(position, terrainType);
                }
            }
            
            Debug.Log($"地形初始化完成: {size.x}x{size.y}");
        }
        
        /// <summary>
        /// 创建地形瓦片
        /// </summary>
        private void CreateTerrainTile(Vector2Int position, TerrainType terrainType)
        {
            Vector3 worldPosition = new Vector3(position.x, position.y, 0);
            GameObject tilePrefab = GetTerrainPrefab(terrainType);
            Color tileColor = GetTerrainColor(terrainType);
            
            if (tilePrefab != null)
            {
                GameObject tile = Instantiate(tilePrefab, worldPosition, Quaternion.identity, transform);
                terrainTiles[position] = tile;
                
                // 设置颜色
                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = tileColor;
                }
                
                // 设置名称
                tile.name = $"Terrain_{position.x}_{position.y}_{terrainType}";
            }
            
            // 创建环境机关
            if (IsEnvironmentObject(terrainType))
            {
                CreateEnvironmentObject(position, terrainType);
            }
        }
        
        /// <summary>
        /// 获取地形预制体
        /// </summary>
        private GameObject GetTerrainPrefab(TerrainType terrainType)
        {
            switch (terrainType)
            {
                case TerrainType.Deployable:
                case TerrainType.UnDeployable:
                    return terrainTilePrefab;
                case TerrainType.HighGround:
                    return highGroundPrefab;
                case TerrainType.Pit:
                    return pitPrefab;
                case TerrainType.EnergyRune:
                    return energyRunePrefab;
                case TerrainType.IceCrystal:
                    return iceCrystalPrefab;
                default:
                    return terrainTilePrefab;
            }
        }
        
        /// <summary>
        /// 获取地形颜色
        /// </summary>
        private Color GetTerrainColor(TerrainType terrainType)
        {
            switch (terrainType)
            {
                case TerrainType.Deployable:
                    return deployableColor;
                case TerrainType.UnDeployable:
                    return unDeployableColor;
                case TerrainType.HighGround:
                    return highGroundColor;
                case TerrainType.Pit:
                    return pitColor;
                case TerrainType.EnergyRune:
                    return energyRuneColor;
                case TerrainType.IceCrystal:
                    return iceCrystalColor;
                default:
                    return Color.gray;
            }
        }
        
        /// <summary>
        /// 检查是否为环境机关
        /// </summary>
        private bool IsEnvironmentObject(TerrainType terrainType)
        {
            return terrainType == TerrainType.EnergyRune || terrainType == TerrainType.IceCrystal;
        }
        
        /// <summary>
        /// 创建环境机关
        /// </summary>
        private void CreateEnvironmentObject(Vector2Int position, TerrainType terrainType)
        {
            Vector3 worldPosition = new Vector3(position.x, position.y, 0);
            
            EnvironmentObject envObj = new EnvironmentObject
            {
                objectId = $"{terrainType}_{position.x}_{position.y}",
                position = worldPosition,
                effectRadius = 2f,
                effectInterval = 5f,
                effectType = terrainType.ToString()
            };
            
            environmentObjects[position] = envObj;
            activeEnvironmentObjects.Add(envObj);
            
            // 启动环境机关效果
            StartEnvironmentEffect(envObj);
        }
        
        /// <summary>
        /// 启动环境机关效果
        /// </summary>
        private void StartEnvironmentEffect(EnvironmentObject envObj)
        {
            switch (envObj.effectType)
            {
                case "EnergyRune":
                    StartCoroutine(EnergyRuneEffect(envObj));
                    break;
                case "IceCrystal":
                    StartCoroutine(IceCrystalEffect(envObj));
                    break;
            }
        }
        
        /// <summary>
        /// 能量符文效果
        /// </summary>
        private System.Collections.IEnumerator EnergyRuneEffect(EnvironmentObject envObj)
        {
            while (true)
            {
                yield return new WaitForSeconds(envObj.effectInterval);
                
                // 回复范围内干员的技力
                var operators = GetOperatorsInRange(envObj.position, envObj.effectRadius);
                foreach (var op in operators)
                {
                    // 这里应该回复干员的技力
                    Debug.Log($"能量符文回复 {op.operatorName} 的技力");
                }
            }
        }
        
        /// <summary>
        /// 冰冻源石效果
        /// </summary>
        private System.Collections.IEnumerator IceCrystalEffect(EnvironmentObject envObj)
        {
            while (true)
            {
                yield return new WaitForSeconds(envObj.effectInterval);
                
                // 冻结范围内的单位
                var enemies = GetEnemiesInRange(envObj.position, envObj.effectRadius);
                foreach (var enemy in enemies)
                {
                    // 这里应该冻结敌人
                    Debug.Log($"冰冻源石冻结敌人 {enemy.config.enemyName}");
                }
            }
        }
        
        /// <summary>
        /// 获取指定范围内的干员
        /// </summary>
        private List<Role.Operator> GetOperatorsInRange(Vector3 center, float range)
        {
            List<Role.Operator> result = new List<Role.Operator>();
            
            var operatorManager = FindObjectOfType<Role.OperatorManager>();
            if (operatorManager != null)
            {
                var deployedOperators = operatorManager.GetDeployedOperators();
                
                foreach (var op in deployedOperators)
                {
                    if (Vector3.Distance(op.transform.position, center) <= range)
                    {
                        result.Add(op);
                    }
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// 获取指定范围内的敌人
        /// </summary>
        private List<Enemy> GetEnemiesInRange(Vector3 center, float range)
        {
            List<Enemy> result = new List<Enemy>();
            
            var enemyManager = FindObjectOfType<EnemyManager>();
            if (enemyManager != null)
            {
                result = enemyManager.GetEnemiesInRange(center, range);
            }
            
            return result;
        }
        
        /// <summary>
        /// 清理地形
        /// </summary>
        public void ClearTerrain()
        {
            // 清理地形瓦片
            foreach (var tile in terrainTiles.Values)
            {
                if (tile != null)
                {
                    Destroy(tile);
                }
            }
            terrainTiles.Clear();
            
            // 清理环境机关
            activeEnvironmentObjects.Clear();
            environmentObjects.Clear();
            
            // 停止所有协程
            StopAllCoroutines();
        }
        
        /// <summary>
        /// 获取地形类型
        /// </summary>
        public TerrainType GetTerrainType(Vector2Int position)
        {
            if (terrainMap == null || 
                position.x < 0 || position.x >= mapSize.x ||
                position.y < 0 || position.y >= mapSize.y)
            {
                return TerrainType.UnDeployable;
            }
            
            return terrainMap[position.x, position.y];
        }
        
        /// <summary>
        /// 检查位置是否可部署
        /// </summary>
        public bool IsPositionDeployable(Vector2Int position)
        {
            TerrainType terrain = GetTerrainType(position);
            return terrain == TerrainType.Deployable || terrain == TerrainType.HighGround;
        }
        
        /// <summary>
        /// 检查是否为高台位置
        /// </summary>
        public bool IsHighGround(Vector2Int position)
        {
            return GetTerrainType(position) == TerrainType.HighGround;
        }
        
        /// <summary>
        /// 检查是否为坑洞位置
        /// </summary>
        public bool IsPit(Vector2Int position)
        {
            return GetTerrainType(position) == TerrainType.Pit;
        }
        
        /// <summary>
        /// 设置地形类型
        /// </summary>
        public void SetTerrainType(Vector2Int position, TerrainType terrainType)
        {
            if (terrainMap == null || 
                position.x < 0 || position.x >= mapSize.x ||
                position.y < 0 || position.y >= mapSize.y)
            {
                return;
            }
            
            terrainMap[position.x, position.y] = terrainType;
            
            // 更新地形瓦片
            if (terrainTiles.ContainsKey(position))
            {
                Destroy(terrainTiles[position]);
                terrainTiles.Remove(position);
            }
            
            CreateTerrainTile(position, terrainType);
        }
        
        /// <summary>
        /// 获取地图大小
        /// </summary>
        public Vector2Int GetMapSize()
        {
            return mapSize;
        }
        
        /// <summary>
        /// 获取活跃的环境机关数量
        /// </summary>
        public int GetActiveEnvironmentObjectCount()
        {
            return activeEnvironmentObjects.Count;
        }
        
        /// <summary>
        /// 获取环境机关信息
        /// </summary>
        public string GetEnvironmentObjectInfo()
        {
            return $"活跃环境机关: {activeEnvironmentObjects.Count}";
        }
        
        /// <summary>
        /// 高亮可部署区域
        /// </summary>
        public void HighlightDeployableAreas(bool highlight)
        {
            foreach (var kvp in terrainTiles)
            {
                Vector2Int position = kvp.Key;
                GameObject tile = kvp.Value;
                
                if (IsPositionDeployable(position))
                {
                    SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        Color color = sr.color;
                        color.a = highlight ? 0.8f : 1f;
                        sr.color = color;
                    }
                }
            }
        }
        
        /// <summary>
        /// 显示地形信息
        /// </summary>
        public void ShowTerrainInfo(Vector2Int position)
        {
            TerrainType terrain = GetTerrainType(position);
            string info = $"位置 ({position.x}, {position.y}): {terrain}";
            
            if (IsPositionDeployable(position))
            {
                info += " - 可部署";
            }
            
            if (IsHighGround(position))
            {
                info += " - 仅远程";
            }
            
            if (IsPit(position))
            {
                info += " - 敌人坠落即死";
            }
            
            Debug.Log(info);
        }
    }
} 