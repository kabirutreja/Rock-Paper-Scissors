using UnityEngine;
using UnityEditor;

namespace Boxophobic.StyledGUI
{
    [CustomPropertyDrawer(typeof(StyledLayers))]
    public class StyledLayersAttributeDrawer : PropertyDrawer
    {
        private static readonly string[] allLayers = new string[32];

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            StyledLayers a = (StyledLayers)attribute;

            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                allLayers[i] = string.IsNullOrEmpty(layerName) ? "Missing" : layerName;
            }

            string display = string.IsNullOrEmpty(a.display) ? label.text : a.display;

            EditorGUI.BeginChangeCheck();

            int index = EditorGUI.Popup(position, display, property.intValue, allLayers);

            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = index;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}