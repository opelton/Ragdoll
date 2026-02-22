using UnityEngine;

namespace Potato.Core
{
    public class TimescaleBridge : MonoBehaviour
    {
        [SerializeField] private BoolReference pauseState;

        void Start() => SetPauseState(pauseState.Value);
        public void SetPauseState(bool isPaused) => Time.timeScale = isPaused ? 0f : 1f;
    }
}