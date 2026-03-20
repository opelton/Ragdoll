using System;
using UnityEngine;

namespace Potato.Game
{
    public class GameplayCameraData
    {
        [NonSerialized] public Camera gameplayCamera;
        [NonSerialized] public Camera fpsCamera;
    }
}