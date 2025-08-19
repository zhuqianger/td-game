using UnityEngine;
using System.Collections;

namespace TDGame.Task
{
    /// <summary>
    /// 任务系统测试类
    /// </summary>
    public class TaskSystemTest : MonoBehaviour
    {
        [Header("测试设置")]
        public bool autoTest = false;
        public float testInterval = 2f;
        
        [Header("测试任务")]
        public TaskManager taskManager;
        public ExampleTasks exampleTasks;
        
        [Header("测试UI")]
        public TaskUI taskUI;
        
        [Header("测试结果")]
        [TextArea(5, 10)]
        public string testLog = "";
        
        private Coroutine testCoroutine;
        
        private void Start()
        {
            if (taskManager == null)
                taskManager = FindObjectOfType<TaskManager>();
                
            if (exampleTasks == null)
                exampleTasks = FindObjectOfType<ExampleTasks>();
                
            if (taskUI == null)
                taskUI = FindObjectOfType<TaskUI>();
                
            if (autoTest)
            {
                StartAutoTest();
            }
        }
        
        /// <summary>
        /// 开始自动测试
        /// </summary>
        public void StartAutoTest()
        {
            if (testCoroutine != null)
            {
                StopCoroutine(testCoroutine);
            }
            
            testCoroutine = StartCoroutine(AutoTestCoroutine());
        }
        
        /// <summary>
        /// 停止自动测试
        /// </summary>
        public void StopAutoTest()
        {
            if (testCoroutine != null)
            {
                StopCoroutine(testCoroutine);
                testCoroutine = null;
            }
        }
        
        /// <summary>
        /// 自动测试协程
        /// </summary>
        private IEnumerator AutoTestCoroutine()
        {
            LogTest("开始任务系统自动测试");
            
            yield return new WaitForSeconds(1f);
            
            // 测试1：初始化任务系统
            TestTaskInitialization();
            yield return new WaitForSeconds(testInterval);
            
            // 测试2：任务激活
            TestTaskActivation();
            yield return new WaitForSeconds(testInterval);
            
            // 测试3：任务进度更新
            TestTaskProgressUpdate();
            yield return new WaitForSeconds(testInterval);
            
            // 测试4：任务完成
            TestTaskCompletion();
            yield return new WaitForSeconds(testInterval);
            
            // 测试5：任务奖励领取
            TestTaskRewardClaim();
            yield return new WaitForSeconds(testInterval);
            
            // 测试6：任务刷新
            TestTaskRefresh();
            yield return new WaitForSeconds(testInterval);
            
            // 测试7：任务链
            TestTaskChain();
            yield return new WaitForSeconds(testInterval);
            
            // 测试8：UI功能
            TestUIFunctionality();
            yield return new WaitForSeconds(testInterval);
            
            LogTest("任务系统自动测试完成");
        }
        
        /// <summary>
        /// 测试任务初始化
        /// </summary>
        [ContextMenu("测试任务初始化")]
        public void TestTaskInitialization()
        {
            LogTest("=== 测试任务初始化 ===");
            
            if (taskManager == null)
            {
                LogTest("错误: 任务管理器未找到");
                return;
            }
            
            if (exampleTasks == null)
            {
                LogTest("错误: 示例任务配置未找到");
                return;
            }
            
            // 添加示例任务
            var allTaskConfigs = exampleTasks.GetAllTaskConfigs();
            LogTest($"找到 {allTaskConfigs.Count} 个任务配置");
            
            foreach (var config in allTaskConfigs)
            {
                taskManager.AddTask(config);
            }
            
            LogTest($"成功添加 {taskManager.GetActiveTasks().Count} 个活跃任务");
            LogTest($"任务统计: {taskManager.GetTaskStatistics()}");
        }
        
        /// <summary>
        /// 测试任务激活
        /// </summary>
        [ContextMenu("测试任务激活")]
        public void TestTaskActivation()
        {
            LogTest("=== 测试任务激活 ===");
            
            var activeTasks = taskManager.GetActiveTasks();
            LogTest($"当前活跃任务数: {activeTasks.Count}");
            
            foreach (var task in activeTasks)
            {
                LogTest($"任务: {task.config.taskName} - 状态: {task.GetStatusDescription()}");
            }
        }
        
