// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;

namespace Boxophobic.StyledGUI
{
    public class StyledRemapSliderDrawer : MaterialPropertyDrawer
    {
        public float min = 0;
        public float max = 0;
        public float top = 0;
        public float down = 0;

        float internalValueMin;
        float internalValueMax;

        bool showAdvancedSettings = false;

        public StyledRemapSliderDrawer()
        {
            this.min = 0;
            this.max = 1;
            this.top = 0;
            this.down = 0;
        }

        public StyledRemapSliderDrawer(float min, float max)
        {
            this.min = min;
            this.max = max;
            this.top = 0;
            this.down = 0;
        }

        public StyledRemapSliderDrawer(float min, float max, float top, float down)
        {
            this.min = min;
            this.max = max;
            this.top = top;
            this.down = down;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            var stylePopupMini = new GUIStyle(EditorStyles.popup)
            {
                fontSize = 9,
            };

            var styleButton = new GUIStyle(EditorStyles.label)
            {

            };

            Vector4 propVector = prop.vectorValue;

            EditorGUI.BeginChangeCheck();

            if (propVector.w == 0)
            {
                internalValueMin = propVector.x;
                internalValueMax = propVector.y;
            }
            else
            {
                internalValueMin = propVector.y;
                internalValueMax = propVector.x;
            }

            float y = position.y + top;
            float height = EditorGUIUtility.singleLineHeight;

            EditorGUI.showMixedValue = prop.hasMixedValue;

            Rect labelRect = new Rect(position.x, y, EditorGUIUtility.labelWidth, height);
            Rect sliderRect = new Rect(position.x + EditorGUIUtility.labelWidth + 3, y, position.width - EditorGUIUtility.labelWidth - 59, height);
            Rect popupRect = new Rect(position.xMax - 50, y, 50, height);

            if (GUI.Button(labelRect, label, styleButton))
            {
                showAdvancedSettings = !showAdvancedSettings;
            }

            EditorGUI.MinMaxSlider(sliderRect, ref internalValueMin, ref internalValueMax, min, max);

            propVector.w = EditorGUI.Popup(popupRect, (int)propVector.w, new string[] { "Remap", "Invert" }, stylePopupMini);

            y += height + EditorGUIUtility.standardVerticalSpacing;

            if (showAdvancedSettings)
            {
                Rect minRect = new Rect(position.x, y, position.width, height);

                internalValueMin = Mathf.Clamp(EditorGUI.Slider(minRect, "      Remap Min", internalValueMin, min, max), min, internalValueMax);

                y += height + EditorGUIUtility.standardVerticalSpacing;

                Rect maxRect = new Rect(position.x, y, position.width, height);

                internalValueMax = Mathf.Clamp(EditorGUI.Slider(maxRect, "      Remap Max", internalValueMax, min, max), internalValueMin, max);
            }

            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                if (propVector.w == 0)
                {
                    propVector.x = internalValueMin;
                    propVector.y = internalValueMax;
                }
                else
                {
                    propVector.y = internalValueMin;
                    propVector.x = internalValueMax;
                }

                propVector.z = 1 / (propVector.y - propVector.x);

                prop.vectorValue = propVector;
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (showAdvancedSettings)
            {
                return top + EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2 + down;
            }
            else
            {
                return top + EditorGUIUtility.singleLineHeight + down;
            }
        }
    }
}