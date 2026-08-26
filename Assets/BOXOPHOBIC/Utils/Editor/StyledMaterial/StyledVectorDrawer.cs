// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;

namespace Boxophobic.StyledGUI
{
    public class StyledVectorDrawer : MaterialPropertyDrawer
    {
        public float space = 0;
        public float top = 0;
        public float down = 0;

        public StyledVectorDrawer(float space)
        {
            this.space = space;
        }

        public StyledVectorDrawer(float space, float top, float down)
        {
            this.space = space;
            this.top = top;
            this.down = down;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {
            Rect rect = new Rect(position.x, position.y + top, position.width, EditorGUIUtility.singleLineHeight * 2);

            if (EditorGUIUtility.currentViewWidth > 344)
            {
                rect.height -= space;
            }
            else
            {
                rect.height += 2;
            }

            prop.vectorValue = EditorGUI.Vector4Field(rect, prop.displayName, prop.vectorValue);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (EditorGUIUtility.currentViewWidth > 344)
            {
                return top + EditorGUIUtility.singleLineHeight * 2 - space + down;
            }

            return top + EditorGUIUtility.singleLineHeight * 2 + 2 + down;
        }
    }
}