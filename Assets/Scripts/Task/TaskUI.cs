using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace TDGame.Task
{
    /// <summary>
    /// 任务UI类
    /// </summary>
    public class TaskUI : MonoBehaviour
    {
        [Header("任务列表UI")]
        public Transform taskListContainer;
        public GameObject taskItemPrefab;
        public ScrollRect taskScrollRect;
        
        [Header("任务详情UI")]
        public GameObject taskDetailPanel;
        public TextMeshProUGUI taskNameText;
        public TextMeshProUGUI taskTypeText;
        public TextMeshProUGUI taskDescriptionText;
        public TextMeshProUGUI taskStatusText;
        public TextMeshProUGUI taskTimeText;
        
        [Header("任务目标UI")]
        public Transform targetContainer;
        public GameObject targetItemPrefab;
        
        [Header("任务奖励UI")]
        public Transform rewardContainer;
        public GameObject rewardItemPrefab;
        
        [Header("控制按钮")]
        public Button claimAllButton;
        public Button refreshButton;
        public Button closeButton;
        
        [Header("筛选按钮")]
        public Button allTasksButton;
        public Button dailyTasksButton;
        public Button weeklyTasksButton;
        public Button mainStoryTasksButton;
        public Button activityTasksButton;
        
        [Header("统计信息UI")]
        public TextMeshProUGUI statisticsText;
        public TextMeshProUGUI refreshTimeText;
        
        [Header("颜色设置")]
        public Color activeColor = Color.white;
        public Color completedColor = Color.green;
        public Color claimedColor = Color.gray;
        public Color lockedColor = Color.red;
        
        private TaskManager taskManager;
        private Task selectedTask;
        private TaskType currentFilter = TaskType.Daily;
        private List<GameObject> taskItemInstances;
        private List<GameObject> targetItemInstances;
        private List<GameObject> rewardItemInstances;
        
        private void Awake()
        {
            InitializeUI();
        }
        
        private void Start()
        {
            taskManager = FindObjectOfType<TaskManager>();
            SetupButtonEvents();
            RefreshTaskList();
        }
        
        private void Update()
        {
            UpdateStatistics();
            UpdateRefreshTime();
        }
        
        /// <summary>
        /// 初始化UI
        /// </summary>
        private void InitializeUI()
        {
            taskItemInstances = new List<GameObject>();
            targetItemInstances = new List<GameObject>();
            rewardItemInstances = new List<GameObject>();
            
            if (taskDetailPanel != null)
                taskDetailPanel.SetActive(false);
        }
        
        /// <summary>
        /// 设置按钮事件
        /// </summary>
        private void SetupButtonEvents()
        {
            if (claimAllButton != null)
                claimAllButton.onClick.AddListener(OnClaimAllButtonClick);
                
            if (refreshButton != null)
                refreshButton.onClick.AddListener(OnRefreshButtonClick);
                
            if (closeButton != null)
                closeButton.onClick.AddListener(OnCloseButtonClick);
                
            if (allTasksButton != null)
                allTasksButton.onClick.AddListener(() => SetTaskFilter(TaskType.Daily)); // 临时使用Daily表示全部
                
            if (dailyTasksButton != null)
                dailyTasksButton.onClick.AddListener(() => SetTaskFilter(TaskType.Daily));
                
            if (weeklyTasksButton != null)
                weeklyTasksButton.onClick.AddListener(() => SetTaskFilter(TaskType.Weekly));
                
            if (mainStoryTasksButton != null)
                mainStoryTasksButton.onClick.AddListener(() => SetTaskFilter(TaskType.MainStory));
                
            if (activityTasksButton != null)
                activityTasksButton.onClick.AddListener(() => SetTaskFilter(TaskType.Activity));
        }
        
        /// <summary>
        /// 刷新任务列表
        /// </summary>
        public void RefreshTaskList()
        {
            if (taskManager == null) return;
            
            // 清理现有任务项
            ClearTaskItems();
            
            // 获取任务列表
            List<Task> tasks = GetFilteredTasks();
            
            // 创建任务项
            foreach (var task in tasks)
            {
                CreateTaskItem(task);
            }
            
            // 更新按钮状态
            UpdateButtonStates();
        }
        
        /// <summary>
        /// 获取筛选后的任务
        /// </summary>
        private List<Task> GetFilteredTasks()
        {
            if (currentFilter == TaskType.Daily) // 临时使用Daily表示全部
            {
                return taskManager.GetActiveTasks();
            }
            else
            {
                return taskManager.GetTasksByType(currentFilter);
            }
        }
        
        /// <summary>
        /// 创建任务项
        /// </summary>
        private void CreateTaskItem(Task task)
        {
            if (taskItemPrefab == null || taskListContainer == null) return;
            
            GameObject taskItem = Instantiate(taskItemPrefab, taskListContainer);
            taskItemInstances.Add(taskItem);
            
            // 设置任务项内容
            SetupTaskItem(taskItem, task);
        }
        
        /// <summary>
        /// 设置任务项内容
        /// </summary>
        private void SetupTaskItem(GameObject taskItem, Task task)
        {
            // 获取UI组件
            TextMeshProUGUI nameText = taskItem.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI typeText = taskItem.transform.Find("TypeText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI statusText = taskItem.transform.Find("StatusText")?.GetComponent<TextMeshProUGUI>();
            Slider progressSlider = taskItem.transform.Find("ProgressSlider")?.GetComponent<Slider>();
            Button claimButton = taskItem.transform.Find("ClaimButton")?.GetComponent<Button>();
            Button detailButton = taskItem.transform.Find("DetailButton")?.GetComponent<Button>();
            
            // 设置文本
            if (nameText != null)
                nameText.text = task.config.taskName;
                
            if (typeText != null)
                typeText.text = GetTaskTypeName(task.config.taskType);
                
            if (statusText != null)
                statusText.text = task.GetStatusDescription();
                
            // 设置进度条
            if (progressSlider != null)
            {
                progressSlider.value = task.GetProgressPercentage();
                
                // 根据状态设置颜色
                Image fillImage = progressSlider.fillRect.GetComponent<Image>();
                if (fillImage != null)
                {
                    switch (task.status)
                    {
                        case TaskStatus.Active:
                            fillImage.color = activeColor;
                            break;
                        case TaskStatus.Completed:
                            fillImage.color = completedColor;
                            break;
                        case TaskStatus.Claimed:
                            fillImage.color = claimedColor;
                            break;
                        case TaskStatus.Locked:
                            fillImage.color = lockedColor;
                            break;
                    }
                }
            }
            
            // 设置按钮
            if (claimButton != null)
            {
                claimButton.gameObject.SetActive(task.status == TaskStatus.Completed);
                claimButton.onClick.AddListener(() => OnClaimButtonClick(task));
            }
            
            if (detailButton != null)
            {
                detailButton.onClick.AddListener(() => OnDetailButtonClick(task));
            }
        }
        
        /// <summary>
        /// 显示任务详情
        /// </summary>
        public void ShowTaskDetail(Task task)
        {
            selectedTask = task;
            
            if (taskDetailPanel != null)
                taskDetailPanel.SetActive(true);
                
            // 设置任务信息
            if (taskNameText != null)
                taskNameText.text = task.config.taskName;
                
            if (taskTypeText != null)
                taskTypeText.text = GetTaskTypeName(task.config.taskType);
                
            if (taskDescriptionText != null)
                taskDescriptionText.text = task.config.description;
                
            if (taskStatusText != null)
                taskStatusText.text = task.GetStatusDescription();
                
            if (taskTimeText != null)
            {
                if (task.config.isTimeLimited)
                {
                    TimeSpan remaining = task.GetRemainingTime();
                    taskTimeText.text = $"剩余时间: {remaining.ToString(@"dd\:hh\:mm")}";
                }
                else
                {
                    taskTimeText.text = "无时间限制";
                }
            }
            
            // 显示任务目标
            ShowTaskTargets(task);
            
            // 显示任务奖励
            ShowTaskRewards(task);
        }
        
        /// <summary>
        /// 显示任务目标
        /// </summary>
        private void ShowTaskTargets(Task task)
        {
            if (targetContainer == null) return;
            
            // 清理现有目标项
            ClearTargetItems();
            
            foreach (var target in task.currentTargets)
            {
                CreateTargetItem(target);
            }
        }
        
        /// <summary>
        /// 创建目标项
        /// </summary>
        private void CreateTargetItem(TaskTarget target)
        {
            if (targetItemPrefab == null) return;
            
            GameObject targetItem = Instantiate(targetItemPrefab, targetContainer);
            targetItemInstances.Add(targetItem);
            
            // 设置目标内容
            TextMeshProUGUI nameText = targetItem.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI progressText = targetItem.transform.Find("ProgressText")?.GetComponent<TextMeshProUGUI>();
            Slider progressSlider = targetItem.transform.Find("ProgressSlider")?.GetComponent<Slider>();
            
            if (nameText != null)
                nameText.text = target.targetName;
                
            if (progressText != null)
                progressText.text = $"{target.currentAmount}/{target.requiredAmount}";
                
            if (progressSlider != null)
            {
                progressSlider.value = (float)target.currentAmount / target.requiredAmount;
            }
        }
        
        /// <summary>
        /// 显示任务奖励
        /// </summary>
        private void ShowTaskRewards(Task task)
        {
            if (rewardContainer == null) return;
            
            // 清理现有奖励项
            ClearRewardItems();
            
            foreach (var reward in task.config.rewards)
            {
                CreateRewardItem(reward);
            }
        }
        
        /// <summary>
        /// 创建奖励项
        /// </summary>
        private void CreateRewardItem(TaskReward reward)
        {
            if (rewardItemPrefab == null) return;
            
            GameObject rewardItem = Instantiate(rewardItemPrefab, rewardContainer);
            rewardItemInstances.Add(rewardItem);
            
            // 设置奖励内容
            TextMeshProUGUI nameText = rewardItem.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI amountText = rewardItem.transform.Find("AmountText")?.GetComponent<TextMeshProUGUI>();
            Image iconImage = rewardItem.transform.Find("IconImage")?.GetComponent<Image>();
            
            if (nameText != null)
                nameText.text = reward.rewardName;
                
            if (amountText != null)
                amountText.text = $"x{reward.amount}";
                
            if (iconImage != null && reward.rewardIcon != null)
                iconImage.sprite = reward.rewardIcon;
        }
        
        /// <summary>
        /// 设置任务筛选
        /// </summary>
        public void SetTaskFilter(TaskType taskType)
        {
            currentFilter = taskType;
            RefreshTaskList();
        }
        
        /// <summary>
        /// 更新统计信息
        /// </summary>
        private void UpdateStatistics()
        {
            if (taskManager == null || statisticsText == null) return;
            
            statisticsText.text = taskManager.GetTaskStatistics();
        }
        
        /// <summary>
        /// 更新刷新时间
        /// </summary>
        private void UpdateRefreshTime()
        {
            if (taskManager == null || refreshTimeText == null) return;
            
            DateTime nextDaily = taskManager.GetNextRefreshTime(TaskType.Daily);
            DateTime nextWeekly = taskManager.GetNextRefreshTime(TaskType.Weekly);
            
            refreshTimeText.text = $"下次日常刷新: {nextDaily.ToString(@"MM-dd HH:mm")}\n";
            refreshTimeText.text += $"下次周常刷新: {nextWeekly.ToString(@"MM-dd HH:mm")}";
        }
        
        /// <summary>
        /// 更新按钮状态
        /// </summary>
        private void UpdateButtonStates()
        {
            if (taskManager == null) return;
            
            if (claimAllButton != null)
                claimAllButton.interactable = taskManager.HasClaimableRewards();
        }
        
        /// <summary>
        /// 清理任务项
        /// </summary>
        private void ClearTaskItems()
        {
            foreach (var item in taskItemInstances)
            {
                if (item != null)
                    Destroy(item);
            }
            taskItemInstances.Clear();
        }
        
        /// <summary>
        /// 清理目标项
        /// </summary>
        private void ClearTargetItems()
        {
            foreach (var item in targetItemInstances)
            {
                if (item != null)
                    Destroy(item);
            }
            targetItemInstances.Clear();
        }
        
        /// <summary>
        /// 清理奖励项
        /// </summary>
        private void ClearRewardItems()
        {
            foreach (var item in rewardItemInstances)
            {
                if (item != null)
                    Destroy(item);
            }
            rewardItemInstances.Clear();
        }
        
        /// <summary>
        /// 获取任务类型名称
        /// </summary>
        private string GetTaskTypeName(TaskType taskType)
        {
            switch (taskType)
            {
                case TaskType.Daily: return "日常任务";
                case TaskType.Weekly: return "周常任务";
                case TaskType.MainStory: return "主线任务";
                case TaskType.Activity: return "活动任务";
                case TaskType.Tutorial: return "见习任务";
                default: return "未知";
            }
        }
        
        // 按钮事件处理方法
        private void OnClaimAllButtonClick()
        {
            if (taskManager == null) return;
            
            int claimedCount = taskManager.ClaimAllRewards();
            Debug.Log($"领取了 {claimedCount} 个任务奖励");
            
            RefreshTaskList();
        }
        
        private void OnRefreshButtonClick()
        {
            RefreshTaskList();
        }
        
        private void OnCloseButtonClick()
        {
            if (taskDetailPanel != null)
                taskDetailPanel.SetActive(false);
        }
        
        private void OnClaimButtonClick(Task task)
        {
            if (task.ClaimReward())
            {
                Debug.Log($"任务奖励已领取: {task.config.taskName}");
                RefreshTaskList();
            }
        }
        
        private void OnDetailButtonClick(Task task)
        {
            ShowTaskDetail(task);
        }
        
        /// <summary>
        /// 设置UI可见性
        /// </summary>
        public void SetUIVisibility(bool visible)
        {
            gameObject.SetActive(visible);
            
            if (visible)
            {
                RefreshTaskList();
            }
        }
        
        /// <summary>
        /// 显示任务完成提示
        /// </summary>
        public void ShowTaskCompleteNotification(Task task)
        {
            // 这里应该显示任务完成的通知
            Debug.Log($"任务完成: {task.config.taskName}");
        }
        
        /// <summary>
        /// 显示奖励领取提示
        /// </summary>
        public void ShowRewardClaimNotification(Task task)
        {
            // 这里应该显示奖励领取的通知
            Debug.Log($"奖励已领取: {task.config.taskName}");
        }
    }
} 