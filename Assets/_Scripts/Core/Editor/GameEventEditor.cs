#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Potato.Core.Editor
{
    [CustomEditor(typeof(GameEvent))]
    public class GameEventEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // only want to fire while playing
            GUI.enabled = Application.isPlaying;
            GameEvent gameEvent = (GameEvent)target;

            if (GUILayout.Button("Invoke Event"))
            {
                gameEvent.Invoke();
            }
            // be kind, reenable the rest of the GUI
            GUI.enabled = true;
        }
    }
}
#endif