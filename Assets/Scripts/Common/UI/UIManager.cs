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
    public static class UIManager
    {
        // UI层级根节点
        private static Dictionary<UILayer, Transform> layerRoots = new Dictionary<UILayer, Transform>();
        
        // 已创建的窗口实例
        private static Dictionary<Type, WndBase> windowInstances = new Dictionary<Type, WndBase>();
        
        // 预制体路径前缀
        private static string prefabPathPrefix = "Prefabs/UI/";
        
        /// <summary>
        /// 初始化UI管理器
        /// </summary>
        public static void Initialize()
        {
            // 创建UI层级根节点
            CreateUILayers();
        }
        
        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="wnd">窗口实例</param>
        public static void Show(WndBase wnd)
        {
            if (wnd == null)
            {
                Debug.LogError("Cannot show null window");
                return;
            }
            
            Type windowType = wnd.GetType();
            
            // 检查是否已经存在该窗口实例
            if (windowInstances.TryGetValue(windowType, out WndBase existingWindow))
            {
                existingWindow.Show();
                return;
            }
            
            // 创建新窗口实例
            WndBase windowInstance = CreateWindow(wnd);
            if (windowInstance != null)
            {
                windowInstances[windowType] = windowInstance;
                windowInstance.Show();
            }
        }
        
        /// <summary>
        /// 创建窗口实例
        /// </summary>
        /// <param name="wnd">窗口类型</param>
        /// <returns>窗口实例</returns>
        private static WndBase CreateWindow(WndBase wnd)
        {
            Type windowType = wnd.GetType();
            string windowName = windowType.Name;
            
            // 构建预制体路径
            string prefabPath = prefabPathPrefix + windowName;
            
            // 加载预制体
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogError($"Failed to load prefab at path: {prefabPath}");
                return null;
            }
            
            // 实例化预制体
            GameObject windowObj = GameObject.Instantiate(prefab);
            if (windowObj == null)
            {
                Debug.LogError($"Failed to instantiate prefab: {windowName}");
                return null;
            }
            
            // 添加窗口脚本组件
            WndBase windowComponent = windowObj.AddComponent(windowType) as WndBase;
            if (windowComponent == null)
            {
                Debug.LogError($"Failed to add component: {windowName}");
                GameObject.Destroy(windowObj);
                return null;
            }
            
            // 获取窗口的UI层级
            UILayer uiLayer = GetUILayer(windowComponent);
            
            // 将窗口放到对应的层级下
            Transform layerRoot = GetLayerRoot(uiLayer);
            if (layerRoot != null)
            {
                windowObj.transform.SetParent(layerRoot, false);
            }
            else
            {
                Debug.LogWarning($"Layer root not found for UILayer: {uiLayer}");
            }
            
            // 设置窗口名称
            windowObj.name = windowName;
            
            Debug.Log($"Created window: {windowName} in layer: {uiLayer}");
            
            return windowComponent;
        }
        
        /// <summary>
        /// 获取窗口的UI层级
        /// </summary>
        /// <param name="window">窗口实例</param>
        /// <returns>UI层级</returns>
        private static UILayer GetUILayer(WndBase window)
        {
            // 通过反射获取UILayer属性或字段
            var uiLayerField = window.GetType().GetField("uiLayer", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (uiLayerField != null && uiLayerField.FieldType == typeof(UILayer))
            {
                return (UILayer)uiLayerField.GetValue(window);
            }
            
            // 如果没有找到uiLayer字段，返回默认层级
            return UILayer.Normal;
        }
        
        /// <summary>
        /// 创建UI层级根节点
        /// </summary>
        private static void CreateUILayers()
        {
            // 查找或创建UI根节点
            GameObject uiRoot = GameObject.Find("UIRoot");
            if (uiRoot == null)
            {
                uiRoot = new GameObject("UIRoot");
                uiRoot.AddComponent<Canvas>();
                uiRoot.AddComponent<CanvasScaler>();
                uiRoot.AddComponent<GraphicRaycaster>();
            }
            
            // 创建各个层级
            foreach (UILayer layer in System.Enum.GetValues(typeof(UILayer)))
            {
                string layerName = layer.ToString();
                Transform layerRoot = uiRoot.transform.Find(layerName);
                
                if (layerRoot == null)
                {
                    GameObject layerObj = new GameObject(layerName);
                    layerObj.transform.SetParent(uiRoot.transform, false);
                    
                    // 添加Canvas组件
                    Canvas canvas = layerObj.AddComponent<Canvas>();
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = (int)layer * 100; // 每个层级间隔100
                    
                    // 添加GraphicRaycaster组件
                    layerObj.AddComponent<GraphicRaycaster>();
                    
                    layerRoot = layerObj.transform;
                }
                
                layerRoots[layer] = layerRoot;
            }
        }
        
        /// <summary>
        /// 获取层级根节点
        /// </summary>
        /// <param name="layer">UI层级</param>
        /// <returns>层级根节点</returns>
        private static Transform GetLayerRoot(UILayer layer)
        {
            if (layerRoots.TryGetValue(layer, out Transform root))
            {
                return root;
            }
            
            Debug.LogError($"Layer root not found for UILayer: {layer}");
            return null;
        }
        
        /// <summary>
        /// 隐藏窗口
        /// </summary>
        /// <param name="wnd">窗口实例</param>
        public static void Hide(WndBase wnd)
        {
            if (wnd != null)
            {
                wnd.Hide();
            }
        }
        
        /// <summary>
        /// 销毁窗口
        /// </summary>
        /// <param name="wnd">窗口实例</param>
        public static void Destroy(WndBase wnd)
        {
            if (wnd != null)
            {
                Type windowType = wnd.GetType();
                wnd.Destroy();
                windowInstances.Remove(windowType);
            }
        }
    }
} 