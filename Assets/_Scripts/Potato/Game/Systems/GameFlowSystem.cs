using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Potato.Core;

namespace Potato.Game
{
    // for now, string-based scene references
    [CreateAssetMenu(menuName = "ScriptableObjects/Config/Systems/GameFlow")]
    public class GameFlowSystem : ScriptableObject
    {
        [Header("Out data")]
        [SerializeField] StringReference activeSceneReference;

        [Header("out events")]
        [SerializeField] GameEvent reloadSceneCommand;
        [SerializeField] StringEvent sceneTransitionCommand;

        [SerializeField] private string splash;
        [SerializeField] private string mainMenu;
        [SerializeField] private string play;

        public void SceneTransition_Splash() => sceneTransitionCommand.Invoke(splash, this);
        public void SceneTransition_MainMenu() => sceneTransitionCommand.Invoke(mainMenu, this);
        public void SceneTransition_Play() => sceneTransitionCommand.Invoke(play, this);
        public void ReloadCurrentScene() => reloadSceneCommand.Invoke(this);
        public void QuitToDesktop()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}