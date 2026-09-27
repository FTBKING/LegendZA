using UnityEngine;
namespace Narakacomponent
{
 
    //负责输入的东西的对外契约
    public struct ActorIntent
    {
        public Vector2 Move;      // 世界空间水平方向 (x, z)，长度 0~1
        public bool Jump;
        public bool LightAttack;
        public bool Parry;        // 振刀
        public bool Grapple;      // 飞索
        public bool Ability;      // 英雄技能
        public static readonly ActorIntent None = default;
    }
    //IIntentSource接口，怎么实现的？
    //读取用户或AI行为，返回ActorIntent结构体
    public interface IIntentSource
    {
        ActorIntent Read();
    }
    public sealed class PlayerIntentSource : MonoBehaviour, IIntentSource
    {
        public ActorIntent Read()
        {
            return default;
        }
    }
    
}