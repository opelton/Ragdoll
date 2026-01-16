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
            DrawDefaultInspector();
            GameEvent e = (GameEvent)target;

            GUI.enabled = false;
            EditorGUILayout.IntField("Number of Listeners", e.NumListeners);
            GUI.enabled = true;

            if (GUILayout.Button("Invoke Event"))
                e.Invoke();
        }
    }
}
#endif