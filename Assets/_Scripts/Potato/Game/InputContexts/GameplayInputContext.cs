using Potato.Core;
using UnityEngine;

namespace Potato.Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Config/InputContext/GameplayContext")]
    public class GameplayInputContext : InputContext
    {
        [Header("Axis Inputs")]
        [SerializeField] InputFloatAxis playerMoveInput;
        [SerializeField] InputFloatAxis playerLookInput;
        [SerializeField] InputFloatAxis rawPlayerLookInput;

        [Header("Button Inputs")]
        [SerializeField] InputButton fire1Button;
        [SerializeField] InputButton fire2Button;
        [SerializeField] InputButton sprintButton;
        [SerializeField] InputButton jumpButton;
        [SerializeField] InputButton useButton;
        [SerializeField] InputButton quitButton;

        [Header("Settings Data")]
        [SerializeField] FloatReference lookSensitivity;
        [SerializeField] FloatReference lookSensitivityModifierWebGL;

        public override void UpdateInputState(IInputPollingProvider provider)
        {
            // axis
            playerMoveInput.UpdateState(
                provider.GetAxis(InputConstants.KBM.MoveAxis_X),
                provider.GetAxis(InputConstants.KBM.MoveAxis_Y));

            float lookX = provider.GetAxis(InputConstants.KBM.LookAxis_X);
            float lookY = provider.GetAxis(InputConstants.KBM.LookAxis_Y);
            float lookModifier = lookSensitivity.Value;

#if UNITY_WEBGL
            lookModifier *= lookSensitivityModifierWebGL.Value;
#endif
            rawPlayerLookInput.UpdateState(lookX, lookY);
            playerLookInput.UpdateState(lookX * lookModifier, lookY * lookModifier);

            // buttons
            fire1Button.UpdateState(provider.GetKeyDown(InputConstants.KBM.Game_Fire1));
            fire2Button.UpdateState(provider.GetKeyDown(InputConstants.KBM.Game_Fire2));
            sprintButton.UpdateState(provider.GetKeyDown(InputConstants.KBM.Game_Sprint));
            jumpButton.UpdateState(provider.GetKeyDown(InputConstants.KBM.Game_Jump));
            useButton.UpdateState(provider.GetKeyDown(InputConstants.KBM.Game_Use));
            quitButton.UpdateState(provider.GetKeyDown(InputConstants.KBM.Game_Quit));
        }

        public override void ResetInputStates(IInputPollingProvider provider)
        {
            // axis
            playerMoveInput.ResetState(
                provider.GetAxis(InputConstants.KBM.MoveAxis_X),
                provider.GetAxis(InputConstants.KBM.MoveAxis_Y));

            float lookX = provider.GetAxis(InputConstants.KBM.LookAxis_X);
            float lookY = provider.GetAxis(InputConstants.KBM.LookAxis_Y);
            float lookModifier = lookSensitivity.Value;

#if UNITY_WEBGL
            lookModifier *= lookSensitivityModifierWebGL.Value;
#endif
            rawPlayerLookInput.ResetState(lookX, lookY);
            playerLookInput.ResetState(lookX * lookModifier, lookY * lookModifier);

            // buttons
            fire1Button.ResetState(provider.GetKeyDown(InputConstants.KBM.Game_Fire1));
            fire2Button.ResetState(provider.GetKeyDown(InputConstants.KBM.Game_Fire2));
            sprintButton.ResetState(provider.GetKeyDown(InputConstants.KBM.Game_Sprint));
            jumpButton.ResetState(provider.GetKeyDown(InputConstants.KBM.Game_Jump));
            useButton.ResetState(provider.GetKeyDown(InputConstants.KBM.Game_Use));
            quitButton.ResetState(provider.GetKeyDown(InputConstants.KBM.Game_Quit));
        }
    }
}