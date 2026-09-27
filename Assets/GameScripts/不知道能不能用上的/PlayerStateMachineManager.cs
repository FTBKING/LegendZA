using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace FSMBrainandState
{
    public class PlayerStateMachineManager : StateMachine
    {
        //----------------------------------------------------------------------------
        #region 映射字符串和状态的双向字典

        private string CurrentStateID;

        // 内部仍是双向字典：string -> IState 与 IState -> string 同时存在
        private BiDictionary<string, IState> stateMap;

        #endregion

        #region 索引类对象

        private readonly TransitionRuleIndex _transitionRuleIndex = new();

        #endregion

        //----------------------------------------------------------------------------

        private void Awake()
        {
            //初始化规则数据与状态
            InitStateAndRules();
        }

        #region 重写虚函数实现玩家状态机的功能

        // 核心查询方法：输入当前状态和指令，返回是否可以转换及目标数据
        public override bool TryGetTargetState(CommandType? command)
        {
            if (command == null || CurrentState.Value == null)
            {
                return false;
            }

            // 通过“反向字典”查出当前状态实例对应的字符串 ID（即类名）
            if (!stateMap.TryGetByValue(CurrentState.Value, out var currentStateId))
            {
                return false;
            }

            CurrentStateID = currentStateId;
            // 在二级索引里按 状态名+命令 取候选规则
            if (!_transitionRuleIndex.TryGetCandidates(currentStateId, command.Value, out var candidates))
            {
                return false;
            }

            foreach (TransitionRule rule in candidates)
            {
                if (rule.RequiredInner == CurrentState.Value.CurrentInnerState)
                {
                    if (!stateMap.TryGetByKey(rule.TargetState, out IState targetState))
                    {
                        // JSON 里写了字符串，但反射阶段没注册到这个类（拼写错了/类不存在）
                        return false;
                    }

                    ChangeState(targetState);
                    return true;
                }
            }

            return false;
        }


        // 切换状态时同步维护字符串形式的当前状态 ID
        public override void ChangeState(IState newState)
        {
            if (newState == null)
            {
                return;
            }

            var previousStateId = CurrentStateID;
            CurrentState.Value?.Exit();
            CurrentState.Value = newState;
            if (stateMap.TryGetByValue(CurrentState.Value, out var currentStateId))
            {
                this.CurrentStateID = currentStateId;
            }

            CurrentState.Value.Enter();
        }

        public override void UpdateState()
        {
            base.UpdateState();
        }

        public override void PhysicsUpdateState()
        {
            base.PhysicsUpdateState();
        }

        #endregion

        #region 一些长函数的拆分

        //初始化状态和规则
        private void InitStateAndRules()
        {
            //转换成字符串
            // var rulesText = rulesJson != null ? rulesJson.text : null;
            // var stateDataText = stateDataJson != null ? stateDataJson.text : null;
            //初始化stateMap
            stateMap = new();
            // 注册状态并应用状态配置。
            RegisterAllStatesByReflection(stateMap);
            // ApplyStateData(rulesText, stateMap);
            // // 3. 转换规则 JSON 本身就是索引结构，直接反序列化加载
            // _transitionRuleIndex.LoadFromJson(stateDataText);
            //注册自己
        }

        #endregion

        #region 反射注册方法

        /// <summary>
        /// 反射扫描当前程序集中所有非抽象的 IState 子类，要求其拥有
        /// (PlayerStateMachine) 构造函数；以 type.Name 作为字符串键注册到 stateMap。
        /// </summary>
        public void RegisterAllStatesByReflection(BiDictionary<string, IState> stateMap)
        {
            var stateTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsAbstract
                            && typeof(IState).IsAssignableFrom(t)
                            && t != typeof(IState));

            foreach (var type in stateTypes)
            {
                var ctor = type.GetConstructor(new[] { typeof(PlayerStateMachineManager) });
                if (ctor == null)
                {
                    // 没有 (PlayerStateMachine) 构造函数的子类视为基类/工具类，跳过
                    Debug.LogWarning($"[FSM] 跳过 {type.Name}：缺少 (PlayerStateMachine) 构造函数。");
                    continue;
                }

                var instance = (IState)ctor.Invoke(new object[] { this });
                stateMap.Add(type.Name, instance);
            }
        }

        public void ApplyStateData(string stateDataJson, BiDictionary<string, IState> stateMap)
        {
            if (string.IsNullOrWhiteSpace(stateDataJson)) return;
            //字典无法有相同的键怎么办？
            Dictionary<string, JObject> dataByStateName;
            try
            {
                dataByStateName = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(stateDataJson);
            }
            catch (Exception e)
            {
                Debug.LogError($"[FSM] 状态数据 JSON 解析失败：{e.Message}");
                return;
            }

            if (dataByStateName == null) return;

            // 反序列化时不允许多余字段静默忽略，便于及时发现 JSON 里写错的字段名
            var populateSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error,
            };

            foreach (var pair in dataByStateName)
            {
                if (pair.Value == null) continue;

                if (!stateMap.TryGetByKey(pair.Key, out var instance))
                {
                    Debug.LogWarning($"[FSM] stateData 配了 “{pair.Key}”，但反射阶段没注册到该状态类，已跳过。");
                    continue;
                }

                try
                {
                    JsonConvert.PopulateObject(pair.Value.ToString(Formatting.None), instance, populateSettings);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[FSM] 应用状态 “{pair.Key}” 配置失败：{e.Message}");
                }
            }
        }

        #endregion
    }
}