using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDGame.Task
{
    /// <summary>
    /// 任务管理器类
    /// </summary>
    public class TaskManager : MonoBehaviour
    {
        [Header("任务设置")]
        public List<TaskConfig> taskConfigs;
        public List<Task> activeTasks;
        public List<Task> completedTasks;
        
        [Header("刷新设置")]
        public DateTime lastDailyRefresh;
        public DateTime lastWeeklyRefresh;
        public DateTime lastActivityRefresh;
        
        [Header("统计信息")]
        public int totalTasksCompleted;
        public int totalRewardsClaimed;
        public Dictionary<TaskType, int> tasksCompletedByType;
        
        // 事件
        public System.Action<Task> OnTaskActivated;
        public System.Action<Task> OnTaskCompleted;
        public System.Action<Task> OnTaskClaimed;
        public System.Action<TaskType> OnTasksRefreshed;
        
        private Dictionary<string, Task> taskLookup;
        private Dictionary<TaskType, List<Task>> tasksByType;
        
        private void Awake()
        {
            InitializeTaskManager();
        }
        
        private void Start()
        {
            LoadTasks();
            CheckTaskRefresh();
        }
        
        private void Update()
        {
            CheckTaskRefresh();
        }
        
        /// <summary>
        /// 初始化任务管理器
        /// </summary>
        private void InitializeTaskManager()
        {
            activeTasks = new List<Task>();
            completedTasks = new List<Task>();
            taskLookup = new Dictionary<string, Task>();
            tasksByType = new Dictionary<TaskType, List<Task>>();
            tasksCompletedByType = new Dictionary<TaskType, int>();
            
            // 初始化统计
            foreach (TaskType type in Enum.GetValues(typeof(TaskType)))
            {
                tasksByType[type] = new List<Task>();
                tasksCompletedByType[type] = 0;
            }
            
            // 设置刷新时间
            lastDailyRefresh = DateTime.Today.AddHours(4); // 每日04:00
            lastWeeklyRefresh = GetLastMonday().AddHours(4); // 每周一04:00
        }
        
        /// <summary>
        /// 加载任务
        /// </summary>
        private void LoadTasks()
        {
            foreach (var config in taskConfigs)
            {
                CreateTask(config);
            }
            
            // 激活符合条件的任务
            ActivateAvailableTasks();
        }
        
        /// <summary>
        /// 创建任务实例
        /// </summary>
        private Task CreateTask(TaskConfig config)
        {
            GameObject taskObj = new GameObject($"Task_{config.taskId}");
            taskObj.transform.SetParent(transform);
            
            Task task = taskObj.AddComponent<Task>();
            task.config = config;
            
            // 订阅事件
            task.OnTaskActivated += OnTaskActivatedHandler;
            task.OnTaskCompleted += OnTaskCompletedHandler;
            task.OnTaskClaimed += OnTaskClaimedHandler;
            
            // 添加到查找表
            taskLookup[config.taskId] = task;
            tasksByType[config.taskType].Add(task);
            
            return task;
        }
        
        /// <summary>
        /// 激活可用任务
        /// </summary>
        private void ActivateAvailableTasks()
        {
            foreach (var task in taskLookup.Values)
            {
                if (task.CanActivate() && task.CheckPrerequisites(this))
                {
                    task.Activate();
                    activeTasks.Add(task);
                }
            }
        }
        
        /// <summary>
        /// 检查任务刷新
        /// </summary>
        private void CheckTaskRefresh()
        {
            DateTime now = DateTime.Now;
            
            // 检查日常任务刷新
            if (now >= lastDailyRefresh.AddDays(1))
            {
                RefreshDailyTasks();
                lastDailyRefresh = now.Date.AddHours(4);
            }
            
            // 检查周常任务刷新
            if (now >= lastWeeklyRefresh.AddDays(7))
            {
                RefreshWeeklyTasks();
                lastWeeklyRefresh = GetNextMonday().AddHours(4);
            }
        }
        
        /// <summary>
        /// 刷新日常任务
        /// </summary>
        public void RefreshDailyTasks()
        {
            var dailyTasks = GetTasksByType(TaskType.Daily);
            
            foreach (var task in dailyTasks)
            {
                if (task.status == TaskStatus.Active || task.status == TaskStatus.Completed)
                {
                    task.ResetProgress();
                }
            }
            
            OnTasksRefreshed?.Invoke(TaskType.Daily);
            Debug.Log("日常任务已刷新");
        }
        
        /// <summary>
        /// 刷新周常任务
        /// </summary>
        public void RefreshWeeklyTasks()
        {
            var weeklyTasks = GetTasksByType(TaskType.Weekly);
            
            foreach (var task in weeklyTasks)
            {
                if (task.status == TaskStatus.Active || task.status == TaskStatus.Completed)
                {
                    task.ResetProgress();
                }
            }
            
            OnTasksRefreshed?.Invoke(TaskType.Weekly);
            Debug.Log("周常任务已刷新");
        }
        
        /// <summary>
        /// 刷新活动任务
        /// </summary>
        public void RefreshActivityTasks()
        {
            var activityTasks = GetTasksByType(TaskType.Activity);
            
            foreach (var task in activityTasks)
            {
                if (task.IsExpired())
                {
                    // 处理过期任务
                    HandleExpiredTask(task);
                }
            }
            
            OnTasksRefreshed?.Invoke(TaskType.Activity);
            Debug.Log("活动任务已刷新");
        }
        
        /// <summary>
        /// 处理过期任务
        /// </summary>
        private void HandleExpiredTask(Task task)
        {
            if (task.status == TaskStatus.Completed)
            {
                // 通过邮件补发奖励
                SendRewardByMail(task);
            }
            
            // 移除过期任务
            RemoveTask(task);
        }
        
        /// <summary>
        /// 通过邮件发送奖励
        /// </summary>
        private void SendRewardByMail(Task task)
        {
            // 这里应该调用邮件系统发送奖励
            Debug.Log($"通过邮件补发任务奖励: {task.config.taskName}");
        }
        
        /// <summary>
        /// 移除任务
        /// </summary>
        private void RemoveTask(Task task)
        {
            activeTasks.Remove(task);
            taskLookup.Remove(task.config.taskId);
            tasksByType[task.config.taskType].Remove(task);
            
            Destroy(task.gameObject);
        }
        
        /// <summary>
        /// 更新任务进度
        /// </summary>
        public void UpdateTaskProgress(TaskTargetType targetType, string targetId, int amount = 1)
        {
            foreach (var task in activeTasks)
            {
                task.UpdateProgress(targetType, targetId, amount);
            }
        }
        
        /// <summary>
        /// 更新共享进度的任务
        /// </summary>
        public void UpdateSharedTaskProgress(TaskTargetType targetType, string targetId, int amount = 1)
        {
            foreach (var task in activeTasks)
            {
                if (task.config.shareProgress)
                {
                    task.UpdateProgress(targetType, targetId, amount);
                }
            }
        }
        
        /// <summary>
        /// 获取任务
        /// </summary>
        public Task GetTask(string taskId)
        {
            return taskLookup.ContainsKey(taskId) ? taskLookup[taskId] : null;
        }
        
        /// <summary>
        /// 根据类型获取任务
        /// </summary>
        public List<Task> GetTasksByType(TaskType taskType)
        {
            return tasksByType.ContainsKey(taskType) ? tasksByType[taskType] : new List<Task>();
        }
        
        /// <summary>
        /// 获取活跃任务
        /// </summary>
        public List<Task> GetActiveTasks()
        {
            return new List<Task>(activeTasks);
        }
        
        /// <summary>
        /// 获取已完成任务
        /// </summary>
        public List<Task> GetCompletedTasks()
        {
            return new List<Task>(completedTasks);
        }
        
        /// <summary>
        /// 获取可领取奖励的任务
        /// </summary>
        public List<Task> GetClaimableTasks()
        {
            List<Task> claimableTasks = new List<Task>();
            
            foreach (var task in activeTasks)
            {
                if (task.status == TaskStatus.Completed)
                {
                    claimableTasks.Add(task);
                }
            }
            
            return claimableTasks;
        }
        
        /// <summary>
        /// 领取所有可领取的奖励
        /// </summary>
        public int ClaimAllRewards()
        {
            int claimedCount = 0;
            var claimableTasks = GetClaimableTasks();
            
            foreach (var task in claimableTasks)
            {
                if (task.ClaimReward())
                {
                    claimedCount++;
                }
            }
            
            return claimedCount;
        }
        
        /// <summary>
        /// 添加新任务
        /// </summary>
        public Task AddTask(TaskConfig config)
        {
            if (taskLookup.ContainsKey(config.taskId))
            {
                Debug.LogWarning($"任务已存在: {config.taskId}");
                return taskLookup[config.taskId];
            }
            
            taskConfigs.Add(config);
            Task task = CreateTask(config);
            
            if (task.CanActivate() && task.CheckPrerequisites(this))
            {
                task.Activate();
                activeTasks.Add(task);
            }
            
            return task;
        }
        
        /// <summary>
        /// 移除任务
        /// </summary>
        public void RemoveTask(string taskId)
        {
            Task task = GetTask(taskId);
            if (task != null)
            {
                RemoveTask(task);
            }
        }
        
        /// <summary>
        /// 获取任务统计信息
        /// </summary>
        public string GetTaskStatistics()
        {
            string stats = "任务统计:\n";
            stats += $"总任务数: {taskLookup.Count}\n";
            stats += $"活跃任务: {activeTasks.Count}\n";
            stats += $"已完成任务: {completedTasks.Count}\n";
            stats += $"总完成数: {totalTasksCompleted}\n";
            stats += $"总领取数: {totalRewardsClaimed}\n";
            
            foreach (var kvp in tasksCompletedByType)
            {
                stats += $"{GetTaskTypeName(kvp.Key)}: {kvp.Value}\n";
            }
            
            return stats;
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
        
        /// <summary>
        /// 获取上次周一
        /// </summary>
        private DateTime GetLastMonday()
        {
            DateTime today = DateTime.Today;
            int daysSinceMonday = (int)today.DayOfWeek - (int)DayOfWeek.Monday;
            if (daysSinceMonday < 0) daysSinceMonday += 7;
            return today.AddDays(-daysSinceMonday);
        }
        
        /// <summary>
        /// 获取下次周一
        /// </summary>
        private DateTime GetNextMonday()
        {
            return GetLastMonday().AddDays(7);
        }
        
        /// <summary>
        /// 获取下次刷新时间
        /// </summary>
        public DateTime GetNextRefreshTime(TaskType taskType)
        {
            switch (taskType)
            {
                case TaskType.Daily:
                    return lastDailyRefresh.AddDays(1);
                case TaskType.Weekly:
                    return lastWeeklyRefresh.AddDays(7);
                default:
                    return DateTime.MaxValue;
            }
        }
        
        /// <summary>
        /// 检查是否有新任务
        /// </summary>
        public bool HasNewTasks()
        {
            foreach (var task in activeTasks)
            {
                if (task.status == TaskStatus.Active)
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 检查是否有可领取的奖励
        /// </summary>
        public bool HasClaimableRewards()
        {
            return GetClaimableTasks().Count > 0;
        }
        
        // 事件处理方法
        private void OnTaskActivatedHandler(Task task)
        {
            OnTaskActivated?.Invoke(task);
        }
        
        private void OnTaskCompletedHandler(Task task)
        {
            totalTasksCompleted++;
            tasksCompletedByType[task.config.taskType]++;
            
            activeTasks.Remove(task);
            completedTasks.Add(task);
            
            OnTaskCompleted?.Invoke(task);
            
            // 检查任务链
            CheckTaskChain(task);
        }
        
        private void OnTaskClaimedHandler(Task task)
        {
            totalRewardsClaimed++;
            OnTaskClaimed?.Invoke(task);
        }
        
        /// <summary>
        /// 检查任务链
        /// </summary>
        private void CheckTaskChain(Task completedTask)
        {
            if (!completedTask.config.isChainTask) return;
            
            string nextTaskId = completedTask.config.nextTaskId;
            if (!string.IsNullOrEmpty(nextTaskId))
            {
                Task nextTask = GetTask(nextTaskId);
                if (nextTask != null && nextTask.CanActivate())
                {
                    nextTask.Activate();
                    activeTasks.Add(nextTask);
                }
            }
        }
        
        /// <summary>
        /// 保存任务数据
        /// </summary>
        public void SaveTaskData()
        {
            // 这里应该保存任务数据到本地存储
            Debug.Log("任务数据已保存");
        }
        
        /// <summary>
        /// 加载任务数据
        /// </summary>
        public void LoadTaskData()
        {
            // 这里应该从本地存储加载任务数据
            Debug.Log("任务数据已加载");
        }
    }
} 