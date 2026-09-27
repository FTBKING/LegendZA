using UnityEngine;

namespace VaceonEngine
{
    /// <summary>
    /// 日志（最小实现，仅覆盖模块化脚本当前依赖）。
    /// </summary>
    public static class Log
    {
        public static void Info(object message)
        {
            Debug.Log(message);
        }

        public static void Info(string message)
        {
            Debug.Log(message);
        }

        public static void Warning(string message)
        {
            Debug.LogWarning(message);
        }

        public static void Error(string message)
        {
            Debug.LogError(message);
        }

        public static void Assert(bool condition)
        {
            if (!condition)
            {
                Debug.LogError($"Assert Failed\n{System.Environment.StackTrace}");
            }
        }

        public static void Assert(bool condition, string retStr)
        {
            if (!condition)
            {
                Debug.LogError($"Assert Failed{retStr}\n{System.Environment.StackTrace}");
            }
        }
    }
}
