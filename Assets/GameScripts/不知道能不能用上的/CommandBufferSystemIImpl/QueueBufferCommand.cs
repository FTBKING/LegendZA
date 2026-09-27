using System.Collections.Generic;
using UnityEngine;

namespace FSMBrainandState
{
    public class QueueBufferCommand:输入缓冲队列接口
    {
        //注意
        protected struct TimedCommand
        {
            public CommandType CommandType;
            public float ExpirationTime;
        }

        protected PlayerStateMachineManager StateMachineManager;
        protected Queue<TimedCommand> commands = new Queue<TimedCommand>();

        #region 公共构造函数

        public QueueBufferCommand(PlayerStateMachineManager mStateMachineManager)
        {
            StateMachineManager = mStateMachineManager;
        }

        #endregion

        #region 外部可调用

        public void bufferCommand(CommandType m_commandType, float m_EndTime)
        { 
            commands.Enqueue(new TimedCommand
            {
                CommandType = m_commandType,
                ExpirationTime = Time.time + m_EndTime
            });
        }

        public void UpdateAction()
        {
            //这里每帧只执行1次，为了保持性能
            while (commands.Count > 0)
            {
                TimedCommand current = commands.Peek();
                if (Time.time > current.ExpirationTime)
                {
                    ClearCommands();
                    continue;
                }

                if (StateMachineManager.TryGetTargetState(current.CommandType))
                {
                    ClearCommands();
                }

                return;
            }
        }

        #endregion

        #region 私有内部方法

        private void ClearCommands()
        {
            commands.Dequeue();
        }

        #endregion
    }
}