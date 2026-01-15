#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Potato.Core.Editor
{
    [CustomEditor(typeof(DataVariableBase), true)]
    public class DataVariableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DataVariableBase data = (DataVariableBase)target;

            if (GUILayout.Button("Reset Value"))
            {
                data.ResetValue();
            }

        }
    }
}
#endif