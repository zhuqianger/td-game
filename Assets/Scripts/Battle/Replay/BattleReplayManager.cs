using UnityEngine;
using System.Collections.Generic;
using Battle.Models;
using Battle.Core;

namespace Battle.Replay
{
    public class BattleReplayManager : MonoBehaviour
    {
        // 单例实例
        public static BattleReplayManager Instance { get; private set; }

        // 回放数据
        private GameMatchData matchData;
        private List<TurnRecordData> turnRecords;
        private List<OperationRecordData> operationRecords;
        private List<UnitStateRecordData> unitStateRecords;
        private List<ReplayBookmarkData> bookmarks;

        // 回放状态
        private int currentTurnIndex = -1;
        private int currentOperationIndex = -1;
        private bool isPlaying = false;
        private float playbackSpeed = 1.0f;

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
        }

        // 加载回放数据
        public void LoadReplayData(GameMatchData match)
        {
            matchData = match;
            turnRecords = new List<TurnRecordData>();
            operationRecords = new List<OperationRecordData>();
            unitStateRecords = new List<UnitStateRecordData>();
            bookmarks = new List<ReplayBookmarkData>();

            // TODO: 从服务器加载回放数据
            // 这里应该调用网络管理器获取数据
        }

        // 开始回放
        public void StartReplay()
        {
            if (matchData == null) return;

            isPlaying = true;
            currentTurnIndex = 0;
            currentOperationIndex = 0;

            // 重置战斗状态
            BattleCoreManager.Instance.InitializeBattle(matchData, null);
        }

        // 暂停回放
        public void PauseReplay()
        {
            isPlaying = false;
        }

        // 继续回放
        public void ResumeReplay()
        {
            isPlaying = true;
        }

        // 停止回放
        public void StopReplay()
        {
            isPlaying = false;
            currentTurnIndex = -1;
            currentOperationIndex = -1;
        }

        // 设置回放速度
        public void SetPlaybackSpeed(float speed)
        {
            playbackSpeed = Mathf.Clamp(speed, 0.1f, 3.0f);
        }

        // 跳转到指定回合
        public void JumpToTurn(int turnNumber)
        {
            if (turnNumber < 0 || turnNumber >= turnRecords.Count) return;

            // 重置到初始状态
            ResetToInitialState();

            // 执行到目标回合
            for (int i = 0; i <= turnNumber; i++)
            {
                ExecuteTurn(turnRecords[i]);
            }

            currentTurnIndex = turnNumber;
            currentOperationIndex = operationRecords.Count;
        }

        // 跳转到指定书签
        public void JumpToBookmark(ReplayBookmarkData bookmark)
        {
            JumpToTurn(bookmark.TurnNumber);
        }

        // 添加书签
        public void AddBookmark(string name, string description)
        {
            var bookmark = new ReplayBookmarkData
            {
                MatchId = matchData.Id,
                Name = name,
                TurnNumber = currentTurnIndex,
                Description = description,
                CreatedAt = System.DateTime.Now
            };

            bookmarks.Add(bookmark);
        }

        // 删除书签
        public void RemoveBookmark(ReplayBookmarkData bookmark)
        {
            bookmarks.Remove(bookmark);
        }

        // 重置到初始状态
        private void ResetToInitialState()
        {
            // TODO: 实现重置逻辑
            // 这里应该重置所有单位到初始状态
        }

        // 执行回合
        private void ExecuteTurn(TurnRecordData turn)
        {
            // 获取该回合的所有操作
            var turnOperations = operationRecords.FindAll(op => op.TurnId == turn.Id);

            // 按顺序执行操作
            foreach (var operation in turnOperations)
            {
                ExecuteOperation(operation);
            }
        }

        // 执行操作
        private void ExecuteOperation(OperationRecordData operation)
        {
            switch (operation.OperationType)
            {
                case 1: // 移动
                    BattleCoreManager.Instance.ExecuteUnitMove(operation.UnitId, operation.TargetPosition);
                    break;
                case 2: // 攻击
                    var targetUnit = BattleCoreManager.Instance.Units[operation.TargetPosition.x];
                    BattleCoreManager.Instance.ExecuteUnitAttack(operation.UnitId, targetUnit.UnitId);
                    break;
                // TODO: 添加其他操作类型
            }
        }

        private void Update()
        {
            if (!isPlaying) return;

            // 检查是否需要执行下一个操作
            if (currentOperationIndex < operationRecords.Count - 1)
            {
                currentOperationIndex++;
                ExecuteOperation(operationRecords[currentOperationIndex]);
            }
            // 检查是否需要进入下一个回合
            else if (currentTurnIndex < turnRecords.Count - 1)
            {
                currentTurnIndex++;
                currentOperationIndex = -1;
                ExecuteTurn(turnRecords[currentTurnIndex]);
            }
            else
            {
                // 回放结束
                StopReplay();
            }
        }
    }
} 