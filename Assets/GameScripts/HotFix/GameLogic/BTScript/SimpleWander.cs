using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;

namespace GameScripts.HotFix.GameLogic.BTScript
{
    [Name("极简随机移动")]
    [Category("Movement/Pathfinding")]
    [Description("在指定中心点和半径内随机取一个点并移动，到达后立刻返回成功。")]
    public class SimpleWander : ActionTask<NavMeshAgent>
    {
        [Tooltip("圆心的位置")]
        public BBParameter<Vector3> centerPosition;

        [Tooltip("随机的半径范围")]
        public BBParameter<float> radius = 5f;

        [Tooltip("距离目标多近算到达")]
        public float stopDistance = 0.5f;

        // 记录我们随机选出的目标点
        private Vector3 targetPosition;

        protected override void OnExecute()
        {
            // 1. 在圆内生成一个随机的 2D 坐标
            Vector2 randomCircle = Random.insideUnitCircle * radius.value;
            
            // 2. 将 2D 坐标转换到 3D 空间，并以 centerPosition 为中心
            Vector3 randomPoint = centerPosition.value + new Vector3(randomCircle.x, 0, randomCircle.y);

            // 3. 在 NavMesh 上寻找离这个随机点最近的有效行走点 (最大查找范围设为 2 米)
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                targetPosition = hit.position;

                // 确保代理处于激活状态并放置在 NavMesh 上
                if (agent.enabled && agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.SetDestination(targetPosition);
                }
                else
                {
                    Debug.LogError("代理有问题，直接失败");
                    // 代理有问题，直接失败
                    EndAction(false);
                }
            }
            else
            {
                Debug.LogError("随机到的点附近没有有效的 NavMesh 可以走，直接返回失败");
                // 如果随机到的点附近没有有效的 NavMesh 可以走，直接返回失败
                EndAction(false);
            }
        }

        protected override void OnUpdate()
        {
            // 如果还在计算路径中，先等待
            if (agent.pathPending) 
            {
                return;
            }

            // 判断是否到达目的地：剩余距离 <= 停止距离
            if (agent.remainingDistance <= stopDistance)
            {
                // 停止移动
                agent.isStopped = true;
                Debug.Log("EndAction(true);");
                // 返回成功，退出节点
                EndAction(true);
            }
        }

        protected override void OnStop()
        {
            // 无论节点是成功、失败还是被打断，退出时都清空路径
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.ResetPath();
            }
        }
    }
}