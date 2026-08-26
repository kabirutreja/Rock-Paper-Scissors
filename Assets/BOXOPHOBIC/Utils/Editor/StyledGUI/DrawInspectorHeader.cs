// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using Boxophobic.Constants;

namespace Boxophobic.StyledGUI
{
    public partial class StyledGUI 
    {
        public static void DrawInspectorHeader(string bannerText)
        {
            GUI.contentColor = Color.white;
            GUI.color = new Color(1, 1, 1, 0.9f);

            var fullRect = GUILayoutUtility.GetRect(0, 0, 18, 0);
            var titleRect = new Rect(fullRect.position.x - 1, fullRect.position.y, fullRect.width, 18);

            bannerText = DrawHeaderText(bannerText);

            GUI.Label(titleRect, bannerText, Constant.HeaderStyle);

            GUI.color = Color.white;
        }

        public static bool DrawInspectorHeader(string bannerText, bool enabled, bool colapsable, float top, float down)
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

            bannerText = DrawHeaderText(bannerText);

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

        public static bool DrawInspectorHeader(string bannerText, bool enabled, bool colapsable, string infoText, float top, float down)
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

            bannerText = DrawHeaderText(bannerText);

            var bannerContext = new GUIContent(bannerText, infoText);
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

        static string DrawHeaderText(string bannerText)
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

