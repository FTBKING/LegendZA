namespace FSMBrainandState
{
    public interface 输入缓冲队列接口
    {
        public void bufferCommand(CommandType m_commandType, float m_EndTime);
        public void UpdateAction();
    }
}