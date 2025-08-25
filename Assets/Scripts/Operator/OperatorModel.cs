using UnityEngine;

namespace Operator
{
    public static class OperatorModel
    {
        // 这里可以添加干员数据管理的相关方法和属性
        private static readonly object _lock = new object();
        
        // 示例：初始化方法
        public static void Initialize()
        {
            lock (_lock)
            {
                // 初始化逻辑
                Debug.Log("OperatorModel initialized.");
            }
        }
    }
}