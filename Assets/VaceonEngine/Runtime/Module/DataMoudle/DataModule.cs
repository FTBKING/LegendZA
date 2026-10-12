using System;
using System.Collections;
using System.Collections.Generic;
using VaceonEngine;
using VaceonEngine.Runtime.Module;
using YooAsset;

namespace GameScripts.HotFix.GameBase.DataSystem
{
    /// <summary>
    /// YooAsset 的最小数据模块实现。
    /// 不包含 TEngine 对象池、AssetObject 包装或 Luban。
    /// </summary>
    public sealed class DataModule : Module, IDataModule
    {
        private readonly Dictionary<string, AssetHandle> _loadedHandles = new();
        //只存了1个Package，只能加载1个
        private ResourcePackage _package;
        private bool _yooAssetsInitialized;

        public bool IsInitialized { get; private set; }

        public string DefaultPackageName { get; private set; } = "DefaultPackage";

        public override void OnInit()
        {
        }

        /// <summary>
        /// 编辑器使用模拟模式；构建后使用单机模式。
        /// </summary>
        public IEnumerator InitPackage(string packageName = "DefaultPackage")
        {
            if (string.IsNullOrWhiteSpace(packageName))
            {
                packageName = "DefaultPackage";
            }

            if (IsInitialized && _package != null && _package.PackageName == packageName)
            {
                yield break;
            }

            if (!_yooAssetsInitialized)
            {
                YooAssets.Initialize();
                _yooAssetsInitialized = true;
            }

            ResourcePackage package =
                YooAssets.TryGetPackage(packageName) ?? YooAssets.CreatePackage(packageName);

            if (package.InitializeStatus == EOperationStatus.Succeed)
            {
                CompleteInitialization(package, packageName);
                yield break;
            }

            if (package.InitializeStatus == EOperationStatus.Processing)
            {
                Log.Warning($"资源包正在初始化，不能重复初始化：{packageName}");
                yield break;
            }

            YooAssets.SetDefaultPackage(package);

            InitializationOperation initOperation;
#if UNITY_EDITOR
            const int maxSimulateBuildAttempts = 3;
            PackageInvokeBuildResult buildResult = null;
            Exception buildException = null;

            for (int attempt = 1; attempt <= maxSimulateBuildAttempts; attempt++)
            {
                try
                {
                    buildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
                    buildException = null;
                }
                catch (Exception exception)
                {
                    buildException = exception;
                }

                if (buildResult != null)
                {
                    break;
                }

                if (attempt == maxSimulateBuildAttempts)
                {
                    Log.Error(
                        $"编辑器模拟清单构建失败，已重试 {maxSimulateBuildAttempts} 次：{buildException}");
                    yield break;
                }

                Log.Warning(
                    $"编辑器模拟清单构建失败，将在短暂等待后重试（{attempt}/{maxSimulateBuildAttempts}）。");
                yield return new UnityEngine.WaitForSecondsRealtime(0.25f);
            }

            var parameters = new EditorSimulateModeParameters
            {
                EditorFileSystemParameters =
                    FileSystemParameters.CreateDefaultEditorFileSystemParameters(
                        buildResult.PackageRootDirectory)
            };
            initOperation = package.InitializeAsync(parameters);
#else
            var parameters = new OfflinePlayModeParameters
            {
                BuildinFileSystemParameters =
                    FileSystemParameters.CreateDefaultBuildinFileSystemParameters()
            };
            initOperation = package.InitializeAsync(parameters);
#endif

            yield return initOperation;
            if (initOperation.Status != EOperationStatus.Succeed)
            {
                Log.Error($"资源包初始化失败：{initOperation.Error}");
                yield break;
            }

            RequestPackageVersionOperation versionOperation = package.RequestPackageVersionAsync();
            yield return versionOperation;
            if (versionOperation.Status != EOperationStatus.Succeed)
            {
                Log.Error($"获取资源包版本失败：{versionOperation.Error}");
                yield break;
            }

            UpdatePackageManifestOperation manifestOperation =
                package.UpdatePackageManifestAsync(versionOperation.PackageVersion);
            yield return manifestOperation;
            if (manifestOperation.Status != EOperationStatus.Succeed)
            {
                Log.Error($"更新资源清单失败：{manifestOperation.Error}");
                yield break;
            }

            CompleteInitialization(package, packageName);
            Log.Info($"DataModule 初始化完成：{packageName}");
        }

        /// <summary>
        /// 按 YooAsset 定位地址同步加载任意 Unity 资源。
        /// </summary>
        public T LoadAsset<T>(string location) where T : UnityEngine.Object
        {
            if (!IsInitialized || _package == null)
            {
                throw new GameFrameworkException("请先调用 GameModule.Data.InitPackage()。");
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                throw new GameFrameworkException("资源定位地址不能为空。");
            }

            if (!_package.CheckLocationValid(location))
            {
                throw new GameFrameworkException($"无效的资源定位地址：{location}");
            }

            string handleKey = $"{typeof(T).FullName}|{location}";
            if (_loadedHandles.TryGetValue(handleKey, out AssetHandle cachedHandle) &&
                cachedHandle.IsValid)
            {
                return cachedHandle.AssetObject as T;
            }

            AssetHandle handle = _package.LoadAssetSync<T>(location);
            if (handle.Status != EOperationStatus.Succeed)
            {
                string error = handle.LastError;
                handle.Dispose();
                throw new GameFrameworkException($"同步加载资源失败：{location}，{error}");
            }

            T asset = handle.AssetObject as T;
            if (asset == null)
            {
                handle.Dispose();
                throw new GameFrameworkException(
                    $"资源类型不匹配：{location} 不能转换为 {typeof(T).FullName}");
            }

            _loadedHandles[handleKey] = handle;
            return asset;
        }

        /// <summary>
        /// 释放 LoadAsset 返回的资源及其 YooAsset 句柄。
        /// </summary>
        public void UnloadAsset(UnityEngine.Object asset)
        {
            if (asset == null)
            {
                return;
            }

            string matchedKey = null;
            foreach (KeyValuePair<string, AssetHandle> pair in _loadedHandles)
            {
                if (ReferenceEquals(pair.Value.AssetObject, asset))
                {
                    matchedKey = pair.Key;
                    break;
                }
            }

            if (matchedKey == null)
            {
                return;
            }

            AssetHandle handle = _loadedHandles[matchedKey];
            if (handle.IsValid)
            {
                handle.Dispose();
            }

            _loadedHandles.Remove(matchedKey);
        }

        public override void Shutdown()
        {
            foreach (AssetHandle handle in _loadedHandles.Values)
            {
                if (handle.IsValid)
                {
                    handle.Dispose();
                }
            }

            _loadedHandles.Clear();
            _package = null;
            IsInitialized = false;
        }

        private void CompleteInitialization(ResourcePackage package, string packageName)
        {
            _package = package;
            DefaultPackageName = packageName;
            YooAssets.SetDefaultPackage(package);
            IsInitialized = true;
        }
    }
}
