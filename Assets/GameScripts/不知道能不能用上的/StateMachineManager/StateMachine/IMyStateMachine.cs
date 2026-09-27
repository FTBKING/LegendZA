namespace FSMBrainandState
{
    public interface IMyStateMachine
    {
        public void ChangeState(IState newState);
        public bool TryGetTargetState(CommandType? command);
        public void AnimTrigger前摇();
        //进行事件(进行中)
        public void AnimTrigger进行();
        //后摇事件(进入后摇)
        public void AnimTrigger后摇();
        //结束事件(最后一帧)
        public void AnimTrigger结束();
    }
}