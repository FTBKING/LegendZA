using System.Collections.Generic;
using UnityEngine;

namespace FSMBrainandState
{
    public class BufferCommand:输入缓冲队列接口
    {
        protected PlayerStateMachineManager StateMachineManager;
        //单存储
        protected CommandType? commandType;
        //改为过期时间
        protected float ExpirationTime = 0f;

        #region 公共构造函数

        public BufferCommand(PlayerStateMachineManager mStateMachineManager)
        {
            StateMachineManager = mStateMachineManager;
        }

        #endregion

        #region 外部可调用

        public void bufferCommand(CommandType m_commandType, float m_EndTime)
        { 
            this.commandType =  m_commandType;
            //初始化时采用当前时间加指定缓冲时间
            this.ExpirationTime = Time.time + m_EndTime;
        }

        public void UpdateAction()
        {
            //这样就可以只判断1次是否过期了
            if (Time.time > ExpirationTime || StateMachineManager.TryGetTargetState(commandType))
            {
                ClearCommands();
                return;
            }
        }

        #endregion

        #region 私有内部方法

        private void ClearCommands()
        {
            this.commandType = null;
            this.ExpirationTime = 0f;
        }

        #endregion
    }
}

