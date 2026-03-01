using UnityEngine;

namespace Potato.Game
{
    public interface IInputPollingProvider
    {
        bool GetButtonDown(string buttonName);
        bool GetKeyDown(KeyCode key);
        float GetAxis(string axisName);
    }

    public abstract class InputContext : ScriptableObject
    {
        public abstract void UpdateInputState(IInputPollingProvider provider);
        public abstract void ResetInputStates(IInputPollingProvider provider);
    }
}