using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JollyBit.BS.Core.Utility
{
    public abstract class DictionaryBase<TKEY, TVALUE> : IDictionary<TKEY, TVALUE>
    {
        public abstract void Add(TKEY key, TVALUE value);
        public virtual bool ContainsKey(TKEY key)
        {
            TVALUE value;
            return TryGetValue(key, out value);
        }
        public virtual ICollection<TKEY> Keys
        {
            get
            {
                return this.Select(i => i.Key).ToList();
            }
        }
        public abstract bool Remove(TKEY key);
        public abstract bool TryGetValue(TKEY key, out TVALUE value);
        public virtual ICollection<TVALUE> Values
        {
            get
            {
                return this.Select(i => i.Value).ToList();
            }
        }
        public virtual TVALUE this[TKEY key]
        {
            get
            {
                TVALUE value;
                if(TryGetValue(key, out value)) return value;
                throw new KeyNotFoundException();
            }
            set
            {
                setValue(key, value);
            }
        }
        protected abstract void setValue(TKEY key, TVALUE value);
        public virtual void Add(KeyValuePair<TKEY, TVALUE> item)
        {
            Add(item.Key, item.Value);
        }
        public abstract void Clear();
        public virtual bool Contains(KeyValuePair<TKEY, TVALUE> item)
        {
            TVALUE value;
            if (TryGetValue(item.Key, out value) && value.Equals(item.Value))
            {
                return value.Equals(item.Value);
            }
            return false;
        }
        public virtual void CopyTo(KeyValuePair<TKEY, TVALUE>[] array, int arrayIndex)
        {
            foreach (var item in this)
            {
                array[arrayIndex] = item;
                arrayIndex++;
            }
        }
        public abstract int Count { get; }
        public virtual bool IsReadOnly
        {
            get { return true; }
        }
        public virtual bool Remove(KeyValuePair<TKEY, TVALUE> item)
        {
            TVALUE value;
            if (TryGetValue(item.Key, out value) && value.Equals(item.Value))
            {
                Remove(item.Key);
            }
            return false;
        }
        public abstract IEnumerator<KeyValuePair<TKEY, TVALUE>> GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
