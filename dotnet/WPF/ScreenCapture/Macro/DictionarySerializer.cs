using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
    [Serializable]
    public class DictionarySerializer<T, U> : ISerializable, IEnumerable<KeyValuePair<T, U>>
    {
        [NonSerialized]
        private Dictionary<T, U> _dictionary;
        private Dictionary<T, U> Dictionary
        {
            get
            {
                if (_dictionary is null)
                    _dictionary = new Dictionary<T, U>();
                return _dictionary;
            }
            set => _dictionary = value;
        }

        public U this[T key]
        {
            get => Dictionary[key];
            set => Dictionary[key] = value;
        }

        public int Count
            => Dictionary.Count;

        public bool ContainsKey(T key)
            => Dictionary.ContainsKey(key);

        public bool ContainsValue(U value)
            => Dictionary.ContainsValue(value);

        public Dictionary<T, U>.ValueCollection Values
            => Dictionary.Values;

        public Dictionary<T, U>.KeyCollection Keys
            => Dictionary.Keys;

        public bool Remove(T key)
            => Dictionary.Remove(key);

        public DictionarySerializer() { }

        public DictionarySerializer(IEnumerable<KeyValuePair<T, U>> collection)
        {
            Dictionary = new Dictionary<T, U>();
            foreach (var pair in collection)
            {
                Dictionary[pair.Key] = pair.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        IEnumerator<KeyValuePair<T, U>> IEnumerable<KeyValuePair<T, U>>.GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        // ISerializable.GetObjectData メソッドの実装
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var keyValuePairs = Dictionary.ToList();
            info.AddValue("KeyValuePairs", keyValuePairs);
        }

        public void Add(T key, U value)
            => Dictionary.Add(key, value);

        // 逆シリアライズ用のコンストラクタ
        private DictionarySerializer(SerializationInfo info, StreamingContext context)
        {
            var keyValuePairs = (List<KeyValuePair<T, U>>)info.GetValue("KeyValuePairs", typeof(List<KeyValuePair<T, U>>));

            Dictionary = new Dictionary<T, U>();
            foreach (var pair in keyValuePairs)
            {
                Dictionary[pair.Key] = pair.Value;
            }
        }
    }
}
