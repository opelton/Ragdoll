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

        T MakeListener<T, D>(string name = "Listener")
            where T : GameEventListener<D>
        {
            GameObject go = new GameObject(name);
            go.SetActive(false);
            return go.AddComponent<T>();
        }

        void PayloadTest<L, E, D>(D testData)
            where L : GameEventListener<D>
            where E : GameEvent<D>
        {
            L listener = MakeListener<L, D>();
            E evt = ScriptableObject.CreateInstance<E>();
            D outData = default;
            listener.EventSource = evt;
            listener.Response.AddListener((D data) => { outData = data; });
            listener.gameObject.SetActive(true);
            evt.Invoke(testData, this);
            Assert.AreEqual(testData, outData);
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
            testEvent.Invoke(this);

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
                testEvent.Invoke(this);
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
            testEvent.Invoke(this);

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

            testEvent.Invoke(this);
            testEvent.Invoke(this);

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

            testEvent.Invoke(this);

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

            testEvent.Invoke(this);
            alice.UnregisterEvent();
            testEvent.Invoke(this);


            Assert.AreEqual(1, aliceCount);
            Assert.AreEqual(2, bobCount);
            Object.DestroyImmediate(testEvent);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PayloadTest_Bool()
        {
            PayloadTest<BoolEventListener, BoolEvent, bool>(true);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PayloadTest_Int()
        {
            PayloadTest<IntEventListener, IntEvent, int>(42);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PayloadTest_Float()
        {
            PayloadTest<FloatEventListener, FloatEvent, float>(11f);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PayloadTest_String()
        {
            PayloadTest<StringEventListener, StringEvent, string>("captain placeholder");
            yield return null;
        }

        [UnityTest]
        public IEnumerator PayloadTest_Vec2Int()
        {
            PayloadTest<Vec2IntEventListener, Vec2IntEvent, Vector2Int>(Vector2Int.down);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PayloadTest_Vec2()
        {
            PayloadTest<Vec2EventListener, Vec2Event, Vector2>(Vector2.left);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator PayloadTest_Vec3()
        {
            PayloadTest<Vec3EventListener, Vec3Event, Vector3>(Vector3.back);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PayloadTest_Vec4()
        {
            PayloadTest<Vec4EventListener, Vec4Event, Vector4>(Vector4.one);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PayloadTest_GameObject()
        {
            var obj = new GameObject("dummy");
            PayloadTest<GameObjectEventListener, GameObjectEvent, GameObject>(obj);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PayloadTest_Transform()
        {
            var obj = new GameObject("dummy");
            PayloadTest<TransformEventListener, TransformEvent, Transform>(obj.transform);
            yield return null;
        }

            

    }
}