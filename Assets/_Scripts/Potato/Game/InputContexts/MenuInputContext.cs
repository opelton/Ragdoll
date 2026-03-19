using UnityEngine;

namespace Potato.Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Config/InputContext/MenuContext")]
    public class MenuInputContext : InputContext
    {
        [Header("Special Inputs")]
        [SerializeField] InputIntAxis uiMoveInput;
        [SerializeField] InputButton uiSubmitInput;
        [SerializeField] InputButton uiCancelInput;

        [Header("Settings")]
        [SerializeField] KeyCode altSubmitKey1 = KeyCode.Space;
        [SerializeField] KeyCode altSubmitKey2 = KeyCode.Return;


        public override void UpdateInputState(IInputPollingProvider provider)
        {
            base.UpdateInputState(provider);

            uiMoveInput.UpdateState(
                provider.GetAxis(uiMoveInput.HorizontalKey),
                provider.GetAxis(uiMoveInput.VerticalKey));

            uiSubmitInput.UpdateState(
                provider.GetKeyDown(uiSubmitInput.Key)
                || provider.GetKeyDown(altSubmitKey1)
                || provider.GetKeyDown(altSubmitKey2));

            uiCancelInput.UpdateState(provider.GetKeyDown(InputConstants.Menu_Cancel));
        }

        public override void InitializeInputStates(IInputPollingProvider provider)
        {
            base.InitializeInputStates(provider);
            
            uiMoveInput.InitializeState(
                provider.GetAxis(uiMoveInput.HorizontalKey),
                provider.GetAxis(uiMoveInput.HorizontalKey));

            uiSubmitInput.InitializeState(
                provider.GetKeyDown(uiSubmitInput.Key)
                || provider.GetKeyDown(altSubmitKey1)
                || provider.GetKeyDown(altSubmitKey2));

            uiCancelInput.InitializeState(provider.GetKeyDown(InputConstants.Menu_Cancel));
        }
    }
}