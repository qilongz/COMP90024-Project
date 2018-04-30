#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace TwitterUtil.Util
{
    public class FixedSortedList<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        private readonly TKey[] _keys;


        public FixedSortedList(TKey[] keySrc, IEnumerable<TValue> valSrc)
        {
            _keys = keySrc; // assume distinct

            var i = 0;
            Values = new TValue[_keys.Length];
            foreach (var val in valSrc) Values[i++] = val;
        }


        public FixedSortedList(int size, IEnumerable<KeyValuePair<TKey, TValue>> src)
        {
            _keys = new TKey[size];
            Values = new TValue[size];


            var offset = 0;
            foreach (var kvp in src.OrderBy(x => x.Key))
            {
                _keys[offset] = kvp.Key;
                Values[offset++] = kvp.Value;
            }
        }

        public FixedSortedList(IDictionary<TKey, TValue> src)
        {
            var size = src.Count;
            _keys = new TKey[size];
            Values = new TValue[size];


            var offset = 0;
            foreach (var kvp in src.OrderBy(x => x.Key))
            {
                _keys[offset] = kvp.Key;
                Values[offset++] = kvp.Value;
            }
        }


        public int Count => _keys.Length;
        public IReadOnlyList<TKey> Keys => _keys;
        public TValue[] Values { get; private set; }

        public TValue this[TKey key]
        {
            get
            {
                var num = Array.BinarySearch(_keys, 0, Count, key, null);
                if (num < 0) throw new ArgumentException("missing key " + key);
                return Values[num];
            }
            set
            {
                var num = Array.BinarySearch(_keys, 0, Count, key, null);
                if (num < 0) throw new ArgumentException("missing key " + key);
                Values[num] = value;
            }
        }

        public TKey GetKey(int offset) => _keys[offset];
        public TValue GetValue(int offset) => Values[offset];


        public bool ContainsKey(TKey key) => _keys.Contains(key);
        public int Search(TKey key) => Array.BinarySearch(_keys, 0, Count, key, null);


        public IEnumerable<KeyValuePair<TKey, TValue>> From(TKey start)
        {
            // find start
            var loc = Array.BinarySearch(_keys, 0, Count, start, null);
            if (loc < 0) loc = ~loc - 1; // not explicitly found, so start from prior
            if (loc < 0) loc = 0;

            for (var offset = loc; offset < _keys.Length; offset++)
                yield return new KeyValuePair<TKey, TValue>(_keys[offset], Values[offset]);
        }


        public void ReplaceValues(TValue[] replacement)
        {
            if (Values.Length != replacement.Length) throw new InvalidOperationException("misaligned");
            Values = replacement;
        }


        #region Nested type: Enumerator

        private struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IEnumerator
        {
            private readonly FixedSortedList<TKey, TValue> _src;
            private int _index;
            private readonly int _size;

            internal Enumerator(FixedSortedList<TKey, TValue> src)
            {
                _src = src;
                _size = src.Count;
                _index = -1;
            }

            public void Dispose()
            {
            }

            public bool MoveNext() => ++_index < _size;
            public void Reset() => _index = -1;


            public KeyValuePair<TKey, TValue> Current
                => new KeyValuePair<TKey, TValue>(_src.Keys[_index], _src.Values[_index]);

            object IEnumerator.Current => Current;
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}