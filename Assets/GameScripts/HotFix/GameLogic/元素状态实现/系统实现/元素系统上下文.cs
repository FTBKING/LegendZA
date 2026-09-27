using GameScripts.HotFix.GameBase.元素状态系统;
using GameScripts.HotFix.GameLogic.元素状态实现.元素形态;

namespace GameScripts.HotFix.GameLogic.元素状态实现.系统实现
{
    public class 元素系统上下文:抽象元素上下文
    {
        //持有当前元素状态
        // public 抽象元素形态 _currentElementState;
        public EleType _type;
    }
}