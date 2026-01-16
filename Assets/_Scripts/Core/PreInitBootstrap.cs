using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Potato.Core
{
    public interface IPreInit { void PreInit(); }

    public static class PreInitBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RunPreInit()
        {
            PreInitializeAll();
        }

        // ScriptableOBjects used in the scene are guaranteed to be loaded by now
        // others that aren't used may incidently be loaded, and be returned here, but that's not guaranteed
        public static IEnumerable<IPreInit> FindPreInitializables()
        {
            return Resources
                .FindObjectsOfTypeAll<ScriptableObject>()
                .OfType<IPreInit>();
        }

        public static void PreInitializeAll()
        {
            var earlyBirds = FindPreInitializables();
            Debug.Log($"{earlyBirds.Count()} ScriptableObjects were pre-initialized");

            foreach (var birb in earlyBirds)
                birb.PreInit();
        }
    }

        // todo when game flow manager refactor
        /*
        public void StartNewGame()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("GameScene");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            PreInitBootstrap.Run();
        }
        */
}