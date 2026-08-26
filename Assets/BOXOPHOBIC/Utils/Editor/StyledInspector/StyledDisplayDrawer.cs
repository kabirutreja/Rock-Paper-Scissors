// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic.StyledGUI
{
    [CustomPropertyDrawer(typeof(StyledDisplay))]
    public class StyledDisplayAttributeDrawer : PropertyDrawer
    {
        StyledDisplay a;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            a = (StyledDisplay)attribute;

            // Override the default label
            label.text = a.displayName;

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}

