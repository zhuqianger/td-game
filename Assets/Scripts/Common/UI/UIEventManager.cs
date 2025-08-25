using UnityEngine;
using System.Collections.Generic;
using System;

namespace Common.UI
{
    public class UIEventManager : MonoBehaviour
    {
        private Dictionary<string, List<Delegate>> eventListeners = new Dictionary<string, List<Delegate>>();

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <typeparam name="T">事件数据类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="callback">回调函数</param>
        public void Register<T>(string eventName, Action<T> callback)
        {
            if (!eventListeners.ContainsKey(eventName))
            {
                eventListeners[eventName] = new List<Delegate>();
            }
            eventListeners[eventName].Add(callback);
        }

        /// <summary>
        /// 注册无参数事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="callback">回调函数</param>
        public void Register(string eventName, Action callback)
        {
            if (!eventListeners.ContainsKey(eventName))
            {
                eventListeners[eventName] = new List<Delegate>();
            }
            eventListeners[eventName].Add(callback);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        /// <typeparam name="T">事件数据类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="callback">回调函数</param>
        public void Unregister<T>(string eventName, Action<T> callback)
        {
            if (eventListeners.ContainsKey(eventName))
            {
                eventListeners[eventName].Remove(callback);
                if (eventListeners[eventName].Count == 0)
                {
                    eventListeners.Remove(eventName);
                }
            }
        }

        /// <summary>
        /// 注销无参数事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="callback">回调函数</param>
        public void Unregister(string eventName, Action callback)
        {
            if (eventListeners.ContainsKey(eventName))
            {
                eventListeners[eventName].Remove(callback);
                if (eventListeners[eventName].Count == 0)
                {
                    eventListeners.Remove(eventName);
                }
            }
        }

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <typeparam name="T">事件数据类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="data">事件数据</param>
        public void Send<T>(string eventName, T data)
        {
            if (eventListeners.ContainsKey(eventName))
            {
                foreach (var listener in eventListeners[eventName])
                {
                    if (listener is Action<T> typedListener)
                    {
                        typedListener(data);
                    }
                }
            }
        }

        /// <summary>
        /// 发送无参数事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        public void Send(string eventName)
        {
            if (eventListeners.ContainsKey(eventName))
            {
                foreach (var listener in eventListeners[eventName])
                {
                    if (listener is Action typedListener)
                    {
                        typedListener();
                    }
                }
            }
        }

        /// <summary>
        /// 清除所有事件监听
        /// </summary>
        public void Clear()
        {
            eventListeners.Clear();
        }
    }
}