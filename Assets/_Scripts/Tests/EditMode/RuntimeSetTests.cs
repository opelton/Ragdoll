using UnityEngine;
using Potato.Core;
using NUnit.Framework;

namespace Potato.Tests.EditMode
{
    // since this class uses GameEvents, these tests will also fail if GameEvents have a bug
    public class RuntimeSetTests
    {
        T CreateMember<T>()
            where T : RuntimeSetMemberBase
            => new GameObject().AddComponent<T>();

        T CreateMember<T, C>()
            where T : RuntimeSetMemberBase
            where C : Component
        {
            T go = CreateMember<T>();
            go.gameObject.AddComponent<C>();
            return go;
        }

        void BlankInit<T>()
            where T : RuntimeSetBase
        {
            T set = ScriptableObject.CreateInstance<T>();
            Assert.AreEqual(0, set.Count);
        }

        void NullGuard<T>()
            where T : RuntimeSetBase
        {
            T set = ScriptableObject.CreateInstance<T>();
            set.AddMember(null);

            Assert.AreEqual(0, set.Count);
            Object.DestroyImmediate(set);
        }

        void Add<T, V>()
            where T : RuntimeSetBase
            where V : RuntimeSetMemberBase
        {
            T set = ScriptableObject.CreateInstance<T>();
            var go = CreateMember<V>().GetValue();
            set.AddMember(go);

            Assert.IsNotNull(go);
            Assert.AreEqual(1, set.Count);
            Object.DestroyImmediate(set);
        }

        void Add<T, V, C>()
            where T : RuntimeSetBase
            where V : RuntimeSetMemberBase
            where C : Component
        {
            T set = ScriptableObject.CreateInstance<T>();
            var go = CreateMember<V, C>().GetValue();
            set.AddMember(go);

            Assert.IsNotNull(go);
            Assert.AreEqual(1, set.Count);
            Object.DestroyImmediate(set);
        }

        void AddRemove<T, V>()
            where T : RuntimeSetBase
            where V : RuntimeSetMemberBase
        {
            T set = ScriptableObject.CreateInstance<T>();
            var go = CreateMember<V>().GetValue();
            set.AddMember(go);
            set.RemoveMember(go);

            Assert.IsNotNull(go);
            Assert.AreEqual(0, set.Count);
            Object.DestroyImmediate(set);
        }

        void AddRemove<T, V, C>()
            where T : RuntimeSetBase
            where V : RuntimeSetMemberBase
            where C : Component
        {
            T set = ScriptableObject.CreateInstance<T>();
            var go = CreateMember<V, C>().GetValue();
            set.AddMember(go);
            set.RemoveMember(go);

            Assert.IsNotNull(go);
            Assert.AreEqual(0, set.Count);
            Object.DestroyImmediate(set);
        }

        void DoubleAdd<T, V>()
            where T : RuntimeSetBase
            where V : RuntimeSetMemberBase
        {
            T set = ScriptableObject.CreateInstance<T>();
            var go = CreateMember<V>().GetValue();
            set.AddMember(go);
            set.AddMember(go);

            Assert.IsNotNull(go);
            Assert.AreEqual(1, set.Count);
            Object.DestroyImmediate(set);
        }

        void DoubleAdd<T, V, C>()
            where T : RuntimeSetBase
            where V : RuntimeSetMemberBase
            where C : Component
        {
            T set = ScriptableObject.CreateInstance<T>();
            var go = CreateMember<V, C>().GetValue();
            set.AddMember(go);
            set.AddMember(go);

            Assert.IsNotNull(go);
            Assert.AreEqual(1, set.Count);
            Object.DestroyImmediate(set);
        }

        void AddTwo<T, V>()
            where T : RuntimeSetBase
            where V : RuntimeSetMemberBase
        {
            T set = ScriptableObject.CreateInstance<T>();
            var alice = CreateMember<V>().GetValue();
            var bob = CreateMember<V>().GetValue();
            set.AddMember(alice);
            set.AddMember(bob);

            Assert.AreEqual(2, set.Count);
            Object.DestroyImmediate(set);
        }

