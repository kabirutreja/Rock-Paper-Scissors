// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic.StyledGUI
{
    [CustomPropertyDrawer(typeof(StyledText))]
    public class StyledTextAttributeDrawer : PropertyDrawer
    {
        StyledText a;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            a = (StyledText)attribute;

            GUIStyle styleLabel = new GUIStyle(EditorStyles.label)
            {
                richText = true,
                wordWrap = true
            };

            styleLabel.alignment = a.alignment;

            float height = styleLabel.CalcHeight(new GUIContent(property.stringValue), position.width);

            Rect textRect = new Rect(position.x, position.y + a.top, position.width, height);

            EditorGUI.LabelField(textRect, property.stringValue, styleLabel);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            a = (StyledText)attribute;

            GUIStyle styleLabel = new GUIStyle(EditorStyles.label)
            {
                richText = true,
                wordWrap = true
            };

            styleLabel.alignment = a.alignment;

            float height = styleLabel.CalcHeight(new GUIContent(property.stringValue), EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth);

            return a.top + height + a.down;
        }
    }
}