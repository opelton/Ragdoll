using UnityEngine;

namespace Potato.Core
{
    // this class exists so I can cast the class to something non-generic for custom editor stuff
    public abstract class RuntimeSetBase : ScriptableObject
    {
        public abstract int Count { get; }
        public abstract void DropSet();
    }
}
