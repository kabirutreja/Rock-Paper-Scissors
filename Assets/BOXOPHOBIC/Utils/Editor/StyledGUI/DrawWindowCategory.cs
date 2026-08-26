// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic.Constants;
using Boxophobic.Utility;

namespace Boxophobic.StyledGUI
{
    public partial class StyledGUI 
    {
        public static void DrawWindowCategory(string bannerText)
        {
            GUI.color = new Color(1, 1, 1, 0.9f);

            var fullRect = GUILayoutUtility.GetRect(0, 0, 18, 0);
            var fillRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 18);
            var lineRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 1);
            var titleRect = new Rect(fullRect.position.x + 4, fullRect.position.y, fullRect.width, 18);

            EditorGUI.DrawRect(fillRect, Constant.CategoryColor);
            EditorGUI.DrawRect(lineRect, Constant.LineColor);

            GUI.Label(titleRect, bannerText, Constant.HeaderStyle);

            GUI.color = Color.white;
        }

        public static void DrawWindowCategory(string bannerText, string message)
        {
            GUI.color = new Color(1, 1, 1, 0.9f);

            var fullRect = GUILayoutUtility.GetRect(0, 0, 18, 0);
            var fillRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 18);
            var lineRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 1);
            var titleRect = new Rect(fullRect.position.x + 4, fullRect.position.y, fullRect.width, 18);

            EditorGUI.DrawRect(fillRect, Constant.CategoryColor);
            EditorGUI.DrawRect(lineRect, Constant.LineColor);

            bannerText = FormatBannerText(bannerText);
            message = BoxoUtils.FormatMessage(message);

            var bannerContext = new GUIContent(bannerText, message);
            GUI.Label(titleRect, bannerContext, Constant.HeaderStyle);

            GUI.color = Color.white;
        }

        public static bool DrawWindowCategory(string bannerText, bool enabled, float top, float down, bool colapsable)
        {
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
            var lineRect = new Rect(0, fullRect.y, fullRect.xMax + 10, 1);
            var titleRect = new Rect(fullRect.position.x + 4, fullRect.position.y, fullRect.width, 18);

            if (EditorGUIUtility.isProSkin)
            {
                GUI.color = Constant.ColorDarkGray;
            }
            else
            {
                GUI.color = Constant.ColorLightGray;
            }

            if (colapsable)
            {
                if (GUI.Button(fullRect, "", GUIStyle.none))
                {
                    enabled = !enabled;
                }
            }

            EditorGUI.DrawRect(fillRect, Constant.CategoryColor);
            EditorGUI.DrawRect(lineRect, Constant.LineColor);

            GUI.Label(titleRect, bannerText, Constant.HeaderStyle);

            if (colapsable)
            {
                if (enabled)
                {
                    GUILayout.Space(down);
                }
                else
                {
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
    }
}

