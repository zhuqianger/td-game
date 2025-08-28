using System;

namespace Common
{
    [Serializable]
    public class CommonResponse<T>
    {
        public bool success;
        public string message;
        public T data;
    }
}