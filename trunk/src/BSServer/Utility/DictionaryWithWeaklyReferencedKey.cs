using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JollyBit.BS.Core.Utility
{
    public class DictionaryWithWeaklyReferencedKey<TKEY,TVALUE> : IDictionary<TKEY, TVALUE>
    {
        private Dictionary<int, ICollection<KeyValuePair<WeakReference, TVALUE>>> _dict = new Dictionary<int, ICollection<KeyValuePair<WeakReference, TVALUE>>>();
        public void Add(TKEY key, TVALUE value)
        {
            ICollection<KeyValuePair<WeakReference, TVALUE>> refs;
            KeyValuePair<WeakReference, TVALUE> pair;
            TVALUE v;
            if (tryGetValue(key, out refs, out pair, out v))
            {
                throw new ArgumentException("An element with the same key already exists in the Dictionary<TKey, TValue>.");
            }
            if(refs == null)
            {
                refs = new List<KeyValuePair<WeakReference, TVALUE>>();
                _dict.Add(key.GetHashCode(), refs);
            }
            refs.Add(new KeyValuePair<WeakReference, TVALUE>(new WeakReference(key), value));
        }

        public bool ContainsKey(TKEY key)
        {
            TVALUE value;
            return TryGetValue(key, out value);
        }

        public ICollection<TKEY> Keys
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(TKEY key)
        {
            ICollection<KeyValuePair<WeakReference, TVALUE>> refs;
            KeyValuePair<WeakReference, TVALUE> pair;
            TVALUE value;
            if (tryGetValue(key, out refs, out pair, out value))
            {
                refs.Remove(pair);
                return true;
            }
            return false;
        }

        private bool tryGetValue(TKEY key, out ICollection<KeyValuePair<WeakReference, TVALUE>> collection, out KeyValuePair<WeakReference, TVALUE> pair, out TVALUE value)
        {
            ICollection<KeyValuePair<WeakReference, TVALUE>> refs;
            if (_dict.TryGetValue(key.GetHashCode(), out refs))
            {
                foreach (var reference in refs)
                {
                    if (!reference.Key.IsAlive) //Remove dead weak refs
                    {
                        refs.Remove(reference);
                    }
                    else if (Object.Equals(reference.Key.Target, key))
                    {
                        value = (TVALUE)reference.Key.Target;
                        collection = refs;
                        pair = reference;
                        return true;
                    }
                }
                if (refs.Count == 0)
                {
                    _dict.Remove(key.GetHashCode()); //Remove empty collections
                }
            }
            collection = refs;
            pair = default(KeyValuePair<WeakReference, TVALUE>);
            value = default(TVALUE);
            return false;
        }

        public bool TryGetValue(TKEY key, out TVALUE value)
        {
            ICollection<KeyValuePair<WeakReference, TVALUE>> refs;
            KeyValuePair<WeakReference, TVALUE> pair;
            return tryGetValue(key, out refs, out pair, out value);
        }

        public ICollection<TVALUE> Values
        {
            get { throw new NotImplementedException(); }
        }

        public TVALUE this[TKEY key]
        {
            get
            {
                TVALUE value;
                if (TryGetValue(key, out value)) return value;
                throw new KeyNotFoundException();
            }
            set
            {
                ICollection<KeyValuePair<WeakReference, TVALUE>> refs;
                KeyValuePair<WeakReference, TVALUE> pair;
                TVALUE v;
                if (!tryGetValue(key, out refs, out pair, out v))
                    throw new KeyNotFoundException();
                refs.Remove(pair);
                refs.Add(new KeyValuePair<WeakReference, TVALUE>(pair.Key, value));
            }
        }

        public void Add(KeyValuePair<TKEY, TVALUE> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dict.Clear();
        }

        public bool Contains(KeyValuePair<TKEY, TVALUE> item)
        {
            TVALUE value;
            if (TryGetValue(item.Key, out value))
            {
                return value.Equals(item.Value);
            }
            return false;
        }

        public void CopyTo(KeyValuePair<TKEY, TVALUE>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKEY, TVALUE> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKEY, TVALUE>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
