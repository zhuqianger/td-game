using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    /// <summary>
    /// 主窗口基类 - 管理页签和子窗口
    /// </summary>
    public abstract class WndBase : UIBase
    {
        public UILayer layer = UILayer.Normal;
        public string prefabAssetPath;
    }
} 