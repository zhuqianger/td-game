using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.UI;

namespace Common.UI
{
    public abstract class UIBase : MonoBehaviour
    {
        public bool isInitialized = false;
        public bool isVisible = false;
        
        #region 生命周期
        
        protected virtual void Awake()
        {
            Initialize();
        }
        
        protected virtual void OnDestroy()
        {
            Cleanup();
        }
        
        #endregion
        
        #region 初始化
        
        protected virtual void Initialize()
        {
            if (isInitialized) return;
            
            InitUI();
            isInitialized = true;
        }
        
        protected virtual void InitUI()
        {
            // 子类可以重写此方法进行UI初始化
        }
        
        #endregion
        
        #region 显示控制
        public virtual void Show()
        {
            if (!isInitialized)
            {
                Initialize();
            }
            
            gameObject.SetActive(true);
            isVisible = true;
        }
        
        public virtual void Hide()
        {
            gameObject.SetActive(false);
            isVisible = false;
        }
        
        public virtual void Destroy()
        {
            Destroy(gameObject);
        }
        
        #endregion
        
        #region 清理
        
        protected virtual void Cleanup()
        {
            // 子类可以重写此方法进行清理工作
        }
        
        #endregion
    }
} 