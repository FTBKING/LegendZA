using Animancer;

namespace FSMBrainandState
{
    public interface IAnimancerManager
    {
        public void PlayAnim(System.Action doaciton, StringReference StringRefname, float Time = 0,
            float normalizedtime = 0, float speed = 1f,bool isLooping = false, FadeMode fademode = FadeMode.FixedSpeed);
    }
}