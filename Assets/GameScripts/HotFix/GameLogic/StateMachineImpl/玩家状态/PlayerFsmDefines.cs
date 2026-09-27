namespace GameScripts.HotFix.GameLogic.PlayerHfsm
{
    /// <summary>
    /// 根状态机的状态。
    /// </summary>
    public enum PlayerState
    {
        Patrol,
        Fight
    }

    /// <summary>
    /// Patrol 子状态机的状态。
    /// </summary>
    public enum PatrolState
    {
        Idle,
        Move
    }

    /// <summary>
    /// Fight 子状态机的状态。
    /// </summary>
    public enum FightState
    {
        FightIdle,
        Attack,
        Skill1,
        Skill2,
        Skill3
    }

    /// <summary>
    /// 整棵层次共用的事件类型（UnityHFSM 的 TEvent）。
    /// 状态图上的每一条触发条件都对应这里的一个值，由输入层 / 动画事件抛给状态机。
    /// </summary>
    public enum PlayerFsmEvent
    {
        /// <summary>任意移动键按下：Idle → Move。</summary>
        AnyMoveKey,

        /// <summary>移动键全部松开：Move → Idle。</summary>
        NoMoveKey,

        /// <summary>任意战斗键按下：Patrol → Fight。</summary>
        AnyFightKey,

        /// <summary>左键单击：FightIdle → Attack。</summary>
        LightAttack,

        /// <summary>Q：FightIdle → Skill1。</summary>
        Skill1,

        /// <summary>E：FightIdle → Skill2。</summary>
        Skill2,

        /// <summary>R：FightIdle → Skill3。</summary>
        Skill3,

        /// <summary>End：攻击 / 技能动作结束，回 FightIdle。</summary>
        ActionEnd
    }
}
