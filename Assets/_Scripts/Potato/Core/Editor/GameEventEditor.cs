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

            if (GUILayout.Button("EditorInvoke Event"))
                e.Invoke(this);
        }
    }

    [CustomEditor(typeof(GameEventGenericBase), true)]
    public class GameEventGenericEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GameEventGenericBase e = (GameEventGenericBase)target;

            GUI.enabled = false;
            EditorGUILayout.IntField("Number of Listeners", e.NumListeners);
            GUI.enabled = true;

            if (GUILayout.Button("EditorInvoke Event"))
                e.DummyInvoke(this);
        }
    }
}
#endif