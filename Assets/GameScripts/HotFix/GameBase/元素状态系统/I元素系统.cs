using UnityEngine;

namespace GameScripts.HotFix.GameBase.元素状态系统
{
    public interface I元素系统
    {
        //更改元素形态方法
        public void 改变元素形态(抽象元素形态 元素形态);
        //更改元素攻击形态方法
        public void 改变元素攻击形态(抽象攻击形态 攻击形态);
        //所有释放技能的方法
        public void BaseAttk();
        public void Sk1();
        public void Sk2();
        public void BigSkill();
    }
}