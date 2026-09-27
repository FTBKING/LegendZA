using GameScripts.HotFix.GameBase.CommandSystem;
using GameScripts.HotFix.GameLogic.CommandSystemImpl;
using Narakacomponent;

namespace GameScripts.HotFix.GameLogic.CommandSystemImpl.具体系统
{
    public class StateCommandManager : ActorComponent, ICommandSystem
    {
        public AbstractCommandInvoker _invoker;

        public override int TickOrder => ActorTickOrder.Command;

        protected override void OnBind()
        {
            Init();
        }

        public override void Tick(float dt)
        {
        }

        private void Init()
        {
            if (_invoker == null)
            {
                _invoker = new StateCommandInvoker();
            }
        }

        public void ExecuteCommand(ICommand command)
        {
            _invoker.ExecuteCommand(command);
        }

        public void Undo()
        {
            _invoker.Undo();
        }
    }
}
