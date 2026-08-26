//Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic.Utility;
using System.Collections.Generic;

namespace SkyboxExtended
{
    public class MaterialGUI : ShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            //base.OnGUI(materialEditor, props);

            var material0 = materialEditor.target as Material;

            DrawDynamicInspector(material0, materialEditor, props);
        }

        void DrawDynamicInspector(Material material, MaterialEditor materialEditor, MaterialProperty[] props)
        {
            var customPropsList = new List<MaterialProperty>();

            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];

                if (BoxoUtils.IsShaderGUIPropertyHidden(prop))
                {
                    continue;
                }

                customPropsList.Add(prop);
            }

            //Draw Custom GUI
            for (int i = 0; i < customPropsList.Count; i++)
            {
                var prop = customPropsList[i];

                materialEditor.ShaderProperty(prop, prop.displayName);
            }

            GUILayout.Space(10);
        }
    }
}
