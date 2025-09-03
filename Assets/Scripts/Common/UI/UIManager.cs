using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    /// <summary>
    /// UI层级枚举
    /// </summary>
    public enum UILayer
    {
        Background = 0,    // 背景层
        Normal = 1,        // 普通层
        Popup = 2,         // 弹窗层
        Top = 3            // 顶层
    }

    /// <summary>
    /// UI管理器 - 单例模式
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        
    }
} 