using System;
using FSMBrainandState;
using FSMBrainandState.输入映射到行为的实体类;
using GameScripts.HotFix.GameBase.CommandSystem;
using GameScripts.HotFix.GameLogic.CommandSystemImpl.具体系统;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

namespace GameScripts.HotFix.GameLogic.CommandSystemImpl
{
    public class CommandInput : IModifyInput
    {
        //Unity的对象池
        private ObjectPool<BigSkill> _bigskillpool;
        private ObjectPool<NormalAttack> _normalAttackpool;
        private ObjectPool<Skill1> _skill1pool;
        private ObjectPool<Skill2> _skill2pool;
        //命令系统接口
        private ICommandSystem _commandSystem;
        public CommandInput()
        {
            //等注册中心完成后获取
            InitSkillPool();
            SubscribeInputFunc();
        }

        private void SubscribeInputFunc()
        {
            _bigskillpool.Get(out BigSkill _bigskill);
            _normalAttackpool.Get(out NormalAttack _normalAttack);
            _skill1pool.Get(out Skill1 _skill1);
            _skill2pool.Get(out Skill2 _skill2);
        }

        private void InitSkillPool()
        {
            //初始化各种对象池
            _bigskillpool = new ObjectPool<BigSkill>(
                //第一个参数，Func<T>,只有返回值
                //对应我学的，返回在CreateNew里创建obj
                createFunc: () => new BigSkill(),
                actionOnGet: skill => skill.清空状态(),
                actionOnRelease: skill => skill.清空状态(),
                //检查同一对象是否被还2次
                collectionCheck: true, 
                defaultCapacity: 20,
                maxSize: 100
            );
            _normalAttackpool = new ObjectPool<NormalAttack>(
                createFunc: () => new NormalAttack(),
                actionOnGet: skill => skill.清空状态(),
                actionOnRelease: skill => skill.清空状态(),
                collectionCheck: true, 
                defaultCapacity: 20,
                maxSize: 100
            );
            _skill1pool = new ObjectPool<Skill1>(
                createFunc: () => new Skill1(),
                actionOnGet: skill => skill.清空状态(),
                
                collectionCheck: true, 
                defaultCapacity: 20,
                maxSize: 100
            );
            _skill2pool = new ObjectPool<Skill2>(
                createFunc: () => new Skill2(),
                actionOnGet: skill => skill.清空状态(),
                actionOnRelease: skill => skill.清空状态(),
                collectionCheck: true, 
                defaultCapacity: 20,
                maxSize: 100
            );
        }

        public void MappingKey(ActionEntity actionEntity,InputSystemManager _inputSystemManager)
        {
            //输入系统的表现+键位已经做好了，随时可配置，只需要映射对应的命令即可
            _inputSystemManager.PLinput.Player.BigSkill.performed += actionEntity._onBigSkill;
            _inputSystemManager.PLinput.Player.NormalAttk.performed += actionEntity._onNormalAttack;
            _inputSystemManager.PLinput.Player.Skill1.performed += actionEntity._onSkill1;
            _inputSystemManager.PLinput.Player.Skill2.performed += actionEntity._onSkill2;
        }

        public void UnMappingKey(ActionEntity actionEntity,InputSystemManager _inputSystemManager)
        {
            //解绑时必须用与注册时相同的委托引用，否则减不掉
            _inputSystemManager.PLinput.Player.BigSkill.performed -= actionEntity._onBigSkill;
            _inputSystemManager.PLinput.Player.NormalAttk.performed -= actionEntity._onNormalAttack;
            _inputSystemManager.PLinput.Player.Skill1.performed -= actionEntity._onSkill1;
            _inputSystemManager.PLinput.Player.Skill2.performed -= actionEntity._onSkill2;
        }

        #region 命令的具体行为

        private void BigSkill(InputAction.CallbackContext context)
        {   
            using(_bigskillpool.Get(out BigSkill bigSkill)) // 当离开这个作用域时，bigSkill 会被归还到对象池中。
            {
                // 在这里使用 bigSkill
                _commandSystem.ExecuteCommand(bigSkill);
            }
        }

        private void NormalAttack(InputAction.CallbackContext context)
        {
            using(_normalAttackpool.Get(out NormalAttack normalAttack)) // 当离开这个作用域时，normalAttack 会被归还到对象池中。
            {
                // 在这里使用 normalAttack
                _commandSystem.ExecuteCommand(normalAttack);
            }
        }

        private void Skill1(InputAction.CallbackContext context)
        {
            using(_skill1pool.Get(out Skill1 skill1)) // 当离开这个作用域时，skill1 会被归还到对象池中。
            {
                // 在这里使用 skill1
                _commandSystem.ExecuteCommand(skill1);
            }
        }

        private void Skill2(InputAction.CallbackContext context)
        {
            using(_skill2pool.Get(out Skill2 skill2)) // 当离开这个作用域时，skill2 会被归还到对象池中。
            {
                // 在这里使用 skill2
                _commandSystem.ExecuteCommand(skill2);
            }
        }

        #endregion
    }
}
