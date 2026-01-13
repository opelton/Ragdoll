using System.Collections.Generic;
using UnityEngine;

namespace Potato.Core
{
    // todo -- event args? templated?
    // event that can hook up to listeners in-editor
    [CreateAssetMenu(menuName = "ScriptableObjects/GameEvent")]
    public class GameEvent : ScriptableObject
    {
        private List<GameEventListener> listeners = new();

        public void Invoke()
        {
            // iterate backwards in case a listener's callback includes removing itself from this list
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void AddListener(GameEventListener listener)
        {
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
            }
        }

        public void RemoveListener(GameEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}