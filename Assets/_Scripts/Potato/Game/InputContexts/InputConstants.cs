using UnityEngine;

namespace Potato.Game
{
    // meaning of the key presses is interpreted elsewhere
    public static class InputConstants
    {
        const KeyCode kShowMenuButton = 
#if UNITY_EDITOR || UNITY_WEBGL
        KeyCode.Tab;        // Tab plays nicer with web browsers and the unity editor
#else
        KeyCode.Escape;     // Escape key is expected in shipped builds
#endif
        public const KeyCode Game_Quit = kShowMenuButton;
        public const KeyCode Menu_Cancel = kShowMenuButton; 
    }
}