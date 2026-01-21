using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Potato.Core
{
    // central registry / observable collection. Headcounts, decoupled event subscription, replaces FindObjectOfType
    public abstract class RuntimeSet<T> : RuntimeSetBase, IPreInit
    {
        [SerializeField] internal GameEvent<T> onAdded;
        [SerializeField] internal GameEvent<T> onRemoved;

        protected readonly List<T> _items = new();
        public IReadOnlyList<T> Items => _items;
        public override int Count { get => _items.Count; }

        public void PreInit() => Clear();

        public bool Add(T item)
        {
            if (item != null && !_items.Contains(item))
            {
                _items.Add(item);

                if (onAdded)
                    onAdded.Invoke(item, this);

                return true;
            }
            return false;
        }

        public bool Remove(T item)
        {
            if(item == null)
                return false;

            if (onRemoved)
                onRemoved.Invoke(item, this);

            return _items.Remove(item);
        }

        void Clear() => _items.Clear();

#if UNITY_EDITOR
        internal override bool AddMember(object obj)
        {
            if (obj == null)
                return false;

            return Add((T)obj);
        }
        internal override bool RemoveMember(object obj)
        {
            if (obj == null)
                return false;

            return Remove((T)obj);
        }
        internal override void ClearSet() => Clear();
        internal override IReadOnlyList<object> GetItems() => Items.Cast<object>().ToList();
#endif
    }
}