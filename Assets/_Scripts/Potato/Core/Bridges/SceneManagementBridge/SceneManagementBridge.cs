using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Potato.Core
{
    public class SceneManagementBridge : MonoBehaviour
    {
        [Header("Out Events")]
        [SerializeField] StringEvent sceneLoadStartedEvent;
        [SerializeField] StringEvent sceneLoadFinishedEvent;
        [SerializeField] StringEvent sceneUnloadStartedEvent;
        [SerializeField] StringEvent sceneUnloadFinishedEvent;
        [SerializeField] StringEvent sceneReloadStartedEvent;
        [SerializeField] StringEvent sceneReloadFinishedEvent;
        [SerializeField] StringEvent activeSceneChangedEvent;

        [Header("Out Variables")]
        [SerializeField] FloatReference sceneLoadAmount = 0;

        public void HandleLoadSceneCommand(string sceneName) => StartCoroutine(HandleLoadSceneAsync(sceneName));
        public void HandleUnloadSceneCommand(string sceneName) => StartCoroutine(HandleUnloadSceneAsync(sceneName));
        public void HandleReloadSceneCommand(string sceneName) => StartCoroutine(HandleReloadSceneAsync(sceneName));
        public void HandleChangeActiveSceneCommand(string sceneName) => ChangeActiveScene(sceneName, true);
        

        private IEnumerator HandleLoadSceneAsync(string sceneName)
        {
            sceneLoadStartedEvent.Invoke(sceneName, this);
            yield return LoadSceneAsync(sceneName);
            ChangeActiveScene(sceneName, true);
            sceneLoadFinishedEvent.Invoke(sceneName, this);
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            op.allowSceneActivation = false;

            // activation counts for the last 10% of progress, so isDone would never be true with scene activation disallowed
            while(op.progress <= .9f)
            {
                sceneLoadAmount.Value = op.progress;
                yield return null;
            }
            sceneLoadAmount.Value = 0;
            op.allowSceneActivation = true;
        }

        private IEnumerator HandleUnloadSceneAsync(string sceneName)
        {
            sceneUnloadStartedEvent.Invoke(sceneName, this);
            yield return UnloadSceneAsync(sceneName);
            ChangeActiveScene(gameObject.scene, false);
            sceneUnloadFinishedEvent.Invoke(sceneName, this);
        }

        private IEnumerator UnloadSceneAsync(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if(!scene.IsValid() || !scene.isLoaded)
                yield break;

            var op = SceneManager.UnloadSceneAsync(scene);

            while (op.isDone)
                yield return null;
        }

        private IEnumerator HandleReloadSceneAsync(string sceneName)
        {
            sceneReloadStartedEvent.Invoke(sceneName, this);
            ChangeActiveScene(gameObject.scene, false);
            yield return HandleUnloadSceneAsync(sceneName);
            yield return HandleLoadSceneAsync(sceneName);
            ChangeActiveScene(sceneName, false);
            sceneReloadFinishedEvent.Invoke(sceneName, this);
        }

        private void ChangeActiveScene(string sceneName, bool echo) => ChangeActiveScene(SceneManager.GetSceneByName(sceneName), echo);

        private void ChangeActiveScene(Scene scene, bool echo)
        {
            bool result = false;

            if(scene.IsValid())
                result = SceneManager.SetActiveScene(scene);

            if(echo && result)
                activeSceneChangedEvent.Invoke(scene.name, this);
        }
    }
}