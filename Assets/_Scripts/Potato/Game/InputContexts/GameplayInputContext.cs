using Potato.Core;
using UnityEngine;

namespace Potato.Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Config/InputContext/GameplayContext")]
    public class GameplayInputContext : InputContext
    {
        [Header("Input Out Data")]
        [SerializeField] Vector2Reference playerMoveInput;
        [SerializeField] Vector2Reference playerLookInput;
        [SerializeField] Vector2Reference rawPlayerLookInput;

        [Header("Input Out Events")]
        [SerializeField] GameEvent fire1ButtonDown;
        [SerializeField] GameEvent fire1ButtonUp;
        [SerializeField] GameEvent fire2ButtonDown;
        [SerializeField] GameEvent fire2ButtonUp;
        [SerializeField] GameEvent sprintButtonDown;
        [SerializeField] GameEvent sprintButtonUp;
        [SerializeField] GameEvent jumpButtonDown;
        [SerializeField] GameEvent jumpButtonUp;
        [SerializeField] GameEvent useButtonDown;
        [SerializeField] GameEvent useButtonUp;
        [SerializeField] GameEvent quitButtonDown;
        [SerializeField] GameEvent quitButtonUp;

        [Header("Settings Data")]
        [SerializeField] FloatReference lookSensitivity;
        [SerializeField] FloatReference lookSensitivityModifierWebGL;

        InputFloatAxis _move = new();
        InputFloatAxis _look = new();
        InputButton _fire1 = new();
        InputButton _fire2 = new();
        InputButton _sprint = new();
        InputButton _jump = new();
        InputButton _use = new();
        InputButton _quit = new();

        public override void UpdateInputState(IInputPollingProvider provider)
        {
            _move.UpdateState(
                provider.GetAxis(InputConstants.KBM.MoveAxis_X),
                provider.GetAxis(InputConstants.KBM.MoveAxis_Y));
            
            _look.UpdateState(
                provider.GetAxis(InputConstants.KBM.LookAxis_X),
                provider.GetAxis(InputConstants.KBM.LookAxis_Y));

            _fire1.UpdateState(provider.GetKeyDown(InputConstants.KBM.Game_Fire1));
            _fire2.UpdateState(provider.GetKeyDown(InputConstants.KBM.Game_Fire2));
            _sprint.UpdateState(provider.GetKeyDown(InputConstants.KBM.Game_Sprint));
            _jump.UpdateState(provider.GetKeyDown(InputConstants.KBM.Game_Jump));
            _use.UpdateState(provider.GetKeyDown(InputConstants.KBM.Game_Use));
            _quit.UpdateState(provider.GetKeyDown(InputConstants.KBM.Game_Quit));

            // dispatch move/look updates, apply relevant settings
            playerMoveInput.Value = _move.Value;
            rawPlayerLookInput.Value = _look.Value;

#if UNITY_WEBGL
            playerLookInput.Value = _look.Value * lookSensitivity.Value * lookSensitivityModifierWebGL.Value;
#else
            playerLookInput.Value = _look.Value * lookSensitivity.Value;
#endif

            // instead of setting up proper lifecycle management of event subscriptions, I'm going to be lazy and poll all these buttons
            if(_fire1.ButtonPressed)        fire1ButtonDown.Invoke(this);
            else if(_fire1.ButtonReleased)  fire1ButtonUp.Invoke(this);

            if(_fire2.ButtonPressed)        fire2ButtonDown.Invoke(this);
            else if(_fire2.ButtonReleased)  fire2ButtonUp.Invoke(this);

            if(_sprint.ButtonPressed)       sprintButtonDown.Invoke(this);
            else if(_sprint.ButtonReleased) sprintButtonUp.Invoke(this);

            if(_jump.ButtonPressed)         jumpButtonDown.Invoke(this);
            else if(_jump.ButtonReleased)   jumpButtonUp.Invoke(this);

            if(_use.ButtonPressed)          useButtonDown.Invoke(this);
            else if(_use.ButtonReleased)    useButtonUp.Invoke(this);

            if(_quit.ButtonPressed)         quitButtonDown.Invoke(this);
            else if(_quit.ButtonReleased)   quitButtonUp.Invoke(this);
        }

        public override void ResetInputStates()
        {
            playerMoveInput.Value = Vector2.zero;
            playerLookInput.Value = Vector2.zero;
            rawPlayerLookInput.Value = Vector2.zero;

            _move.ResetState();
            _look.ResetState();
            _fire1.ResetState();
            _fire2.ResetState();
            _sprint.ResetState();
            _jump.ResetState();
            _use.ResetState();
            _quit.ResetState();
        }
    }
}