using UnityEngine;

namespace VaceonEngine
{
    /// <summary>
    /// 通用工具（最小实现，仅覆盖模块化脚本当前依赖）。
    /// </summary>
    public static partial class Utility
    {
        public static partial class Text
        {
            public static string Format(string format, object arg0)
            {
                return string.Format(format, arg0);
            }

            public static string Format(string format, object arg0, object arg1)
            {
                return string.Format(format, arg0, arg1);
            }

            public static string Format(string format, object arg0, object arg1, object arg2)
            {
                return string.Format(format, arg0, arg1, arg2);
            }

            public static string Format(string format, params object[] args)
            {
                return string.Format(format, args);
            }
        }

        public static partial class Unity
        {
            public static T FindObjectOfType<T>() where T : Object
            {
#if UNITY_6000_0_OR_NEWER
                return Object.FindFirstObjectByType<T>();
#else
                return Object.FindObjectOfType<T>();
#endif
            }
        }
    }
}
