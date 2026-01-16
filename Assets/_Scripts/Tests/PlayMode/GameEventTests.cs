using UnityEngine;
using UnityEngine.TestTools;
using Potato.Core;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Collections;

namespace Potato.Tests.PlayMode
{
    public class GameEventTests
    {
        GameEventListener MakeListener(string name = "Listener")
        {
            GameObject go = new GameObject(name);
            go.SetActive(false);
            return go.AddComponent<GameEventListener>();
        }

        [UnityTest]
        public IEnumerator InstantiatesCorrectly()
        {
            var listener = MakeListener();
            Assert.IsNull(listener.EventSource);
            yield return null;
        }

        [UnityTest]
        public IEnumerator NonDestructiveSourcelessOnEnable()
        {
            var listener = MakeListener();

            LogAssert.Expect(LogType.Warning, new Regex("was enabled without a source!"));
            listener.gameObject.SetActive(true);

            yield return null;
        }

        [UnityTest]
        public IEnumerator NonDestructiveEmptyResponse()
        {
            var listener = MakeListener();
            GameEvent testEvent = ScriptableObject.CreateInstance<GameEvent>();

            listener.EventSource = testEvent;
            Assert.DoesNotThrow(() =>
            {
                listener.gameObject.SetActive(true);
            });

            Object.DestroyImmediate(testEvent);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ListenerRespondsToEvent()
        {
            bool wasInvoked = false;
            GameEventListener listener = MakeListener();
            GameEvent testEvent = ScriptableObject.CreateInstance<GameEvent>();
            listener.EventSource = testEvent;

            listener.Response.AddListener(() => wasInvoked = true);
            listener.gameObject.SetActive(true);
            testEvent.Invoke();

            Assert.IsTrue(wasInvoked);
            Object.DestroyImmediate(testEvent);
            yield return null;
        }

        [UnityTest]
        public IEnumerator NonDestructiveSelfRemoval()
        {
            GameEvent testEvent = ScriptableObject.CreateInstance<GameEvent>();
            var listener = MakeListener();

            listener.EventSource = testEvent;
            listener.gameObject.SetActive(true);

            listener.Response.AddListener(() => listener.UnregisterEvent());
            Assert.DoesNotThrow(() =>
            {
                testEvent.Invoke();
            });
            Object.DestroyImmediate(testEvent);
            yield return null;
        }

        [UnityTest]
        public IEnumerator NoResponseAfterRemoval()
        {
            GameEventListener listener = MakeListener();
            GameEvent testEvent = ScriptableObject.CreateInstance<GameEvent>();
            listener.EventSource = testEvent;
            listener.gameObject.SetActive(true);

            bool wasInvoked = false;
            listener.Response.AddListener(() => wasInvoked = true);
            listener.UnregisterEvent();
            testEvent.Invoke();

            Assert.IsFalse(wasInvoked);
            Object.DestroyImmediate(testEvent);
            yield return null;
        }

        [UnityTest]
        public IEnumerator RemovalDuringCallback()
        {
            GameEvent testEvent = ScriptableObject.CreateInstance<GameEvent>();
            var listener = MakeListener();

            int invokeCount = 0;
            listener.EventSource = testEvent;
            listener.Response.AddListener(() =>
            {
                invokeCount += 1;
                listener.UnregisterEvent();
            });
            listener.gameObject.SetActive(true);

            testEvent.Invoke();
            testEvent.Invoke();

            Assert.AreEqual(1, invokeCount);

            yield return null;
        }

        [UnityTest]
        public IEnumerator MultipleListeners()
        {
            int pokes = 0;
            GameEventListener alice = MakeListener();
            GameEventListener bob = MakeListener();
            GameEvent testEvent = ScriptableObject.CreateInstance<GameEvent>();
            alice.EventSource = testEvent;
            bob.EventSource = testEvent;

            alice.Response.AddListener(() => pokes += 1);
            bob.Response.AddListener(() => pokes += 1);
            alice.gameObject.SetActive(true);
            bob.gameObject.SetActive(true);

            testEvent.Invoke();

            Assert.AreEqual(2, pokes);
            Object.DestroyImmediate(testEvent);
            yield return null;
        }

        [UnityTest]
        public IEnumerator SingleRemoval()
        {
            int aliceCount = 0;
            int bobCount = 0;
            GameEventListener alice = MakeListener();
            GameEventListener bob = MakeListener();
            GameEvent testEvent = ScriptableObject.CreateInstance<GameEvent>();
            alice.EventSource = testEvent;
            bob.EventSource = testEvent;

            alice.Response.AddListener(() => aliceCount += 1);
            bob.Response.AddListener(() => bobCount += 1);
            alice.gameObject.SetActive(true);
            bob.gameObject.SetActive(true);

            testEvent.Invoke();
            alice.UnregisterEvent();
            testEvent.Invoke();


            Assert.AreEqual(1, aliceCount);
            Assert.AreEqual(2, bobCount);
            Object.DestroyImmediate(testEvent);
            yield return null;
        }
    }
}