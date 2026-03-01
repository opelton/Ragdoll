using Potato.Core;
using UnityEngine;

namespace Potato.Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Config/InputContext/MenuContext")]
    public class MenuInputContext : InputContext
    {
        [Header("Axis Input")]
        [SerializeField] InputIntAxis uiMoveInput;

        [Header("ButtonInputs")]
        [SerializeField] InputButton uiSubmitInput;
        [SerializeField] InputButton uiCancelInput;

        public override void UpdateInputState(IInputPollingProvider provider)
        {
            uiMoveInput.UpdateState(
                provider.GetAxis(InputConstants.KBM.MoveAxis_X),
                provider.GetAxis(InputConstants.KBM.MoveAxis_Y));

            uiSubmitInput.UpdateState(
                provider.GetKeyDown(InputConstants.KBM.Menu_Submit_Return)
                || provider.GetKeyDown(InputConstants.KBM.Menu_Submit_Space));

            uiCancelInput.UpdateState(provider.GetKeyDown(InputConstants.KBM.Menu_Cancel));
        }

        public override void ResetInputStates(IInputPollingProvider provider)
        {
            uiMoveInput.ResetState(
                provider.GetAxis(InputConstants.KBM.MoveAxis_X),
                provider.GetAxis(InputConstants.KBM.MoveAxis_Y));

            uiSubmitInput.ResetState(
                provider.GetKeyDown(InputConstants.KBM.Menu_Submit_Return)
                || provider.GetKeyDown(InputConstants.KBM.Menu_Submit_Space));

            uiCancelInput.ResetState(provider.GetKeyDown(InputConstants.KBM.Menu_Cancel));
        }
    }
}