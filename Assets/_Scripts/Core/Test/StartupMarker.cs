using UnityEngine;

namespace Potato.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Test/StartupMarker")]
    public class StartupMarker : ScriptableObject, IPreInit
    {
        public bool WasPreInitialized = false;
        public void PreInit() => WasPreInitialized = true;
    }
}