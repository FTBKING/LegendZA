namespace GameScripts.HotFix.GameBase.CommandSystem
{
    public interface ICommandSystem
    {
        //执行命令
        public void ExecuteCommand(ICommand command);
        //撤销命令
        public void Undo();
    }
}