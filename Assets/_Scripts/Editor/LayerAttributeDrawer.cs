#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Potato.FPS.Editor
{
    [CustomPropertyDrawer(typeof(LayerIndexAttribute))]
    public class LayerAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.LabelField(
                    position,
                    label.text,
                    "Use [LayerIndex] with int."
                );
                return;
            }

            property.intValue = EditorGUI.LayerField(
                position,
                label,
                property.intValue
            );
        }
    }
}
#endif