// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic.StyledGUI
{
    public class StyledTextureDrawer : MaterialPropertyDrawer
    {
        public float size;
        public float top;
        public float down;
        public string tooltip = "";

        public StyledTextureDrawer()
        {
            this.size = 50;
            this.top = 0;
            this.down = 0;
        }

        public StyledTextureDrawer(float size)
        {
            this.size = size;
            this.top = 0;
            this.down = 0;
        }

        public StyledTextureDrawer(float size, string tooltip)
        {
            this.size = size;
            this.top = 0;
            this.down = 0;
            this.tooltip = tooltip;
        }

        public StyledTextureDrawer(float size, float top, float down)
        {
            this.size = size;
            this.top = top;
            this.down = down;
        }

        public StyledTextureDrawer(float size, string tooltip, float top, float down)
        {
            this.size = size;
            this.top = top;
            this.down = down;
            this.tooltip = tooltip;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor materialEditor)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.showMixedValue = prop.hasMixedValue;

            Texture tex = null;

            Rect textureRect = new Rect(position.x, position.y + top, position.width, size);

            GUIContent content = new GUIContent(prop.displayName, tooltip);

            if (prop.textureDimension == UnityEngine.Rendering.TextureDimension.Tex2D)
            {
                tex = (Texture2D)EditorGUI.ObjectField(textureRect, content, prop.textureValue, typeof(Texture2D), false);
            }

            if (prop.textureDimension == UnityEngine.Rendering.TextureDimension.Cube)
            {
                tex = (Cubemap)EditorGUI.ObjectField(textureRect, content, prop.textureValue, typeof(Cubemap), false);
            }

            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                prop.textureValue = tex;
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return top + size + down;
        }
    }
}