using UnityEngine;
using System;
using System.Collections.Generic;
using Battle.Models;
using Network;
using System.Text;

namespace Battle.Network
{
    public class BattleNetworkManager : MonoBehaviour
    {
        // 单例实例
        public static BattleNetworkManager Instance { get; private set; }

        // 消息ID定义
        private const int MSG_GET_MATCH_DATA = 1001;
        private const int MSG_GET_MAP_DATA = 1002;
        private const int MSG_GET_UNIT_STATES = 1003;
        private const int MSG_SEND_OPERATION = 1004;
        private const int MSG_SEND_TURN_END = 1005;
        private const int MSG_GET_REPLAY_DATA = 1006;
        private const int MSG_SEND_BOOKMARK = 1007;

        // 事件回调
        public event Action<GameMatchData> OnMatchDataReceived;
        public event Action<GameMapData> OnMapDataReceived;
        public event Action<List<UnitStateRecordData>> OnUnitStatesReceived;
        public event Action<OperationRecordData> OnOperationReceived;
        public event Action<TurnRecordData> OnTurnEndReceived;
        public event Action<string> OnError;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                RegisterMessageHandlers();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void RegisterMessageHandlers()
        {
            // 注册消息处理器
            GameClient.Instance.RegisterMessageHandler(MSG_GET_MATCH_DATA, HandleMatchDataResponse);
            GameClient.Instance.RegisterMessageHandler(MSG_GET_MAP_DATA, HandleMapDataResponse);
            GameClient.Instance.RegisterMessageHandler(MSG_GET_UNIT_STATES, HandleUnitStatesResponse);
            GameClient.Instance.RegisterMessageHandler(MSG_SEND_OPERATION, HandleOperationResponse);
            GameClient.Instance.RegisterMessageHandler(MSG_SEND_TURN_END, HandleTurnEndResponse);
            GameClient.Instance.RegisterMessageHandler(MSG_GET_REPLAY_DATA, HandleReplayDataResponse);
            GameClient.Instance.RegisterMessageHandler(MSG_SEND_BOOKMARK, HandleBookmarkResponse);
        }

        private void OnDestroy()
        {
            // 取消注册消息处理器
            GameClient.Instance.UnregisterMessageHandler(MSG_GET_MATCH_DATA);
            GameClient.Instance.UnregisterMessageHandler(MSG_GET_MAP_DATA);
            GameClient.Instance.UnregisterMessageHandler(MSG_GET_UNIT_STATES);
            GameClient.Instance.UnregisterMessageHandler(MSG_SEND_OPERATION);
            GameClient.Instance.UnregisterMessageHandler(MSG_SEND_TURN_END);
            GameClient.Instance.UnregisterMessageHandler(MSG_GET_REPLAY_DATA);
            GameClient.Instance.UnregisterMessageHandler(MSG_SEND_BOOKMARK);
        }

        // 消息处理器
        private void HandleMatchDataResponse(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            var matchData = JsonUtility.FromJson<GameMatchData>(json);
            OnMatchDataReceived?.Invoke(matchData);
        }

        private void HandleMapDataResponse(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            var mapData = JsonUtility.FromJson<GameMapData>(json);
            OnMapDataReceived?.Invoke(mapData);
        }

        private void HandleUnitStatesResponse(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            var unitStates = JsonUtility.FromJson<List<UnitStateRecordData>>(json);
            OnUnitStatesReceived?.Invoke(unitStates);
        }

        private void HandleOperationResponse(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            var result = JsonUtility.FromJson<OperationRecordData>(json);
            OnOperationReceived?.Invoke(result);
        }

        private void HandleTurnEndResponse(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            var turnData = JsonUtility.FromJson<TurnRecordData>(json);
            OnTurnEndReceived?.Invoke(turnData);
        }

        private void HandleReplayDataResponse(byte[] data)
        {
            // TODO: 处理回放数据响应
        }

        private void HandleBookmarkResponse(byte[] data)
        {
            // TODO: 处理书签响应
        }

        // 获取对局数据
        public void GetMatchData(long matchId)
        {
            var data = new { matchId = matchId };
            GameClient.Instance.SendJsonMessage(MSG_GET_MATCH_DATA, data);
        }

        // 获取地图数据
        public void GetMapData(int mapId)
        {
            var data = new { mapId = mapId };
            GameClient.Instance.SendJsonMessage(MSG_GET_MAP_DATA, data);
        }

        // 获取单位状态
        public void GetUnitStates(long matchId, int turnNumber)
        {
            var data = new { matchId = matchId, turnNumber = turnNumber };
            GameClient.Instance.SendJsonMessage(MSG_GET_UNIT_STATES, data);
        }

        // 发送操作
        public void SendOperation(OperationRecordData operation)
        {
            GameClient.Instance.SendJsonMessage(MSG_SEND_OPERATION, operation);
        }

        // 发送回合结束
        public void SendTurnEnd(int turnNumber)
        {
            var data = new { turnNumber = turnNumber };
            GameClient.Instance.SendJsonMessage(MSG_SEND_TURN_END, data);
        }

        // 获取回放数据
        public void GetReplayData(long matchId)
        {
            var data = new { matchId = matchId };
            GameClient.Instance.SendJsonMessage(MSG_GET_REPLAY_DATA, data);
        }

        // 发送书签
        public void SendBookmark(ReplayBookmarkData bookmark)
        {
            GameClient.Instance.SendJsonMessage(MSG_SEND_BOOKMARK, bookmark);
        }
    }
} 