        /// <summary>
        /// 测试任务进度更新
        /// </summary>
        [ContextMenu("测试任务进度更新")]
        public void TestTaskProgressUpdate()
        {
            LogTest("=== 测试任务进度更新 ===");
            
            // 模拟完成3次作战关卡
            taskManager.UpdateTaskProgress(TaskTargetType.StageCompleteCount, "any_stage", 3);
            LogTest("更新任务进度: 完成3次作战关卡");
            
            // 模拟制造1份赤金
            taskManager.UpdateTaskProgress(TaskTargetType.ItemCraft, "gold_bar", 1);
            LogTest("更新任务进度: 制造1份赤金");
            
            // 模拟消耗100理智
            taskManager.UpdateTaskProgress(TaskTargetType.ResourceSpend, "sanity", 100);
            LogTest("更新任务进度: 消耗100理智");
            
            var activeTasks = taskManager.GetActiveTasks();
            foreach (var task in activeTasks)
            {
                if (task.status == TaskStatus.Completed)
                {
                    LogTest($"任务完成: {task.config.taskName}");
                }
            }
        }
        
        /// <summary>
        /// 测试任务完成
        /// </summary>
        [ContextMenu("测试任务完成")]
        public void TestTaskCompletion()
        {
            LogTest("=== 测试任务完成 ===");
            
            var completedTasks = taskManager.GetCompletedTasks();
            LogTest($"已完成任务数: {completedTasks.Count}");
            
            foreach (var task in completedTasks)
            {
                LogTest($"已完成任务: {task.config.taskName}");
            }
        }
        
        /// <summary>
        /// 测试任务奖励领取
        /// </summary>
        [ContextMenu("测试任务奖励领取")]
        public void TestTaskRewardClaim()
        {
            LogTest("=== 测试任务奖励领取 ===");
            
            var claimableTasks = taskManager.GetClaimableTasks();
            LogTest($"可领取奖励的任务数: {claimableTasks.Count}");
            
            if (claimableTasks.Count > 0)
            {
                int claimedCount = taskManager.ClaimAllRewards();
                LogTest($"成功领取 {claimedCount} 个任务奖励");
            }
        }
        
        /// <summary>
        /// 测试任务刷新
        /// </summary>
        [ContextMenu("测试任务刷新")]
        public void TestTaskRefresh()
        {
            LogTest("=== 测试任务刷新 ===");
            
            // 测试日常任务刷新
            taskManager.RefreshDailyTasks();
            LogTest("日常任务已刷新");
            
            // 测试周常任务刷新
            taskManager.RefreshWeeklyTasks();
            LogTest("周常任务已刷新");
            
            var activeTasks = taskManager.GetActiveTasks();
            LogTest($"刷新后活跃任务数: {activeTasks.Count}");
        }
        
        /// <summary>
        /// 测试任务链
        /// </summary>
        [ContextMenu("测试任务链")]
        public void TestTaskChain()
        {
            LogTest("=== 测试任务链 ===");
            
            // 模拟完成主线任务1
            var mainTask1 = taskManager.GetTask("main_001");
            if (mainTask1 != null)
            {
                mainTask1.UpdateProgress(TaskTargetType.Custom, "complete_tutorial", 1);
                LogTest("完成主线任务1: 新手教程");
                
                if (mainTask1.status == TaskStatus.Completed)
                {
                    mainTask1.ClaimReward();
                    LogTest("领取主线任务1奖励");
                }
            }
            
            // 检查任务链是否激活
            var mainTask2 = taskManager.GetTask("main_002");
            if (mainTask2 != null && mainTask2.status == TaskStatus.Active)
            {
                LogTest("任务链激活: 主线任务2已激活");
            }
        }
        
        /// <summary>
        /// 测试UI功能
        /// </summary>
        [ContextMenu("测试UI功能")]
        public void TestUIFunctionality()
        {
            LogTest("=== 测试UI功能 ===");
            
            if (taskUI == null)
            {
                LogTest("错误: 任务UI未找到");
                return;
            }
            
            // 测试UI显示
            taskUI.SetUIVisibility(true);
            LogTest("任务UI已显示");
            
            // 测试任务列表刷新
            taskUI.RefreshTaskList();
            LogTest("任务列表已刷新");
            
            // 测试筛选功能
            taskUI.SetTaskFilter(TaskType.Daily);
            LogTest("切换到日常任务筛选");
            
            taskUI.SetTaskFilter(TaskType.Weekly);
            LogTest("切换到周常任务筛选");
            
            taskUI.SetTaskFilter(TaskType.MainStory);
            LogTest("切换到主线任务筛选");
        }
        
