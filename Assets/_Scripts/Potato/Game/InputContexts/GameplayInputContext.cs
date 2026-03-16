using Potato.Core;
using UnityEngine;

namespace Potato.Game
{
    // todo -- better alt keycode/condition handling (webgl/pie/etc)
    [CreateAssetMenu(menuName = "ScriptableObjects/Config/InputContext/GameplayContext")]
    public class GameplayInputContext : InputContext
    {
        [Header("Special Inputs")]
        [SerializeField] InputFloatAxis playerMoveInput;
        [SerializeField] InputFloatAxis playerLookInput;
        [SerializeField] InputFloatAxis rawPlayerLookInput;
        [SerializeField] InputButton quitButton;

        [Header("Settings")]
        [SerializeField] FloatReference lookSensitivity;
        [SerializeField] FloatReference lookSensitivityModifierWebGL;
        [SerializeField] float lookSensitivityConstModifier = 1f;

        float ScaledLookModifier =>
#if UNITY_WEBGL
            lookSensitivity.Value * lookSensitivityConstModifier * lookSensitivityModifierWebGL.Value;
#else
            lookSensitivity.Value * lookSensitivityConstModifier;
#endif

        public override void UpdateInputState(IInputPollingProvider provider)
        {
            base.UpdateInputState(provider);

            // move axis
            playerMoveInput.UpdateState(
                provider.GetAxis(playerMoveInput.HorizontalKey),
                provider.GetAxis(playerMoveInput.VerticalKey));

            // look axis
            float lookX = provider.GetAxis(playerLookInput.HorizontalKey);
            float lookY = provider.GetAxis(playerLookInput.VerticalKey);
            float lookModifier = ScaledLookModifier;

            rawPlayerLookInput.UpdateState(lookX, lookY);
            playerLookInput.UpdateState(lookX * lookModifier, lookY * lookModifier);

            // quit key changes based on build type
            quitButton.UpdateState(provider.GetKeyDown(InputConstants.Game_Quit));
        }

        public override void InitializeInputStates(IInputPollingProvider provider)
        {
            base.InitializeInputStates(provider);

            // move axis
            playerMoveInput.InitializeState(
                provider.GetAxis(playerMoveInput.HorizontalKey),
                provider.GetAxis(playerMoveInput.VerticalKey));

            // look axis
            float lookX = provider.GetAxis(playerLookInput.HorizontalKey);
            float lookY = provider.GetAxis(playerLookInput.VerticalKey);
            float lookModifier = ScaledLookModifier;
            
            rawPlayerLookInput.InitializeState(lookX, lookY);
            playerLookInput.InitializeState(lookX * lookModifier, lookY * lookModifier);

            // quit key changes depending on build type
            quitButton.InitializeState(provider.GetKeyDown(InputConstants.Game_Quit));
        }
    }
}