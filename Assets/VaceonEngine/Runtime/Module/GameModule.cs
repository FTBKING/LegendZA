using GameScripts.HotFix.GameBase.DataSystem;
using VaceonEngine.Runtime.Module.FsmModule;

namespace VaceonEngine.Runtime.Module
{
    public class GameModule
    {
        #region 框架模块

        /// <summary>
        /// 获取游戏基础模块。
        /// </summary>
        public static RootModule Base
        {
            get => _base ??= Utility.Unity.FindObjectOfType<RootModule>();
            private set => _base = value;
        }

        private static RootModule _base;

        /// <summary>
        /// 获取数据模块。
        /// </summary>
        public static IDataModule Data => _data ??= Get<IDataModule>();

        private static IDataModule _data;

        /// <summary>
        /// 获取有限状态机模块。
        /// </summary>
        public static IFsmModule Fsm => _fsm ??= Get<IFsmModule>();

        private static IFsmModule _fsm;

        /// <summary>
        /// 获取游戏框架模块类。
        /// </summary>
        /// <typeparam name="T">游戏框架模块类。</typeparam>
        /// <returns>游戏框架模块实例。</returns>
        private static T Get<T>() where T : class
        {
            T module = ModuleSystem.GetModule<T>();

            Log.Assert(condition: module != null, $"{typeof(T)} is null");

            return module;
        }

        public static void Shutdown()
        {
            Log.Info("GameModule Shutdown");

            _base = null;
            _data = null;
            _fsm = null;
        }
        #endregion
    }
}