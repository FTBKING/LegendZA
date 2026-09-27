using GameScripts.HotFix.GameBase.元素状态系统;
using UnityEngine;

namespace GameScripts.HotFix.GameLogic.元素状态实现.元素形态
{
    //具体形态：光形态
    public class 光形态 : 抽象元素形态
    {
        public override void BaseAttk(抽象元素上下文 元素上下文)
        {
            //光刃和远程的浦东攻击
            //传递给远战/近战处理
            当前形态.BaseAttk(元素上下文);
        }

        public override void Sk1(抽象元素上下文 元素上下文)
        {
            当前形态.Sk1(元素上下文);
        }

        public override void Sk2(抽象元素上下文 元素上下文)
        {
            当前形态.Sk2(元素上下文);
        }

        public override void BigSkill(抽象元素上下文 元素上下文)
        {
            当前形态.BigSkill(元素上下文);
        }
    }
}