using UnityEngine;
using Potato.Core;
using NUnit.Framework;

namespace Potato.Tests.EditMode
{
    public class TestPreInitSO : NonPreInitSO, IPreInit { }

    public class NonPreInitSO : ScriptableObject
    {
        public bool WasInitialized = false;

        public void PreInit()
        {
            WasInitialized = true;
        }
    }


    public class PreInitBootstrapTests
    {
        [Test]
        public void MethodInitializesInstances()
        {
            TestPreInitSO test = ScriptableObject.CreateInstance<TestPreInitSO>();

            PreInitBootstrap.PreInitializeAll();

            Assert.IsTrue(test.WasInitialized);

            Object.DestroyImmediate(test);
        }

        [Test]
        public void NonPreInitializableIsSafe()
        {
            NonPreInitSO test = ScriptableObject.CreateInstance<NonPreInitSO>();

            Assert.DoesNotThrow(() =>
            {
                PreInitBootstrap.PreInitializeAll();
            });

            Object.DestroyImmediate(test);
        }

        [Test]
        public void RepeatInitializationIsSafe()
        {
            TestPreInitSO test = ScriptableObject.CreateInstance<TestPreInitSO>();

            Assert.DoesNotThrow(() =>
            {
                PreInitBootstrap.PreInitializeAll();
                PreInitBootstrap.PreInitializeAll();
            });

            Assert.IsTrue(test.WasInitialized);

            Object.DestroyImmediate(test);
        }

    }
}