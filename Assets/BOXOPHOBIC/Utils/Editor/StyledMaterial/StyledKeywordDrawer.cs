// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;

namespace Boxophobic.StyledGUI
{
    public class StyledKeywordDrawer : MaterialPropertyDrawer
    {
        public string allParentOnStr = "false";
        public string parentsStr = "";
        public string propertiesStr = "";
        public string keywordsStr = "";

        public StyledKeywordDrawer(string allParentOnStr, string propertiesStr, string keywordsStr)
        {
            this.allParentOnStr = allParentOnStr;
            this.propertiesStr = propertiesStr;
            this.keywordsStr = keywordsStr;
        }

        public StyledKeywordDrawer(string allParentOnStr, string parentsStr, string propertiesStr, string keywordsStr)
        {
            this.allParentOnStr = allParentOnStr;
            this.propertiesStr = propertiesStr;
            this.keywordsStr = keywordsStr;
            this.parentsStr = parentsStr;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {
            Material material = materialEditor.target as Material;

            var parents = parentsStr.Split(" ");
            var properties = propertiesStr.Split(" ");
            var keywords = keywordsStr.Split(" ");

            var parentCount = parents.Length;
            var propertiesCount = properties.Length;
            var keywordsCount = keywords.Length;

            bool allParentsOn = false;

            if (allParentOnStr == "true")
            {
                allParentsOn = true;
            }

            if (parentCount == 0)
            {
                if (propertiesCount == 0)
                {
                    if (keywordsCount == 0)
                    {
                        SetMaterialKeyword(material, properties[0], keywords[0]);
                    }
                    else
                    {
                        SetMaterialKeyword(material, properties[0], keywords);
                    }
                }
                else
                {
                    SetMaterialKeyword(material, allParentsOn, properties, keywords[0]);
                }
            }
            else if (parentCount == 1)
            {
                if (propertiesCount == 0)
                {
                    if (keywordsCount == 0)
                    {
                        SetMaterialKeyword(material, parents[0], properties[0], keywords[0]);
                    }
                    else
                    {
                        SetMaterialKeyword(material, parents[0], properties[0], keywords);
                    }
                }
            }
            else
            {
                if (propertiesCount == 0)
                {
                    if (keywordsCount == 0)
                    {
                        SetMaterialKeyword(material, allParentsOn, parents, properties[0], keywords[0]);
                    }
                    else
                    {
                        SetMaterialKeyword(material, allParentsOn, parents, properties[0], keywords);
                    }
                }
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return -2;
        }

        void SetMaterialKeyword(Material material, string property, string keyword)
        {
            if (material.HasFloat(property))
            {
                var mode = material.GetFloat(property);

                if (mode == 0)
                {
                    material.DisableKeyword(keyword);
                }
                else
                {
                    material.EnableKeyword(keyword);
                }
            }
        }

        void SetMaterialKeyword(Material material, string property, string[] keywords)
        {
            if (material.HasFloat(property))
            {
                var mode = material.GetFloat(property);

                for (int i = 0; i < keywords.Length; i++)
                {
                    if (i == mode)
                    {
                        material.EnableKeyword(keywords[i]);
                    }
                    else
                    {
                        material.DisableKeyword(keywords[i]);
                    }
                }
            }
            else
            {
                material.EnableKeyword(keywords[0]);

                for (int i = 1; i < keywords.Length; i++)
                {
                    material.DisableKeyword(keywords[i]);
                }
            }
        }

        void SetMaterialKeyword(Material material, string parent, string property, string keyword)
        {
            if (material.HasFloat(parent) && material.HasFloat(property))
            {
                var parentMode = material.GetFloat(parent);

                if (parentMode > 0)
                {
                    var propertyMode = material.GetFloat(property);

                    if (propertyMode == 0)
                    {
                        material.DisableKeyword(keyword);
                    }
                    else
                    {
                        material.EnableKeyword(keyword);
                    }
                }
                else
                {
                    material.DisableKeyword(keyword);
                }
            }
        }

        void SetMaterialKeyword(Material material, string parent, string property, string[] keywords)
        {
            if (material.HasFloat(parent) && material.HasFloat(property))
            {
                var parentMode = material.GetFloat(parent);

                if (parentMode > 0)
                {
                    var propertyMode = material.GetFloat(property);

                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (i == propertyMode)
                        {
                            material.EnableKeyword(keywords[i]);
                        }
                        else
                        {
                            material.DisableKeyword(keywords[i]);
                        }
                    }
                }
                else
                {
                    material.EnableKeyword(keywords[0]);

                    for (int i = 1; i < keywords.Length; i++)
                    {
                        material.DisableKeyword(keywords[i]);
                    }
                }
            }
        }

        void SetMaterialKeyword(Material material, bool allParentsOn, string[] parents, string property, string keyword)
        {
            bool parentMode = false;
            float propertyMode = 0;

            if (allParentsOn)
            {
                int enableCount = 0;

                for (int i = 0; i < parents.Length; i++)
                {
                    var parent = parents[i];

                    if (material.HasProperty(parent))
                    {
                        if (material.GetFloat(parent) > 0)
                        {
                            enableCount++;
                        }
                    }
                }

                if (parents.Length == enableCount)
                {
                    parentMode = true;
                }
            }
            else
            {
                float enableFloat = 0;

                for (int i = 0; i < parents.Length; i++)
                {
                    var parent = parents[i];

                    if (material.HasProperty(parent))
                    {
                        enableFloat += material.GetFloat(parent);
                    }
                }

                if (enableFloat > 0)
                {
                    parentMode = true;
                }
            }

            if (material.HasProperty(property))
            {
                propertyMode = material.GetFloat(property);
            }

            if (parentMode && propertyMode > 0)
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
            }
        }

