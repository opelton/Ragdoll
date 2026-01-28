using UnityEngine;

namespace Potato.Core
{
    public static class MainBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void StartupBootstrap()
        {
            // initialize scriptable objects
            PreInitScriptableObjectBootstrap.Run();

            // ensure bridge scene is additively loaded
            PersistentBridgeSceneBootstrap.Run();
        }
    }
}
