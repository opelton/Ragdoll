using UnityEngine;

namespace Potato.Core
{
    // like enums, but flexible, can contain their own data/rules, without losing editor friendliness
    public abstract class ScriptableEnum : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField] internal string _description;
#endif
    }
}