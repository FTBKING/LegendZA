using System;
using UnityEngine.TextCore.Text;

namespace VaceonEngine.Runtime.SimpleEventbus
{
    public static class GameEvents
    {
        //资源初始化完成事件
        public static event Action OnResourceInitComplete;
        //发布资源初始化完成事件
        public static void RaiseResourceInitComplete() => OnResourceInitComplete?.Invoke();
    }
}