using Animancer;
using Narakacomponent;
using UnityEngine;

namespace FSMBrainandState
{
    [RequireComponent(typeof(AnimancerComponent))]
    public class AnimancerManager : ActorComponent,IAnimancerManager
    {
        public AnimancerComponent _Animancer;

        public override int TickOrder => ActorTickOrder.Presentation;

        protected override void OnBind()
        {
            if (_Animancer == null)
            {
                _Animancer = GetComponentInChildren<AnimancerComponent>();
            }
        }

        public override void Tick(float dt)
        {
        }

        #region Playanimation

        public void PlayAnim(System.Action doaciton, StringReference StringRefname, float Time = 0,
            float normalizedtime = 0, float speed = 1f,bool isLooping = false, FadeMode fademode = FadeMode.FixedSpeed)
        {
            AnimancerState tempState = _Animancer.TryPlay(StringRefname);
            tempState.Time = Time;
            tempState.NormalizedTime = normalizedtime;
            tempState.Speed = speed;
            tempState.Events(this).OnEnd += doaciton;
            if(isLooping) tempState.Events(this).OnEnd += () => tempState.Time = 0;
        }

        #endregion
    }
}
