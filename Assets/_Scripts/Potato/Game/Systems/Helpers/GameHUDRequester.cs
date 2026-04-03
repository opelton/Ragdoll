using Potato.Core;
using UnityEngine;

namespace Potato.Game
{
    public class GameHUDRequester : MonoBehaviour
    {
        [SerializeField] private BoolReference showHudControl;

        void OnEnable() => showHudControl.Value = true;
        void OnDisable() => showHudControl.Value = false;
    }
}