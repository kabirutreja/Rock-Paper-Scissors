// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;

namespace Boxophobic.StyledGUI
{
    public class StyledVector3Drawer : MaterialPropertyDrawer
    {
        public float space = 0;
        public float top = 0;
        public float down = 0;

        public StyledVector3Drawer()
        {
            this.space = 0;
        }

        public StyledVector3Drawer(float space)
        {
            this.space = space;
        }

        public StyledVector3Drawer(float space, float top, float down)
        {
            this.space = space;
            this.top = top;
            this.down = down;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {
            float y = position.y + top;
            float height = EditorGUIUtility.singleLineHeight;

            if (EditorGUIUtility.currentViewWidth > 330)
            {
                DrawVectorProperty(new Rect(position.x, y, position.width, height), prop, label);

                y += height - space;
            }
            else
            {
                DrawVectorPropertyNextLine(new Rect(position.x, y, position.width, height * 2 + 2), prop, label);

                y += height * 2 + 2;
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (EditorGUIUtility.currentViewWidth > 330)
            {
                return top + height - space + down;
            }

            return top + height * 2 + 2 + down;
        }

        void DrawVectorProperty(Rect position, MaterialProperty prop, string label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;

            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth - 1, position.height);
            Rect fieldRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, position.height);

            EditorGUI.LabelField(labelRect, label);

            Vector4 vec = EditorGUI.Vector3Field(fieldRect, GUIContent.none, prop.vectorValue);

            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = vec;
            }
        }

        void DrawVectorPropertyNextLine(Rect position, MaterialProperty prop, string label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;

            Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Rect fieldRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(labelRect, label);

            Vector4 vec = EditorGUI.Vector3Field(fieldRect, GUIContent.none, prop.vectorValue);

            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = vec;
            }
        }
    }
}