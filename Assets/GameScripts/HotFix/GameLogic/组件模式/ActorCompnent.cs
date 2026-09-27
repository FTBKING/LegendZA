using System.Dynamic;
using UnityEngine;

namespace Narakacomponent
{
    //Actor组件的执行顺序
    //低的优先级高
    public static class ActorTickOrder
    {
        public const int Input = 50;         // 输入最先，供后续组件消费意图
        public const int Command = 150;      // 命令系统执行输入映射出的指令
        public const int Fsm = 175;          // 状态机驱动逻辑
        public const int Element = 600;      // 元素形态结算
        public const int Presentation = 900; // 表现永远最后，保证读到的是本帧结果
    }

    public interface IActorComponent
    {
        int TickOrder { get; }
        //所有组件的基类
        void Bind(ActorRoot root);
        void Tick(float dt);
    }

    public abstract class ActorComponent : MonoBehaviour, IActorComponent
    {
        protected ActorRoot Root { get; private set; }
        protected ActorBlackBoard Board => Root.Board;
        protected ActorEventBus Events => Root.Events;
        public abstract int TickOrder { get; }
        public void Bind(ActorRoot root)
        {
            Root = root;
            OnBind();
        }

        protected virtual void OnBind()
        {
        }

        public abstract void Tick(float dt);
    }
    
}