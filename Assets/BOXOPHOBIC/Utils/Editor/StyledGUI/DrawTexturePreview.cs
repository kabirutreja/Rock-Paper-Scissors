// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic.StyledGUI
{
    public partial class StyledGUI
    {
        public static int DrawTexturePreview(Texture texture, Material previewMaterial, int previewChannel)
        {
            GUILayout.Space(10);

            var styledText = new GUIStyle(EditorStyles.toolbarButton)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Normal,
                fontSize = 10,
            };

            var styledPopup = new GUIStyle(EditorStyles.toolbarPopup)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10,
            };

            previewMaterial.SetTexture("_PreviewTex", texture);
            previewMaterial.SetInt("_PreviewChannel", previewChannel);

#if UNITY_2022_3_OR_NEWER
            if (texture.isDataSRGB)
            {
                previewMaterial.SetInt("_PreviewLinear", 0);
            }
            else
            {
                previewMaterial.SetInt("_PreviewLinear", 1);
            }
#endif

            var rect = GUILayoutUtility.GetRect(0, 0, Screen.width, 0);

            EditorGUI.DrawPreviewTexture(rect, texture, previewMaterial, ScaleMode.ScaleAndCrop, 1, 0);

            GUILayout.Space(2);

            GUILayout.BeginHorizontal();

            GUILayout.Label((UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(texture) / 1024f / 1024f).ToString("F2") + " mb", styledText);
            GUILayout.Space(-1);
            GUILayout.Label(texture.width.ToString() + " px", styledText);
            GUILayout.Space(-1);
            GUILayout.Label(texture.graphicsFormat.ToString(), styledText);
            GUILayout.Space(-1);

#if UNITY_2022_3_OR_NEWER
            if (texture.isDataSRGB)
            {
                GUILayout.Label("sRGB", styledText);
            }
            else
            {
                GUILayout.Label("Linear", styledText);
            }
#endif

            GUILayout.Space(-1);

            previewChannel = EditorGUILayout.Popup(previewChannel, new string[] { "RGB", "R", "G", "B", "A", "Split" }, styledPopup, GUILayout.MaxWidth(60));

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            return previewChannel;
        }
    }
}

