using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class RootMotionNavAgent : MonoBehaviour
{
    [Tooltip("额外转向辅助速率（度/秒）。走跑 clip 是直线根运动，需要靠它拐弯")]
    [SerializeField] private float steerDegreesPerSecond = 360f;
    [Tooltip("剩余距离小于此值开始收速，防止根运动冲过终点；0 = 只靠 agent.autoBraking")]
    [SerializeField] private float brakeDistance = 1.5f;
    [SerializeField] private float paramDamp = 0.12f;

    private static readonly int SpeedParam = Animator.StringToHash("Speed");
    //private static readonly int TurnParam = Animator.StringToHash("Turn");

    private NavMeshAgent agent;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updatePosition = false;
        agent.updateRotation = false;
        animator.applyRootMotion = true;
    }

    private void Update()
    {
        Vector3 desired = agent.desiredVelocity;
        //到目的地前缓入，brakeDistance
        if (!agent.pathPending && brakeDistance > 0f)
            desired *= Mathf.Clamp01(agent.remainingDistance / brakeDistance);

        float speed = desired.magnitude;
        float turn = speed > 0.01f
            ? Vector3.SignedAngle(transform.forward, desired, Vector3.up)
            : 0f;

        animator.SetFloat(SpeedParam, speed, paramDamp, Time.deltaTime);
        //animator.SetFloat(TurnParam, turn / 180f, paramDamp, Time.deltaTime);

        if (speed > 0.01f && steerDegreesPerSecond > 0f)
        {
            var look = Quaternion.LookRotation(new Vector3(desired.x, 0f, desired.z));
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, look, steerDegreesPerSecond * Time.deltaTime);
        }
    }

    private void OnAnimatorMove()
    {
        Vector3 next = transform.position + animator.deltaPosition;

        // 高度取导航网格表面，避免根运动的竖直分量把角色抬离地面
        next.y = agent.nextPosition.y;

        transform.rotation = animator.deltaRotation * transform.rotation;
        transform.position = next;

        agent.nextPosition = transform.position;
    }
}