using UnityEngine;

namespace Potato.Game
{
    // meaning of the key presses is interpreted elsewhere
    public static class InputConstants
    {
        public static class KBM
        {
            public const string MoveAxis_X = "Horizontal";
            public const string MoveAxis_Y = "Vertical";
            public const string LookAxis_X = "Look X";
            public const string LookAxis_Y = "Look Y";

            public const KeyCode Game_Fire1 = KeyCode.Mouse0;
            public const KeyCode Game_Fire2 = KeyCode.Mouse1;
            public const KeyCode Game_Sprint = KeyCode.LeftShift;
            public const KeyCode Game_Jump = KeyCode.Space;
            public const KeyCode Game_Crouch = KeyCode.LeftControl;
            public const KeyCode Game_Use = KeyCode.E;
            public const KeyCode Game_Reload = KeyCode.R;
            public const KeyCode Game_SwapWeapon = KeyCode.Q;
            public const KeyCode Game_Punch = KeyCode.Q;
            public const KeyCode Game_Quit = KeyCode.Tab;

            public const KeyCode Menu_Cancel = KeyCode.Tab;
            public const KeyCode Menu_Submit_Space = KeyCode.Space;
            public const KeyCode Menu_Submit_Return = KeyCode.Return;
        }
        // todo gamepad?
    }
}