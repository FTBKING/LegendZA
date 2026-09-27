using GameScripts.HotFix.GameLogic.元素状态实现.元素形态;
using UnityEngine;

namespace GameScripts.HotFix.GameLogic.WorldEntity
{
    [DisallowMultipleComponent]
    public class WorldEntity : MonoBehaviour, I元素载体
    {
        [SerializeField] private EleType _elementType;

        public EleType 获取元素类型() => _elementType;
    }
}
