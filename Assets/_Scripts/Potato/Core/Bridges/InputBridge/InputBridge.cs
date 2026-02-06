using UnityEngine;
using Potato.Core;

namespace Core.Potato
{
    public class InputBridge : MonoBehaviour, IInputPollingProvider
    {
        [SerializeField] InputContextReference CurrentInputContext;

        bool IInputPollingProvider.GetButtonDown(string buttonName) => Input.GetButtonDown(buttonName);
        bool IInputPollingProvider.GetKeyDown(KeyCode key) => Input.GetKeyDown(key);
        float IInputPollingProvider.GetAxis(string axisName) => Input.GetAxisRaw(axisName);

        void Update() => CurrentInputContext.Value.UpdateInputState(this);
    }
}