using System.Collections.Generic;

namespace FSMBrainandState
{
    public sealed class BiDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> keyToValue = new();
        private readonly Dictionary<TValue, TKey> valueToKey = new();

        public void Add(TKey key, TValue value)
        {
            if (keyToValue.ContainsKey(key) || valueToKey.ContainsKey(value))
            {
                return;
            }

            keyToValue.Add(key, value);
            valueToKey.Add(value, key);
        }

        public bool TryGetByKey(TKey key, out TValue value)
        {
            return keyToValue.TryGetValue(key, out value);
        }

        public bool TryGetByValue(TValue value, out TKey key)
        {
            return valueToKey.TryGetValue(value, out key);
        }
    }
}