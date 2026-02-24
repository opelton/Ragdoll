using UnityEngine;
using UnityEngine.SceneManagement;

namespace Potato.Core
{
    // this can't use our own scene management because it isn't available yet
    public static class PersistentSceneBootstrap
    {
        public static void Run()
        {
            var config = Resources.Load<MainBootstrapConfig>(MainBootstrapConfig.RelativePath);
            if (config == null)
            {
                Debug.LogError("Main Bootstrap Config not found!");
                return;
            }

            // check which scenes are already loaded, and if one of them is the persistent bridge scene
            string sceneName = config.PersistentBridgeScene;
            bool persistentBridgeIsLoaded = false;
            bool persistentUiIsLoaded = false;
            string activeSceneName = string.Empty;
            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                string name = SceneManager.GetSceneAt(i).name;

                if (name == sceneName)
                    persistentBridgeIsLoaded = true;
                else if (name == config.PersistentUiScene)
                    persistentUiIsLoaded = true;
                else
                    activeSceneName = name;
            }

            // only one persist bridge scene allowed
            if (!persistentBridgeIsLoaded)
            {
                // non-async because I absolutely want the scene loaded by next frame, a startup hitch is acceptable
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            }
            else
            {
                Debug.LogWarning($"PersistentScene:{sceneName} already loaded!");
            }

            // only one persist ui scene allowed
            if (!persistentUiIsLoaded)
            {
                SceneManager.LoadScene(config.PersistentUiScene, LoadSceneMode.Additive);
            }
            else
            {
                Debug.LogWarning($"PersistentUiScene:{sceneName} already loaded!");
            }
            // Debug.Log($"Active scene bootstrapped: {activeSceneName}");

            // normally the scene manager would set this
            config.ActiveSceneName.Value = activeSceneName;
        }
    }
}