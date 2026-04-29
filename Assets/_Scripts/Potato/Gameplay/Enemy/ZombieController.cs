using UnityEngine;

namespace Potato.Gameplay
{
    public class ZombieController : MonoBehaviour
    {
        public enum AiState { Idle, Chasing, Attacking }
        public enum MotorState { Upright, Downed }
    }
}