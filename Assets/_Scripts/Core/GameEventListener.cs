using UnityEngine;
using UnityEngine.Events;

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

        public void DropEventSource()
        {
            UnregisterEvent();
            EventSource = null;
        }

        public bool UnregisterEvent()
        {
            if(EventSource != null)
                return EventSource.RemoveListener(this);
            return false;
        }
    }
}