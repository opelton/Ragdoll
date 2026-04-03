using UnityEngine;
using Potato.Core;

namespace Potato.Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/GameEvent<T>/AudioRequestEvent")]
    public class AudioRequestEvent : GameEvent<AudioClip> {}
}