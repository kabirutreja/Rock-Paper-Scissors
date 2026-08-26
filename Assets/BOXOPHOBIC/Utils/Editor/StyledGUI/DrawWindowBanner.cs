// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic.Constants;

namespace Boxophobic.StyledGUI
{
    public partial class StyledGUI
    {
        public static void DrawWindowBanner(Color color, string title)
        {
            GUILayout.Space(15);

            var fullRect = GUILayoutUtility.GetRect(0, 0, 36, 0);
            var fillRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 36);
            var lineRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 1);

            if (EditorGUIUtility.isProSkin)
            {
                color = new Color(color.r, color.g, color.b, 1f);
            }
            else
            {
                color = Constant.ColorLightGray;
            }

            EditorGUI.DrawRect(fillRect, color);
            EditorGUI.DrawRect(lineRect, Constant.LineColor);

            Color guiColor = Constant.ColorDarkGray;

            GUI.Label(fullRect, "<size=16><color=#" + ColorUtility.ToHtmlStringRGB(guiColor) + ">" + title + "</color></size>", Constant.TitleStyle);

            GUILayout.Space(15);
        }

        public static void DrawWindowBanner(string title)
        {
            GUILayout.Space(15);

            var fullRect = GUILayoutUtility.GetRect(0, 0, 36, 0);
            var fillRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 36);
            var lineRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 1);

            Color color;
            Color guiColor;

            if (EditorGUIUtility.isProSkin)
            {
                color = Constant.ColorDarkGray;
                guiColor = Constant.ColorLightGray;
            }
            else
            {
                color = Constant.ColorLightGray;
                guiColor = Constant.ColorDarkGray;
            }

            EditorGUI.DrawRect(fillRect, color);
            EditorGUI.DrawRect(lineRect, Constant.LineColor);

            GUI.Label(fullRect, "<size=16><color=#" + ColorUtility.ToHtmlStringRGB(guiColor) + ">" + title + "</color></size>", Constant.TitleStyle);

            GUILayout.Space(15);
        }

        public static void DrawWindowBanner(Color color, string title, string subtitle)
        {
            GUIStyle titleStyle = new GUIStyle("label")
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter
            };

            GUIStyle subTitleStyle = new GUIStyle("label")
            {
                richText = true,
                alignment = TextAnchor.MiddleRight
            };

            GUILayout.Space(15);

            var fullRect = GUILayoutUtility.GetRect(0, 0, 36, 0);
            var fillRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 36);
            var subRect = new Rect(0, fullRect.y, fullRect.xMax - 14, 36);
            var lineRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 1);

            if (EditorGUIUtility.isProSkin)
            {
                color = new Color(color.r, color.g, color.b, 1f);
            }
            else
            {
                color = Constant.ColorLightGray;
            }

            EditorGUI.DrawRect(fillRect, color);
            EditorGUI.DrawRect(lineRect, Constant.LineColor);

            Color guiColor = Constant.ColorDarkGray;

            GUI.Label(fullRect, "<size=16><color=#" + ColorUtility.ToHtmlStringRGB(guiColor) + ">" + title + "</color></size>", titleStyle);
            GUI.Label(subRect, "<b><size=11><color=#" + ColorUtility.ToHtmlStringRGB(guiColor) + ">" + subtitle + "</color></size></b>", subTitleStyle);

            GUILayout.Space(15);
        }

        public static void DrawWindowBanner(string title, string subtitle)
        {
            GUIStyle titleStyle = new GUIStyle("label")
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter
            };

            GUIStyle subTitleStyle = new GUIStyle("label")
            {
                richText = true,
                alignment = TextAnchor.MiddleRight
            };

            GUILayout.Space(15);

            var fullRect = GUILayoutUtility.GetRect(0, 0, 36, 0);
            var subRect = new Rect(0, fullRect.y, fullRect.xMax - 18, 36);
            var fillRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 36);

            Color color;
            Color guiColor;

            if (EditorGUIUtility.isProSkin)
            {
                color = Constant.ColorDarkGray;
                guiColor = Constant.ColorLightGray;
            }
            else
            {
                color = Constant.ColorLightGray;
                guiColor = Constant.ColorDarkGray;
            }
            EditorGUI.DrawRect(fillRect, color);

            GUI.Label(fullRect, "<size=16><color=#" + ColorUtility.ToHtmlStringRGB(guiColor) + ">" + title + "</color></size>", titleStyle);
            GUI.Label(subRect, "<b><size=11><color=#" + ColorUtility.ToHtmlStringRGB(guiColor) + ">" + subtitle + "</color></size></b>", subTitleStyle);

            GUILayout.Space(15);
        }
    }
}

