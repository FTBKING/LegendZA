using System;

namespace FSMBrainandState
{
    // 单条转换规则：命中后切换到的目标状态 + 内状态要求。
    // 输入命令(CommandType)已作为索引的二级字典键，不再需要在规则内重复存储。
    // TargetState 使用 IState 子类的类名（反射 type.Name）。
    [Serializable]
    public struct TransitionRule
    {
        public string TargetState;
        public IState.InnerState RequiredInner;
    }
}
