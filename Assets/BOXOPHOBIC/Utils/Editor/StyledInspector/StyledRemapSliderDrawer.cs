using UnityEditor;
using UnityEngine;

namespace Boxophobic.StyledGUI
{
    [CustomPropertyDrawer(typeof(StyledRemap))]
    public class StyledRemapAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            StyledRemap a = (StyledRemap)attribute;

            if (!string.IsNullOrEmpty(a.label))
            {
                label.text = a.label;
            }

            var popupStyle = new GUIStyle(EditorStyles.popup)
            {
                fontSize = 9
            };

            var styleButton = new GUIStyle(EditorStyles.label)
            {

            };

            EditorGUI.BeginChangeCheck();

            Vector4 value;

            if (a.supportInvert)
            {
                value = property.vector4Value;
            }
            else
            {
                value = property.vector2Value;
            }

            float minValue;
            float maxValue;

            if (value.w == 0)
            {
                minValue = value.x;
                maxValue = value.y;
            }
            else
            {
                minValue = value.y;
                maxValue = value.x;
            }

            Rect line = position;
            line.height = EditorGUIUtility.singleLineHeight;

            Rect labelRect = line;
            labelRect.width = EditorGUIUtility.labelWidth;

            if (a.supportInvert)
            {
                if (GUI.Button(labelRect, "", styleButton))
                {
                    a.showAdvancedSettings = !a.showAdvancedSettings;
                }

                Rect sliderRect = line;
                sliderRect.width -= EditorGUIUtility.labelWidth + 54;

                EditorGUI.MinMaxSlider(sliderRect, label, ref minValue, ref maxValue, a.min, a.max);

                Rect popupRect = line;
                popupRect.x = position.xMax - 50;
                popupRect.width = 50;

                value.w = EditorGUI.Popup(popupRect, (int)value.w, new[] { "Remap", "Invert" });
            }
            else
            {
                if (GUI.Button(labelRect, "", styleButton))
                {
                    a.showAdvancedSettings = !a.showAdvancedSettings;
                }

                EditorGUI.MinMaxSlider(line, label, ref minValue, ref maxValue, a.min, a.max);
            }

            if (a.showAdvancedSettings)
            {
                line.y += EditorGUIUtility.singleLineHeight + 2;

                minValue = EditorGUI.Slider(line, "      Remap Min", minValue, a.min, maxValue);

                line.y += EditorGUIUtility.singleLineHeight + 2;

                maxValue = EditorGUI.Slider(line, "      Remap Max", maxValue, minValue, a.max);
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (value.w == 0)
                {
                    value.x = minValue;
                    value.y = maxValue;
                }
                else
                {
                    value.x = maxValue;
                    value.y = minValue;
                }

                value.z = Mathf.Abs(value.y - value.x) > 0.0001f ? 1.0f / (value.y - value.x) : 0.0f;

                if (a.supportInvert)
                {
                    property.vector4Value = value;
                }
                else
                {
                    property.vector2Value = new Vector2(value.x, value.y);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            StyledRemap a = (StyledRemap)attribute;

            if (a.showAdvancedSettings)
            {
                return EditorGUIUtility.singleLineHeight * 3 + 4;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }
    }
}