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
        [SerializeField] protected InputButton[] standardInputs;

        public virtual void UpdateInputState(IInputPollingProvider provider)
        {
            if(standardInputs != null)
                foreach(var button in standardInputs)
                    button.UpdateState(provider.GetKeyDown(button.Key));
        }
        public virtual void InitializeInputStates(IInputPollingProvider provider)
        {
            if(standardInputs != null)
                foreach(var button in standardInputs)
                    button.InitializeState(provider.GetKeyDown(button.Key));
        }
    }
}