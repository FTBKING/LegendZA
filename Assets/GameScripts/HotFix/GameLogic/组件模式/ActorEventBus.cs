using System;
using UnityEngine;

namespace Narakacomponent
{
    //Actor触发事件的枚举
    public enum ActorSignal
    {
    }

    public struct ActorEvent
    {
        //枚举
        public ActorSignal Signal;
        //数量
        public float Amount;
        //Instigator是?
        //发起人
        public GameObject Instigator;

        public ActorEvent(ActorSignal signal, float amount, GameObject instigator)
        {
            Signal = signal;
            Amount = amount;
            Instigator = instigator;
        }
    }
    //广播事件总线
    public class ActorEventBus : MonoBehaviour
    {
        //待状态数据的订阅者函数
        public event Action<ActorEvent> Received;
        public void Send(ActorSignal signal, float amount = 0f, GameObject instigator = null)
        {
            Received?.Invoke(new ActorEvent(signal, amount, instigator));
        }
    }
}