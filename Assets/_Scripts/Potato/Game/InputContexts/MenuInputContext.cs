using Potato.Core;
using UnityEngine;

namespace Potato.Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Config/InputContext/MenuContext")]
    public class MenuInputContext : InputContext
    {
        [Header("Input Out Data")]
        [SerializeField] Vector2IntReference uiMoveInput;

        [Header("Input Out Events")]
        [SerializeField] GameEvent uiSubmitInput;
        [SerializeField] GameEvent uiCancelInput;

        InputButton _submitEnter = new();
        InputButton _submitSpacebar = new();
        InputButton _cancelEsc = new();
        InputIntAxis _uiMove = new();

        public override void UpdateInputState(IInputPollingProvider provider)
        {
            UpdateButtons(provider);

            // confirm event
            if(_submitEnter.ButtonPressed || _submitSpacebar.ButtonPressed)
                uiSubmitInput.Invoke(this);

            // cancel event
            if(_cancelEsc.ButtonPressed)
                uiCancelInput.Invoke(this);
        }

        void UpdateButtons(IInputPollingProvider provider)
        {
            _submitEnter.UpdateState(provider.GetKeyDown(InputConstants.KBM.Menu_Submit_Return));
            _submitSpacebar.UpdateState(provider.GetKeyDown(InputConstants.KBM.Menu_Submit_Space));
            _cancelEsc.UpdateState(provider.GetKeyDown(InputConstants.KBM.Menu_Cancel));

            // ui cursor move
            _uiMove.UpdateState(
                provider.GetAxis(InputConstants.KBM.MoveAxis_X),
                provider.GetAxis(InputConstants.KBM.MoveAxis_Y));

            uiMoveInput.Value = _uiMove.Value;
        }

        public override void ResetInputStates(IInputPollingProvider provider) => UpdateButtons(provider);
    }
}