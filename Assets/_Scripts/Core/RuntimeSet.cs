using System;
using System.Collections.Generic;
using UnityEngine;

namespace Potato.Core
{
    // like DataVariables, but a set
    public abstract class RuntimeSet<T> : RuntimeSetBase
    {
        [NonSerialized, HideInInspector]
        public List<T> Items = new();

        public override void DropSet() => Items.Clear();
        public override int Count { get { return Items.Count; } }

        public void Add(T item)
        {
            if (!Items.Contains(item))
                Items.Add(item);
        }

        public void Remove(T item)
        {
            Items.Remove(item);
        }
    }
}