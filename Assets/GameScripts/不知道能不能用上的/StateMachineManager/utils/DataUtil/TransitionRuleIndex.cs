using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace FSMBrainandState
{
    /// <summary>
    /// 转换规则索引：结构就是 “状态名 -> 命令 -> 候选规则列表”。
    /// JSON 直接写成这个嵌套结构，Newtonsoft 一次反序列化即得到索引，无需再手动 Build。
    /// 一级键是 IState 子类的类名字符串（运行时由反射注册产生）。
    /// </summary>
    public sealed class TransitionRuleIndex
    {
        private Dictionary<string, Dictionary<CommandType, List<TransitionRule>>> _index = new();

        /// <summary>
        /// 直接把 JSON 反序列化成索引字典。
        /// - 二级键 CommandType：Newtonsoft 默认按枚举名（"MoveStart"）解析字典键。
        /// - TransitionRule.RequiredInner（枚举值字段）：需 StringEnumConverter 才能读 "无" 等名字。
        /// </summary>
        public void LoadFromJson(string json)
        {
            // ── 第 1 步：空值保护 ──────────────────────────────────────────────
            // 反序列化前先挡住 “没传文本 / 全是空白” 的情况。
            // IsNullOrWhiteSpace 同时覆盖 null、""、"   \n" 这几种。
            // 直接给 _index 一个空字典，调用方 TryGetCandidates 不会因为 _index 为 null 崩。
            if (string.IsNullOrWhiteSpace(json))
            {
                _index = new Dictionary<string, Dictionary<CommandType, List<TransitionRule>>>();
                return;
            }

            // ── 第 2 步：配置反序列化行为 ───────────────────────────────────────
            // JsonSerializerSettings 是 “反序列化的选项面板”，告诉 Newtonsoft 该怎么解析。
            //
            // Converter（转换器）= 针对某种类型的 “自定义翻译规则”。
            // 默认情况下 Newtonsoft 把枚举当成整数：JSON 里得写 RequiredInner: 0。
            // 加上 StringEnumConverter 后，它就能把字符串 "无" 翻译成枚举 InnerState.无，
            // 让 JSON 用人能读懂的名字而不是数字。
            // 注意：这个转换器只作用于 “枚举出现在值的位置” 的情况（比如 RequiredInner 字段）。
            // 而 Dictionary 的 enum 键（CommandType）本来就按名字解析，不依赖这个转换器。
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());

            // ── 第 3 步：真正的反序列化（可能抛异常，所以包在 try 里）─────────────
            try
            {
                // JsonConvert.DeserializeObject<T>(json, settings)：
                //   把 json 文本 “填” 进类型 T 的结构里。这里的 T 是三层嵌套：
                //   Dictionary<string,                       ← 最外层：状态类名 -> ...
                //       Dictionary<CommandType,              ← 中间层：命令 -> ...
                //           List<TransitionRule>>>           ← 最内层：候选规则列表
                //
                // Newtonsoft 会 “照着 T 的形状递归地解析 JSON”：
                //   JSON 的 { } 对象  → 对应字典的一批键值对（key 是字符串字面量）
                //   JSON 的 [ ] 数组  → 对应 List<TransitionRule>
                //   List 里每个 { }   → 对应一个 TransitionRule 结构体，按字段名一一赋值
                //     "TargetState"   → string 字段，直接拷字符串
                //     "RequiredInner" → 枚举字段，靠上面的 StringEnumConverter 把 "无" 转成枚举
                //
                // 之所以不用再手动 foreach 建索引：JSON 的层级 = 字典的层级，
                // Newtonsoft 一次性就按这个形状把整棵树构造出来了。
                _index = JsonConvert
                    .DeserializeObject<Dictionary<string, Dictionary<CommandType, List<TransitionRule>>>>(json, settings)

                    // ?? 兜底：如果传进来的 json 字面量是 "null"，DeserializeObject 会返回 C# 的 null。
                    // 用空字典接住，保证 _index 永远是可用对象，而不是 null。
                    ?? new Dictionary<string, Dictionary<CommandType, List<TransitionRule>>>();
            }
            catch (System.Exception e)
            {
                // ── 第 4 步：解析失败兜底 ──────────────────────────────────────
                // JSON 写错（括号不配对、字段拼错、枚举名不存在等）时，
                // DeserializeObject 会抛 JsonException。这里捕获后打印可读日志，
                // 并把索引退化成空字典，让游戏 “规则失效但不崩溃”，方便定位问题。
                Debug.LogError($"[FSM] 转换规则 JSON 解析失败：{e.Message}");
                _index = new Dictionary<string, Dictionary<CommandType, List<TransitionRule>>>();
            }
        }

        public bool TryGetCandidates(string state, CommandType command, out List<TransitionRule> candidates)
        {
            candidates = null;
            if (string.IsNullOrEmpty(state)) return false;
            if (!_index.TryGetValue(state, out var byCommand)) return false;
            return byCommand.TryGetValue(command, out candidates) && candidates != null && candidates.Count > 0;
        }
    }
}
