namespace FSMBrainandState
{
    public interface ICommandBufferSystem
    {
        public void BufferCommand(CommandType command, float cleartime);
    }
}