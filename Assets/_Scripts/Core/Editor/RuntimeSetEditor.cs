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

            base.OnInspectorGUI();

            EditorGUI.BeginDisabledGroup(true); // make fields read-only
            EditorGUILayout.IntField("Count", ((RuntimeSetBase)target).Count);
            EditorGUI.EndDisabledGroup();

            GUI.enabled = true; 
            RuntimeSetBase rts = (RuntimeSetBase)target;

            if (GUILayout.Button("Empty Set"))
            {
                rts.DropSet();
            }
        }
    }
}