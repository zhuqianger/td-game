using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Common;

namespace Operator
{
    public static class OperatorModel
    {
        // 干员数据存储，键为干员ID，值为干员对象
        private static Dictionary<int, Operator> _operators = new Dictionary<int, Operator>();

        public static void Init()
        {
            _operators = new Dictionary<int, Operator>();
        }

        public static void OnOperatorDataReceive(List<Operator> operatorList)
        {
            if (operatorList == null || operatorList.Count == 0)
            {
                Debug.LogWarning("Received empty operator list");
                return;
            }
            
            Debug.Log($"Received {operatorList.Count} operators from server");
            
            // 清空现有数据并添加新数据
            _operators.Clear();
            foreach (var op in operatorList)
            {
                if (op != null && op.operatorId > 0)
                {
                    _operators[op.operatorId] = op;
                }
            }
            
            Debug.Log($"Successfully stored {_operators.Count} operators in model");
            
            
        }

        public static void OnOperatorDataUpdate(Operator op)
        {
            if (op == null || op.operatorId <= 0)
            {
                Debug.LogWarning("Invalid operator data for update");
                return;
            }
            
            // 更新或添加干员数据
            _operators[op.operatorId] = op;
            
            Debug.Log($"Updated operator {op.operatorId} (Level: {op.level}, Elite: {op.eliteLevel})");
            
        }
    }
}