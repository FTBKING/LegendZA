using System.Collections.Generic;
using UnityEngine;

namespace GameScripts.HotFix.GameBase.CommandSystem
{
    public abstract class AbstractCommandInvoker
    {
        //撤销一般是撤销最近的一次，用List，可以处理栈暴的问题，超过多少条直接删除
        private readonly LinkedList<ICommand> _history = new LinkedList<ICommand>();
        //执行命令
        public abstract void ExecuteCommand(ICommand command);
        //撤销命令
        public abstract void Undo();

    }
}