using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.Interpreter
{
    class Scope
        : IDictionary<string, Cell>
    {
        public ICollection<string> Keys => _inner.Keys;
        public ICollection<Cell> Values => _inner.Values;
        public int Count => _inner.Count;
        public bool IsReadOnly => false;

        public Scope RootScope => _root;

        public Cell this[string key]
        {
            get
            {
                if (_inner.TryGetValue (key, out Cell result))
                    return result;

                if (_root != null)
                    return _root[key];

                return Cell.NULL;
            }

            set => _inner[key] = value;
        }

        public Scope () => _root = null;

        Scope (Scope parent, Scope root)
        {
            _parent = parent;
            _root = parent._root;
        }

        public Scope Spawn () => new Scope (this, this._root ?? this);

        public void Add (string key, Cell value) => _inner.Add (key, value);
        public bool ContainsKey (string key) => _inner.ContainsKey (key);
        public bool Remove (string key) => _inner.Remove (key);
        public bool TryGetValue (string key, out Cell value) => _inner.TryGetValue (key, out value);
        public void Add (KeyValuePair<string, Cell> item) => _inner.Add (item.Key, item.Value);
        public void Clear () => _inner.Clear ();
        public bool Contains (KeyValuePair<string, Cell> item) => _inner.ContainsKey (item.Key) && _inner[item.Key].Equals (item.Value);
        public void CopyTo (KeyValuePair<string, Cell>[] array, int arrayIndex) => throw new NotImplementedException ();
        public bool Remove (KeyValuePair<string, Cell> item) => _inner.Remove (item.Key);
        public IEnumerator<KeyValuePair<string, Cell>> GetEnumerator () => _inner.GetEnumerator ();
        IEnumerator IEnumerable.GetEnumerator () => _inner.GetEnumerator ();

        readonly Scope _parent;
        readonly Scope _root;
        readonly Dictionary<string, Cell> _inner = new Dictionary<string, Cell> ();
    }
}
