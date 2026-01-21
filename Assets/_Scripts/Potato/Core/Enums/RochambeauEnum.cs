using UnityEngine;

namespace Potato.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Enums/Rochambeau")]
    public class RochambeauEnum : ScriptableEnum
    {
        [SerializeField] RochambeauEnum winsAgainst;
        [SerializeField] RochambeauEnum losesAgainst;
    }

}