using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Potato.Core
{
    public class SceneManagementBridge : MonoBehaviour
    {
        [Serializable]
        public class TransitionRequest
        {
            public string currentScene;
            public string nextScene;
        }

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
        public void HandleReloadSceneCommand(string sceneName) => StartCoroutine(HandleReloadSceneAsync(sceneName));
        public void HandleSceneTransitionCommand(TransitionRequest request) => StartCoroutine(HandleSceneTransitionAsync(request));
        public void HandleChangeActiveSceneCommand(string sceneName) => ChangeActiveScene(sceneName, true);

        void Start() => sceneLoadFinishedEvent.Invoke(gameObject.scene.name, this);
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
            // unload current scene
            sceneUnloadStartedEvent.Invoke(sceneName, this);
            ChangeActiveScene(gameObject.scene, false);
            yield return HandleUnloadSceneAsync(sceneName);
            sceneUnloadFinishedEvent.Invoke(sceneName, this);

            // load current scene
            sceneLoadStartedEvent.Invoke(sceneName, this);
            yield return HandleLoadSceneAsync(sceneName);
            ChangeActiveScene(sceneName, false);
            sceneLoadFinishedEvent.Invoke(sceneName, this);
        }

        private IEnumerator HandleSceneTransitionAsync(TransitionRequest request)
        {
            // unload old scene
            sceneUnloadStartedEvent.Invoke(request.currentScene, this);
            yield return UnloadSceneAsync(request.currentScene);
            sceneUnloadFinishedEvent.Invoke(request.currentScene, this);

            // load new scene
            sceneLoadStartedEvent.Invoke(request.nextScene, this);
            yield return LoadSceneAsync(request.nextScene);
            sceneLoadFinishedEvent.Invoke(request.nextScene, this);
            ChangeActiveScene(request.nextScene, true);
        }

        private void ChangeActiveScene(string sceneName, bool update) => ChangeActiveScene(SceneManager.GetSceneByName(sceneName), update);

        // don't update active scene when it's briefly flipped back and forth for reloads
        private void ChangeActiveScene(Scene scene, bool update)
        {
            bool result = false;

            if(scene.IsValid())
                result = SceneManager.SetActiveScene(scene);

            if(update && result)
                activeSceneReference.Value = scene.name;
        }
    }
}