        void AddTwo<T, V, C>()
            where T : RuntimeSetBase
            where V : RuntimeSetMemberBase
            where C : Component
        {
            T set = ScriptableObject.CreateInstance<T>();
            var alice = CreateMember<V, C>().GetValue();
            var bob = CreateMember<V, C>().GetValue();
            set.AddMember(alice);
            set.AddMember(bob);

            Assert.AreEqual(2, set.Count);
            Object.DestroyImmediate(set);
        }

        void DoubleRemove<T, V>()
            where T : RuntimeSetBase
            where V : RuntimeSetMemberBase
        {
            T set = ScriptableObject.CreateInstance<T>();
            var go = CreateMember<V>().GetValue();
            set.AddMember(go);
            set.RemoveMember(go);
            set.RemoveMember(go);

            Assert.IsNotNull(go);
            Assert.AreEqual(0, set.Count);
            Object.DestroyImmediate(set);
        }

        void DoubleRemove<T, V, C>()
            where T : RuntimeSetBase
            where V : RuntimeSetMemberBase
            where C : Component
        {
            T set = ScriptableObject.CreateInstance<T>();
            var go = CreateMember<V, C>().GetValue();
            set.AddMember(go);
            set.RemoveMember(go);
            set.RemoveMember(go);

            Assert.IsNotNull(go);
            Assert.AreEqual(0, set.Count);
            Object.DestroyImmediate(set);
        }

        void DoubleRemove2<T, V>()
            where T : RuntimeSetBase
            where V : RuntimeSetMemberBase
        {
            T set = ScriptableObject.CreateInstance<T>();
            var go = CreateMember<V>().GetValue();
            var go2 = CreateMember<V>().GetValue();
            set.AddMember(go);
            set.AddMember(go2);
            set.RemoveMember(go);
            set.RemoveMember(go);

            Assert.IsNotNull(go);
            Assert.IsNotNull(go2);
            Assert.AreEqual(1, set.Count);
            Object.DestroyImmediate(set);
        }

        void DoubleRemove2<T, V, C>()
            where T : RuntimeSetBase
            where V : RuntimeSetMemberBase
            where C : Component
        {
            T set = ScriptableObject.CreateInstance<T>();
            var go = CreateMember<V, C>().GetValue();
            var go2 = CreateMember<V, C>().GetValue();
            set.AddMember(go);
            set.AddMember(go2);
            set.RemoveMember(go);
            set.RemoveMember(go);

            Assert.IsNotNull(go);
            Assert.IsNotNull(go2);
            Assert.AreEqual(1, set.Count);
            Object.DestroyImmediate(set);
        }

        void AddRemoveReturn<T, V>()
            where T : RuntimeSetBase
            where V : RuntimeSetMemberBase
        {
            T set = ScriptableObject.CreateInstance<T>();
            var go = CreateMember<V>().GetValue();
            set.AddMember(go);
            set.RemoveMember(go);
            set.AddMember(go);

            Assert.IsNotNull(go);
            Assert.AreEqual(1, set.Count);
            Object.DestroyImmediate(set);
        }

        void AddRemoveReturn<T, V, C>()
            where T : RuntimeSetBase
            where V : RuntimeSetMemberBase
            where C : Component
        {
            T set = ScriptableObject.CreateInstance<T>();
            var go = CreateMember<V, C>().GetValue();
            set.AddMember(go);
            set.RemoveMember(go);
            set.AddMember(go);

            Assert.IsNotNull(go);
            Assert.AreEqual(1, set.Count);
            Object.DestroyImmediate(set);
        }

        void MultipleAddsMultipleRemoves<T, V>()
            where T : RuntimeSetBase
            where V : RuntimeSetMemberBase
        {
            const int count = 10;
            object[] objs = new object[count];
            T set = ScriptableObject.CreateInstance<T>();

            for (int i = 0; i < count; ++i)
            {
                objs[i] = CreateMember<V>().GetValue();
                set.AddMember(objs[i]);
            }

            Assert.AreEqual(count, set.Count);

            for (int i = 0; i < 3; ++i)
                set.RemoveMember(objs[i]);

            Assert.AreEqual(count - 3, set.Count);
            Object.DestroyImmediate(set);
        }

