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
        protected Dictionary<string, TabInfo> tabs = new Dictionary<string, TabInfo>();
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
                    TabInfo tabInfo = new TabInfo(tabName, tabButton, tabName);
                    tabs[tabName] = tabInfo;
                    
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

            // 关闭当前页签
            if (!string.IsNullOrEmpty(currentTab) && tabs.ContainsKey(currentTab))
            {
                CloseTab(currentTab);
            }

            // 打开新页签
            TabInfo tabInfo = tabs[tabName];
            tabInfo.isActive = true;
            
            // 更新按钮状态
            UpdateTabButtonState(tabInfo, true);

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
        /// 关闭页签
        /// </summary>
        /// <param name="tabName">页签名称</param>
        public virtual void CloseTab(string tabName)
        {
            if (!hasTabs || !tabs.ContainsKey(tabName))
            {
                return;
            }

            TabInfo tabInfo = tabs[tabName];
            if (tabInfo.isActive)
            {
                tabInfo.isActive = false;
                
                // 更新按钮状态
                UpdateTabButtonState(tabInfo, false);
            }

            if (currentTab == tabName)
            {
                currentTab = "";
            }
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

        /// <summary>
        /// 更新页签按钮状态
        /// </summary>
        /// <param name="tabInfo">页签信息</param>
        /// <param name="isActive">是否激活</param>
        protected virtual void UpdateTabButtonState(TabInfo tabInfo, bool isActive)
        {
            if (tabInfo.tabIcon != null)
            {
                tabInfo.tabIcon.color = isActive ? activeTabColor : inactiveTabColor;
            }
            
            if (tabInfo.tabText != null)
            {
                tabInfo.tabText.color = isActive ? activeTextColor : inactiveTextColor;
            }
        }

        /// <summary>
        /// 获取页签
        /// </summary>
        /// <param name="tabName">页签名称</param>
        /// <returns>页签信息</returns>
        public TabInfo GetTab(string tabName)
        {
            return tabs.ContainsKey(tabName) ? tabs[tabName] : null;
        }

        /// <summary>
        /// 获取当前页签
        /// </summary>
        /// <returns>当前页签信息</returns>
        public TabInfo GetCurrentTab()
        {
            return !string.IsNullOrEmpty(currentTab) ? tabs[currentTab] : null;
        }

        /// <summary>
        /// 检查页签是否激活
        /// </summary>
        /// <param name="tabName">页签名称</param>
        /// <returns>是否激活</returns>
        public bool IsTabActive(string tabName)
        {
            return tabs.ContainsKey(tabName) && tabs[tabName].isActive;
        }

        /// <summary>
        /// 获取所有页签名称
        /// </summary>
        /// <returns>页签名称列表</returns>
        public List<string> GetAllTabNames()
        {
            return new List<string>(tabs.Keys);
        }

        /// <summary>
        /// 设置页签文本
        /// </summary>
        /// <param name="tabName">页签名称</param>
        /// <param name="text">文本内容</param>
        public void SetTabText(string tabName, string text)
        {
            if (tabs.ContainsKey(tabName) && tabs[tabName].tabText != null)
            {
                tabs[tabName].tabText.text = text;
            }
        }

        /// <summary>
        /// 设置页签图标
        /// </summary>
        /// <param name="tabName">页签名称</param>
        /// <param name="sprite">图标精灵</param>
        public void SetTabIcon(string tabName, Sprite sprite)
        {
            if (tabs.ContainsKey(tabName) && tabs[tabName].tabIcon != null)
            {
                tabs[tabName].tabIcon.sprite = sprite;
            }
        }

        /// <summary>
        /// 启用/禁用页签
        /// </summary>
        /// <param name="tabName">页签名称</param>
        /// <param name="enabled">是否启用</param>
        public void SetTabEnabled(string tabName, bool enabled)
        {
            if (tabs.ContainsKey(tabName))
            {
                tabs[tabName].tabButton.interactable = enabled;
            }
        }

        /// <summary>
        /// 显示/隐藏页签
        /// </summary>
        /// <param name="tabName">页签名称</param>
        /// <param name="visible">是否可见</param>
        public void SetTabVisible(string tabName, bool visible)
        {
            if (tabs.ContainsKey(tabName))
            {
                tabs[tabName].tabButton.gameObject.SetActive(visible);
            }
        }

        protected override void OnClose()
        {
            // 关闭当前子窗口
            CloseCurrentSubView();

            // 关闭所有页签
            if (hasTabs)
            {
                foreach (var tab in tabs.Values)
                {
                    if (tab.isActive)
                    {
                        CloseTab(tab.tabName);
                    }
                }
            }

            base.OnClose();
        }

        protected override void OnDestroy()
        {
            // 清理页签按钮事件
            if (hasTabs)
            {
                foreach (var tab in tabs.Values)
                {
                    if (tab.tabButton != null)
                    {
                        tab.tabButton.onClick.RemoveAllListeners();
                    }
                }
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