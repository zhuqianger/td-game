using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.UI;

namespace Common.UI
{
    /// <summary>
    /// 子窗口信息
    /// </summary>
    [System.Serializable]
    public class SubviewInfo
    {
        public string subviewName;
        public GameObject subviewObject;
        public UIBase subviewComponent;
        public bool isActive;
        
        public SubviewInfo(string name, GameObject obj, UIBase component)
        {
            subviewName = name;
            subviewObject = obj;
            subviewComponent = component;
            isActive = false;
        }
    }

    /// <summary>
    /// UI基类 - 所有UI界面都应该继承此类
    /// </summary>
    public abstract class UIBase : MonoBehaviour
    {
        [Header("UI基础信息")]
        [SerializeField] protected string uiName;
        [SerializeField] protected UILayer uiLayer = UILayer.Normal;
        [SerializeField] protected bool useAnimation = true;
        [SerializeField] protected bool blockInput = true;

        [Header("子窗口配置")]
        [SerializeField] protected bool hasSubviews = false;
        [SerializeField] protected string defaultSubview = "";
        [SerializeField] protected Transform subviewContainer;

        // UI组件缓存
        protected Dictionary<string, Component> componentCache = new Dictionary<string, Component>();
        
        // 子窗口管理
        protected Dictionary<string, SubviewInfo> subviews = new Dictionary<string, SubviewInfo>();
        protected string currentSubview = "";
        
        // 事件系统
        // 删除关于事件的内容

        // UI状态
        protected bool isInitialized = false;
        protected bool isOpened = false;

        public string UIName 
        { 
            get => uiName; 
            set => uiName = value; 
        }
        
        public UILayer UILayer => uiLayer;
        public bool IsOpened => isOpened;
        public bool HasSubviews => hasSubviews;
        public string CurrentSubview => currentSubview;

        protected virtual void Awake()
        {
            // 删除事件管理器的初始化
            CacheComponents();
            
            // 初始化子窗口
            if (hasSubviews)
            {
                InitializeSubviews();
            }
        }

        /// <summary>
        /// 初始化子窗口
        /// </summary>
        protected virtual void InitializeSubviews()
        {
            if (subviewContainer == null)
            {
                Debug.LogWarning($"Subview container not set for UI: {uiName}");
                return;
            }

            // 查找所有子窗口
            for (int i = 0; i < subviewContainer.childCount; i++)
            {
                Transform child = subviewContainer.GetChild(i);
                UIBase subviewComponent = child.GetComponent<UIBase>();
                
                if (subviewComponent != null)
                {
                    SubviewInfo subviewInfo = new SubviewInfo(child.name, child.gameObject, subviewComponent);
                    subviews[child.name] = subviewInfo;
                    
                    // 默认隐藏所有子窗口
                    child.gameObject.SetActive(false);
                }
            }

            // 设置默认子窗口
            if (!string.IsNullOrEmpty(defaultSubview) && subviews.ContainsKey(defaultSubview))
            {
                SwitchSubview(defaultSubview);
            }
        }

        /// <summary>
        /// 切换子窗口
        /// </summary>
        /// <param name="subviewName">子窗口名称</param>
        /// <param name="data">传递数据</param>
        public virtual void SwitchSubview(string subviewName, object data = null)
        {
            if (!hasSubviews || !subviews.ContainsKey(subviewName))
            {
                Debug.LogWarning($"Subview {subviewName} not found in UI {uiName}");
                return;
            }

            // 关闭当前子窗口
            if (!string.IsNullOrEmpty(currentSubview) && subviews.ContainsKey(currentSubview))
            {
                CloseSubview(currentSubview);
            }

            // 打开新子窗口
            SubviewInfo subviewInfo = subviews[subviewName];
            subviewInfo.subviewObject.SetActive(true);
            subviewInfo.isActive = true;
            
            // 初始化子窗口
            if (subviewInfo.subviewComponent != null)
            {
                subviewInfo.subviewComponent.Init(data);
                subviewInfo.subviewComponent.Open();
            }

            currentSubview = subviewName;
            
            // 调用子类方法
            OnSubviewSwitched(subviewName, data);
        }

        /// <summary>
        /// 关闭子窗口
        /// </summary>
        /// <param name="subviewName">子窗口名称</param>
        public virtual void CloseSubview(string subviewName)
        {
            if (!hasSubviews || !subviews.ContainsKey(subviewName))
            {
                return;
            }

            SubviewInfo subviewInfo = subviews[subviewName];
            if (subviewInfo.isActive)
            {
                if (subviewInfo.subviewComponent != null)
                {
                    subviewInfo.subviewComponent.Close();
                }
                
                subviewInfo.subviewObject.SetActive(false);
                subviewInfo.isActive = false;
            }

            if (currentSubview == subviewName)
            {
                currentSubview = "";
            }
        }

        /// <summary>
        /// 获取子窗口
        /// </summary>
        /// <typeparam name="T">子窗口类型</typeparam>
        /// <param name="subviewName">子窗口名称</param>
        /// <returns>子窗口实例</returns>
        public T GetSubview<T>(string subviewName) where T : UIBase
        {
            if (subviews.ContainsKey(subviewName))
            {
                return subviews[subviewName].subviewComponent as T;
            }
            return null;
        }

