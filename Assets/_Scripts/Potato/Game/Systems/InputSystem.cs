using UnityEngine;
using Potato.Core;

namespace Potato.Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Systems/Input")]
    public class InputSystem : ScriptableObject
    {
        [Header("Input contexts")]
        [SerializeField] private InputContext menuContext;
        [SerializeField] private InputContext gameplayContext;

        [Header("Out events")]
        [SerializeField] private InputContextEvent notifyContextChanged;

        public void SetMenuContext() => notifyContextChanged.Invoke(menuContext, this);
        public void SetGameplayContext() => notifyContextChanged.Invoke(gameplayContext, this);
    }
}