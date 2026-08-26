// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic.StyledGUI
{
    [CustomPropertyDrawer(typeof(StyledButton))]
    public class StyledButtonAttributeDrawer : PropertyDrawer
    {
        StyledButton a;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            a = (StyledButton)attribute;

            position.y += a.top;
            position.height = EditorGUIUtility.singleLineHeight;

            if (GUI.Button(position, a.text))
            {
                property.boolValue = true;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return a.top + EditorGUIUtility.singleLineHeight + a.down;
        }
    }
}