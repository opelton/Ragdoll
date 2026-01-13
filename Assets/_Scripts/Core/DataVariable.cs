using UnityEngine;

namespace Potato.Core
{
    // holds the variable where it can be referenced in the editor
    public abstract class DataVariable<T> : ScriptableObject
    {
        public T Value;
    }
}