using System;
using FSMBrainandState.输入映射到行为的实体类;
using GameScripts.HotFix.GameBase.CommandSystem;
using UnityEngine.InputSystem;

namespace FSMBrainandState
{
    public interface IInputSystem
    {
        public void SetMoveStrategy(IModifyInput m_InputStrategy);
        public void MappingKey(ActionEntity actionEntity);
        public void DeMappingKey(ActionEntity actionEntity);
    }
}