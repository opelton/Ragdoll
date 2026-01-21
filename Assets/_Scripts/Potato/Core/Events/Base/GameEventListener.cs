using UnityEngine;
using UnityEngine.Events;

// duplication warning -- GameEventListener<T> is basically copy-paste of GameEventListener, because I can't think of a smarter way without horrible side-effects
// so remember to copy/paste all changes! Ideally we're just done making changes to this
namespace Potato.Core
{
    // subscribe to an event and assign its response in-editor
    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] internal GameEvent EventSource;
        [SerializeField] internal UnityEvent Response = new();

        private void OnEnable()
        {
            if(EventSource != null)
                EventSource.AddListener(this);
            else
                Debug.LogWarning($"GameEventListener {name} was enabled without a source!");
        }

        private void OnDisable()=> UnregisterEvent();
        public void OnEventRaised() => Response.Invoke();

        public bool UnregisterEvent()
        {
            if(EventSource != null)
                return EventSource.RemoveListener(this);
            return false;
        }
    }

    public abstract class GameEventListener<T> : MonoBehaviour
    {
        [SerializeField] internal GameEvent<T> EventSource;
        [SerializeField] internal UnityEvent<T> Response = new();

        private void OnEnable()
        {
            if(EventSource != null)
                EventSource.AddListener(this);
            else
                Debug.LogWarning($"GameEventListener {name} was enabled without a source!");
        }

        private void OnDisable()=> UnregisterEvent();
        public void OnEventRaised(T data) => Response.Invoke(data);

        public bool UnregisterEvent()
        {
            if(EventSource != null)
                return EventSource.RemoveListener(this);
            return false;
        }
    }
}