namespace GameScripts.HotFix.GameBase.元素状态系统
{
    public abstract class 抽象元素形态
    {
        //持有攻击形态的引用
        protected 抽象攻击形态 当前形态;
        //暴露方法提供外部灵活设置
        public void 设置形态(抽象攻击形态 m_形态)
        {
            当前形态 = m_形态;
        }
        //所有形态共有的接口
        public abstract void BaseAttk(抽象元素上下文 元素上下文);
        public abstract void Sk1(抽象元素上下文 元素上下文);
        public abstract void Sk2(抽象元素上下文 元素上下文);
        public abstract void BigSkill(抽象元素上下文 元素上下文);

    }
}