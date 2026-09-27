namespace FSMBrainandState
{
    // 玩家可以发出的指令类型
    public enum CommandType
    {
        MoveStart, MoveCancel, LowJump, HighJump, SprintStart, SprintCancel, MPress,Mup, Landed, InAir,
        // 战斗指令：普通攻击 / 1 技能(冰锥)
        BasicAttack, Skill1
    }
}
