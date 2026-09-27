using System;
using FSMBrainandState;
using FSMBrainandState.输入映射到行为的实体类;
using GameScripts.HotFix.GameBase.CommandSystem;
using Narakacomponent;
using UnityEngine;

namespace FSMBrainandState
{
    public class InputSystemManager : ActorComponent, IInputSystem
    {
        public PLcon PLinput;

        public override int TickOrder => ActorTickOrder.Input;

        #region Unity

        private void Awake()
        {
            Init();
        }

        private void OnEnable()
        {
            PLinput?.Enable();
        }

        private void OnDisable()
        {
            PLinput?.Disable();
        }

        #endregion

        #region ActorComponent

        protected override void OnBind()
        {
        }

        public override void Tick(float dt)
        {
        }

        #endregion

        #region a

        private IModifyInput _InputStrategy;

        public void SetMoveStrategy(IModifyInput m_InputStrategy)
        {
            _InputStrategy = m_InputStrategy;
        }

        public void MappingKey(ActionEntity actionEntity)
        {
            if (_InputStrategy == null) throw new Exception("������������Բ���ӳ�䰴��");
            _InputStrategy.MappingKey(actionEntity, this);
        }

        public void DeMappingKey(ActionEntity actionEntity)
        {
            if (_InputStrategy == null) throw new Exception("������������Բ���ȡ��ӳ�䰴��");
            _InputStrategy.UnMappingKey(actionEntity, this);
        }

        #endregion

        private void Init()
        {
            PLinput = new PLcon();
        }
    }
}
