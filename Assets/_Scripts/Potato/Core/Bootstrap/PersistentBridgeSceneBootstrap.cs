using UnityEngine;
using UnityEngine.SceneManagement;

namespace Potato.Core
{
    // this can't use our own scene management because it isn't available yet
    public static class PersistentBridgeSceneBootstrap
    {
        public static void Run()
        {
            var config = Resources.Load<MainBootstrapConfig>(MainBootstrapConfig.RelativePath);
            if(config == null)
            {
                Debug.LogError("Main Bootstrap Config not found!");
                return;
            }
            string sceneName = config.PersistentBridgeScene;

            if(IsSceneLoaded(sceneName))
            {
                Debug.LogWarning($"PersistentBridgeScene:{sceneName} already loaded!");
                return;
            }
            
            // LoadScene using LoadSceneMode.Single would unload this scene
            // GameFlowManager should NEVER single-load, always explicitly unload unwanted scenes and then load the next additively
            // Remember to set active scene to the gameplay scene so new gameplay objects and etc target the correct scene

            // non-async because I absolutely want the scene loaded by next frame, a startup hitch is acceptable
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        // TODO -- SO-based presence-checking
        private static bool IsSceneLoaded(string sceneName)
        {
            for(int i = 0; i < SceneManager.sceneCount; ++i)
            {
                if(SceneManager.GetSceneAt(i).name == sceneName)
                    return true;
            }
            return false;
        }
    }
}