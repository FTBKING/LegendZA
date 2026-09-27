using System;
using System.Collections.Generic;
using FSMBrainandState;
using UnityEngine;

namespace Narakacomponent
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ActorBlackBoard))]
    [RequireComponent(typeof(ActorEventBus))]
    public sealed class ActorRoot : MonoBehaviour
    {
        //持有黑板和事件总线
        public ActorBlackBoard Board { get; private set; }

        public ActorEventBus Events { get; private set; }

        //持有意图源接口
        //调用的时候不知道是什么实现，只管用功能即可
        public IIntentSource intentSource;
        //持有动画机View层接口
        //public IAnimancerManager _animancerManager;
        //持有组件接口
        private IActorComponent[] components;

        //缓冲区，干什么的？
        //初始化组件，排序后放到数组
        //避免Unity的GetComponents<IActorComponent>()每次都会new1个数组，会产生GC
        //直接GetComponents(List<T> results);就可以避免GC
        protected List<IActorComponent> buffer = new List<IActorComponent>(16);

        private void Awake()
        {
            Board = GetComponent<ActorBlackBoard>();
            Events = GetComponent<ActorEventBus>();
            intentSource = GetComponent<IIntentSource>();
            //_animancerManager = GetComponent<IAnimancerManager>();
            buffer.Clear();
            GetComponents(buffer);
            buffer.Sort((a, b) => a.TickOrder.CompareTo(b.TickOrder));
            components = buffer.ToArray();
            //遍历组件，绑定到自己身上
            for (int i = 0; i < components.Length; i++)
                components[i].Bind(this);
        }
        //设置意图源
        public void SetIntentSource(IIntentSource source) => intentSource = source;
        //Update事件函数
        private void Update()
        {
            float dt = Time.deltaTime;
            Board.Advance(dt);
            for(int i = 0; i < components.Length; i++)
                components[i].Tick(dt);
        }
    }
}