        /// <summary>
        /// 获取当前子窗口
        /// </summary>
        /// <typeparam name="T">子窗口类型</typeparam>
        /// <returns>当前子窗口实例</returns>
        public T GetCurrentSubview<T>() where T : UIBase
        {
            if (!string.IsNullOrEmpty(currentSubview))
            {
                return GetSubview<T>(currentSubview);
            }
            return null;
        }

        /// <summary>
        /// 检查子窗口是否激活
        /// </summary>
        /// <param name="subviewName">子窗口名称</param>
        /// <returns>是否激活</returns>
        public bool IsSubviewActive(string subviewName)
        {
            return subviews.ContainsKey(subviewName) && subviews[subviewName].isActive;
        }

        /// <summary>
        /// 获取所有子窗口名称
        /// </summary>
        /// <returns>子窗口名称列表</returns>
        public List<string> GetAllSubviewNames()
        {
            return new List<string>(subviews.Keys);
        }

        /// <summary>
        /// 初始化UI
        /// </summary>
        /// <param name="data">传递的数据</param>
        public virtual void Init(object data = null)
        {
            if (isInitialized) return;
            
            OnInit(data);
            isInitialized = true;
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        public virtual void Open()
        {
            if (!isInitialized)
            {
                Debug.LogError($"UI {uiName} not initialized!");
                return;
            }

            gameObject.SetActive(true);
            OnOpen();
            
            if (useAnimation)
            {
                //todo 动画效果 
            }
            
            isOpened = true;
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        public virtual void Close()
        {
            if (!isOpened) return;

            // 关闭所有子窗口
            if (hasSubviews)
            {
                foreach (var subview in subviews.Values)
                {
                    if (subview.isActive)
                    {
                        CloseSubview(subview.subviewName);
                    }
                }
            }

            if (useAnimation)
            {
                // PlayCloseAnimation(() => {
                //     OnClose();
                //     gameObject.SetActive(false);
                //     isOpened = false;
                // });
                //关闭时的动画效果
            }
            else
            {
                OnClose();
                gameObject.SetActive(false);
                isOpened = false;
            }
        }

        /// <summary>
        /// 销毁UI
        /// </summary>
        public virtual void Destroy()
        {
            OnDestroy();
            // 删除事件清理
            componentCache.Clear();
            subviews.Clear();
        }

        #region 生命周期方法 - 子类重写

        /// <summary>
        /// 初始化时调用
        /// </summary>
        protected virtual void OnInit(object data) { }

        /// <summary>
        /// 打开时调用
        /// </summary>
        protected virtual void OnOpen() { }

        /// <summary>
        /// 关闭时调用
        /// </summary>
        protected virtual void OnClose() { }

        /// <summary>
        /// 销毁时调用
        /// </summary>
        protected virtual void OnDestroy() { }

        /// <summary>
        /// 子窗口切换时调用
        /// </summary>
        /// <param name="subviewName">子窗口名称</param>
        /// <param name="data">传递数据</param>
        protected virtual void OnSubviewSwitched(string subviewName, object data) { }

        #endregion

        #region 组件查找工具

        /// <summary>
        /// 获取组件（带缓存）
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="path">组件路径</param>
        /// <returns>组件实例</returns>
        protected T GetComponent<T>(string path = "") where T : Component
        {
            string key = $"{typeof(T).Name}_{path}";
            
            if (componentCache.ContainsKey(key))
            {
                return componentCache[key] as T;
            }

            T component = null;
            if (string.IsNullOrEmpty(path))
            {
                component = GetComponent<T>();
            }
            else
            {
                Transform target = transform.Find(path);
                if (target != null)
                {
                    component = target.GetComponent<T>();
                }
            }

            if (component != null)
            {
                componentCache[key] = component;
            }
            else
            {
                Debug.LogWarning($"Component {typeof(T).Name} not found at path: {path}");
            }

            return component;
        }

        /// <summary>
        /// 获取子对象
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>Transform</returns>
        protected Transform GetChild(string path)
        {
            return transform.Find(path);
        }

        /// <summary>
        /// 缓存常用组件
        /// </summary>
        protected virtual void CacheComponents()
        {
            // 子类可以重写此方法来缓存特定组件
        }

        #endregion

        #region 事件系统

        // 删除整个事件系统部分

        #endregion

        #region 便捷方法

        /// <summary>
        /// 设置按钮点击事件
        /// </summary>
        /// <param name="buttonPath">按钮路径</param>
        /// <param name="onClick">点击回调</param>
        protected void SetButtonClick(string buttonPath, Action onClick)
        {
            Button button = GetComponent<Button>(buttonPath);
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => onClick?.Invoke());
            }
        }

        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="textPath">文本路径</param>
        /// <param name="content">文本内容</param>
        protected void SetText(string textPath, string content)
        {
            Text text = GetComponent<Text>(textPath);
            if (text != null)
            {
                text.text = content;
            }
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <param name="sprite">精灵</param>
        protected void SetImage(string imagePath, Sprite sprite)
        {
            Image image = GetComponent<Image>(imagePath);
            if (image != null)
            {
                image.sprite = sprite;
            }
        }

        /// <summary>
        /// 设置对象显示状态
        /// </summary>
        /// <param name="path">对象路径</param>
        /// <param name="active">是否激活</param>
        protected void SetActive(string path, bool active)
        {
            Transform target = GetChild(path);
            if (target != null)
            {
                target.gameObject.SetActive(active);
            }
        }

        #endregion
    }
} 