using UnityEngine;

namespace Potato.Core
{
    public class CursorBridge : MonoBehaviour
    {
        [SerializeField] private BoolReference cursorLockState;

        void Start() => SetCursorLockState(cursorLockState.Value);

        public void SetCursorLockState(bool isLocked)
        {
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isLocked;
        }     
    }
}