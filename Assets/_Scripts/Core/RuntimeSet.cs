using System.Collections.Generic;

namespace Potato.Core
{
    // like DataVariables, but a set
    public abstract class RuntimeSet<T> : RuntimeSetBase, IPreInit
    {
        protected readonly List<T> _items = new();
        public IReadOnlyList<T> Items => _items;
        public override int Count { get => _items.Count; }

        public void PreInit() => _items.Clear();
        //void OnEnable() => _items.Clear();

        public void Add(T item)
        {
            if (!_items.Contains(item))
                _items.Add(item);
        }

        public bool Remove(T item) => _items.Remove(item);
    }
}