using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDGame.Task
{
    /// <summary>
    /// 示例任务配置类
    /// </summary>
    [CreateAssetMenu(fileName = "ExampleTasks", menuName = "TDGame/Task/Example Tasks")]
    public class ExampleTasks : ScriptableObject
    {
        [Header("日常任务示例")]
        public List<TaskConfig> dailyTasks;
        
        [Header("周常任务示例")]
        public List<TaskConfig> weeklyTasks;
        
        [Header("主线任务示例")]
        public List<TaskConfig> mainStoryTasks;
        
        [Header("活动任务示例")]
        public List<TaskConfig> activityTasks;
        
        [Header("见习任务示例")]
        public List<TaskConfig> tutorialTasks;
        
        /// <summary>
        /// 创建示例任务配置
        /// </summary>
        [ContextMenu("Create Example Tasks")]
        public void CreateExampleTasks()
        {
            CreateDailyTasks();
            CreateWeeklyTasks();
            CreateMainStoryTasks();
            CreateActivityTasks();
            CreateTutorialTasks();
        }
        
        /// <summary>
        /// 创建日常任务
        /// </summary>
        private void CreateDailyTasks()
        {
            dailyTasks = new List<TaskConfig>();
            
            // 日常任务1：完成3次作战关卡
            TaskConfig dailyTask1 = new TaskConfig
            {
                taskId = "daily_001",
                taskName = "日常作战",
                taskType = TaskType.Daily,
                description = "完成3次作战关卡，获得基础资源奖励",
                priority = 1,
                targets = new List<TaskTarget>
                {
                    new TaskTarget
                    {
                        targetType = TaskTargetType.StageCompleteCount,
                        targetId = "any_stage",
                        targetName = "完成作战关卡",
                        requiredAmount = 3,
                        currentAmount = 0,
                        description = "完成任意作战关卡3次"
                    }
                },
                rewards = new List<TaskReward>
                {
                    new TaskReward
                    {
                        rewardId = "lmd",
                        rewardName = "龙门币",
                        rewardType = "currency",
                        amount = 2000
                    },
                    new TaskReward
                    {
                        rewardId = "originite_prime",
                        rewardName = "合成玉",
                        rewardType = "currency",
                        amount = 100
                    }
                },
                isTimeLimited = true,
                refreshInterval = 24
            };
            dailyTasks.Add(dailyTask1);
            
            // 日常任务2：制造1份赤金
            TaskConfig dailyTask2 = new TaskConfig
            {
                taskId = "daily_002",
                taskName = "制造赤金",
                taskType = TaskType.Daily,
                description = "在制造站制造1份赤金",
                priority = 2,
                targets = new List<TaskTarget>
                {
                    new TaskTarget
                    {
                        targetType = TaskTargetType.ItemCraft,
                        targetId = "gold_bar",
                        targetName = "制造赤金",
                        requiredAmount = 1,
                        currentAmount = 0,
                        description = "在制造站制造1份赤金"
                    }
                },
                rewards = new List<TaskReward>
                {
                    new TaskReward
                    {
                        rewardId = "lmd",
                        rewardName = "龙门币",
                        rewardType = "currency",
                        amount = 1500
                    }
                },
                isTimeLimited = true,
                refreshInterval = 24
            };
            dailyTasks.Add(dailyTask2);
            
            // 日常任务3：消耗100理智
            TaskConfig dailyTask3 = new TaskConfig
            {
                taskId = "daily_003",
                taskName = "理智消耗",
                taskType = TaskType.Daily,
                description = "消耗100理智进行作战",
                priority = 3,
                targets = new List<TaskTarget>
                {
                    new TaskTarget
                    {
                        targetType = TaskTargetType.ResourceSpend,
                        targetId = "sanity",
                        targetName = "消耗理智",
                        requiredAmount = 100,
                        currentAmount = 0,
                        description = "消耗100理智"
                    }
                },
                rewards = new List<TaskReward>
                {
                    new TaskReward
                    {
                        rewardId = "originite_prime",
                        rewardName = "合成玉",
                        rewardType = "currency",
                        amount = 50
                    }
                },
                isTimeLimited = true,
                refreshInterval = 24
            };
            dailyTasks.Add(dailyTask3);
        }
        
        /// <summary>
        /// 创建周常任务
        /// </summary>
        private void CreateWeeklyTasks()
        {
            weeklyTasks = new List<TaskConfig>();
            
            // 周常任务1：累计完成20次作战
            TaskConfig weeklyTask1 = new TaskConfig
            {
                taskId = "weekly_001",
                taskName = "作战达人",
                taskType = TaskType.Weekly,
                description = "累计完成20次作战关卡",
                priority = 1,
                targets = new List<TaskTarget>
                {
                    new TaskTarget
                    {
                        targetType = TaskTargetType.StageCompleteCount,
                        targetId = "any_stage",
                        targetName = "完成作战关卡",
                        requiredAmount = 20,
                        currentAmount = 0,
                        description = "累计完成20次作战关卡"
                    }
                },
                rewards = new List<TaskReward>
                {
                    new TaskReward
                    {
                        rewardId = "originite",
                        rewardName = "至纯源石",
                        rewardType = "currency",
                        amount = 1
                    }
                },
                isTimeLimited = true,
                refreshInterval = 168 // 7天
            };
            weeklyTasks.Add(weeklyTask1);
            
            // 周常任务2：剿灭作战累计400杀
            TaskConfig weeklyTask2 = new TaskConfig
            {
                taskId = "weekly_002",
                taskName = "剿灭专家",
                taskType = TaskType.Weekly,
                description = "在剿灭作战中累计击杀400个敌人",
                priority = 2,
                targets = new List<TaskTarget>
                {
                    new TaskTarget
                    {
                        targetType = TaskTargetType.EnemyKillCount,
                        targetId = "annihilation",
                        targetName = "剿灭作战击杀",
                        requiredAmount = 400,
                        currentAmount = 0,
                        description = "在剿灭作战中累计击杀400个敌人"
                    }
                },
                rewards = new List<TaskReward>
                {
                    new TaskReward
                    {
                        rewardId = "advanced_material_box",
                        rewardName = "高级材料箱",
                        rewardType = "item",
                        amount = 1
                    }
                },
                isTimeLimited = true,
                refreshInterval = 168
            };
            weeklyTasks.Add(weeklyTask2);
            
            // 周常任务3：精英化1名干员
            TaskConfig weeklyTask3 = new TaskConfig
            {
                taskId = "weekly_003",
                taskName = "精英化干员",
                taskType = TaskType.Weekly,
                description = "精英化1名干员",
                priority = 3,
                targets = new List<TaskTarget>
                {
                    new TaskTarget
                    {
                        targetType = TaskTargetType.OperatorElite,
                        targetId = "any_operator",
                        targetName = "精英化干员",
                        requiredAmount = 1,
                        currentAmount = 0,
                        description = "精英化任意1名干员"
                    }
                },
                rewards = new List<TaskReward>
                {
                    new TaskReward
                    {
                        rewardId = "recruitment_permit",
                        rewardName = "招募许可",
                        rewardType = "item",
                        amount = 3
                    }
                },
                isTimeLimited = true,
                refreshInterval = 168
            };
            weeklyTasks.Add(weeklyTask3);
        }
        
        /// <summary>
        /// 创建主线任务
        /// </summary>
        private void CreateMainStoryTasks()
        {
            mainStoryTasks = new List<TaskConfig>();
            
            // 主线任务1：通关TR-15（教学关卡）
            TaskConfig mainStoryTask1 = new TaskConfig
            {
                taskId = "main_001",
                taskName = "新手教程",
                taskType = TaskType.MainStory,
                description = "完成新手教程关卡TR-15",
                priority = 1,
                targets = new List<TaskTarget>
                {
                    new TaskTarget
                    {
                        targetType = TaskTargetType.StageComplete,
                        targetId = "TR-15",
                        targetName = "通关TR-15",
                        requiredAmount = 1,
                        currentAmount = 0,
                        description = "完成新手教程关卡TR-15"
                    }
                },
                rewards = new List<TaskReward>
                {
                    new TaskReward
                    {
                        rewardId = "recruitment_permit",
                        rewardName = "招募许可",
                        rewardType = "item",
                        amount = 1
                    }
                },
                isFirstTimeReward = true,
                isChainTask = true,
                nextTaskId = "main_002"
            };
            mainStoryTasks.Add(mainStoryTask1);
            
            // 主线任务2：精英化1名干员
            TaskConfig mainStoryTask2 = new TaskConfig
            {
                taskId = "main_002",
                taskName = "干员精英化",
                taskType = TaskType.MainStory,
                description = "精英化1名干员，解锁更多功能",
                priority = 2,
                targets = new List<TaskTarget>
                {
                    new TaskTarget
                    {
                        targetType = TaskTargetType.OperatorElite,
                        targetId = "any_operator",
                        targetName = "精英化干员",
                        requiredAmount = 1,
                        currentAmount = 0,
                        description = "精英化任意1名干员"
                    }
                },
                rewards = new List<TaskReward>
                {
                    new TaskReward
                    {
                        rewardId = "tactical_record",
                        rewardName = "战术演习记录",
                        rewardType = "item",
                        amount = 5
                    }
                },
                prerequisiteTaskId = "main_001",
                isChainTask = true,
                nextTaskId = "main_003"
            };
            mainStoryTasks.Add(mainStoryTask2);
            
            // 主线任务3：解锁基建系统
            TaskConfig mainStoryTask3 = new TaskConfig
            {
                taskId = "main_003",
                taskName = "基建系统",
                taskType = TaskType.MainStory,
                description = "解锁基建系统，开始资源生产",
                priority = 3,
                targets = new List<TaskTarget>
                {
                    new TaskTarget
                    {
                        targetType = TaskTargetType.Custom,
                        targetId = "unlock_base",
                        targetName = "解锁基建",
                        requiredAmount = 1,
                        currentAmount = 0,
                        description = "解锁基建系统"
                    }
                },
                rewards = new List<TaskReward>
                {
                    new TaskReward
                    {
                        rewardId = "originite",
                        rewardName = "至纯源石",
                        rewardType = "currency",
                        amount = 1
                    }
                },
                prerequisiteTaskId = "main_002"
            };
            mainStoryTasks.Add(mainStoryTask3);
        }
        
        /// <summary>
        /// 创建活动任务
        /// </summary>
        private void CreateActivityTasks()
        {
            activityTasks = new List<TaskConfig>();
            
            // 活动任务1：活动关卡累计胜利10次
            TaskConfig activityTask1 = new TaskConfig
            {
                taskId = "activity_001",
                taskName = "活动挑战",
                taskType = TaskType.Activity,
                description = "在活动关卡中累计胜利10次",
                priority = 1,
                targets = new List<TaskTarget>
                {
                    new TaskTarget
                    {
                        targetType = TaskTargetType.StageCompleteCount,
                        targetId = "activity_stage",
                        targetName = "活动关卡胜利",
                        requiredAmount = 10,
                        currentAmount = 0,
                        description = "在活动关卡中累计胜利10次"
                    }
                },
                rewards = new List<TaskReward>
                {
                    new TaskReward
                    {
                        rewardId = "activity_furniture",
                        rewardName = "活动限定家具",
                        rewardType = "item",
                        amount = 1
                    }
                },
                isTimeLimited = true,
                startTime = DateTime.Now,
                endTime = DateTime.Now.AddDays(14) // 14天活动
            };
            activityTasks.Add(activityTask1);
            
            // 活动任务2：收集活动代币500个
            TaskConfig activityTask2 = new TaskConfig
            {
                taskId = "activity_002",
                taskName = "代币收集",
                taskType = TaskType.Activity,
                description = "收集500个活动代币",
                priority = 2,
                targets = new List<TaskTarget>
                {
                    new TaskTarget
                    {
                        targetType = TaskTargetType.ItemCollect,
                        targetId = "activity_token",
                        targetName = "收集活动代币",
                        requiredAmount = 500,
                        currentAmount = 0,
                        description = "收集500个活动代币"
                    }
                },
                rewards = new List<TaskReward>
                {
                    new TaskReward
                    {
                        rewardId = "operator_token",
                        rewardName = "专属干员信物",
                        rewardType = "item",
                        amount = 1
                    }
                },
                isTimeLimited = true,
                startTime = DateTime.Now,
                endTime = DateTime.Now.AddDays(14)
            };
            activityTasks.Add(activityTask2);
        }
        
        /// <summary>
        /// 创建见习任务
        /// </summary>
        private void CreateTutorialTasks()
        {
            tutorialTasks = new List<TaskConfig>();
            
            // 见习任务1：完成新手教程
            TaskConfig tutorialTask1 = new TaskConfig
            {
                taskId = "tutorial_001",
                taskName = "新手引导",
                taskType = TaskType.Tutorial,
                description = "完成新手教程，学习基础操作",
                priority = 1,
                targets = new List<TaskTarget>
                {
                    new TaskTarget
                    {
                        targetType = TaskTargetType.Custom,
                        targetId = "complete_tutorial",
                        targetName = "完成新手教程",
                        requiredAmount = 1,
                        currentAmount = 0,
                        description = "完成新手教程"
                    }
                },
                rewards = new List<TaskReward>
                {
                    new TaskReward
                    {
                        rewardId = "three_star_operator",
                        rewardName = "三星干员",
                        rewardType = "operator",
                        amount = 1
                    }
                },
                isChainTask = true,
                nextTaskId = "tutorial_002"
            };
            tutorialTasks.Add(tutorialTask1);
            
            // 见习任务2：招募1名干员
            TaskConfig tutorialTask2 = new TaskConfig
            {
                taskId = "tutorial_002",
                taskName = "首次招募",
                taskType = TaskType.Tutorial,
                description = "招募你的第一名干员",
                priority = 2,
                targets = new List<TaskTarget>
                {
                    new TaskTarget
                    {
                        targetType = TaskTargetType.OperatorRecruit,
                        targetId = "any_operator",
                        targetName = "招募干员",
                        requiredAmount = 1,
                        currentAmount = 0,
                        description = "招募任意1名干员"
                    }
                },
                rewards = new List<TaskReward>
                {
                    new TaskReward
                    {
                        rewardId = "sanity_potion",
                        rewardName = "应急理智合剂",
                        rewardType = "item",
                        amount = 1
                    }
                },
                prerequisiteTaskId = "tutorial_001"
            };
            tutorialTasks.Add(tutorialTask2);
        }
        
        /// <summary>
        /// 获取所有任务配置
        /// </summary>
        public List<TaskConfig> GetAllTaskConfigs()
        {
            List<TaskConfig> allTasks = new List<TaskConfig>();
            
            allTasks.AddRange(dailyTasks);
            allTasks.AddRange(weeklyTasks);
            allTasks.AddRange(mainStoryTasks);
            allTasks.AddRange(activityTasks);
            allTasks.AddRange(tutorialTasks);
            
            return allTasks;
        }
        
        /// <summary>
        /// 根据类型获取任务配置
        /// </summary>
        public List<TaskConfig> GetTaskConfigsByType(TaskType taskType)
        {
            switch (taskType)
            {
                case TaskType.Daily:
                    return dailyTasks;
                case TaskType.Weekly:
                    return weeklyTasks;
                case TaskType.MainStory:
                    return mainStoryTasks;
                case TaskType.Activity:
                    return activityTasks;
                case TaskType.Tutorial:
                    return tutorialTasks;
                default:
                    return new List<TaskConfig>();
            }
        }
    }
} 