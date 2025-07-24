using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDGame.Task
{
    /// <summary>
    /// 任务类型枚举
    /// </summary>
    public enum TaskType
    {
        Daily,      // 日常任务
        Weekly,     // 周常任务
        MainStory,  // 主线任务
        Activity,   // 活动任务
        Tutorial    // 见习任务
    }
    
    /// <summary>
    /// 任务状态枚举
    /// </summary>
    public enum TaskStatus
    {
        Locked,     // 锁定
        Active,     // 进行中
        Completed,  // 已完成
        Claimed     // 已领取
    }
    
    /// <summary>
    /// 任务目标类型枚举
    /// </summary>
    public enum TaskTargetType
    {
        StageComplete,      // 完成关卡
        StageCompleteCount, // 完成关卡次数
        OperatorRecruit,    // 招募干员
        OperatorElite,      // 精英化干员
        ItemCraft,          // 制造物品
        ItemCollect,        // 收集物品
        ResourceSpend,      // 消耗资源
        EnemyKill,          // 击杀敌人
        EnemyKillCount,     // 击杀敌人数
        Custom              // 自定义目标
    }
    
    /// <summary>
    /// 任务目标
    /// </summary>
    [Serializable]
    public class TaskTarget
    {
        public TaskTargetType targetType;    // 目标类型
        public string targetId;              // 目标ID
        public string targetName;            // 目标名称
        public int requiredAmount;           // 需要数量
        public int currentAmount;            // 当前数量
        public string description;           // 目标描述
    }
    
    /// <summary>
    /// 任务奖励
    /// </summary>
    [Serializable]
    public class TaskReward
    {
        public string rewardId;              // 奖励ID
        public string rewardName;            // 奖励名称
        public string rewardType;            // 奖励类型
        public int amount;                   // 数量
        public Sprite rewardIcon;            // 奖励图标
    }
    
    /// <summary>
    /// 任务配置
    /// </summary>
    [Serializable]
    public class TaskConfig
    {
        [Header("基础信息")]
        public string taskId;                // 任务ID
        public string taskName;              // 任务名称
        public TaskType taskType;            // 任务类型
        public string description;           // 任务描述
        public int priority;                 // 优先级
        
        [Header("任务目标")]
        public List<TaskTarget> targets;     // 任务目标列表
        public bool isChainTask;             // 是否为任务链
        public string nextTaskId;            // 下一个任务ID
        public string prerequisiteTaskId;    // 前置任务ID
        
        [Header("任务奖励")]
        public List<TaskReward> rewards;     // 奖励列表
        public bool isFirstTimeReward;       // 是否为首通奖励
        
        [Header("时间设置")]
        public DateTime startTime;           // 开始时间
        public DateTime endTime;             // 结束时间
        public bool isTimeLimited;           // 是否限时
        public int refreshInterval;          // 刷新间隔（小时）
        
        [Header("特殊设置")]
        public bool isHidden;                // 是否为隐藏任务
        public bool shareProgress;           // 是否共享进度
        public string[] sharedTaskIds;       // 共享进度的任务ID列表
    }
    
    /// <summary>
    /// 任务类
    /// </summary>
    public class Task : MonoBehaviour
    {
        [Header("任务配置")]
        public TaskConfig config;
        
        [Header("运行时状态")]
        public TaskStatus status;
        public DateTime lastUpdateTime;
        public DateTime completedTime;
        public DateTime claimedTime;
        
        [Header("进度跟踪")]
        public List<TaskTarget> currentTargets;
        public bool isAllTargetsCompleted;
        
        // 事件
        public event Action<Task> OnTaskActivated;
        public event Action<Task> OnTaskProgressUpdated;
        public event Action<Task> OnTaskCompleted;
        public event Action<Task> OnTaskClaimed;
        
        private void Awake()
        {
            InitializeTask();
        }
        
        /// <summary>
        /// 初始化任务
        /// </summary>
        private void InitializeTask()
        {
            if (config == null) return;
            
            // 初始化目标
            currentTargets = new List<TaskTarget>();
            foreach (var target in config.targets)
            {
                TaskTarget newTarget = new TaskTarget
                {
                    targetType = target.targetType,
                    targetId = target.targetId,
                    targetName = target.targetName,
                    requiredAmount = target.requiredAmount,
                    currentAmount = 0,
                    description = target.description
                };
                currentTargets.Add(newTarget);
            }
            
            // 设置初始状态
            status = TaskStatus.Locked;
            lastUpdateTime = DateTime.Now;
            isAllTargetsCompleted = false;
        }
        
        /// <summary>
        /// 激活任务
        /// </summary>
        public void Activate()
        {
            if (status != TaskStatus.Locked) return;
            
            status = TaskStatus.Active;
            lastUpdateTime = DateTime.Now;
            
            OnTaskActivated?.Invoke(this);
            Debug.Log($"任务激活: {config.taskName}");
        }
        
        /// <summary>
        /// 更新任务进度
        /// </summary>
        public void UpdateProgress(TaskTargetType targetType, string targetId, int amount = 1)
        {
            if (status != TaskStatus.Active) return;
            
            bool progressUpdated = false;
            
            foreach (var target in currentTargets)
            {
                if (target.targetType == targetType && target.targetId == targetId)
                {
                    int oldAmount = target.currentAmount;
                    target.currentAmount = Mathf.Min(target.currentAmount + amount, target.requiredAmount);
                    
                    if (target.currentAmount != oldAmount)
                    {
                        progressUpdated = true;
                    }
                }
            }
            
            if (progressUpdated)
            {
                lastUpdateTime = DateTime.Now;
                CheckCompletion();
                OnTaskProgressUpdated?.Invoke(this);
            }
        }
        
        /// <summary>
        /// 检查任务完成状态
        /// </summary>
        private void CheckCompletion()
        {
            isAllTargetsCompleted = true;
            
            foreach (var target in currentTargets)
            {
                if (target.currentAmount < target.requiredAmount)
                {
                    isAllTargetsCompleted = false;
                    break;
                }
            }
            
            if (isAllTargetsCompleted && status == TaskStatus.Active)
            {
                Complete();
            }
        }
        
        /// <summary>
        /// 完成任务
        /// </summary>
        public void Complete()
        {
            if (status != TaskStatus.Active) return;
            
            status = TaskStatus.Completed;
            completedTime = DateTime.Now;
            
            OnTaskCompleted?.Invoke(this);
            Debug.Log($"任务完成: {config.taskName}");
        }
        
        /// <summary>
        /// 领取奖励
        /// </summary>
        public bool ClaimReward()
        {
            if (status != TaskStatus.Completed) return false;
            
            // 发放奖励
            if (GiveRewards())
            {
                status = TaskStatus.Claimed;
                claimedTime = DateTime.Now;
                
                OnTaskClaimed?.Invoke(this);
                Debug.Log($"任务奖励已领取: {config.taskName}");
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// 发放奖励
        /// </summary>
        private bool GiveRewards()
        {
            if (config.rewards == null || config.rewards.Count == 0) return true;
            
            foreach (var reward in config.rewards)
            {
                // 这里应该调用资源管理器发放奖励
                Debug.Log($"发放奖励: {reward.rewardName} x{reward.amount}");
            }
            
            return true;
        }
        
        /// <summary>
        /// 重置任务进度
        /// </summary>
        public void ResetProgress()
        {
            foreach (var target in currentTargets)
            {
                target.currentAmount = 0;
            }
            
            status = TaskStatus.Active;
            isAllTargetsCompleted = false;
            lastUpdateTime = DateTime.Now;
            
            Debug.Log($"任务进度重置: {config.taskName}");
        }
        
        /// <summary>
        /// 检查任务是否过期
        /// </summary>
        public bool IsExpired()
        {
            if (!config.isTimeLimited) return false;
            
            return DateTime.Now > config.endTime;
        }
        
        /// <summary>
        /// 检查任务是否可以激活
        /// </summary>
        public bool CanActivate()
        {
            if (config.isTimeLimited && DateTime.Now > config.endTime) return false;
            
            return status == TaskStatus.Locked;
        }
        
        /// <summary>
        /// 获取任务进度百分比
        /// </summary>
        public float GetProgressPercentage()
        {
            if (currentTargets.Count == 0) return 0f;
            
            float totalProgress = 0f;
            foreach (var target in currentTargets)
            {
                totalProgress += (float)target.currentAmount / target.requiredAmount;
            }
            
            return totalProgress / currentTargets.Count;
        }
        
        /// <summary>
        /// 获取任务剩余时间
        /// </summary>
        public TimeSpan GetRemainingTime()
        {
            if (!config.isTimeLimited) return TimeSpan.Zero;
            
            return config.endTime - DateTime.Now;
        }
        
        /// <summary>
        /// 获取任务状态描述
        /// </summary>
        public string GetStatusDescription()
        {
            switch (status)
            {
                case TaskStatus.Locked:
                    return "锁定";
                case TaskStatus.Active:
                    return $"进行中 ({GetProgressPercentage():P0})";
                case TaskStatus.Completed:
                    return "已完成";
                case TaskStatus.Claimed:
                    return "已领取";
                default:
                    return "未知";
            }
        }
        
        /// <summary>
        /// 获取任务详细信息
        /// </summary>
        public string GetTaskDetails()
        {
            string details = $"任务: {config.taskName}\n";
            details += $"类型: {GetTaskTypeName(config.taskType)}\n";
            details += $"状态: {GetStatusDescription()}\n";
            details += $"描述: {config.description}\n";
            
            if (currentTargets.Count > 0)
            {
                details += "目标:\n";
                foreach (var target in currentTargets)
                {
                    details += $"  {target.targetName}: {target.currentAmount}/{target.requiredAmount}\n";
                }
            }
            
            if (config.rewards.Count > 0)
            {
                details += "奖励:\n";
                foreach (var reward in config.rewards)
                {
                    details += $"  {reward.rewardName} x{reward.amount}\n";
                }
            }
            
            if (config.isTimeLimited)
            {
                details += $"剩余时间: {GetRemainingTime().ToString(@"dd\:hh\:mm")}";
            }
            
            return details;
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
        /// 设置任务进度
        /// </summary>
        public void SetProgress(TaskTargetType targetType, string targetId, int amount)
        {
            if (status != TaskStatus.Active) return;
            
            foreach (var target in currentTargets)
            {
                if (target.targetType == targetType && target.targetId == targetId)
                {
                    target.currentAmount = Mathf.Clamp(amount, 0, target.requiredAmount);
                    break;
                }
            }
            
            lastUpdateTime = DateTime.Now;
            CheckCompletion();
            OnTaskProgressUpdated?.Invoke(this);
        }
        
        /// <summary>
        /// 检查前置任务是否完成
        /// </summary>
        public bool CheckPrerequisites(TaskManager taskManager)
        {
            if (string.IsNullOrEmpty(config.prerequisiteTaskId)) return true;
            
            var prerequisiteTask = taskManager.GetTask(config.prerequisiteTaskId);
            return prerequisiteTask != null && prerequisiteTask.status == TaskStatus.Claimed;
        }
    }
} 