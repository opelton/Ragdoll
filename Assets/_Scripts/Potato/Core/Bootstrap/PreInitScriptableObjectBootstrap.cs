using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Potato.Core
{
    public interface IPreInitScriptableObject { void PreInit(); }

    public static class PreInitScriptableObjectBootstrap
    {
        public static void Run()
        {
            var preInitObjs = FindAllPreInitializableScriptableObjects();
            Debug.Log($"{preInitObjs.Count()} ScriptableObjects were pre-initialized");

            foreach (var obj in preInitObjs)
                obj.PreInit();
        }

        private static IEnumerable<IPreInitScriptableObject> FindAllPreInitializableScriptableObjects()
        {
            return Resources
                .FindObjectsOfTypeAll<ScriptableObject>()
                .OfType<IPreInitScriptableObject>();
        }
    }
}