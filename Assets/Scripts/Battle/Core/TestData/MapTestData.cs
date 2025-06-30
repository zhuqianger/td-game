using UnityEngine;
using System.Collections.Generic;
using Battle.Models;
using UnityEngine.Tilemaps;

namespace Battle.Core.TestData
{
    public static class MapTestData
    {
        // 测试地图数据
        public static GameMapData GetTestMapData()
        {
            return new GameMapData
            {
                Id = 1,
                MapName = "测试地图",
                Width = 10,
                Height = 10,
                TerrainTypes = new int[100], // 10x10的地图
                SpawnPoints = new Vector2Int[]
                {
                    new Vector2Int(1, 1),  // 玩家1出生点
                    new Vector2Int(8, 8)   // 玩家2出生点
                }
            };
        }

        // 测试单位数据
        public static List<UnitStateRecordData> GetTestUnits()
        {
            return new List<UnitStateRecordData>
            {
                // 玩家1的单位
                new UnitStateRecordData
                {
                    UnitId = 1,
                    Position = new Vector2Int(1, 1),
                    Hp = 100,
                    MaxHp = 100,
                    Mp = 50,
                    IsAlive = true,
                    CanMove = true,
                    CanAttack = true
                },
                new UnitStateRecordData
                {
                    UnitId = 2,
                    Position = new Vector2Int(2, 1),
                    Hp = 80,
                    MaxHp = 80,
                    Mp = 30,
                    IsAlive = true,
                    CanMove = true,
                    CanAttack = true
                },
                // 玩家2的单位
                new UnitStateRecordData
                {
                    UnitId = 3,
                    Position = new Vector2Int(8, 8),
                    Hp = 100,
                    MaxHp = 100,
                    Mp = 50,
                    IsAlive = true,
                    CanMove = true,
                    CanAttack = true
                },
                new UnitStateRecordData
                {
                    UnitId = 4,
                    Position = new Vector2Int(7, 8),
                    Hp = 80,
                    MaxHp = 80,
                    Mp = 30,
                    IsAlive = true,
                    CanMove = true,
                    CanAttack = true
                }
            };
        }
        
        // 获取测试地形数据
        public static int[,] GetTestTerrainData()
        {
            int[,] terrain = new int[10, 10];
            
            // 设置一些基础地形
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    // 默认都是平原
                    terrain[x, y] = (int)TerrainType.Plain;
                }
            }

            // 添加一些特殊地形
            // 山地
            terrain[4, 4] = (int)TerrainType.Mountain;
            terrain[4, 5] = (int)TerrainType.Mountain;
            terrain[5, 4] = (int)TerrainType.Mountain;
            terrain[5, 5] = (int)TerrainType.Mountain;

            // 森林
            terrain[2, 7] = (int)TerrainType.Forest;
            terrain[3, 7] = (int)TerrainType.Forest;
            terrain[2, 8] = (int)TerrainType.Forest;

            // 水域
            terrain[7, 2] = (int)TerrainType.Water;
            terrain[7, 3] = (int)TerrainType.Water;
            terrain[8, 2] = (int)TerrainType.Water;

            // 道路
            for (int x = 0; x < 10; x++)
            {
                terrain[x, 5] = (int)TerrainType.Road;
            }

            return terrain;
        }
    }
} 