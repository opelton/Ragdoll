#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Potato.Core.Editor
{
    [CustomEditor(typeof(RuntimeSetBase), true)]
    public class RuntimeSetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            EditorGUI.BeginDisabledGroup(true); // make fields read-only
            EditorGUILayout.IntField("Count", ((RuntimeSetBase)target).Count);
            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif