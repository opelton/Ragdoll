using UnityEngine;

namespace Potato.Game
{
    // sets gamestate to gameplay. Useful for loading scenes directly, bypassing game flow
    public class AssertGameplayState : MonoBehaviour
    {
        [SerializeField] private GameFlowSystem gameFlowSystem;
        void Start() => gameFlowSystem.SetGamestate(gameFlowSystem.GameplayState);
    }
}