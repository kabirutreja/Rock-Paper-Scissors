// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;

namespace Boxophobic.StyledGUI
{
    public class StyledToggleDrawer : MaterialPropertyDrawer
    {
        public float width = 0;

        public StyledToggleDrawer()
        {

        }

        public StyledToggleDrawer(float width)
        {
            this.width = width;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.showMixedValue = prop.hasMixedValue;

            bool toggle = prop.floatValue > 0.5f;

            if (width == 0)
            {
                toggle = EditorGUI.Toggle(position, label, toggle);
            }
            else
            {
                Rect labelRect = new Rect(position.x, position.y, position.width - width, position.height);

                Rect toggleRect = new Rect(position.xMax - width, position.y, width, position.height);

                EditorGUI.LabelField(labelRect, label);
                toggle = EditorGUI.Toggle(toggleRect, GUIContent.none, toggle);
            }

            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                prop.floatValue = toggle ? 1 : 0;
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}