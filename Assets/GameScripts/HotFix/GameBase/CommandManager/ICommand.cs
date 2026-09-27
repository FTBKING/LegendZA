namespace GameScripts.HotFix.GameBase.CommandSystem
{
    public interface ICommand
    {
        void 调用();
        void 撤回();
        void 清空状态();
    }
}