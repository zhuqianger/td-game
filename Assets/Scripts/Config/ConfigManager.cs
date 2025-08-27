namespace Config
{
    using UnityEngine;
    using System.Collections.Generic;

    public static class ConfigManager
    {
        private static Dictionary<string, string> configData = new Dictionary<string, string>();

        public static void LoadConfigs()
        {
            configData.Clear();
            TextAsset[] configTexts = Resources.LoadAll<TextAsset>("Config");
            foreach (var configText in configTexts)
            {
                if (configText.name.EndsWith(".json"))
                {
                    configData[configText.name.Replace(".json", "")] = configText.text;
                }
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
    }
}