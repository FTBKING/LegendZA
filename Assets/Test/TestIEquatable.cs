using System;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
// ===== 精简版：模仿 TEngine 的 TypeNamePair =====
    readonly struct TypeNamePair : IEquatable<TypeNamePair>
    {
        public Type Type { get; }
        public string Name { get; }

        public TypeNamePair(Type type, string name = "")
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Name = name ?? "";
        }

        // Dictionary 查找依赖这两样
        public bool Equals(TypeNamePair other)
            => Type == other.Type && Name == other.Name;

        public override bool Equals(object obj)
            => obj is TypeNamePair other && Equals(other);

        public override int GetHashCode()
            => Type.GetHashCode() ^ Name.GetHashCode();

        public override string ToString()
            => string.IsNullOrEmpty(Name) ? Type.Name : $"{Type.Name}.{Name}";
    }

    class Player { }
    class Enemy { }

    public class TestIEquatable:MonoBehaviour
    {
        void Start()
        {
            // 模拟 FsmModule._fsmMap
            var fsmMap = new Dictionary<TypeNamePair, string>();

            // 创建：同一 Owner 类型，不同名字 = 两台 FSM
            fsmMap.Add(new TypeNamePair(typeof(Player)), "Player默认状态机");
            fsmMap.Add(new TypeNamePair(typeof(Player), "Combat"), "Player战斗状态机");
            fsmMap.Add(new TypeNamePair(typeof(Enemy)), "Enemy默认状态机");

            // 查询：必须 new 一个“内容相同”的键 —— IEquatable 让它命中
            var key1 = new TypeNamePair(typeof(Player));           // 新实例！
            var key2 = new TypeNamePair(typeof(Player), "Combat"); // 新实例！

            Debug.Log(key1.Equals(new TypeNamePair(typeof(Player)))); // True
            Debug.Log(fsmMap.ContainsKey(key1)); // True —— 靠 IEquatable
            Debug.Log(fsmMap[key1]);             // Player默认状态机
            Debug.Log(fsmMap[key2]);             // Player战斗状态机

            // 没有 IEquatable / 错误的 Equals 时，Dictionary 可能找不到“内容相同但实例不同”的键
            // （struct 默认 Equals 会比较字段，但 IEquatable 避免装箱，且和 GetHashCode 配套是 Dictionary 正确性的关键）

            Debug.Log($"共 {fsmMap.Count} 台 FSM");
        }
    }
}