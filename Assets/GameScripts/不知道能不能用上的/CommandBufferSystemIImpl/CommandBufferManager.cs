using System;
using UnityEngine;

namespace FSMBrainandState
{
    public class CommandBufferManager:MonoBehaviour,ICommandBufferSystem
    {
        private 输入缓冲队列接口 _commandQueue;
        #region Unity事件函数
        private void Awake()
        {
            Init();
        }
        private void Start()
        {
            //初始化命令队列
            
        }
        private void Update()
        {
            _commandQueue.UpdateAction();
        }
        #endregion
        #region 外观接口
        public void BufferCommand(CommandType command,float cleartime)
        {
            _commandQueue.bufferCommand(command,cleartime);
        }
        #endregion
        private void Init()
        {
            //注册自己
        }
    }
}