using System;
using UnityEngine;
using VaceonEngine;
using VaceonEngine.Runtime.Module;
using Object = UnityEngine.Object;

namespace Test
{
    public class Test:MonoBehaviour
    {
        private void Start()
        {
            TextAsset json = GameModule.Data.LoadAsset<TextAsset>("testconfig");
            GameObject prefab = GameModule.Data.LoadAsset<GameObject>("PlayerModel");
            GameObject model = Object.Instantiate(prefab);
        }

    }
}