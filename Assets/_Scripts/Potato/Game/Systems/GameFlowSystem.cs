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
        [SerializeField] GameStateReference gameStateReference;
        [SerializeField] BoolReference showLoadScreenRef;

        [Header("out events")]
        [SerializeField] GameEvent reloadSceneCommand;
        [SerializeField] StringEvent sceneTransitionCommand;

        [Header("game states")]
        [SerializeField] GameState menuState;
        [SerializeField] GameState gameplayState;

        [SerializeField] private string splash;
        [SerializeField] private string mainMenu;
        [SerializeField] private string play;

        public GameState MenuState => menuState;
        public GameState GameplayState => gameplayState;

        public void SceneTransition_Splash()
        {
            SetGamestate(menuState);
            sceneTransitionCommand.Invoke(splash, this);
        }

        public void SceneTransition_MainMenu()
        {
            SetGamestate(menuState);
            sceneTransitionCommand.Invoke(mainMenu, this);
        }

        public void SceneTransition_Play()
        {
            showLoadScreenRef.Value = true;
            SetGamestate(gameplayState);
            sceneTransitionCommand.Invoke(play, this);
        }

        public void ReloadCurrentScene()
        {
            showLoadScreenRef.Value = true;
            reloadSceneCommand.Invoke(this);
        }

        public void SetGamestate(GameState newState)
        {
            // check that it's actually different to avoid invoking unnecessary onChanged
            if(gameStateReference.Value != newState)
                gameStateReference.Value = newState;
        }

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