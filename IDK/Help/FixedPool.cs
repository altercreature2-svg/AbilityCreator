using System.Collections.Generic;
using System.Linq;

namespace AC.Help
{
    struct Slot<T>
    {
        public bool isActive;
        public int nextSlotEmpty;
        public T Value;
    }
    
    public class FixedPool<T>
    {
        Slot<T>[] slots;
        private int _free = 0;
        public FixedPool(int size)
        {
            slots = new Slot<T>[size];
            for (int i = 0; i < size - 1; i++)
            {
                slots[i] = new Slot<T>()
                {
                    isActive = false,
                    nextSlotEmpty = i + 1,
                    Value = default
                };
            }
            slots[size - 1] = new Slot<T>()
            {
                isActive = false,
                nextSlotEmpty = 0, // loop (smart)
                Value = default,
            };
        }
        public void Insert(T value)
        {
            slots[_free].Value = value;
            slots[_free].isActive = true;
            _free = slots[_free].nextSlotEmpty;
        }
        public T[] ToArray()
        {
            return slots.Select(n => n.Value).ToArray();
        }
        public T this[int index]
        {
            get
            {
                return slots[index].Value;
            }
        }
        public int Length
        {
            get
            {
                int activeCount = 0;
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].isActive)
                    {
                        activeCount++;
                    }
                }
                return activeCount;
            }
        }
    }
    struct KeyValueSlot<TKey, TValue>
    {
        public bool isActive;
        public int nextSlotEmpty;
        public TKey key;
        public TValue value;
    }
    public class FixedDictionaryPool<TKey,TValue>
    {
        KeyValueSlot<TKey, TValue>[] slots;
        private int _free = 0;
        public FixedDictionaryPool(int size)
        {
            slots = new KeyValueSlot<TKey, TValue>[size];
            for (int i = 0; i < size - 1; i++)
            {
                slots[i] = new KeyValueSlot<TKey, TValue>()
                {
                    isActive = false,
                    nextSlotEmpty = i + 1,
                    key = default,
                    value = default
                };
            }
            slots[size - 1] = new KeyValueSlot<TKey, TValue>()
            {
                isActive = false,
                nextSlotEmpty = 0, // loop (smart)
                key = default,
                value = default
            };
        }
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                value = default;
                return false;
            }
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].isActive && ReferenceEquals(slots[i].key, key))
                {
                    value = slots[i].value;
                    return true;
                }
            }

            value = default;
            return false;
        }
        public void Insert(TKey key, TValue value)
        {
            slots[_free].value = value;
            slots[_free].key = key;
            slots[_free].isActive = true;
            _free = slots[_free].nextSlotEmpty;
        }
        public Dictionary<TKey, TValue> ToDictionary()
        {
            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>(slots.Length);
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[_free].isActive)
                    continue;
                result.Add(slots[i].key, slots[i].value);
            }
            return result;
        }
        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out TValue value))
                {
                    return value;
                }
                throw new KeyNotFoundException($"The given key '{key}' was not present in the pool.");
            }
        }
    }
    
}
