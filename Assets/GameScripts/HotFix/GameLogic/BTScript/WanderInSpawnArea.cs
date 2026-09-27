using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;

namespace GameScripts.HotFix.GameLogic.BTScript
{
    [Name("半径内随机寻路")]
    [Category("Movement/Pathfinding")]
    [Description("在黑板生成区域中心与半径构成的圆盘内随机取点，经 NavMesh.SamplePosition 投影后由 NavMeshAgent.SetDestination 移动；到达后停留指定秒数。")]
    public class WanderInSpawnArea : ActionTask<NavMeshAgent>
    {
        [Tooltip("生成区域中心，对应黑板 SpawnerPos")]
        public BBParameter<Vector3> spawnCenter = new BBParameter<Vector3> { name = "SpawnerPos" };

        [Tooltip("生成区域半径，对应黑板 radius")]
        public BBParameter<float> spawnRadius = new BBParameter<float> { name = "radius" };

        [Tooltip("到达判定的额外距离。根运动驱动时留太小会永远判不到，建议 0.5 以上")]
        public BBParameter<float> keepDistance = 0.5f;

        [Tooltip("随机点与当前位置的最小水平距离，避免原地踏步")]
        public BBParameter<float> minMoveDistance = 1f;

        [Tooltip("到达随机点后停留的秒数，0 表示到达后立刻结束")]
        public BBParameter<float> stayDuration = 2f;

        [Tooltip("单次移动的最长秒数，超时按到达处理。防止被挤住或绕点时一直走，0 表示不限制")]
        public BBParameter<float> moveTimeout = 10f;

        [Tooltip("找不到可行走点时的最大采样次数")]
        public int maxSampleAttempts = 8;

        private Vector3 destination;
        private bool arrived;
        private float stayStartElapsed;

        protected override string info
        {
            get { return string.Format("随机寻路 {0} r:{1} 停留:{2}s", spawnCenter, spawnRadius, stayDuration); }
        }

        protected override void OnExecute()
        {
            if (spawnRadius.value <= 0f)
            {
                EndAction(false);
                return;
            }

            if (!TryPickDestination())
            {
                EndAction(false);
                return;
            }

            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
            }

            if (!agent.SetDestination(destination))
            {
                EndAction(false);
            }
        }

        protected override void OnUpdate()
        {
            if(HasArrived()) EndAction(true);
        }

        protected override void OnPause()
        {
            OnStop();
        }

        protected override void OnStop()
        {
            arrived = false;
            if (agent == null || !agent.gameObject.activeSelf || !agent.isOnNavMesh)
            {
                return;
            }

            agent.ResetPath();
            agent.isStopped = false;
        }
        

        // 根运动下 agent 内部位置与实际位置可能不同步，实际距离与路径剩余距离各判一次
        private bool HasArrived()
        {
            // 1. 检查是否正在计算路径（路径计算中 remainingDistance 通常为 0 或不准确）
            if (agent.pathPending)
                return false;

            // 2. 检查剩余距离是否小于等于设定的停止距离
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // 3. 确保角色没有待处理的路径或速度已经基本归零
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool TryPickDestination()
        {
            Vector3 center = spawnCenter.value;
            // 给角色自身宽度留边，避免角色中心虽在圆内，但胶囊/模型看起来越过 Gizmo。
            float radius = Mathf.Max(0f, spawnRadius.value - Mathf.Max(0f, agent.radius));
            float minDist = Mathf.Min(Mathf.Max(0f, minMoveDistance.value), radius);
            float sampleRange = Mathf.Max(2f, agent.height * 2f);
            Vector3 current = agent.transform.position;
            int mask = agent.areaMask;
            int attempts = Mathf.Max(1, maxSampleAttempts);

            for (int i = 0; i < attempts; i++)
            {
                Vector2 disk = Random.insideUnitCircle * radius;
                Vector3 candidate = center + new Vector3(disk.x, 0f, disk.y);

                float dx = candidate.x - current.x;
                float dz = candidate.z - current.z;
                if (minDist > 0f && (dx * dx + dz * dz) < minDist * minDist)
                {
                    continue;
                }

                // SamplePosition 可能把圆内候选点投影到圆外，所以投影后还要再校验一次。
                if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, sampleRange, mask)
                    && IsInsideDisc(hit.position, center, radius))
                {
                    destination = hit.position;
                    return true;
                }
            }

            if (NavMesh.SamplePosition(center, out NavMeshHit centerHit, sampleRange, mask)
                && IsInsideDisc(centerHit.position, center, radius))
            {
                destination = centerHit.position;
                return true;
            }

            return false;
        }

        private static bool IsInsideDisc(Vector3 point, Vector3 center, float radius)
        {
            float dx = point.x - center.x;
            float dz = point.z - center.z;
            return dx * dx + dz * dz <= radius * radius + 0.0001f;
        }

        private static void DrawWireDisc(Vector3 center, float radius)
        {
            const int segments = 32;
            Vector3 prev = center + new Vector3(radius, 0f, 0f);
            for (int i = 1; i <= segments; i++)
            {
                float ang = i / (float)segments * Mathf.PI * 2f;
                Vector3 next = center + new Vector3(Mathf.Cos(ang) * radius, 0f, Mathf.Sin(ang) * radius);
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }
    }
}