        void MultipleAddsMultipleRemoves<T, V, C>()
            where T : RuntimeSetBase
            where V : RuntimeSetMemberBase
            where C : Component
        {
            const int count = 10;
            object[] objs = new object[count];
            T set = ScriptableObject.CreateInstance<T>();

            for (int i = 0; i < count; ++i)
            {
                objs[i] = CreateMember<V, C>().GetValue();
                set.AddMember(objs[i]);
            }

            Assert.AreEqual(count, set.Count);

            for (int i = 0; i < 3; ++i)
                set.RemoveMember(objs[i]);

            Assert.AreEqual(count - 3, set.Count);
            Object.DestroyImmediate(set);
        }

        void ClearUnclear<T, V>()
            where T : RuntimeSetBase
            where V : RuntimeSetMemberBase
        {
            const int count = 10;
            object[] objs = new object[count];
            T set = ScriptableObject.CreateInstance<T>();

            for (int i = 0; i < count; ++i)
            {
                objs[i] = CreateMember<V>().GetValue();
                set.AddMember(objs[i]);
            }
            Assert.AreEqual(count, set.Count);

            for (int i = 0; i < count; ++i)
                set.RemoveMember(objs[i]);
            Assert.AreEqual(0, set.Count);

            for (int i = 0; i < count; ++i)
                set.AddMember(objs[i]);
            Assert.AreEqual(count, set.Count);
            Object.DestroyImmediate(set);
        }

        void ClearUnclear<T, V, C>()
            where T : RuntimeSetBase
            where V : RuntimeSetMemberBase
            where C : Component
        {
            const int count = 10;
            object[] objs = new object[count];
            T set = ScriptableObject.CreateInstance<T>();

            for (int i = 0; i < count; ++i)
            {
                objs[i] = CreateMember<V, C>().GetValue();
                set.AddMember(objs[i]);
            }
            Assert.AreEqual(count, set.Count);

            for (int i = 0; i < count; ++i)
                set.RemoveMember(objs[i]);
            Assert.AreEqual(0, set.Count);

            for (int i = 0; i < count; ++i)
                set.AddMember(objs[i]);
            Assert.AreEqual(count, set.Count);
            Object.DestroyImmediate(set);
        }

        [Test]
        public void OnAddedEvent()
        {
            bool wasInvoked = false;

            var objSet = ScriptableObject.CreateInstance<GameObjectSet>();
            var objEvent = ScriptableObject.CreateInstance<GameObjectEvent>();
            var objListener = new GameObject().AddComponent<GameObjectEventListener>();

            objSet.onAdded = objEvent;
            objListener.Response.AddListener((obj) => wasInvoked = true);
            objEvent.AddListener(objListener);

            objSet.Add(new GameObject());

            Assert.IsTrue(wasInvoked);
        }

        [Test]
        public void OnAddedPayload()
        {
            GameObject payload = null;
            var original = new GameObject("test");

            var objSet = ScriptableObject.CreateInstance<GameObjectSet>();
            var objEvent = ScriptableObject.CreateInstance<GameObjectEvent>();
            var objListener = new GameObject().AddComponent<GameObjectEventListener>();

            objSet.onAdded = objEvent;
            objListener.Response.AddListener((obj) => payload = obj);
            objEvent.AddListener(objListener);

            objSet.Add(original);

            Assert.AreEqual(original, payload);
        }

        [Test]
        public void OnRemovedEvent()
        {
            bool wasInvoked = false;
            var original = new GameObject("test");

            var objSet = ScriptableObject.CreateInstance<GameObjectSet>();
            var objEvent = ScriptableObject.CreateInstance<GameObjectEvent>();
            var objListener = new GameObject().AddComponent<GameObjectEventListener>();

            objSet.onRemoved = objEvent;
            objListener.Response.AddListener((obj) => wasInvoked = true);
            objEvent.AddListener(objListener);

            objSet.Add(original);
            objSet.Remove(original);

            Assert.IsTrue(wasInvoked);
        }

        [Test]
        public void OnRemovedPayload()
        {
            GameObject payload = null;
            var original = new GameObject("test");

            var objSet = ScriptableObject.CreateInstance<GameObjectSet>();
            var objEvent = ScriptableObject.CreateInstance<GameObjectEvent>();
            var objListener = new GameObject().AddComponent<GameObjectEventListener>();

            objSet.onRemoved = objEvent;
            objListener.Response.AddListener((obj) => payload = obj);
            objEvent.AddListener(objListener);

            objSet.Add(original);
            objSet.Remove(original);

            Assert.AreEqual(original, payload);
        }

