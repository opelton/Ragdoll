using UnityEngine;

namespace Potato.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/GameEvent<T>/SceneTransitionEvent")]
    public class SceneTransitionEvent : GameEvent<SceneManagementBridge.TransitionRequest> {}
}
