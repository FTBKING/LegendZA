using FSMBrainandState;
using UnityEngine;

namespace GameScripts.HotFix.GameBase.CommandSystem
{
    public class BigSkill : ICommand
    {
        public void 调用()
        {
            //接入命令队列系统，对状态机发起命令
            // CommandBufferSystem commandBufferSystem = SystemLocator.Get<CommandBufferSystem>();
            // commandBufferSystem.BufferCommand(CommandType.BasicAttack,0.5f);
            Debug.Log("调用");
        }
        public void 撤回()
        {
            Debug.Log("撤回)");
        }

        public void 清空状态()
        {
            Debug.Log("清空状态)");
        }
    }
}