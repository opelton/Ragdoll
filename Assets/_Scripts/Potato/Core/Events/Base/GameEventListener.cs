using UnityEngine;
using UnityEngine.Events;

// duplication warning -- GameEventListener<T> is basically copy-paste of GameEventListener, because I can't think of a smarter way without horrible side-effects
// so remember to copy/paste all changes! Ideally we're just done making changes to this
namespace Potato.Core
{
    // subscribe to an event and assign its response in-editor
    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] GameEvent _eventSource;
        internal GameEvent EventSource { get => _eventSource; set => SetEventSource(value); }
        [SerializeField] internal UnityEvent Response = new();

        private void OnEnable() => RegisterEvent();
        private void OnDisable()=> UnregisterEvent();
        public void OnEventRaised() => Response.Invoke();

        void RegisterEvent()
        {
            if(_eventSource != null)
                _eventSource.AddListener(this);
            else
                Debug.LogWarning($"GameEventListener {name} has no EventSource!");
        }

        // same as EventSource = null;
        public bool UnregisterEvent()
        {
            if(_eventSource != null)
                return _eventSource.RemoveListener(this);
            return false;
        }

        void SetEventSource(GameEvent eventSource)
        {
            UnregisterEvent();
            _eventSource = eventSource;
            RegisterEvent();
        }
    }

    public abstract class GameEventListener<T> : MonoBehaviour
    {
        [SerializeField] GameEvent<T> _eventSource;
        internal GameEvent<T> EventSource { get => _eventSource; set => SetEventSource(value); }
        [SerializeField] internal UnityEvent<T> Response = new();

        private void OnEnable() => RegisterEvent();
        private void OnDisable()=> UnregisterEvent();
        public void OnEventRaised(T data) => Response.Invoke(data);

        void RegisterEvent()
        {
            if(_eventSource != null)
                _eventSource.AddListener(this);
            else
                Debug.LogWarning($"GameEventListener {name} has no EventSource!");
        }

        public bool UnregisterEvent()
        {
            if(_eventSource != null)
                return _eventSource.RemoveListener(this);
            return false;
        }

        void SetEventSource(GameEvent<T> eventSource)
        {
            UnregisterEvent();
            _eventSource = eventSource;
            RegisterEvent();
        }
    }
}