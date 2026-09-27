using System.Collections;

namespace GameScripts.HotFix.GameBase.DataSystem
{
    /// <summary>
    /// 数据模块：初始化默认 YooAsset 包，并按定位地址同步加载资源。
    /// </summary>
    public interface IDataModule
    {
        bool IsInitialized { get; }

        string DefaultPackageName { get; }

        IEnumerator InitPackage(string packageName = "DefaultPackage");

        T LoadAsset<T>(string location) where T : UnityEngine.Object;

        void UnloadAsset(UnityEngine.Object asset);
    }
}
