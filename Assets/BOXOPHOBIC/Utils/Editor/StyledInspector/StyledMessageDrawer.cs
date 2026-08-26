using UnityEditor;
using UnityEngine;

namespace Boxophobic.StyledGUI
{
    [CustomPropertyDrawer(typeof(StyledMessage))]
    public class StyledMessageAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!property.boolValue)
                return;

            StyledMessage a = (StyledMessage)attribute;

            MessageType messageType = MessageType.None;

            if (a.type == "None")
            {
                messageType = MessageType.None;
            }
            else if (a.type == "Info")
            {
                messageType = MessageType.Info;
            }
            else if (a.type == "Warning")
            {
                messageType = MessageType.Warning;
            }
            else if (a.type == "Error")
            {
                messageType = MessageType.Error;
            }

            float helpHeight = Mathf.Max(36, EditorStyles.helpBox.CalcHeight(new GUIContent(a.message), position.width - 30f));

            Rect helpRect = new Rect(position.x, position.y + a.top, position.width, helpHeight);

            EditorGUI.HelpBox(helpRect, a.message, messageType);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.boolValue)
                return 0;

            StyledMessage a = (StyledMessage)attribute;

            float viewWidth = EditorGUIUtility.currentViewWidth - 30f;

            float helpHeight = Mathf.Max(36, EditorStyles.helpBox.CalcHeight(new GUIContent(a.message), viewWidth));

            return a.top + helpHeight + a.down;
        }
    }
}