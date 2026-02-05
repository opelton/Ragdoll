using Potato.Core;
using UnityEngine;

namespace Potato.Game
{
    public class MenuInputContext : InputContext
    {
        [SerializeField] GameEvent uiSubmitInput;
        [SerializeField] GameEvent uiCancelInput;
        [SerializeField] Vector2IntReference uiMoveInput;

        InputButton _submitEnter;
        InputButton _submitSpacebar;
        InputButton _cancelEsc;
        InputAxisButtons _uiMove;

        public override void UpdateInputState(IInputPollingProvider provider)
        {

            _submitEnter.UpdateState(provider.GetKeyDown(KeyCode.Return));
            _submitSpacebar.UpdateState(provider.GetKeyDown(KeyCode.Space));
            _cancelEsc.UpdateState(provider.GetKeyDown(KeyCode.Escape));

            // confirm
            if(_submitEnter.ButtonPressed || _submitSpacebar.ButtonPressed)
                uiSubmitInput.Invoke(this);

            // cancel
            if(_cancelEsc.ButtonPressed)
                uiCancelInput.Invoke(this);

            // ui cursor move
            _uiMove.UpdateState(
                provider.GetKeyDown(KeyCode.LeftArrow) || provider.GetKeyDown(KeyCode.A),
                provider.GetKeyDown(KeyCode.RightArrow) || provider.GetKeyDown(KeyCode.D),
                provider.GetKeyDown(KeyCode.UpArrow) || provider.GetKeyDown(KeyCode.W),
                provider.GetKeyDown(KeyCode.DownArrow) || provider.GetKeyDown(KeyCode.S));

            uiMoveInput.Value = _uiMove.Value;
        }
        
    }
}