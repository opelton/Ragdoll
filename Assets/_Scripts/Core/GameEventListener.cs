using UnityEngine;
using UnityEngine.Events;

namespace Potato.Core
{
    // subscribe to an event and assign its response in-editor
    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] GameEvent Event;
        [SerializeField] UnityEvent Response;

        private void OnEnable() => Event.AddListener(this);
        private void OnDisable() => Event.RemoveListener(this);
        public void OnEventRaised() => Response.Invoke();
    }
}