using System.Collections;
using UnityEngine;
using VaceonEngine;
using VaceonEngine.Runtime.Module;
using VaceonEngine.Runtime.SimpleEventbus;

/// <summary>
/// DataModule 最小闭环验证。
/// 用法：挂到场景空物体上，Play 后看 Console。
/// </summary>
public class GameApp :MonoBehaviour
{
    [SerializeField] private string packageName = "DefaultPackage";

    private void Awake()
    {
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        yield return GameModule.Data.InitPackage(packageName);

        if (!GameModule.Data.IsInitialized)
        {
            Debug.LogError("DataModule 初始化失败。");
            //终止协程
            yield break;
        }
        
        //发布资源初始化完成事件
        GameEvents.RaiseResourceInitComplete();
        Log.Info("YooAsset:资源初始化完成");
        // GameObject prefab = GameModule.Data.LoadAsset<GameObject>("Glaceon");
        // GameObject prefab1 = GameModule.Data.LoadAsset<GameObject>("Leafy");
        // GameObject prefab2 = GameModule.Data.LoadAsset<GameObject>("Sylveon");
        // GameObject model = Object.Instantiate(prefab);
        // GameObject model1 = Object.Instantiate(prefab1);
        // GameObject model2 = Object.Instantiate(prefab2);
        
    }
}
