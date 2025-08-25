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
        [Header("主窗口配置")]
        [SerializeField] protected bool hasTabs = false;
        [SerializeField] protected string defaultTab = "";
        [SerializeField] protected Transform tabContainer;
        [SerializeField] protected Color activeTabColor = Color.white;
        [SerializeField] protected Color inactiveTabColor = Color.gray;
        [SerializeField] protected Color activeTextColor = Color.black;
        [SerializeField] protected Color inactiveTextColor = Color.gray;

        // 页签管理
        protected Dictionary<string, object> tabs = new Dictionary<string, object>();
        protected string currentTab = "";

        // 子窗口管理
        protected Dictionary<string, SubViewBase> subViews = new Dictionary<string, SubViewBase>();
        protected SubViewBase currentSubView = null;

        public bool HasTabs => hasTabs;
        public string CurrentTab => currentTab;
        public SubViewBase CurrentSubView => currentSubView;

        protected override void Awake()
        {
            base.Awake();
            
            // 初始化页签
            if (hasTabs)
            {
                InitializeTabs();
            }
            
            // 初始化子窗口
            InitializeSubViews();
        }

        /// <summary>
        /// 初始化页签
        /// </summary>
        protected virtual void InitializeTabs()
        {
            if (tabContainer == null)
            {
                Debug.LogWarning($"Tab container not set for Window: {uiName}");
                return;
            }

            // 查找所有页签按钮
            for (int i = 0; i < tabContainer.childCount; i++)
            {
                Transform tabChild = tabContainer.GetChild(i);
                Button tabButton = tabChild.GetComponent<Button>();
                
                if (tabButton != null)
                {
                    string tabName = tabChild.name;
                    
                    // 页签名称应该与对应的子窗口名称一致
                    tabs[tabName] = new object();
                    
                    // 设置按钮点击事件
                    string capturedTabName = tabName; // 闭包捕获
                    tabButton.onClick.AddListener(() => SwitchTab(capturedTabName));
                }
            }

            // 设置默认页签
            if (!string.IsNullOrEmpty(defaultTab) && tabs.ContainsKey(defaultTab))
            {
                SwitchTab(defaultTab);
            }
        }

        /// <summary>
        /// 初始化子窗口
        /// </summary>
        protected virtual void InitializeSubViews()
        {
            if (subviewContainer == null)
            {
                Debug.LogWarning($"Subview container not set for Window: {uiName}");
                return;
            }

            // 查找所有子窗口
            for (int i = 0; i < subviewContainer.childCount; i++)
            {
                Transform child = subviewContainer.GetChild(i);
                SubViewBase subViewComponent = child.GetComponent<SubViewBase>();
                
                if (subViewComponent != null)
                {
                    subViews[child.name] = subViewComponent;
                    
                    // 默认隐藏所有子窗口
                    child.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 切换页签
        /// </summary>
        /// <param name="tabName">页签名称</param>
        public virtual void SwitchTab(string tabName)
        {
            if (!hasTabs || !tabs.ContainsKey(tabName))
            {
                Debug.LogWarning($"Tab {tabName} not found in Window {uiName}");
                return;
            }

            currentTab = tabName;
            
            // 切换到对应的子窗口
            if (subViews.ContainsKey(tabName))
            {
                SwitchSubView(tabName);
            }
            
            // 调用子类方法
            OnTabSwitched(tabName);
        }

        /// <summary>
        /// 切换子窗口
        /// </summary>
        /// <param name="subViewName">子窗口名称</param>
        /// <param name="data">传递数据</param>
        public virtual void SwitchSubView(string subViewName, object data = null)
        {
            if (!subViews.ContainsKey(subViewName))
            {
                Debug.LogWarning($"SubView {subViewName} not found in Window {uiName}");
                return;
            }

            // 关闭当前子窗口
            if (currentSubView != null)
            {
                CloseCurrentSubView();
            }

            // 打开新子窗口
            SubViewBase subView = subViews[subViewName];
            subView.gameObject.SetActive(true);
            subView.Init(data);
            subView.Open();
            
            currentSubView = subView;
            
            // 调用子类方法
            OnSubViewSwitched(subViewName, data);
        }

        /// <summary>
        /// 关闭当前子窗口
        /// </summary>
        public virtual void CloseCurrentSubView()
        {
            if (currentSubView != null)
            {
                currentSubView.Close();
                currentSubView.gameObject.SetActive(false);
                currentSubView = null;
            }
        }

        /// <summary>
        /// 获取子窗口
        /// </summary>
        /// <typeparam name="T">子窗口类型</typeparam>
        /// <param name="subViewName">子窗口名称</param>
        /// <returns>子窗口实例</returns>
        public T GetSubView<T>(string subViewName) where T : SubViewBase
        {
            if (subViews.ContainsKey(subViewName))
            {
                return subViews[subViewName] as T;
            }
            return null;
        }

        /// <summary>
        /// 检查子窗口是否激活
        /// </summary>
        /// <param name="subViewName">子窗口名称</param>
        /// <returns>是否激活</returns>
        public bool IsSubViewActive(string subViewName)
        {
            return subViews.ContainsKey(subViewName) && subViews[subViewName].IsOpened;
        }

        /// <summary>
        /// 获取所有子窗口名称
        /// </summary>
        /// <returns>子窗口名称列表</returns>
        public List<string> GetAllSubViewNames()
        {
            return new List<string>(subViews.Keys);
        }

        protected override void OnClose()
        {
            // 关闭当前子窗口
            CloseCurrentSubView();

            base.OnClose();
        }

        protected override void OnDestroy()
        {
            // 清理页签按钮事件
            if (hasTabs)
            {
                tabs.Clear();
            }

            // 清理子窗口
            subViews.Clear();

            base.OnDestroy();
        }

        #region 生命周期方法 - 子类重写

        /// <summary>
        /// 页签切换时调用
        /// </summary>
        /// <param name="tabName">页签名称</param>
        protected virtual void OnTabSwitched(string tabName) { }

        /// <summary>
        /// 子窗口切换时调用
        /// </summary>
        /// <param name="subViewName">子窗口名称</param>
        /// <param name="data">传递数据</param>
        protected virtual void OnSubViewSwitched(string subViewName, object data) { }

        #endregion
    }
} 