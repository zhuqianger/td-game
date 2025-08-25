using UnityEngine;

namespace Common.UI
{
    /// <summary>
    /// UI项基类 - 用于列表项、单个UI元素等
    /// </summary>
    public abstract class UIItemBase : UIBase
    {
        [Header("UI项配置")]
        [SerializeField] protected bool autoInit = false; // UI项通常不自动初始化
        [SerializeField] protected bool usePool = true; // 是否使用对象池
        
        protected int itemIndex = -1; // 在列表中的索引
        protected object itemData; // 项数据

        public int ItemIndex => itemIndex;
        public object ItemData => itemData;
        public bool UsePool => usePool;

        protected override void Awake()
        {
            base.Awake();
            
            // UI项通常不自动初始化，需要手动调用
            if (autoInit)
            {
                Init();
            }
        }

        /// <summary>
        /// 设置项数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="index">索引</param>
        public virtual void SetData(object data, int index = -1)
        {
            itemData = data;
            itemIndex = index;
            
            // 如果还没初始化，先初始化
            if (!isInitialized)
            {
                Init(data);
            }
            
            // 更新显示
            OnDataChanged(data, index);
        }

        /// <summary>
        /// 获取项数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns>数据</returns>
        public T GetData<T>() where T : class
        {
            return itemData as T;
        }

        /// <summary>
        /// 重置项
        /// </summary>
        public virtual void Reset()
        {
            itemIndex = -1;
            itemData = null;
            OnReset();
        }

        /// <summary>
        /// 回收项到对象池
        /// </summary>
        public virtual void Recycle()
        {
            if (usePool)
            {
                Reset();
                gameObject.SetActive(false);
                OnRecycle();
            }
        }

        /// <summary>
        /// 激活项
        /// </summary>
        public virtual void Activate()
        {
            gameObject.SetActive(true);
            OnActivate();
        }

        /// <summary>
        /// 停用项
        /// </summary>
        public virtual void Deactivate()
        {
            gameObject.SetActive(false);
            OnDeactivate();
        }

        #region 生命周期方法 - 子类重写

        /// <summary>
        /// 数据改变时调用
        /// </summary>
        /// <param name="data">新数据</param>
        /// <param name="index">索引</param>
        protected virtual void OnDataChanged(object data, int index) { }

        /// <summary>
        /// 重置时调用
        /// </summary>
        protected virtual void OnReset() { }

        /// <summary>
        /// 回收到对象池时调用
        /// </summary>
        protected virtual void OnRecycle() { }

        /// <summary>
        /// 激活时调用
        /// </summary>
        protected virtual void OnActivate() { }

        /// <summary>
        /// 停用时调用
        /// </summary>
        protected virtual void OnDeactivate() { }

        #endregion

        #region 便捷方法

        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="textPath">文本路径</param>
        /// <param name="content">文本内容</param>
        public void SetItemText(string textPath, string content)
        {
            SetText(textPath, content);
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <param name="sprite">精灵</param>
        public void SetItemImage(string imagePath, Sprite sprite)
        {
            SetImage(imagePath, sprite);
        }

        /// <summary>
        /// 设置按钮点击事件
        /// </summary>
        /// <param name="buttonPath">按钮路径</param>
        /// <param name="onClick">点击回调</param>
        public void SetItemButtonClick(string buttonPath, System.Action onClick)
        {
            SetButtonClick(buttonPath, onClick);
        }

        /// <summary>
        /// 设置项点击事件
        /// </summary>
        /// <param name="onClick">点击回调</param>
        public void SetItemClick(System.Action onClick)
        {
            SetButtonClick("", onClick);
        }

        #endregion
    }
} 