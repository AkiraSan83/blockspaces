using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JollyBit.BS.Server.Utility
{
    public class DictionaryWithWeaklyReferencedValues<TKEY, TVALUE> : DictionaryBase<TKEY, TVALUE>
    {
        private IDictionary<TKEY, WeakReference> _dict = new Dictionary<TKEY, WeakReference>();
        public override void Add(TKEY key, TVALUE value)
        {
            _dict.Add(key, new WeakReference(value));
        }

        public override bool Remove(TKEY key)
        {
            TVALUE value;
            if (TryGetValue(key, out value))
            {
                return _dict.Remove(key);
            }
            return false;
        }

        public override bool TryGetValue(TKEY key, out TVALUE value)
        {
            WeakReference reference;
            if (_dict.TryGetValue(key, out reference))
            {
                if (reference.IsAlive)
                {
                    value = (TVALUE)reference.Target;
                    return true;
                }
                _dict.Remove(key); //Remove dead weak refs
            }
            value = default(TVALUE);
            return false;
        }

        protected override void setValue(TKEY key, TVALUE value)
        {
            _dict[key] = new WeakReference(value);
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
