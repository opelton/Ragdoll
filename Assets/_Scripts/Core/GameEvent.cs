using System.Collections.Generic;
using UnityEngine;

// duplication warning -- GameEvent<T> is basically copy-paste of GameEvent, because I can't think of a smarter way without horrible side-effects
// so remember to copy/paste all changes! Ideally we're just done making changes to this
namespace Potato.Core
{
    // event that can hook up to _listeners in-editor
    [CreateAssetMenu(menuName = "ScriptableObjects/GameEvent")]
    public class GameEvent : ScriptableObject, IPreInit
    {
#if UNITY_EDITOR
        [SerializeField] internal bool _logInvocations = false;
        [SerializeField] internal string _description;
#endif
        private List<GameEventListener> _listeners = new();
        public int NumListeners => _listeners.Count;

        public void PreInit() => _listeners.Clear();

        public void Invoke(object sender)
        {
#if UNITY_EDITOR
            if (_logInvocations)
            {
                string invoker = sender == null ? "anonymous" : sender.ToString();
                Debug.Log($"{name} raised by {invoker}");
            }
#endif
            // iterate backwards in case a listener's callback includes removing itself from this list
            for (int i = _listeners.Count - 1; i >= 0; i--)
                _listeners[i].OnEventRaised();
        }

        public void AddListener(GameEventListener listener)
        {
            if (!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public bool RemoveListener(GameEventListener listener)
        {
            return _listeners.Remove(listener);
        }
    }

    public abstract class GameEvent<T> : GameEventGenericBase
    {
#if UNITY_EDITOR
        [SerializeField] internal bool _logInvocations = false;
        [SerializeField] internal string _description;

        // test payloads from editor
        [SerializeField]
        [Tooltip("EDITOR ONLY -- dummy invoke uses this as payload")]
        internal T _dummyValue;
        internal override void DummyInvoke(object sender) => Invoke(_dummyValue, sender);
#endif
        private List<GameEventListener<T>> _listeners = new();

        public override int NumListeners => _listeners.Count;

        public override void PreInit() => _listeners.Clear();

        public void Invoke(T data, object sender)
        {
#if UNITY_EDITOR
            if (_logInvocations)
            {
                string invoker = sender == null ? "anonymous" : sender.ToString();
                Debug.Log($"{name} raised by {invoker} passing {data}");
            }
#endif
            // iterate backwards in case a listener's callback includes removing itself from this list
            for (int i = _listeners.Count - 1; i >= 0; i--)
                _listeners[i].OnEventRaised(data);
        }

        public void AddListener(GameEventListener<T> listener)
        {
            if (!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public bool RemoveListener(GameEventListener<T> listener)
        {
            return _listeners.Remove(listener);
        }
    }

    public abstract class GameEventGenericBase : ScriptableObject, IPreInit
    {
        public abstract void PreInit();
        public abstract int NumListeners { get; }
#if UNITY_EDITOR
        internal abstract void DummyInvoke(object sender);
#endif
    }
}