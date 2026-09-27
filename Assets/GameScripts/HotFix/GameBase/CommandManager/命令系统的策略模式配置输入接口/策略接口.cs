using FSMBrainandState;
using FSMBrainandState.输入映射到行为的实体类;

namespace GameScripts.HotFix.GameBase.CommandSystem
{
    public interface IModifyInput
    {
        void MappingKey(ActionEntity actionEntity,InputSystemManager inputSystemManager);
        void UnMappingKey(ActionEntity actionEntity,InputSystemManager inputSystemManager);
    }
}