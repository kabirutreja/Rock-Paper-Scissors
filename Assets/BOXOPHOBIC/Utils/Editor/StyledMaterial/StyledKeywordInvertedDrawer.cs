// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;

namespace Boxophobic.StyledGUI
{
    public class StyledKeywordInvertedDrawer : MaterialPropertyDrawer
    {
        public string parentsStr = "";
        public string propertiesStr = "";
        public string keywordsStr = "";

        public StyledKeywordInvertedDrawer(string propertiesStr, string keywordsStr)
        {
            this.propertiesStr = propertiesStr;
            this.keywordsStr = keywordsStr;
        }

        public StyledKeywordInvertedDrawer(string parentsStr, string propertiesStr, string keywordsStr)
        {
            this.propertiesStr = propertiesStr;
            this.keywordsStr = keywordsStr;
            this.parentsStr = parentsStr;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {
            Material material = materialEditor.target as Material;

            var parents = keywordsStr.Split(" ");
            var properties = propertiesStr.Split(" ");
            var keywords = keywordsStr.Split(" ");

            var parentCount = parents.Length;
            var propertiesCount = properties.Length;
            var keywordsCount = keywords.Length;

            if (parentCount == 0)
            {
                if (propertiesCount == 0)
                {
                    if (keywordsCount == 0)
                    {
                        SetMaterialKeywordInverted(material, properties[0], keywords[0]);
                    }
                }
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return -2;
        }

        void SetMaterialKeywordInverted(Material material, string property, string keyword)
        {
            if (material.HasFloat(property))
            {
                var mode = material.GetFloat(property);

                if (mode == 0)
                {
                    material.EnableKeyword(keyword);
                }
                else
                {
                    material.DisableKeyword(keyword);
                }
            }
        }

    }
}
