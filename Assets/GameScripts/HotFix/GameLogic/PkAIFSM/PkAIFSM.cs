using System;
using System.Collections;
using Animancer;
using FSMBrainandState;
using UnityEngine;
using UnityEngine.UI;
using UnityHFSM;
using UnityHFSM.Visualization;
using Random = UnityEngine.Random;
using StateMachine = UnityHFSM.StateMachine;

namespace GameScripts.HotFix.GameLogic.PkAIFSM
{
    [RequireComponent(typeof(AnimancerManager))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AnimancerComponent))]
    public class PkAIFSM : MonoBehaviour
    {
        private StateMachine fsm;
        //待用ActorRoot组合，先拖拽到Inspector中
        [SerializeField] public AnimancerManager animancerManager;
        //解耦就是不让无关信息打扰自己？
        [SerializeField] private Animator fsmAnimator;
        [SerializeField] private AnimancerComponent animancerComponent;
        private int randomStateIndex = 0;
        #region 状态显示

        [SerializeField] private Text stateDisplayText;
        [SerializeField] private Vector3 stateLabelOffset = new Vector3(0f, 2f, 0f);
        [SerializeField] private bool stateLabelFaceCamera = true;

        #endregion
        void Start()
        {
            CollectAllComponent();
            EnsureStateDisplayText();
            initFsm();
            //绑定Animancer中央事件
            animancerComponent.Events.AddTo(AnimancerEvent.EndSit,TriggerSit);
            StartCoroutine(DetectTargets());
        }

        private void TriggerSit()
        {
            fsm.Trigger("EndSit");
            Debug.Log("触发了EndSit事件");
        }

        private void CollectAllComponent()
        {
            animancerManager = GetComponent<AnimancerManager>();
            fsmAnimator = GetComponent<Animator>();
            animancerComponent = GetComponent<AnimancerComponent>();
        }

        private void initFsm()
        {
            fsm = new StateMachine();
            // Root FSM
            fsm.AddState("Patrol", BuildPatrolfsm());
            fsm.AddState("Follow",BuildFollowfsm());
            //设置起始为Patrol状态
            fsm.SetStartState("Patrol");
            //设置Follow到Patrol的ExitTime自动转换
            //问题：一直触发Trigger角色抽搐怎么办？
            //解决：需要needExitTime
            fsm.AddTriggerTransition("玩家靠近","Patrol","Follow");
            fsm.AddTransition("Follow","Patrol");
            HfsmAnimatorGraph.CreateAnimatorFromStateMachine(
                fsm,
                outputFolderPath: "Assets/DebugAnimators",
                animatorName: "StateMachineAnimatorGraph.controller"
            );
            fsm.Init(); 
        }

        private void Update()
        {
            if (fsm == null || !fsm.IsInitialized)
            {
                return;
            }
            fsm.OnLogic();
            HfsmAnimatorGraph.PreviewStateMachineInAnimator(fsm, fsmAnimator);
            UpdateStateDisplay();
        }

        private StateBase<string> BuildFollowfsm()
        {
            StateMachine FollowFsm = new StateMachine(needsExitTime:true);
            //追逐State
            FollowFsm.AddState("followstate", new State(onEnter: state => { PlayAnim("Walk"); },
                onLogic: state =>
                {
                    // 追逐逻辑
                    RotateTowardPlayer();
                    Debug.Log("进入了followstate");
                    if(DistanceToPlayer() < 2f) fsm.Trigger("小于2f");
                })
            );
            //坐下State
            FollowFsm.AddState("sitstate", new State(
                onEnter: state => PlayAnim("Sit"),needsExitTime:true));
            //问题：为何添加了事件退出，但无法退出？
            //因为twoWayTransition是始终存在，强制进入状态，并且sitstate无法任意退出
            //坐下动画播放完后即可告诉主状态机可退出
            FollowFsm.AddExitTriggerTransitionFromAny("EndSit",forceInstantly:true);
            //过渡
            //1.距离玩家为0.5，坐下，Follow->sit
            FollowFsm.AddTriggerTransition("小于2f","followstate", "sitstate");
            //起始状态为followstate
            FollowFsm.SetStartState("followstate");
            return FollowFsm;
        }

        private float DistanceToPlayer()
        {
            return Vector3.Distance(transform.position, player.transform.position);
        }

        private StateBase<string> BuildPatrolfsm()
        {
            StateMachine patrolFsm = new StateMachine();
            // 1. 建立一个“随机枢纽”幽灵状态
            patrolFsm.AddState("RandomHub", new State(
                isGhostState: true, // 核心：设为幽灵状态
                onEnter: state =>
                {
                    var random = Random.Range(0, 4);
                    if (randomStateIndex == random)
                    {
                        randomStateIndex = (randomStateIndex + 1) % 4;
                        return;
                    }

                    randomStateIndex = random;
                }
            ));

            patrolFsm.AddState("Idle", new State(onEnter: state => PlayAnim("Idle")));
            patrolFsm.AddState("TurnL", new State(onEnter: state =>
                PlayAnim("TurnL", onEnd: () => patrolFsm.Trigger("EndTurnL"))));
            patrolFsm.AddState("TurnR", new State(onEnter: state =>
                PlayAnim("TurnR", onEnd: () => patrolFsm.Trigger("EndTurnR"))));
            patrolFsm.AddState("Walk", new State(onEnter: state => PlayAnim("Walk")));

            // 2. 所有具体状态在 3 秒后，都统一先回流到 Hub
            patrolFsm.AddTransition(new TransitionAfter("Idle", "RandomHub", 3f));
            patrolFsm.AddTriggerTransition("EndTurnL","TurnL","RandomHub");
            patrolFsm.AddTriggerTransition("EndTurnR","TurnR","RandomHub");
            patrolFsm.AddTransition(new TransitionAfter("Walk", "RandomHub", 3f));

            // 3. Hub 负责瞬间分发到下一个目标 (因为是幽灵状态，这里用普通 Transition 即可，瞬间完成)
            patrolFsm.AddTransition("RandomHub", "Idle", condition: t => randomStateIndex == 0);
            patrolFsm.AddTransition("RandomHub", "TurnL", condition: t => randomStateIndex == 1);
            patrolFsm.AddTransition("RandomHub", "Walk", condition: t => randomStateIndex == 2);
            patrolFsm.AddTransition("RandomHub", "TurnR", condition: t => randomStateIndex == 3);
            //4.创建退出状态，它可以从任何状态推出
            
            // 只设起始状态。嵌套机不要调用 Init()：此时它还没有父机，
            patrolFsm.SetStartState("RandomHub");
            return patrolFsm;
        }

        private void PlayAnim(string clipName, bool looping = false, Action onEnd = null)
        {
            if (animancerManager == null)
            {
                Debug.LogError($"{nameof(PkAIFSM)}: 未指定 {nameof(animancerManager)}，无法播放 {clipName}。", this);
                return;
            }

            animancerManager.PlayAnim(onEnd, clipName, isLooping: looping);
        }
        public Transform player;
        public float radius = 5f;


        public IEnumerator DetectTargets(string targetTag = "Player",int targetLayer = -1)
        {
            while (true)
            {
                // 1. 获取球形区域内的所有碰撞体
                Collider[] hits = Physics.OverlapSphere(transform.position, radius, targetLayer);

                // 2. 遍历并检查 Tag
                foreach (Collider hit in hits)
                {
                    // 使用 CompareTag 性能优于 hit.tag == targetTag（无内存垃圾）
                    if (hit.CompareTag(targetTag))
                    {
                        Debug.Log($"检测到目标: {hit.name}，位置: {hit.transform.position}");
                        // 在此处理你的业务逻辑
                        if (fsm.ActiveStateName != "Follow")
                        {
                            fsm.Trigger("玩家靠近");
                            Debug.Log("玩家靠近");
                        }

                        player = hit.transform;
                    }
                }

                yield return new WaitForSeconds(1f);
            }
        }

        public void RotateTowardPlayer()
        {
            //修正rotation,LookAt
            if(player != null) transform.LookAt(player);
        }

        // 在 Scene 视口中绘制检测范围方便调试
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        /// <summary>
        /// GuardAI：子物体 World Space Canvas + Text，每帧写入 GetActiveHierarchyPath()。
        /// 没有现成 Text 时自动在头顶建一个。
        /// </summary>
        private void EnsureStateDisplayText()
        {
            if (stateDisplayText == null)
            {
                stateDisplayText = GetComponentInChildren<Text>();
            }

            if (stateDisplayText != null)
            {
                return;
            }

            var canvasGo = new GameObject("StateLabelCanvas");
            canvasGo.transform.SetParent(transform, false);
            canvasGo.transform.localPosition = stateLabelOffset;
            canvasGo.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 10f;

            var textGo = new GameObject("StateText");
            textGo.transform.SetParent(canvasGo.transform, false);

            var rect = textGo.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400f, 80f);

            stateDisplayText = textGo.AddComponent<Text>();
            stateDisplayText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            stateDisplayText.fontSize = 36;
            stateDisplayText.fontStyle = FontStyle.Bold;
            stateDisplayText.alignment = TextAnchor.MiddleCenter;
            stateDisplayText.color = Color.white;
            stateDisplayText.horizontalOverflow = HorizontalWrapMode.Overflow;
            stateDisplayText.raycastTarget = false;
        }

        private void UpdateStateDisplay()
        {
            if (stateDisplayText == null)
            {
                return;
            }

            stateDisplayText.text = fsm.GetActiveHierarchyPath();

            if (!stateLabelFaceCamera)
            {
                return;
            }

            Camera cam = Camera.main;
            if (cam == null)
            {
                return;
            }

            stateDisplayText.canvas.transform.rotation = cam.transform.rotation;
        }
        public void EndSitTrigger()
        {
            fsm.Trigger("EndSit");
        }
           }
}