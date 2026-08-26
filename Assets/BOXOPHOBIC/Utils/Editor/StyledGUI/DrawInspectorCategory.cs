// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic.Constants;
using Boxophobic.Utility;

namespace Boxophobic.StyledGUI
{
    public partial class StyledGUI
    {
        public static void DrawInspectorCategory(string bannerText)
        {
            GUI.contentColor = Color.white;
            GUI.color = new Color(1, 1, 1, 0.9f);

            var fullRect = GUILayoutUtility.GetRect(0, 0, 18, 0);
            var fillRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 18);
            var lineRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 1);
            var titleRect = new Rect(fullRect.position.x - 1, fullRect.position.y, fullRect.width, 18);

            EditorGUI.DrawRect(fillRect, Constant.CategoryColor);
            EditorGUI.DrawRect(lineRect, Constant.LineColor);

            bannerText = FormatBannerText(bannerText);

            GUI.Label(titleRect, bannerText, Constant.HeaderStyle);

            GUI.color = Color.white;
        }

        public static void DrawInspectorCategory(string bannerText, string message)
        {
            GUI.contentColor = Color.white;
            GUI.color = new Color(1, 1, 1, 0.9f);

            var fullRect = GUILayoutUtility.GetRect(0, 0, 18, 0);
            var fillRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 18);
            var lineRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 1);
            var titleRect = new Rect(fullRect.position.x - 1, fullRect.position.y, fullRect.width, 18);

            EditorGUI.DrawRect(fillRect, Constant.CategoryColor);
            EditorGUI.DrawRect(lineRect, Constant.LineColor);

            bannerText = FormatBannerText(bannerText);
            message = BoxoUtils.FormatMessage(message);

            var bannerContext = new GUIContent(bannerText, message);
            GUI.Label(titleRect, bannerContext, Constant.HeaderStyle);

            GUI.color = Color.white;
        }

        public static bool DrawInspectorCategory(string bannerText, bool enabled, bool colapsable, float top, float down)
        {
            GUI.contentColor = Color.white;
            GUI.color = new Color(1, 1, 1, 0.9f);

            if (colapsable)
            {
                if (enabled)
                {
                    GUILayout.Space(top);
                }
                else
                {
                    GUILayout.Space(0);
                }
            }
            else
            {
                GUILayout.Space(top);
            }

            var fullRect = GUILayoutUtility.GetRect(0, 0, 18, 0);
            var fillRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 18);
            var lineRect = new Rect(0, fullRect.y - 1, fullRect.xMax + 10, 1);
            var titleRect = new Rect(fullRect.position.x - 1, fullRect.position.y, fullRect.width, 18);
            var arrowRect = new Rect(fullRect.position.x - 15, fullRect.position.y - 1, fullRect.width, 18);

            if (colapsable)
            {
                if (GUI.Button(arrowRect, "", GUIStyle.none))
                {
                    enabled = !enabled;
                }
            }
            else
            {
                enabled = true;
            }

            EditorGUI.DrawRect(fillRect, Constant.CategoryColor);
            EditorGUI.DrawRect(lineRect, Constant.LineColor);

            bannerText = FormatBannerText(bannerText);

            GUI.Label(titleRect, bannerText, Constant.HeaderStyle);

            GUI.color = new Color(1, 1, 1, 0.39f);

            if (colapsable)
            {
                if (enabled)
                {
                    GUI.Label(arrowRect, "<size=10>▼</size>", Constant.HeaderStyle);
                    GUILayout.Space(down);
                }
                else
                {
                    GUI.Label(arrowRect, "<size=10>►</size>", Constant.HeaderStyle);
                    GUILayout.Space(0);
                }
            }
            else
            {
                GUILayout.Space(down);
            }

            GUI.color = Color.white;
            return enabled;
        }

        public static bool DrawInspectorCategory(string bannerText, bool enabled, bool colapsable, string message, float top, float down)
        {
            GUI.contentColor = Color.white;
            GUI.color = new Color(1, 1, 1, 0.9f);

            if (colapsable)
            {
                if (enabled)
                {
                    GUILayout.Space(top);
                }
                else
                {
                    GUILayout.Space(0);
                }
            }
            else
            {
                GUILayout.Space(top);
            }

            var fullRect = GUILayoutUtility.GetRect(0, 0, 18, 0);
            var fillRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 18);
            var lineRect = new Rect(0, fullRect.y - 1, fullRect.xMax + 10, 1);
            var titleRect = new Rect(fullRect.position.x - 1, fullRect.position.y, fullRect.width, 18);
            var arrowRect = new Rect(fullRect.position.x - 15, fullRect.position.y - 1, fullRect.width, 18);

            if (colapsable)
            {
                if (GUI.Button(arrowRect, "", GUIStyle.none))
                {
                    enabled = !enabled;
                }
            }
            else
            {
                enabled = true;
            }

            EditorGUI.DrawRect(fillRect, Constant.CategoryColor);
            EditorGUI.DrawRect(lineRect, Constant.LineColor);

            bannerText = FormatBannerText(bannerText);
            message = BoxoUtils.FormatMessage(message);

            var bannerContext = new GUIContent(bannerText, message);
            GUI.Label(titleRect, bannerContext, Constant.HeaderStyle);

            GUI.color = new Color(1, 1, 1, 0.39f);

            if (colapsable)
            {
                if (enabled)
                {
                    GUI.Label(arrowRect, "<size=10>▼</size>", Constant.HeaderStyle);
                    GUILayout.Space(down);
                }
                else
                {
                    GUI.Label(arrowRect, "<size=10>►</size>", Constant.HeaderStyle);
                    GUILayout.Space(0);
                }
            }
            else
            {
                GUILayout.Space(down);
            }

            GUI.color = Color.white;
            return enabled;
        }

        public static bool DrawInspectorCategory(string bannerText, bool enabled, bool colapsable, string dotColor, string message, float top, float down)
        {
            GUI.contentColor = Color.white;
            GUI.color = new Color(1, 1, 1, 0.9f);

            if (colapsable)
            {
                if (enabled)
                {
                    GUILayout.Space(top);
                }
                else
                {
                    GUILayout.Space(0);
                }
            }
            else
            {
                GUILayout.Space(top);
            }

            var fullRect = GUILayoutUtility.GetRect(0, 0, 18, 0);
            var fillRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 18);
            var lineRect = new Rect(0, fullRect.y - 1, fullRect.xMax + 10, 1);
            var titleRect = new Rect(fullRect.position.x - 1, fullRect.position.y, fullRect.width, 18);
            var arrowRect = new Rect(fullRect.position.x - 15, fullRect.position.y - 1, fullRect.width, 18);
            var dotRect = new Rect(fullRect.xMax - 18, fullRect.y - 1, 18, 18);

            if (colapsable)
            {
                if (GUI.Button(arrowRect, "", GUIStyle.none))
                {
                    enabled = !enabled;
                }
            }
            else
            {
                enabled = true;
            }

            EditorGUI.DrawRect(fillRect, Constant.CategoryColor);
            EditorGUI.DrawRect(lineRect, Constant.LineColor);

            bannerText = FormatBannerText(bannerText);
            message = BoxoUtils.FormatMessage(message);

            var bannerContext = new GUIContent(bannerText, message);
            GUI.Label(titleRect, bannerContext, Constant.HeaderStyle);

            GUI.color = new Color(1, 1, 1, 0.39f);

            if (colapsable)
            {
                if (enabled)
                {
                    GUI.Label(arrowRect, "<size=10>▼</size>", Constant.HeaderStyle);
                    GUILayout.Space(down);
                }
                else
                {
                    GUI.Label(arrowRect, "<size=10>►</size>", Constant.HeaderStyle);
                    GUILayout.Space(0);
                }
            }
            else
            {
                GUILayout.Space(down);
            }

            GUI.color = Color.white;

            if (dotColor != "NONE")
            {
                string subtitle;

                if (EditorGUIUtility.isProSkin)
                {
                    subtitle = "<color=#" + dotColor + ">●</color>";
                }
                else
                {
                    subtitle = "●";
                }

                GUI.Label(dotRect, "<size=9>" + subtitle + "</size>", Constant.TitleStyle);
            }

            return enabled;
        }

        static string FormatBannerText(string bannerText)
        {
            if (bannerText.Contains("_"))
            {
                var splitBanner = bannerText.Split("_");

                bannerText = splitBanner[0] + " (" + splitBanner[1] + ")";
            }

            return bannerText;
        }
    }
}

