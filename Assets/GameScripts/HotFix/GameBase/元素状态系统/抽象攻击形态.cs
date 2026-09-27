namespace GameScripts.HotFix.GameBase.元素状态系统
{
    public abstract class 抽象攻击形态
    {
        //持有可更换槽位的技能数组
        private string[] 技能数组;
        //也可以通过方法/通过数据配置灵活切换技能
        public void SetSkillArray(){}
        //所有形态共有的接口
        public abstract void BaseAttk(抽象元素上下文 元素上下文);
        public abstract void Sk1(抽象元素上下文 元素上下文);
        public abstract void Sk2(抽象元素上下文 元素上下文); 
        public abstract void BigSkill(抽象元素上下文 元素上下文);
    }
}