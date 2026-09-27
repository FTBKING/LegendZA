using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSMBrainandState
{
    public abstract class StateMachine : IMyStateMachine
    {
        
        public BindableProperty<IState> CurrentState = new BindableProperty<IState>();

        #region Unity事件函数
        //帧更新  
        public void Update()
        {
            UpdateState();
        }
        //物理逻辑更新
        public void FixedUpdate()
        {
            PhysicsUpdateState();
        }
        //虚函数帧更新
        public virtual void UpdateState()
        {
            CurrentState.Value?.HandleInput();
            CurrentState.Value?.Update();
        }
        //虚函数物理逻辑更新
        public virtual void PhysicsUpdateState()
        {
            CurrentState.Value?.PhysicsUpdate();
        }
        #endregion

        #region 接口实现

        public virtual void ChangeState(IState newState)
        {
            CurrentState.Value?.Exit();
            CurrentState.Value = newState;
            CurrentState.Value.Enter();
        }
        //虚函数接口
        public abstract bool TryGetTargetState(CommandType? command);
        //开始事件(进入前摇)
        public virtual void AnimTrigger前摇()
        {
            CurrentState.Value.CurrentInnerState = IState.InnerState.前摇;
        }
        //进行事件(进行中)
        public virtual void AnimTrigger进行()
        {
            CurrentState.Value.CurrentInnerState = IState.InnerState.进行中;
        }
        //后摇事件(进入后摇)
        public virtual void AnimTrigger后摇()
        {
            CurrentState.Value.CurrentInnerState = IState.InnerState.后摇;
        }
        //结束事件(最后一帧)
        public virtual void AnimTrigger结束()
        {
            CurrentState.Value.CurrentInnerState = IState.InnerState.结束;
        }
#endregion

    }
}
