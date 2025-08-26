using UnityEngine;

namespace Common.UI
{
    /// <summary>
    /// 子窗口基类 - 可以被主窗口或其他子窗口管理的子窗口
    /// </summary>
    public abstract class SubViewBase : UIBase
    {
        [Header("子窗口配置")]
        [SerializeField] protected bool autoInit = true; // 是否自动初始化

        protected UIBase parentWindow; // 父窗口引用（可以是主窗口或子窗口）
        
        public UIBase ParentWindow => parentWindow;

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
            // 向上查找父窗口（可以是主窗口或子窗口）
            Transform parent = transform.parent;
            while (parent != null)
            {
                // 优先查找主窗口
                parentWindow = parent.GetComponent<WndBase>();
                if (parentWindow != null)
                {
                    break;
                }
                
                // 如果没有找到主窗口，查找子窗口
                parentWindow = parent.GetComponent<SubViewBase>();
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
        /// <param name="window">父窗口（可以是主窗口或子窗口）</param>
        public virtual void SetParentWindow(UIBase window)
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
                // 删除事件发送
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
                // 删除事件注册
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
                // 删除事件注销
            }
        }

        /// <summary>
        /// 获取根窗口（最顶层的窗口）
        /// </summary>
        /// <returns>根窗口</returns>
        protected WndBase GetRootWindow()
        {
            if (parentWindow == null)
            {
                return null;
            }

            // 如果父窗口是主窗口，直接返回
            if (parentWindow is WndBase wndBase)
            {
                return wndBase;
            }

            // 如果父窗口是子窗口，递归查找根窗口
            if (parentWindow is SubViewBase subView)
            {
                return subView.GetRootWindow();
            }

            return null;
        }

        /// <summary>
        /// 获取窗口层级深度
        /// </summary>
        /// <returns>层级深度（0为根窗口）</returns>
        protected int GetWindowDepth()
        {
            if (parentWindow == null)
            {
                return 0;
            }

            if (parentWindow is WndBase)
            {
                return 1;
            }

            if (parentWindow is SubViewBase subView)
            {
                return subView.GetWindowDepth() + 1;
            }

            return 0;
        }

        #endregion
    }
} 