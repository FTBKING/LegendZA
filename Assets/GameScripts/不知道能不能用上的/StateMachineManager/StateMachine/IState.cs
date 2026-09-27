using System;
using Newtonsoft.Json;
using UnityEngine;
namespace FSMBrainandState
{
    public class IState
    {
        #region 共有数据成员

        public enum InnerState
        {
            无,前摇,进行中,后摇,结束
        }
        // 运行时瞬时数据：JSON 不接管，避免覆盖动画事件设置的值
        [JsonIgnore]
        public InnerState CurrentInnerState = InnerState.无;

        #endregion
        public virtual void Enter(){}
        public virtual void Exit(){}
        public virtual void HandleInput(){}
        public virtual void Update(){}
        public virtual void PhysicsUpdate(){}
    }
}