        /// <summary>
        /// 测试特定任务类型
        /// </summary>
        [ContextMenu("测试特定任务类型")]
        public void TestSpecificTaskType()
        {
            LogTest("=== 测试特定任务类型 ===");
            
            var dailyTasks = taskManager.GetTasksByType(TaskType.Daily);
            LogTest($"日常任务数: {dailyTasks.Count}");
            
            var weeklyTasks = taskManager.GetTasksByType(TaskType.Weekly);
            LogTest($"周常任务数: {weeklyTasks.Count}");
            
            var mainStoryTasks = taskManager.GetTasksByType(TaskType.MainStory);
            LogTest($"主线任务数: {mainStoryTasks.Count}");
            
            var activityTasks = taskManager.GetTasksByType(TaskType.Activity);
            LogTest($"活动任务数: {activityTasks.Count}");
            
            var tutorialTasks = taskManager.GetTasksByType(TaskType.Tutorial);
            LogTest($"见习任务数: {tutorialTasks.Count}");
        }
        
        /// <summary>
        /// 测试任务统计
        /// </summary>
        [ContextMenu("测试任务统计")]
        public void TestTaskStatistics()
        {
            LogTest("=== 测试任务统计 ===");
            
            string statistics = taskManager.GetTaskStatistics();
            LogTest($"任务统计信息:\n{statistics}");
        }
        
        /// <summary>
        /// 测试任务进度模拟
        /// </summary>
        [ContextMenu("测试任务进度模拟")]
        public void TestTaskProgressSimulation()
        {
            LogTest("=== 测试任务进度模拟 ===");
            
            // 模拟各种任务进度
            LogTest("模拟任务进度更新...");
            
            // 作战相关
            taskManager.UpdateTaskProgress(TaskTargetType.StageComplete, "TR-15", 1);
            taskManager.UpdateTaskProgress(TaskTargetType.StageCompleteCount, "any_stage", 5);
            taskManager.UpdateTaskProgress(TaskTargetType.EnemyKillCount, "annihilation", 200);
            
            // 干员相关
            taskManager.UpdateTaskProgress(TaskTargetType.OperatorRecruit, "any_operator", 1);
            taskManager.UpdateTaskProgress(TaskTargetType.OperatorElite, "any_operator", 1);
            
            // 资源相关
            taskManager.UpdateTaskProgress(TaskTargetType.ResourceSpend, "sanity", 50);
            taskManager.UpdateTaskProgress(TaskTargetType.ItemCraft, "gold_bar", 1);
            taskManager.UpdateTaskProgress(TaskTargetType.ItemCollect, "activity_token", 100);
            
            LogTest("任务进度模拟完成");
        }
        
        /// <summary>
        /// 测试任务过期处理
        /// </summary>
        [ContextMenu("测试任务过期处理")]
        public void TestTaskExpiration()
        {
            LogTest("=== 测试任务过期处理 ===");
            
            // 创建过期任务
            TaskConfig expiredTask = new TaskConfig
            {
                taskId = "expired_test",
                taskName = "过期测试任务",
                taskType = TaskType.Activity,
                description = "这是一个过期的测试任务",
                targets = new System.Collections.Generic.List<TaskTarget>
                {
                    new TaskTarget
                    {
                        targetType = TaskTargetType.Custom,
                        targetId = "test_target",
                        targetName = "测试目标",
                        requiredAmount = 1,
                        currentAmount = 0,
                        description = "测试目标"
                    }
                },
                rewards = new System.Collections.Generic.List<TaskReward>
                {
                    new TaskReward
                    {
                        rewardId = "test_reward",
                        rewardName = "测试奖励",
                        rewardType = "item",
                        amount = 1
                    }
                },
                isTimeLimited = true,
                startTime = System.DateTime.Now.AddDays(-7),
                endTime = System.DateTime.Now.AddDays(-1)
            };
            
            Task task = taskManager.AddTask(expiredTask);
            LogTest($"创建过期任务: {task.config.taskName}");
            
            // 检查过期处理
            taskManager.RefreshActivityTasks();
            LogTest("活动任务刷新完成");
        }
        
        /// <summary>
        /// 记录测试日志
        /// </summary>
        private void LogTest(string message)
        {
            string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
            testLog += $"[{timestamp}] {message}\n";
            Debug.Log($"[TaskSystemTest] {message}");
        }
        
        /// <summary>
        /// 清空测试日志
        /// </summary>
        [ContextMenu("清空测试日志")]
        public void ClearTestLog()
        {
            testLog = "";
        }
        
        /// <summary>
        /// 保存测试日志
        /// </summary>
        [ContextMenu("保存测试日志")]
        public void SaveTestLog()
        {
            string fileName = $"TaskSystemTest_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt";
            System.IO.File.WriteAllText(fileName, testLog);
            Debug.Log($"测试日志已保存: {fileName}");
        }
        
        private void OnDestroy()
        {
            StopAutoTest();
        }
    }
} 