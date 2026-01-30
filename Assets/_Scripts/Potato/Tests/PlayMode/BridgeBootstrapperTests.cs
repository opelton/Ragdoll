using System.Collections;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

using NUnit.Framework;
using Potato.Core;

namespace Potato.Tests.PlayMode
{
    public class BridgeBootstrapperTests
    {
        MainBootstrapConfig bootstrapConfig;
        public BridgeBootstrapperTests()
        {
            bootstrapConfig = Resources.Load<MainBootstrapConfig>(MainBootstrapConfig.RelativePath);
        }

        [UnityTest]
        public IEnumerator ConfigExists()
        {
            Assert.IsNotNull(bootstrapConfig);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PersistentBridgeLoaded()
        {
            bool isLoaded = false;
            for(int i = 0; i < SceneManager.sceneCount; ++i)
            {
                if(SceneManager.GetSceneAt(i).name == bootstrapConfig.PersistentBridgeScene)
                {
                    isLoaded = true;
                    break;
                }
            }

            Assert.IsTrue(isLoaded);
            yield return null;
        }

        [UnityTest]
        public IEnumerator DuplicationSafe()
        {
            int sceneCount = 0;
            LogAssert.Expect(LogType.Warning, new Regex("already loaded!"));
            PersistentBridgeSceneBootstrap.Run();
            for(int i = 0; i < SceneManager.sceneCount; ++i)
            {
                if(SceneManager.GetSceneAt(i).name == bootstrapConfig.PersistentBridgeScene)
                    ++sceneCount;
            }

            Assert.AreEqual(1, sceneCount);
            yield return null;
        }

        // todo -- bridge scene needs to survive reloading, scene transitions, and state transitions
        // (under upcoming scene management bridge)
    }
}