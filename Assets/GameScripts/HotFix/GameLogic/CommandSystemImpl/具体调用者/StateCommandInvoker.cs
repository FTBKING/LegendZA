using System.Collections.Generic;
using GameScripts.HotFix.GameBase.CommandSystem;
using UnityEngine;

namespace GameScripts.HotFix.GameLogic.CommandSystemImpl
{
    public class StateCommandInvoker:AbstractCommandInvoker
    {
        //撤销一般是撤销最近的一次，用List，可以处理栈暴的问题，超过多少条直接删除
        private readonly LinkedList<ICommand> _history = new LinkedList<ICommand>();
        //执行命令
        public override void ExecuteCommand(ICommand command)
        {
            //1.调用
            command.调用();
            //2.入栈前先检查是否到最大值(20个),到达则移除最后一个
            if (_history.Count >= 20)
            {
                _history.RemoveLast();
            }
            //3.入栈记录
            _history.AddFirst(command);
        }
        //撤销命令
        public override void Undo()
        {
            //先判断是否有可撤销的操作
            if (_history.Count == 0)
            {
                Debug.Log("无可撤销的操作");
                return;
            }
            //拿出最近的一次命令
            ICommand last = _history.First.Value;
            //删除最近一次命令
            _history.RemoveFirst();
            //执行此命令的撤销操作
            last.撤回();
        }
        
    }
}