        void SetMaterialKeyword(Material material, bool allParentsOn, string[] parents, string property, string[] keywords)
        {
            bool parentsMode = false;

            if (allParentsOn)
            {
                int enableCount = 0;

                for (int i = 0; i < parents.Length; i++)
                {
                    var parent = parents[i];

                    if (material.HasProperty(parent))
                    {
                        if (material.GetFloat(parent) > 0)
                        {
                            enableCount++;
                        }
                    }
                }

                if (parents.Length == enableCount)
                {
                    parentsMode = true;
                }
            }
            else
            {
                float enableFloat = 0;

                for (int i = 0; i < parents.Length; i++)
                {
                    var parent = parents[i];

                    if (material.HasProperty(parent))
                    {
                        enableFloat += material.GetFloat(parent);
                    }
                }

                if (enableFloat > 0)
                {
                    parentsMode = true;
                }
            }

            if (material.HasFloat(property))
            {
                if (parentsMode)
                {
                    var propertyMode = material.GetInt(property);

                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (i == propertyMode)
                        {
                            material.EnableKeyword(keywords[i]);
                        }
                        else
                        {
                            material.DisableKeyword(keywords[i]);
                        }
                    }
                }
                else
                {
                    material.EnableKeyword(keywords[0]);

                    for (int i = 1; i < keywords.Length; i++)
                    {
                        material.DisableKeyword(keywords[i]);
                    }
                }
            }
        }

        void SetMaterialKeyword(Material material, bool allParentsOn, string[] properties, string keyword)
        {
            bool parentMode = false;

            if (allParentsOn)
            {
                int enableCount = 0;

                for (int i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];

                    if (material.HasProperty(property))
                    {
                        if (material.GetFloat(property) > 0)
                        {
                            enableCount++;
                        }
                    }
                }

                if (properties.Length == enableCount)
                {
                    parentMode = true;
                }
            }
            else
            {
                float enableFloat = 0;

                for (int i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];

                    if (material.HasProperty(property))
                    {
                        enableFloat += material.GetFloat(property);
                    }
                }

                if (enableFloat > 0)
                {
                    parentMode = true;
                }
            }

            if (parentMode)
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
