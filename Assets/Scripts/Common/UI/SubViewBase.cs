using UnityEngine;

namespace Common.UI
{
    /// <summary>
    /// 子窗口基类 - 被主窗口管理的子窗口
    /// </summary>
    public abstract class SubViewBase : UIBase
    {
        [Header("子窗口配置")]
        [SerializeField] protected bool autoInit = true; // 是否自动初始化

        protected WndBase parentWindow; // 父窗口引用

        public WndBase ParentWindow => parentWindow;

        protected override void Awake()
        {
            base.Awake();
            
            // 查找父窗口
            FindParentWindow();
        }

        /// <summary>
        /// 查找父窗口
        /// </summary>
        protected virtual void FindParentWindow()
        {
            // 向上查找父窗口
            Transform parent = transform.parent;
            while (parent != null)
            {
                parentWindow = parent.GetComponent<WndBase>();
                if (parentWindow != null)
                {
                    break;
                }
                parent = parent.parent;
            }
        }

        /// <summary>
        /// 设置父窗口引用
        /// </summary>
        /// <param name="window">父窗口</param>
        public virtual void SetParentWindow(WndBase window)
        {
            parentWindow = window;
        }

        /// <summary>
        /// 初始化子窗口
        /// </summary>
        /// <param name="data">传递数据</param>
        public override void Init(object data = null)
        {
            if (isInitialized) return;
            
            OnInit(data);
            isInitialized = true;
        }

        /// <summary>
        /// 打开子窗口
        /// </summary>
        public override void Open()
        {
            if (!isInitialized)
            {
                Debug.LogError($"SubView {uiName} not initialized!");
                return;
            }

            gameObject.SetActive(true);
            OnOpen();
            
            if (useAnimation)
            {
                PlayOpenAnimation();
            }
            
            isOpened = true;
        }

        /// <summary>
        /// 关闭子窗口
        /// </summary>
        public override void Close()
        {
            if (!isOpened) return;

            if (useAnimation)
            {
                PlayCloseAnimation(() => {
                    OnClose();
                    gameObject.SetActive(false);
                    isOpened = false;
                });
            }
            else
            {
                OnClose();
                gameObject.SetActive(false);
                isOpened = false;
            }
        }

        /// <summary>
        /// 销毁子窗口
        /// </summary>
        public override void Destroy()
        {
            OnDestroy();
            eventManager.Clear();
            componentCache.Clear();
            parentWindow = null;
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

        #endregion

        #region 便捷方法

        /// <summary>
        /// 向父窗口发送事件
        /// </summary>
        /// <typeparam name="T">事件数据类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="data">事件数据</param>
        protected void SendEventToParent<T>(string eventName, T data)
        {
            if (parentWindow != null)
            {
                parentWindow.SendEvent(eventName, data);
            }
        }

        /// <summary>
        /// 从父窗口注册事件
        /// </summary>
        /// <typeparam name="T">事件数据类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="callback">回调函数</param>
        protected void RegisterEventFromParent<T>(string eventName, System.Action<T> callback)
        {
            if (parentWindow != null)
            {
                parentWindow.RegisterEvent(eventName, callback);
            }
        }

        /// <summary>
        /// 从父窗口注销事件
        /// </summary>
        /// <typeparam name="T">事件数据类型</typeparam>
        /// <param name="eventName">事件名称</param>
        /// <param name="callback">回调函数</param>
        protected void UnregisterEventFromParent<T>(string eventName, System.Action<T> callback)
        {
            if (parentWindow != null)
            {
                parentWindow.UnregisterEvent(eventName, callback);
            }
        }

        #endregion
    }
} 