        // blank init
        [Test] public void BlankInit_GameObject() => BlankInit<GameObjectSet>();
        [Test] public void BlankInit_Transform() => BlankInit<TransformSet>();
        [Test] public void BlankInit_AudioSource() => BlankInit<AudioSourceSet>();

        // blank init
        [Test] public void NullGuard_GameObject() => NullGuard<GameObjectSet>();
        [Test] public void NullGuard_Transform() => NullGuard<TransformSet>();
        [Test] public void NullGuard_AudioSource() => NullGuard<AudioSourceSet>();

        // add
        [Test] public void Add_GameObject() => Add<GameObjectSet, GameObjectSetMember>();
        [Test] public void Add_Transform() => Add<TransformSet, TransformSetMember>();
        [Test] public void Add_AudioSource() => Add<AudioSourceSet, AudioSourceSetMember, AudioSource>();

        // add + remove
        [Test] public void AddRemove_GameObject() => AddRemove<GameObjectSet, GameObjectSetMember>();
        [Test] public void AddRemove_Transform() => AddRemove<TransformSet, TransformSetMember>();
        [Test] public void AddRemove_AudioSource() => AddRemove<AudioSourceSet, AudioSourceSetMember, AudioSource>();

        // double-add
        [Test] public void DoubleAdd_GameObject() => DoubleAdd<GameObjectSet, GameObjectSetMember>();
        [Test] public void DoubleAdd_Transform() => DoubleAdd<TransformSet, TransformSetMember>();
        [Test] public void DoubleAdd_AudioSource() => DoubleAdd<AudioSourceSet, AudioSourceSetMember, AudioSource>();

        // different members
        [Test] public void AddTwo_GameObject() => AddTwo<GameObjectSet, GameObjectSetMember>();
        [Test] public void AddTwo_Transform() => AddTwo<TransformSet, TransformSetMember>();
        [Test] public void AddTwo_AudioSource() => AddTwo<AudioSourceSet, AudioSourceSetMember, AudioSource>();

        // double-remove
        [Test] public void DoubleRemove_GameObject() => DoubleRemove<GameObjectSet, GameObjectSetMember>();
        [Test] public void DoubleRemove_Transform() => DoubleRemove<TransformSet, TransformSetMember>();
        [Test] public void DoubleRemove_AudioSource() => DoubleRemove<AudioSourceSet, AudioSourceSetMember, AudioSource>();

        // double-remove with other members in the list
        [Test] public void DoubleRemove2_GameObject() => DoubleRemove2<GameObjectSet, GameObjectSetMember>();
        [Test] public void DoubleRemove2_Transform() => DoubleRemove2<TransformSet, TransformSetMember>();
        [Test] public void DoubleRemove2_AudioSource() => DoubleRemove2<AudioSourceSet, AudioSourceSetMember, AudioSource>();

        // add + remove + add back
        [Test] public void AddRemoveReturn_GameObject() => AddRemoveReturn<GameObjectSet, GameObjectSetMember>();
        [Test] public void AddRemoveReturn_Transform() => AddRemoveReturn<TransformSet, TransformSetMember>();
        [Test] public void AddRemoveReturn_AudioSource() => AddRemoveReturn<AudioSourceSet, AudioSourceSetMember, AudioSource>();

        // multi add + multi remove
        [Test] public void MultiAddRemove_GameObject() => MultipleAddsMultipleRemoves<GameObjectSet, GameObjectSetMember>();
        [Test] public void MultiAddRemove_Transform() => MultipleAddsMultipleRemoves<TransformSet, TransformSetMember>();
        [Test] public void MultiAddRemove_AudioSource() => MultipleAddsMultipleRemoves<AudioSourceSet, AudioSourceSetMember, AudioSource>();

        // add + remove
        [Test] public void ClearUnclear_GameObject() => ClearUnclear<GameObjectSet, GameObjectSetMember>();
        [Test] public void ClearUnclear_Transform() => ClearUnclear<TransformSet, TransformSetMember>();
        [Test] public void ClearUnclear_AudioSource() => ClearUnclear<AudioSourceSet, AudioSourceSetMember, AudioSource>();
    }
}