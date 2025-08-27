namespace Config
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Linq;

    public static class ConfigManager
    {
        private static Dictionary<string, string> configData = new Dictionary<string, string>();
        private static Dictionary<string, Dictionary<int, string>> configMapData = new Dictionary<string, Dictionary<int, string>>();

        static ConfigManager()
        {
            LoadConfigs();
        }

        public static void LoadConfigs()
        {
            configData.Clear();
            configMapData.Clear();
            
            TextAsset[] configTexts = Resources.LoadAll<TextAsset>("Config");
            foreach (var configText in configTexts)
            {
                if (configText.name.EndsWith(".json"))
                {
                    string configName = configText.name.Replace(".json", "");
                    string jsonText = configText.text;
                    
                    // 存储原始JSON
                    configData[configName] = jsonText;
                    
                    // 转换为Map格式
                    ConvertToMapFormat(configName, jsonText);
                }
            }
        }

        private static void ConvertToMapFormat(string configName, string jsonText)
        {
            try
            {
                // 使用正则表达式提取每个配置项
                var matches = Regex.Matches(jsonText, @"{[^}]+}");
                if (matches.Count > 0)
                {
                    var mapData = new Dictionary<int, string>();
                    foreach (Match match in matches)
                    {
                        string itemJson = match.Value;
                        
                        // 提取id值
                        var idMatch = Regex.Match(itemJson, @"""id"":\s*(\d+)");
                        if (idMatch.Success && int.TryParse(idMatch.Groups[1].Value, out int id))
                        {
                            mapData[id] = itemJson;
                        }
                    }
                    
                    if (mapData.Count > 0)
                    {
                        configMapData[configName] = mapData;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"转换配置文件 {configName} 为Map格式时出错: {e.Message}");
            }
        }

        public static string GetConfig(string configName)
        {
            if (configData.ContainsKey(configName))
            {
                return configData[configName];
            }
            return null;
        }

        public static T GetConfig<T>(string configName) where T : class
        {
            string json = GetConfig(configName);
            if (!string.IsNullOrEmpty(json))
            {
                return JsonUtility.FromJson<T>(json);
            }
            return null;
        }

        public static T GetConfigById<T>(string configName, int id) where T : class
        {
            // 优先使用Map格式数据
            if (configMapData.ContainsKey(configName) && configMapData[configName].ContainsKey(id))
            {
                string itemJson = configMapData[configName][id];
                return JsonUtility.FromJson<T>(itemJson);
            }
            
            // 回退到原始方法
            string json = GetConfig(configName);
            if (!string.IsNullOrEmpty(json))
            {
                List<T> configList = JsonUtility.FromJson<List<T>>(json);
                if (configList != null)
                {
                    foreach (var config in configList)
                    {
                        var idProperty = config.GetType().GetProperty("id");
                        if (idProperty != null && (int)idProperty.GetValue(config) == id)
                        {
                            return config;
                        }
                    }
                }
            }
            return null;
        }

        // 新增：获取所有配置项的ID列表
        public static List<int> GetAllConfigIds(string configName)
        {
            if (configMapData.ContainsKey(configName))
            {
                return configMapData[configName].Keys.ToList();
            }
            return new List<int>();
        }

        // 新增：检查配置是否存在
        public static bool HasConfig(string configName, int id)
        {
            return configMapData.ContainsKey(configName) && configMapData[configName].ContainsKey(id);
        }

        // 新增：获取配置项数量
        public static int GetConfigCount(string configName)
        {
            if (configMapData.ContainsKey(configName))
            {
                return configMapData[configName].Count;
            }
            return 0;
        }
    }
}