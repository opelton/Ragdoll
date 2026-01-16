using System.Collections.Generic;
using UnityEngine;

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
        //public void OnEnable() => _listeners.Clear();

        public void Invoke(object sender = null)
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
}