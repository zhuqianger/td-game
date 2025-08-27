using System;

namespace Common
{
    [Serializable]
    public class CommonResponse
    {
        public bool success;
        public string message;
        public string data;
    }
}