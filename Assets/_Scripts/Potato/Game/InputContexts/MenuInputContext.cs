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
            _submitEnter.UpdateState(provider.GetKeyDown(KeyCode.Return));
            _submitSpacebar.UpdateState(provider.GetKeyDown(KeyCode.Space));
            _cancelEsc.UpdateState(provider.GetKeyDown(KeyCode.Escape));

            // ui cursor move
            _uiMove.UpdateState(
                provider.GetKeyDown(KeyCode.LeftArrow) || provider.GetKeyDown(KeyCode.A),
                provider.GetKeyDown(KeyCode.RightArrow) || provider.GetKeyDown(KeyCode.D),
                provider.GetKeyDown(KeyCode.UpArrow) || provider.GetKeyDown(KeyCode.W),
                provider.GetKeyDown(KeyCode.DownArrow) || provider.GetKeyDown(KeyCode.S));

            uiMoveInput.Value = _uiMove.Value;

            // confirm
            if(_submitEnter.ButtonPressed || _submitSpacebar.ButtonPressed)
                uiSubmitInput.Invoke(this);

            // cancel
            if(_cancelEsc.ButtonPressed)
                uiCancelInput.Invoke(this);
        }
        
    }
}