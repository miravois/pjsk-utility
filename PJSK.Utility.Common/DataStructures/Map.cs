﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJSK.Utility.Common.DataStructures
{
    public class Map<T1, T2> : IEnumerable<KeyValuePair<T1, T2>>
    {
        private readonly Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
        private readonly Dictionary<T2, T1> _backward = new Dictionary<T2, T1>();

        public Indexer<T1, T2> Forward { get; private set; }
        public Indexer<T2, T1> BackWard { get; private set; }

        public Map()
        {
            Forward = new Indexer<T1, T2>(_forward);
            BackWard = new Indexer<T2, T1>(_backward);
        }

        public void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _backward.Add(t2, t1);
        }

        public void Remove(T1 t1)
        {
            T2 revKey = Forward[t1];
            _forward.Remove(t1);
            _backward.Remove(revKey);
        }

        public void Remove(T2 t2)
        {
            T1 forwardKey = BackWard[t2];
            _backward.Remove(t2);
            _forward.Remove(forwardKey);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            return _forward.GetEnumerator();
        }

        public class Indexer<T3, T4>
        {
            private readonly Dictionary<T3, T4> _dictionary;

            public Dictionary<T3, T4>.KeyCollection Keys
            {
                get { return _dictionary.Keys; }
            }

            public Indexer(Dictionary<T3, T4> dictionary)
            {
                _dictionary = dictionary;
            }

            public T4 this[T3 index]
            {
                get { return _dictionary[index]; }
                set { _dictionary[index] = value; }
            }

            public bool Contains(T3 key)
            {
                return _dictionary.ContainsKey(key);
            }
        }
    }
}
