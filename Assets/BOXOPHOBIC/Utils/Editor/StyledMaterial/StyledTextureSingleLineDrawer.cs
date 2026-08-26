// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic.Utility;

namespace Boxophobic.StyledGUI
{
    public class StyledTextureSingleLineDrawer : MaterialPropertyDrawer
    {
        public float top;
        public float down;
        public string tooltip;

        int previewChannel = 0;
        Material previewMaterial;

        bool showAdvancedSettings = false;

        public StyledTextureSingleLineDrawer()
        {
            this.top = 0;
            this.down = 0;
            this.tooltip = "";
        }
        public StyledTextureSingleLineDrawer(string tooltip)
        {
            this.top = 0;
            this.down = 0;
            this.tooltip = tooltip;
        }

        public StyledTextureSingleLineDrawer(float top, float down)
        {
            this.top = top;
            this.down = down;
            this.tooltip = "";
        }

        public StyledTextureSingleLineDrawer(string tooltip, float top, float down)
        {
            this.top = top;
            this.down = down;
            this.tooltip = tooltip;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor materialEditor)
        {
            GUILayout.Space(top);

            tooltip = BoxoUtils.FormatMessage(tooltip);

            materialEditor.TexturePropertySingleLine(new GUIContent(prop.displayName, tooltip), prop);

            if (prop.textureValue != null && prop.textureValue.dimension == UnityEngine.Rendering.TextureDimension.Tex2D)
            {
                var lastRect = GUILayoutUtility.GetLastRect();

                if (GUI.Button(lastRect, "", GUIStyle.none))
                {
                    showAdvancedSettings = !showAdvancedSettings;
                }

                if (showAdvancedSettings)
                {
                    if (previewMaterial == null)
                    {
                        previewMaterial = new Material(Shader.Find("Hidden/BOXOPHOBIC/Helpers/Channel Preview"));
                    }

                    previewChannel = StyledGUI.DrawTexturePreview(prop.textureValue, previewMaterial, previewChannel);
                }
            }

            GUILayout.Space(down);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return -2;
        }
    }
}
