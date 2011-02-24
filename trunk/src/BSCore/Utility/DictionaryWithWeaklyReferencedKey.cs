using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JollyBit.BS.Core.Utility
{
    public class DictionaryWithWeaklyReferencedKey<TKEY,TVALUE> : DictionaryBase<TKEY, TVALUE>
    {
        private IDictionary<int, ICollection<KeyValuePair<WeakReference, TVALUE>>> _dict = new Dictionary<int, ICollection<KeyValuePair<WeakReference, TVALUE>>>();
        public override void Add(TKEY key, TVALUE value)
        {
            KeyValuePair<WeakReference, TVALUE> reference;
            ICollection<KeyValuePair<WeakReference, TVALUE>> collection;
            TVALUE oldValue;
            if (tryGetValue(key, out oldValue, out reference, out collection))
            {
                throw new ArgumentException();
            }
            if (collection == null)
            {
                collection = new List<KeyValuePair<WeakReference,TVALUE>>();
                _dict.Add(key.GetHashCode(), collection);
            }
            collection.Add(new KeyValuePair<WeakReference, TVALUE>(new WeakReference(key), value));
        }

        public override bool Remove(TKEY key)
        {
            KeyValuePair<WeakReference, TVALUE> reference;
            ICollection<KeyValuePair<WeakReference, TVALUE>> collection;
            TVALUE value;
            if (tryGetValue(key, out value, out reference, out collection))
            {
                collection.Remove(reference);
                return true;
            }
            return false;
        }

        private bool tryGetValue(TKEY key, out TVALUE value, out KeyValuePair<WeakReference, TVALUE> reference, out ICollection<KeyValuePair<WeakReference, TVALUE>> collection)
        {
            if (_dict.TryGetValue(key.GetHashCode(), out collection))
            {
                reference = collection.FirstOrDefault(r => Object.Equals(r.Key.Target, key));
                if (reference.Key != null)
                {
                    if (reference.Key.IsAlive)
                    {
                        value = (TVALUE)reference.Value;
                        return true;
                    }
                    else //Remove dead weak ref
                    {
                        collection.Remove(reference);
                        if (collection.Count == 0) 
                            _dict.Remove(key.GetHashCode()); //Remove dead collection of weak refs
                    }
                }
            }
            reference = default(KeyValuePair<WeakReference, TVALUE>);
            value = default(TVALUE);
            return false;
        }

        public override bool TryGetValue(TKEY key, out TVALUE value)
        {
            KeyValuePair<WeakReference, TVALUE> reference;
            ICollection<KeyValuePair<WeakReference, TVALUE>> collection;
            if (tryGetValue(key, out value, out reference, out collection))
            {
                value = reference.Value;
                return true;
            }
            return false;
        }

        protected override void setValue(TKEY key, TVALUE value)
        {
            KeyValuePair<WeakReference, TVALUE> reference;
            ICollection<KeyValuePair<WeakReference, TVALUE>> collection;
            if (!tryGetValue(key, out value, out reference, out collection))
            {
                collection.Remove(reference); //If reference already exists remove it
            }
            if (collection == null) //If collection does not exist create it
            {
                collection = new List<KeyValuePair<WeakReference, TVALUE>>();
                _dict.Add(key.GetHashCode(), collection);
            }
            collection.Add(new KeyValuePair<WeakReference, TVALUE>(new WeakReference(key), value));
        }

        public override void Clear()
        {
            _dict.Clear();
        }

        public override int Count
        {
            get { throw new NotImplementedException(); }
        }

        public override IEnumerator<KeyValuePair<TKEY, TVALUE>> GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
