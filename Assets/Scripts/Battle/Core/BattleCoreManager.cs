using UnityEngine;
using System;
using System.Collections.Generic;
using Battle.Models;
using Battle.Network;
using Battle.UI;

namespace Battle.Core
{
    public class BattleCoreManager : MonoBehaviour
    {
        // 单例实例
        public static BattleCoreManager Instance { get; private set; }

        // 事件
        public event Action<BattleUnit> OnUnitSelected;
        public event Action OnUnitDeselected;
        public event Action<GameMatchData> OnTurnStarted;
        public event Action<long> OnGameOver;

        // 战斗数据
        public GameMatchData CurrentMatch { get; private set; }
        public GameMapData CurrentMap { get; private set; }
        public Dictionary<long, BattleUnit> Units { get; private set; }
        
        // 组件引用
        [SerializeField] private BattleMap battleMap;
        [SerializeField] private BattleUIManager uiManager;
        [SerializeField] private BattleNetworkManager networkManager;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            Units = new Dictionary<long, BattleUnit>();
        }

        // 注册UI事件
        public void RegisterUIEvents(BattleUIManager uiManager)
        {
            OnUnitSelected += uiManager.OnUnitSelected;
            OnUnitDeselected += uiManager.OnUnitDeselected;
            OnTurnStarted += uiManager.OnTurnStarted;
            OnGameOver += uiManager.OnGameOver;
        }

        // 取消注册UI事件
        public void UnregisterUIEvents(BattleUIManager uiManager)
        {
            OnUnitSelected -= uiManager.OnUnitSelected;
            OnUnitDeselected -= uiManager.OnUnitDeselected;
            OnTurnStarted -= uiManager.OnTurnStarted;
            OnGameOver -= uiManager.OnGameOver;
        }

        // 触发单位选择事件
        public void TriggerUnitSelected(BattleUnit unit)
        {
            OnUnitSelected?.Invoke(unit);
        }

        // 触发单位取消选择事件
        public void TriggerUnitDeselected()
        {
            OnUnitDeselected?.Invoke();
        }

        // 触发回合开始事件
        public void TriggerTurnStarted(GameMatchData matchData)
        {
            OnTurnStarted?.Invoke(matchData);
        }

        // 触发游戏结束事件
        public void TriggerGameOver(long winnerId)
        {
            OnGameOver?.Invoke(winnerId);
        }

        // 初始化战斗
        public void InitializeBattle(GameMatchData matchData, GameMapData mapData)
        {
            CurrentMatch = matchData;
            CurrentMap = mapData;
            
            // 初始化地图
            battleMap.Initialize(mapData);
            
            // 初始化单位
            InitializeUnits();
            
            // 开始第一个回合
            StartNewTurn();
        }

        // 初始化单位
        private void InitializeUnits()
        {
            // TODO: 从服务器获取初始单位数据
            // 这里应该调用网络管理器获取数据
        }

        // 开始新回合
        public void StartNewTurn()
        {
            if (CurrentMatch.IsGameOver) return;

            // 更新回合数
            CurrentMatch.CurrentTurn++;
            
            // 切换当前玩家
            SwitchCurrentPlayer();
            
            // 重置单位状态
            ResetUnitsState();
            
            // 更新UI
            uiManager.UpdateTurnInfo(CurrentMatch);
        }

        // 切换当前玩家
        private void SwitchCurrentPlayer()
        {
            CurrentMatch.CurrentPlayerId = CurrentMatch.CurrentPlayerId == CurrentMatch.Player1Id 
                ? CurrentMatch.Player2Id 
                : CurrentMatch.Player1Id;
        }

        // 重置单位状态
        private void ResetUnitsState()
        {
            foreach (var unit in Units.Values)
            {
                if (unit.TeamId == GetCurrentPlayerTeamId())
                {
                    unit.ResetTurnState();
                }
            }
        }

        // 获取当前玩家队伍ID
        private int GetCurrentPlayerTeamId()
        {
            // TODO: 实现队伍ID获取逻辑
            return 1;
        }

        // 执行单位移动
        // ReSharper disable Unity.PerformanceAnalysis
        public void ExecuteUnitMove(long unitId, Vector2Int targetPosition)
        {
            if (!Units.TryGetValue(unitId, out var unit)) return;
            
            // 检查移动是否合法
            if (!IsValidMove(unit, targetPosition)) return;
            
            // 执行移动
            unit.MoveTo(targetPosition);
            
            // 发送移动操作到服务器
            networkManager.SendOperation(new OperationRecordData
            {
                OperationType = 1, // 移动操作
                UnitId = unitId,
                SourcePosition = unit.Position,
                TargetPosition = targetPosition
            });
        }

        // 执行单位攻击
        public void ExecuteUnitAttack(long attackerId, long targetId)
        {
            if (!Units.TryGetValue(attackerId, out var attacker) || 
                !Units.TryGetValue(targetId, out var target)) return;
            
            // 检查攻击是否合法
            if (!IsValidAttack(attacker, target)) return;
            
            // 执行攻击
            attacker.Attack(target);
            
            // 发送攻击操作到服务器
            networkManager.SendOperation(new OperationRecordData
            {
                OperationType = 2, // 攻击操作
                UnitId = attackerId,
                SourcePosition = attacker.Position,
                TargetPosition = target.Position
            });
        }

        // 检查移动是否合法
        private bool IsValidMove(BattleUnit unit, Vector2Int targetPosition)
        {
            // TODO: 实现移动合法性检查
            return true;
        }

        // 检查攻击是否合法
        private bool IsValidAttack(BattleUnit attacker, BattleUnit target)
        {
            // TODO: 实现攻击合法性检查
            return true;
        }

        // 结束当前回合
        public void EndCurrentTurn()
        {
            // TODO: 实现回合结束逻辑
        }

        // 检查游戏是否结束
        public void CheckGameOver()
        {
            // TODO: 实现游戏结束检查逻辑
        }
    }
} 