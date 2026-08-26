// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;

namespace Boxophobic.StyledGUI
{
    public class StyledHeaderDrawer : MaterialPropertyDrawer
    {
        public bool isEnabled = true;

        public string category;
        public string colapsable;
        public string infoText = "";
        public float top;
        public float down;

        public StyledHeaderDrawer(string category)
        {
            this.category = category;
            this.colapsable = "false";
            this.top = 10;
            this.down = 10;
        }

        public StyledHeaderDrawer(string category, string colapsable)
        {
            this.category = category;
            this.colapsable = colapsable;
            this.top = 10;
            this.down = 10;
        }

        public StyledHeaderDrawer(string category, float top, float down)
        {
            this.category = category;
            this.colapsable = "false";
            this.top = top;
            this.down = down;
        }

        public StyledHeaderDrawer(string category, string colapsable, float top, float down)
        {
            this.category = category;
            this.colapsable = colapsable;
            this.top = top;
            this.down = down;
        }
        public StyledHeaderDrawer(string category, string colapsable, string infoText, float top, float down)
        {
            this.category = category;
            this.colapsable = colapsable;
            this.infoText = infoText;
            this.top = top;
            this.down = down;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {
            GUI.enabled = true;
            EditorGUI.indentLevel = 0;

            DrawInspector(prop);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return -2;
        }

        void DrawInspector(MaterialProperty prop)
        {
            bool isColapsable = false;

            if (colapsable == "true")
            {
                isColapsable = true;
            }

            //bool isEnabled = true;

            if (prop.floatValue < 0.5f)
            {
                isEnabled = false;
            }
            else
            {
                isEnabled = true;
            }

            if (infoText != "")
            {
                isEnabled = StyledGUI.DrawInspectorHeader(category, isEnabled, isColapsable, infoText, top, down);
            }
            else
            {
                isEnabled = StyledGUI.DrawInspectorHeader(category, isEnabled, isColapsable, top, down);
            }

            if (isEnabled)
            {
                prop.floatValue = 1;
            }
            else
            {
                prop.floatValue = 0;
            }
        }
    }
}
