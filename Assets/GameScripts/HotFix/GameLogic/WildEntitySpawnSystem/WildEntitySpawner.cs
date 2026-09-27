using System;
using NodeCanvas.Framework;
using UnityEngine;
using UnityEngine.AI;
using VaceonEngine.Runtime.Module;
using VaceonEngine.Runtime.SimpleEventbus;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameScripts.HotFix.GameLogic.WildEntitySpawnSystem
{
    public class WildEntitySpawner : MonoBehaviour
    {
        [SerializeField] public string name = "Leafy";
        [SerializeField] public float radius = 10f;
        [SerializeField] public int spawnCount = 10;
        [SerializeField] public float stayDuration = 10f;

        private void Awake()
        {
            GameEvents.OnResourceInitComplete += Init;
        }

        //初始遍历所有子物体，以子物体为中心，在圆形区域刷新指定Pokemon
        private void Init()
        {
            GameObject prefab = GameModule.Data.LoadAsset<GameObject>(name);
            if (prefab == null)
            {
                Debug.LogError($"未找到资源：{name}");
                return;
            }

            NavMeshAgent prefabAgent = prefab.GetComponent<NavMeshAgent>();
            float agentRadius = prefabAgent != null ? Mathf.Max(0f, prefabAgent.radius) : 0f;
            float usableRadius = Mathf.Max(0f, radius - agentRadius);

            //在圆形区域内刷新
            for (int i = 0; i < spawnCount; i++)
            {
                // 为实体自身宽度留边，避免实体中心在圆内但外观越过 Gizmo。
                Vector2 disk = Random.insideUnitCircle * usableRadius;
                Vector3 randomPos = transform.position + new Vector3(disk.x, 0f, disk.y);
                GameObject model = Object.Instantiate(prefab, randomPos, Quaternion.identity);
                model.transform.SetParent(this.transform);
                Blackboard blackboard = model.GetComponent<Blackboard>();
                blackboard.SetVariableValue("StayDuration", stayDuration);
                blackboard.SetVariableValue("SpawnerPos", this.transform.position);
                blackboard.SetVariableValue("radius", radius);
            }
        }

        private void OnDrawGizmos()
        {
            utils.DrawSpawnDisc(transform.position, radius);
        }
    }
}