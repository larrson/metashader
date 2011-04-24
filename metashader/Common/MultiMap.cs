using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace metashader.Common
{
    /// <summary>
    /// 1対多の連想配列クラス
    /// 
    /// </summary>
    /// <typeparam name="TKey">キー型</typeparam>
    /// <typeparam name="UValue">キーに対応する格納型</typeparam>
    public class MultiMap<TKey, UValue> : IDictionary<TKey, List<UValue>>
    {
        private Dictionary<TKey, List<UValue>> map =
            new Dictionary<TKey, List<UValue>>();

        public List<UValue> this[TKey key]
        {
            get { return map[key]; }
            set { map[key] = value; }
        }

        public void Add(TKey key, UValue item)
        {
            AddSingleMap(key, item);
        }

        public void Add(TKey key, List<UValue> items)
        {
            foreach (UValue val in items)
                AddSingleMap(key, val);
        }

        public void Add(KeyValuePair<TKey, List<UValue>> keyValuePair)
        {
            foreach (UValue val in keyValuePair.Value)
                AddSingleMap(keyValuePair.Key, val);
        }

        public void Clear()
        {
            map.Clear();
        }

        public int Count
        {
            get { return map.Count; }
        }

        public bool ContainsKey(TKey key)
        {
            return map.ContainsKey(key);
        }

        public bool ContainsValue(UValue item)
        {
            if (item == null)
            {
                foreach (KeyValuePair<TKey, List<UValue>> kvp in map)
                {
                    if (((List<UValue>)kvp.Value).Count == 0)
                    {
                        return (true);
                    }
                }

                return (false);
            }
            else
            {
                foreach (KeyValuePair<TKey, List<UValue>> kvp in map)
                {
                    if (((List<UValue>)kvp.Value).Contains(item))
                    {
                        return (true);
                    }
                }

                return (false);
            }
        }

        public IEnumerator<KeyValuePair<TKey, List<UValue>>> GetEnumerator()
        {
            return map.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return map.GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            return RemoveSingleMap(key);
        }

        public bool Remove(KeyValuePair<TKey, List<UValue>> keyValuePair)
        {
            return Remove(keyValuePair.Key);
        }

        public ICollection<TKey> Keys
        {
            get { return map.Keys; }
        }

        public bool TryGetValue(TKey key, out List<UValue> value)
        {
            throw new NotImplementedException();
        }

        public ICollection<List<UValue>> Values
        {
            get { return map.Values; }
        }

        public bool Contains(KeyValuePair<TKey, List<UValue>> item)
        {
            return map.ContainsKey(item.Key) && map.ContainsValue(item.Value);
        }

        public bool Contains(KeyValuePair<TKey, UValue> item)
        {
            return map.ContainsKey(item.Key) && map[item.Key].Contains(item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, List<UValue>>[] array, int arrayIndex)
        {
            int index = 0;
            foreach (KeyValuePair<TKey, List<UValue>> item in map)
            {
                if (index < arrayIndex)
                    continue;

                array[index++] = item;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        protected void AddSingleMap(TKey key, UValue item)
        {
            // Dictionary型のmapの中でkeyを検索します。
            if (map.ContainsKey(key))
            {
                // mapのリストに値を追加します。
                List<UValue> values = (List<UValue>)map[key];

                // すでにキーが存在していれば、そのキーに対応づけて値を追加します。
                values.Add(item);
            }
            else
            {
                if (item == null)
                {
                    // 新しいキーを作成し、空のリストに対応付けます。
                    map.Add(key, new List<UValue>());
                }
                else
                {
                    List<UValue> values = new List<UValue>();
                    values.Add(item);
                    // 新しいキーを作成し、値に対応付けます。
                    map.Add(key, values);
                }
            }
        }

        protected bool RemoveSingleMap(TKey key)
        {
            if (this.ContainsKey(key))
            {
                // キーを削除します。
                return (map.Remove(key));
            }
            else
            {
                throw (new ArgumentOutOfRangeException("key", key,
                "マップにこのキーは存在しません。"));
            }
        }
    }
}
