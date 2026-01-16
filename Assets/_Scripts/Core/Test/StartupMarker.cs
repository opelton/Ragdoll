using UnityEngine;

namespace Potato.Core
{
    [CreateAssetMenu]
    public class StartupMarker : ScriptableObject, IPreInit
    {
        public bool WasPreInitialized = false;
        public void PreInit() => WasPreInitialized = true;
    }
}