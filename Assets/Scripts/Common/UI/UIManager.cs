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
        private static UIManager _instance;
        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("UIManager");
                    _instance = go.AddComponent<UIManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        [Header("UI配置")]
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private CanvasScaler canvasScaler;
        [SerializeField] private GraphicRaycaster graphicRaycaster;

        // UI栈管理
        private Dictionary<UILayer, Stack<UIBase>> uiStacks = new Dictionary<UILayer, Stack<UIBase>>();
        
        // UI缓存池
        private Dictionary<string, Queue<UIBase>> uiPool = new Dictionary<string, Queue<UIBase>>();
        
        // 当前打开的UI
        private Dictionary<string, UIBase> activeUIs = new Dictionary<string, UIBase>();
        
        // 层级根节点
        private Dictionary<UILayer, Transform> layerRoots = new Dictionary<UILayer, Transform>();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeUI();
        }

        /// <summary>
        /// 初始化UI系统
        /// </summary>
        private void InitializeUI()
        {
            // 创建主Canvas
            if (mainCanvas == null)
            {
                GameObject canvasGO = new GameObject("MainCanvas");
                mainCanvas = canvasGO.AddComponent<Canvas>();
                canvasScaler = canvasGO.AddComponent<CanvasScaler>();
                graphicRaycaster = canvasGO.AddComponent<GraphicRaycaster>();
                
                // 设置Canvas属性
                mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                mainCanvas.sortingOrder = 0;
                
                // 设置CanvasScaler适配
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                canvasScaler.matchWidthOrHeight = 0.5f;
                
                canvasGO.transform.SetParent(transform);
            }

            // 初始化UI栈
            foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
            {
                uiStacks[layer] = new Stack<UIBase>();
            }

            // 创建层级根节点
            CreateLayerRoots();
        }

        /// <summary>
        /// 创建层级根节点
        /// </summary>
        private void CreateLayerRoots()
        {
            foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
            {
                GameObject layerGO = new GameObject($"Layer_{layer}");
                layerGO.transform.SetParent(mainCanvas.transform);
                
                // 设置层级顺序
                Canvas layerCanvas = layerGO.AddComponent<Canvas>();
                layerCanvas.overrideSorting = true;
                layerCanvas.sortingOrder = (int)layer * 100;
                
                layerRoots[layer] = layerGO.transform;
            }
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <typeparam name="T">UI类型</typeparam>
        /// <param name="uiName">UI名称</param>
        /// <param name="layer">UI层级</param>
        /// <param name="data">传递数据</param>
        /// <returns>UI实例</returns>
        public T OpenUI<T>(string uiName, UILayer layer = UILayer.Normal, object data = null) where T : UIBase
        {
            // 检查是否已经打开
            if (activeUIs.ContainsKey(uiName))
            {
                UIBase existingUI = activeUIs[uiName];
                existingUI.transform.SetAsLastSibling();
                return existingUI as T;
            }

            // 从缓存池获取或创建UI
            UIBase ui = GetUIFromPool(uiName);
            if (ui == null)
            {
                ui = CreateUI(uiName);
            }

            if (ui == null)
            {
                Debug.LogError($"Failed to create UI: {uiName}");
                return null;
            }

            // 设置UI层级
            ui.transform.SetParent(layerRoots[layer]);
            ui.transform.localPosition = Vector3.zero;
            ui.transform.localScale = Vector3.one;
            ui.transform.localRotation = Quaternion.identity;

            // 初始化UI
            ui.Init(data);
            
            // 添加到栈和活跃列表
            uiStacks[layer].Push(ui);
            activeUIs[uiName] = ui;

            // 处理输入阻断
            HandleInputBlocking(layer);

            return ui as T;
        }

        /// <summary>
        /// 打开UI并切换到指定子窗口
        /// </summary>
        /// <typeparam name="T">UI类型</typeparam>
        /// <param name="uiName">UI名称</param>
        /// <param name="subviewName">子窗口名称</param>
        /// <param name="layer">UI层级</param>
        /// <param name="data">传递数据</param>
        /// <returns>UI实例</returns>
        public T OpenUIWithSubview<T>(string uiName, string subviewName, UILayer layer = UILayer.Normal, object data = null) where T : UIBase
        {
            T ui = OpenUI<T>(uiName, layer, data);
            if (ui != null && ui.HasSubviews)
            {
                ui.SwitchSubview(subviewName, data);
            }
            return ui;
        }

        /// <summary>
        /// 打开主窗口并切换到指定页签
        /// </summary>
        /// <typeparam name="T">主窗口类型</typeparam>
        /// <param name="uiName">UI名称</param>
        /// <param name="tabName">页签名称</param>
        /// <param name="layer">UI层级</param>
        /// <param name="data">传递数据</param>
        /// <returns>主窗口实例</returns>
        public T OpenWindowWithTab<T>(string uiName, string tabName, UILayer layer = UILayer.Normal, object data = null) where T : WndBase
        {
            T window = OpenUI<T>(uiName, layer, data);
            if (window != null && window.HasTabs)
            {
                window.SwitchTab(tabName);
            }
            return window;
        }

        /// <summary>
        /// 获取主窗口
        /// </summary>
        /// <typeparam name="T">主窗口类型</typeparam>
        /// <param name="uiName">UI名称</param>
        /// <returns>主窗口实例</returns>
        public T GetWindow<T>(string uiName) where T : WndBase
        {
            return GetUI<T>(uiName);
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="uiName">UI名称</param>
        public void CloseUI(string uiName)
        {
            if (!activeUIs.ContainsKey(uiName))
            {
                Debug.LogWarning($"UI {uiName} is not active");
                return;
            }

            UIBase ui = activeUIs[uiName];
            UILayer layer = GetUILayer(ui);

            // 从栈中移除
            if (uiStacks[layer].Count > 0 && uiStacks[layer].Peek() == ui)
            {
                uiStacks[layer].Pop();
            }

            // 关闭UI
            ui.Close();
            
            // 从活跃列表移除
            activeUIs.Remove(uiName);

            // 回收到缓存池
            RecycleUI(ui);
        }

        /// <summary>
        /// 关闭指定层级的所有UI
        /// </summary>
        /// <param name="layer">UI层级</param>
        public void CloseLayer(UILayer layer)
        {
            if (!uiStacks.ContainsKey(layer))
                return;

            while (uiStacks[layer].Count > 0)
            {
                UIBase ui = uiStacks[layer].Pop();
                ui.Close();
                activeUIs.Remove(ui.UIName);
                RecycleUI(ui);
            }
        }

        /// <summary>
        /// 关闭所有UI
        /// </summary>
        public void CloseAllUI()
        {
            foreach (var kvp in activeUIs)
            {
                kvp.Value.Close();
            }

            activeUIs.Clear();
            
            foreach (var stack in uiStacks.Values)
            {
                stack.Clear();
            }
        }

        /// <summary>
        /// 获取UI实例
        /// </summary>
        /// <typeparam name="T">UI类型</typeparam>
        /// <param name="uiName">UI名称</param>
        /// <returns>UI实例</returns>
        public T GetUI<T>(string uiName) where T : UIBase
        {
            if (activeUIs.ContainsKey(uiName))
            {
                return activeUIs[uiName] as T;
            }
            return null;
        }

        /// <summary>
        /// 从缓存池获取UI
        /// </summary>
        private UIBase GetUIFromPool(string uiName)
        {
            if (uiPool.ContainsKey(uiName) && uiPool[uiName].Count > 0)
            {
                return uiPool[uiName].Dequeue();
            }
            return null;
        }

        /// <summary>
        /// 创建UI
        /// </summary>
        private UIBase CreateUI(string uiName)
        {
            // 从Resources加载预制件
            GameObject prefab = Resources.Load<GameObject>($"UI/{uiName}");
            if (prefab == null)
            {
                Debug.LogError($"UI prefab not found: UI/{uiName}");
                return null;
            }

            GameObject uiGO = Instantiate(prefab);
            UIBase ui = uiGO.GetComponent<UIBase>();
            if (ui == null)
            {
                Debug.LogError($"UI component not found on prefab: {uiName}");
                Destroy(uiGO);
                return null;
            }

            ui.UIName = uiName;
            return ui;
        }

        /// <summary>
        /// 回收UI到缓存池
        /// </summary>
        private void RecycleUI(UIBase ui)
        {
            if (!uiPool.ContainsKey(ui.UIName))
            {
                uiPool[ui.UIName] = new Queue<UIBase>();
            }

            ui.gameObject.SetActive(false);
            ui.transform.SetParent(transform);
            uiPool[ui.UIName].Enqueue(ui);
        }

        /// <summary>
        /// 获取UI所属层级
        /// </summary>
        private UILayer GetUILayer(UIBase ui)
        {
            foreach (var kvp in layerRoots)
            {
                if (ui.transform.IsChildOf(kvp.Value))
                {
                    return kvp.Key;
                }
            }
            return UILayer.Normal;
        }

        /// <summary>
        /// 处理输入阻断
        /// </summary>
        private void HandleInputBlocking(UILayer currentLayer)
        {
            // 根据层级决定是否阻断下层输入
            if (currentLayer >= UILayer.Popup)
            {
                // 弹窗层及以上阻断下层输入
                SetLayerInputBlocking(currentLayer, true);
            }
        }

        /// <summary>
        /// 设置层级输入阻断
        /// </summary>
        private void SetLayerInputBlocking(UILayer layer, bool block)
        {
            if (layerRoots.ContainsKey(layer))
            {
                GraphicRaycaster raycaster = layerRoots[layer].GetComponent<GraphicRaycaster>();
                if (raycaster != null)
                {
                    raycaster.enabled = !block;
                }
            }
        }

        /// <summary>
        /// 清空缓存池
        /// </summary>
        public void ClearPool()
        {
            foreach (var queue in uiPool.Values)
            {
                while (queue.Count > 0)
                {
                    UIBase ui = queue.Dequeue();
                    if (ui != null)
                    {
                        Destroy(ui.gameObject);
                    }
                }
            }
            uiPool.Clear();
        }

        private void OnDestroy()
        {
            CloseAllUI();
            ClearPool();
        }
    }
} 