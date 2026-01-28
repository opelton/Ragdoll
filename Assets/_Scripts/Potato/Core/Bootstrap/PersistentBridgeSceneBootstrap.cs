using UnityEngine;
using UnityEngine.SceneManagement;

namespace Potato.Core
{
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

            Debug.Log($"PersistentBridgeSceneBootstrap injected sceneName:{sceneName}");

            if(IsSceneLoaded(sceneName))
            {
                Debug.LogWarning($"PersistentBridgeScene:{sceneName} already loaded!");
                return;
            }
            
            // LoadScene using LoadSceneMode.Single would unload this scene
            // GameFlowManager should NEVER single-load, always explicitly unload unwanted scenes and then load the next additively
            // Remember to set active scene to the gameplay scene so new gameplay objects and etc target the correct scene
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        // TODO -- SO
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