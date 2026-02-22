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

        [Header("Out Variables")]
        [SerializeField] FloatReference sceneLoadAmount = 0;
        [SerializeField] StringReference activeSceneReference;

        public void HandleLoadSceneCommand(string sceneName) => StartCoroutine(HandleLoadSceneAsync(sceneName));
        public void HandleUnloadSceneCommand(string sceneName) => StartCoroutine(HandleUnloadSceneAsync(sceneName));
        public void HandleReloadSceneCommand() => StartCoroutine(HandleReloadSceneAsync());
        public void HandleSceneTransitionCommand(string sceneName) => StartCoroutine(HandleSceneTransitionAsync(sceneName));
        public void HandleChangeActiveSceneCommand(string sceneName) => ChangeActiveScene(sceneName, true);

        void OnEnable() => sceneLoadFinishedEvent.Invoke(gameObject.scene.name, this);
        void OnDestroy() => sceneUnloadFinishedEvent.Invoke(gameObject.scene.name, this);
        
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
            while(op.progress < .9f)
            {
                sceneLoadAmount.Value = op.progress;
                yield return null;
            }
            
            sceneLoadAmount.Value = 0;
            op.allowSceneActivation = true;
            yield return op;
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

            yield return op;
        }

        private IEnumerator HandleReloadSceneAsync()
        {
            // unload current scene
            sceneUnloadStartedEvent.Invoke(activeSceneReference.Value, this);
            ChangeActiveScene(gameObject.scene, false);
            yield return HandleUnloadSceneAsync(activeSceneReference.Value);
            sceneUnloadFinishedEvent.Invoke(activeSceneReference.Value, this);

            // load current scene
            sceneLoadStartedEvent.Invoke(activeSceneReference.Value, this);
            yield return HandleLoadSceneAsync(activeSceneReference.Value);
            ChangeActiveScene(activeSceneReference.Value, false);
            sceneLoadFinishedEvent.Invoke(activeSceneReference.Value, this);
        }

        private IEnumerator HandleSceneTransitionAsync(string sceneName)
        {
            // unload old scene
            sceneUnloadStartedEvent.Invoke(activeSceneReference.Value, this);
            yield return UnloadSceneAsync(activeSceneReference.Value);
            sceneUnloadFinishedEvent.Invoke(activeSceneReference.Value, this);

            // load new scene
            sceneLoadStartedEvent.Invoke(sceneName, this);
            yield return LoadSceneAsync(sceneName);
            sceneLoadFinishedEvent.Invoke(sceneName, this);
            ChangeActiveScene(sceneName, true);
        }

        private void ChangeActiveScene(string sceneName, bool notify) => ChangeActiveScene(SceneManager.GetSceneByName(sceneName), notify);

        // don't update active scene when it's briefly flipped back and forth for reloads
        private void ChangeActiveScene(Scene scene, bool notify)
        {
            bool result = false;

            if(scene.IsValid())
                result = SceneManager.SetActiveScene(scene);
            else
                Debug.Log($"scene {scene.name} is invalid!");

            // update the public reference if scene was correctly set
            if(notify && (result || SceneManager.GetActiveScene().name == scene.name))
                activeSceneReference.Value = scene.name;
        }
    }
}