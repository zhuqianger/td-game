using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Battle.Models;
using Battle.Core.TestData;

namespace Battle.Core
{
    public class BattleMap : MonoBehaviour
    {
        // Tilemap组件引用
        [SerializeField] private Tilemap groundTilemap;
        [SerializeField] private Tilemap objectTilemap;
        [SerializeField] private Tilemap unitTilemap;

        // 地形瓦片
        [SerializeField] private TileBase plainTile;
        [SerializeField] private TileBase mountainTile;
        [SerializeField] private TileBase forestTile;
        [SerializeField] private TileBase waterTile;
        [SerializeField] private TileBase roadTile;

        // 地图数据
        private GameMapData mapData;
        private int[,] terrainGrid;
        private Dictionary<Vector2Int, BattleUnit> unitGrid;

        private void Awake()
        {
            unitGrid = new Dictionary<Vector2Int, BattleUnit>();
        }

        // 初始化地图
        public void Initialize(GameMapData data)
        {
            mapData = data;
            terrainGrid = new int[data.Width, data.Height];
            
            // 清除现有地图
            ClearMap();
            
            // 生成地图网格
            GenerateMapGrid();
        }

        // 清除地图
        private void ClearMap()
        {
            groundTilemap.ClearAllTiles();
            objectTilemap.ClearAllTiles();
            unitTilemap.ClearAllTiles();
            unitGrid.Clear();
        }

        // 生成地图网格
        private void GenerateMapGrid()
        {
            // 使用测试数据
            terrainGrid = MapTestData.GetTestTerrainData();

            // 生成地形
            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = 0; y < mapData.Height; y++)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    TileBase tile = GetTileForTerrainType(terrainGrid[x, y]);
                    groundTilemap.SetTile(tilePosition, tile);
                }
            }

            // 标记出生点
            foreach (var spawnPoint in mapData.SpawnPoints)
            {
                Vector3Int tilePosition = new Vector3Int(spawnPoint.x, spawnPoint.y, 0);
                // 可以在这里添加出生点标记
            }
        }

        // 根据地形类型获取对应的瓦片
        private TileBase GetTileForTerrainType(int terrainType)
        {
            switch ((TerrainType)terrainType)
            {
                case TerrainType.Plain:
                    return plainTile;
                case TerrainType.Mountain:
                    return mountainTile;
                case TerrainType.Forest:
                    return forestTile;
                case TerrainType.Water:
                    return waterTile;
                case TerrainType.Road:
                    return roadTile;
                default:
                    return plainTile;
            }
        }

        // 获取指定位置的地形类型
        public int GetTerrainTypeAt(Vector2Int position)
        {
            if (IsValidPosition(position))
            {
                return terrainGrid[position.x, position.y];
            }
            return -1;
        }

        // 检查位置是否有效
        public bool IsValidPosition(Vector2Int position)
        {
            return position.x >= 0 && position.x < mapData.Width &&
                   position.y >= 0 && position.y < mapData.Height;
        }

        // 检查位置是否可移动
        public bool IsWalkable(Vector2Int position)
        {
            if (!IsValidPosition(position)) return false;

            int terrainType = GetTerrainTypeAt(position);
            return terrainType != (int)TerrainType.Mountain &&
                   terrainType != (int)TerrainType.Water;
        }

        // 添加单位到地图
        public void AddUnit(BattleUnit unit, Vector2Int position)
        {
            if (!IsValidPosition(position)) return;

            unitGrid[position] = unit;
            Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
            // 这里可以设置单位的瓦片显示
        }

        // 移除单位
        public void RemoveUnit(Vector2Int position)
        {
            if (unitGrid.ContainsKey(position))
            {
                unitGrid.Remove(position);
                Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
                unitTilemap.SetTile(tilePosition, null);
            }
        }

        // 移动单位
        public void MoveUnit(Vector2Int from, Vector2Int to)
        {
            if (!IsValidPosition(from) || !IsValidPosition(to)) return;
            if (!unitGrid.ContainsKey(from)) return;

            BattleUnit unit = unitGrid[from];
            unitGrid.Remove(from);
            unitGrid[to] = unit;

            // 更新瓦片显示
            Vector3Int fromTile = new Vector3Int(from.x, from.y, 0);
            Vector3Int toTile = new Vector3Int(to.x, to.y, 0);
            unitTilemap.SetTile(fromTile, null);
            // 这里可以设置单位的瓦片显示
        }

        // 获取指定位置的单位
        public BattleUnit GetUnitAt(Vector2Int position)
        {
            if (unitGrid.TryGetValue(position, out BattleUnit unit))
            {
                return unit;
            }
            return null;
        }
    }
} 