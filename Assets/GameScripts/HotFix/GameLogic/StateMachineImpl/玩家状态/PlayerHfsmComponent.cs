using Narakacomponent;
using UnityEngine;
using UnityHFSM;

namespace GameScripts.HotFix.GameLogic.PlayerHfsm
{
    /// <summary>
    /// 按「状态规则.png」搭出来的角色层次状态机，作为 Actor 组件由 ActorRoot 驱动。
    ///
    /// Root
    /// ├─ Patrol          Idle --AnyMoveKey--> Move（基础 8 向位移），Move --NoMoveKey--> Idle
    /// └─ Fight           FightIdle --左键/Q/E/R--> Attack/Skill1/Skill2/Skill3 --End--> FightIdle
    ///
    /// Patrol --AnyFightKey--> Fight，Fight --FightIdle 空转 4 秒--> Patrol。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlayerHfsmComponent : ActorComponent
    {
        [Header("Patrol")]
        [SerializeField] private float _moveSpeed = 5f;

        [Header("Fight")]
        [Tooltip("战斗待机多久没有任何动作就退回 Patrol（状态图上的 NoAction,4s）")]
        [SerializeField] private float _noActionSeconds = 4f;

        [Tooltip("调试用：动作播这么久后自动抛 ActionEnd。<= 0 表示只认外部 NotifyActionEnd()")]
        [SerializeField] private float _debugAutoEndSeconds = 0.5f;

        [Header("Debug")]
        [SerializeField] private bool _logStateChange;

        private StateMachine<PlayerState, PlayerFsmEvent> _fsm;
        private float _deltaTime;
        private string _lastLoggedPath;

        public override int TickOrder => ActorTickOrder.Fsm;

        /// <summary>当前活动状态的层次路径，例如 "/Fight/Skill1"。</summary>
        public string ActiveHierarchyPath => IsRunning ? _fsm.GetActiveHierarchyPath() : string.Empty;

        public PlayerState ActiveRootState => IsRunning ? _fsm.ActiveStateName : PlayerState.Patrol;

        private bool IsRunning => _fsm != null && _fsm.IsInitialized;

        #region ActorComponent

        protected override void OnBind()
        {
            BuildFsm();
            _fsm.Init();
        }

        public override void Tick(float dt)
        {
            if (!IsRunning) return;

            _deltaTime = dt;
            _fsm.OnLogic();

            if (_logStateChange) LogPathIfChanged();
        }

        #endregion

        #region 对外事件入口

        /// <summary>把外部（输入层 / 动画事件 / GAS）的信号翻译成状态机事件。</summary>
        public void Trigger(PlayerFsmEvent evt)
        {
            if (!IsRunning) return;
            _fsm.Trigger(evt);
        }

        public void OnMoveKeyDown() => Trigger(PlayerFsmEvent.AnyMoveKey);

        public void OnMoveKeyUp() => Trigger(PlayerFsmEvent.NoMoveKey);

        /// <summary>
        /// 战斗键统一入口：先保证从 Patrol 进 Fight，再把具体动作事件下发给 Fight 子机。
        /// 这样同一次按键既能切进战斗，也能直接起手；已经在 Fight 里时第一条事件会被忽略。
        /// </summary>
        public void OnFightKey(PlayerFsmEvent actionEvent)
        {
            Trigger(PlayerFsmEvent.AnyFightKey);
            Trigger(actionEvent);
        }

        public void OnLightAttack() => OnFightKey(PlayerFsmEvent.LightAttack);

        public void OnSkill1() => OnFightKey(PlayerFsmEvent.Skill1);

        public void OnSkill2() => OnFightKey(PlayerFsmEvent.Skill2);

        public void OnSkill3() => OnFightKey(PlayerFsmEvent.Skill3);

        /// <summary>动作（攻击 / 技能）播放结束，对应状态图上的 End。</summary>
        public void NotifyActionEnd() => Trigger(PlayerFsmEvent.ActionEnd);

        #endregion

        #region 状态机搭建

        private void BuildFsm()
        {
            _fsm = new StateMachine<PlayerState, PlayerFsmEvent>();
            
            _fsm.AddState(PlayerState.Patrol, BuildPatrolFsm());
            _fsm.AddState(PlayerState.Fight, BuildFightFsm());
            _fsm.SetStartState(PlayerState.Patrol);

            _fsm.AddTriggerTransition(PlayerFsmEvent.AnyFightKey, PlayerState.Patrol, PlayerState.Fight);

            // Fight → Patrol 不带条件：Fight 是 needsExitTime 的子状态机，
            // "4 秒没动作" 的判定放在它内部的退出转移上，动作没打完不会被拽走。
            _fsm.AddTransition(PlayerState.Fight, PlayerState.Patrol);
        }

        private StateMachine<PlayerState, PatrolState, PlayerFsmEvent> BuildPatrolFsm()
        {
            // 巡逻态不需要退出时间，AnyFightKey 可以随时打断。
            var patrol = new StateMachine<PlayerState, PatrolState, PlayerFsmEvent>();

            patrol.AddState(PatrolState.Idle);
            patrol.AddState(PatrolState.Move, onLogic: _ => MoveEightWay());
            patrol.SetStartState(PatrolState.Idle);

            patrol.AddTriggerTransition(PlayerFsmEvent.AnyMoveKey, PatrolState.Idle, PatrolState.Move);
            patrol.AddTriggerTransition(PlayerFsmEvent.NoMoveKey, PatrolState.Move, PatrolState.Idle);

            return patrol;
        }

        private StateMachine<PlayerState, FightState, PlayerFsmEvent> BuildFightFsm()
        {
            var fight = new StateMachine<PlayerState, FightState, PlayerFsmEvent>(needsExitTime: true);

            fight.AddState(FightState.FightIdle);
            fight.AddState(FightState.Attack, onLogic: AutoEndAction);
            fight.AddState(FightState.Skill1, onLogic: AutoEndAction);
            fight.AddState(FightState.Skill2, onLogic: AutoEndAction);
            fight.AddState(FightState.Skill3, onLogic: AutoEndAction);
            fight.SetStartState(FightState.FightIdle);

            // 退出转移优先级最高，先加：只有在 FightIdle 空转满 _noActionSeconds 秒时才放行父状态机。
            // TransitionAfter 的计时在每次进入 FightIdle 时重置，所以一套技能打完会重新数 4 秒。
            fight.AddExitTransition(
                new TransitionAfter<FightState>(FightState.FightIdle, default, _noActionSeconds));

            fight.AddTriggerTransition(PlayerFsmEvent.LightAttack, FightState.FightIdle, FightState.Attack);
            fight.AddTriggerTransition(PlayerFsmEvent.Skill1, FightState.FightIdle, FightState.Skill1);
            fight.AddTriggerTransition(PlayerFsmEvent.Skill2, FightState.FightIdle, FightState.Skill2);
            fight.AddTriggerTransition(PlayerFsmEvent.Skill3, FightState.FightIdle, FightState.Skill3);

            // End：四个动作各自回 FightIdle。动作态之间不互通，必须先 End。
            fight.AddTriggerTransition(PlayerFsmEvent.ActionEnd, FightState.Attack, FightState.FightIdle);
            fight.AddTriggerTransition(PlayerFsmEvent.ActionEnd, FightState.Skill1, FightState.FightIdle);
            fight.AddTriggerTransition(PlayerFsmEvent.ActionEnd, FightState.Skill2, FightState.FightIdle);
            fight.AddTriggerTransition(PlayerFsmEvent.ActionEnd, FightState.Skill3, FightState.FightIdle);

            return fight;
        }

        #endregion

        #region 状态逻辑

        private void MoveEightWay()
        {
            if (Root == null || Root.intentSource == null) return;

            Vector2 raw = Root.intentSource.Read().Move;
            if (raw.sqrMagnitude < 0.0001f) return;

            Vector2 dir = SnapToEightDirections(raw);
            transform.position += new Vector3(dir.x, 0f, dir.y) * (_moveSpeed * _deltaTime);
        }

        private static Vector2 SnapToEightDirections(Vector2 raw)
        {
            float degrees = Mathf.Round(Mathf.Atan2(raw.y, raw.x) * Mathf.Rad2Deg / 45f) * 45f;
            float radians = degrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        }

        private void AutoEndAction(State<FightState, PlayerFsmEvent> state)
        {
            if (_debugAutoEndSeconds <= 0f) return;
            if (state.timer.Elapsed < _debugAutoEndSeconds) return;

            NotifyActionEnd();
        }

        #endregion

        private void LogPathIfChanged()
        {
            string path = _fsm.GetActiveHierarchyPath();
            if (path == _lastLoggedPath) return;

            _lastLoggedPath = path;
            Debug.Log($"[PlayerHfsm] {path}", this);
        }
    }
}
