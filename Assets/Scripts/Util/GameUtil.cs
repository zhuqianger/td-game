namespace Util
{
    public static class GameUtil
    {
        /// <summary>
        /// 将 byte 数组反序列化为指定的类型
        /// </summary>
        /// <typeparam name="T">要反序列化的类型，可以是类、List 或 Dictionary</typeparam>
        /// <param name="data">要反序列化的 byte 数组</param>
        /// <returns>反序列化后的对象</returns>
        public static T Deserialize<T>(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return default;
            }

            string json = System.Text.Encoding.UTF8.GetString(data);
            return UnityEngine.JsonUtility.FromJson<T>(json);
        }
    }
}