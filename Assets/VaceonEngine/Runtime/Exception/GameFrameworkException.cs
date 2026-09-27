using System;

namespace VaceonEngine
{
    /// <summary>
    /// 游戏框架异常（最小实现）。
    /// </summary>
    public class GameFrameworkException : Exception
    {
        public GameFrameworkException()
        {
        }

        public GameFrameworkException(string message)
            : base(message)
        {
        }

        public GameFrameworkException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
