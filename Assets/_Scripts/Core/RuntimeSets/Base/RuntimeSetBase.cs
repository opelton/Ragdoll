using UnityEngine;
using System.Collections.Generic;

namespace Potato.Core
{
    // this class exists so I can cast the class to something non-generic for custom editor stuff
    public abstract class RuntimeSetBase : ScriptableObject
    {
        public abstract int Count { get; }

#if UNITY_EDITOR
        internal abstract bool AddMember(object obj);
        internal abstract bool RemoveMember(object obj);
        internal abstract void ClearSet();
        internal abstract IReadOnlyList<object> GetItems();
#endif
    }
}
