using FSMBrainandState;
using GameScripts.HotFix.GameLogic.CommandSystemImpl.具体系统;
using GameScripts.HotFix.GameLogic.PlayerHfsm;
using GameScripts.HotFix.GameLogic.元素状态实现.系统实现;
using UnityEngine;

namespace Narakacomponent
{
    /// <summary>
    /// 原文 createBjorn() 的等价物。同一个容器，装不同的零件，出不同的实体。
    /// 生产项目里请用 Prefab 变体代替它——Prefab 本质就是序列化的组件清单。
    /// </summary>
    public static class ActorFactory
    {
        /// <summary>
        /// 创建玩家 Actor：挂上黑板、事件总线、意图源、全部 Manager、FSM Owner，最后挂 ActorRoot。
        /// </summary>
        public static ActorRoot CreatePlayer(string name = "Player", Vector3 position = default)
        {
            var go = new GameObject(name);
            go.transform.position = position;

            // 共享基础设施
            go.AddComponent<ActorBlackBoard>();
            go.AddComponent<ActorEventBus>();
            go.AddComponent<PlayerIntentSource>();

            // 全部 ActorComponent（含 FSM Owner）
            go.AddComponent<InputSystemManager>();
            go.AddComponent<StateCommandManager>();
            go.AddComponent<元素系统实现Manager>();
            go.AddComponent<AnimancerManager>();
            go.AddComponent<PlayerHfsmComponent>();
            // ActorRoot 最后添加：Awake 里收集、排序、Bind；Owner.OnBind 内启动 FSM
            return go.AddComponent<ActorRoot>();
        }
    }
}
