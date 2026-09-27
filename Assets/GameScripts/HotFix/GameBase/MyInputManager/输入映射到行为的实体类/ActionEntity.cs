using System;
using UnityEngine.InputSystem;

namespace FSMBrainandState.输入映射到行为的实体类
{
    public struct ActionEntity
    {
        //缓存回调委托：保证 MappingKey 的 += 与 UnMappingKey 的 -= 是同一个引用，否则无法正确解绑
        public Action<InputAction.CallbackContext> _onBigSkill;
        public Action<InputAction.CallbackContext> _onNormalAttack;
        public Action<InputAction.CallbackContext> _onSkill1;
        public Action<InputAction.CallbackContext> _onSkill2;
    }
}