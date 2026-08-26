// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;
using Boxophobic.Utility;

namespace Boxophobic.StyledGUI
{
    public class StyledMessageDrawer : MaterialPropertyDrawer
    {
        public string type;
        public string message;
        public string messageLong;
        public string keyword;
        public float value;
        public float top;
        public float down;

        public bool useMessageLong;

        MessageType messageType;

        public StyledMessageDrawer(string type, string message)
        {
            this.type = type;
            this.message = message;
            this.messageLong = "";
            keyword = null;

            this.top = 0;
            this.down = 0;
        }

        public StyledMessageDrawer(string type, string message, float top, float down)
        {
            this.type = type;
            this.message = message;
            this.messageLong = "";
            keyword = null;

            this.top = top;
            this.down = down;
        }

        public StyledMessageDrawer(string type, string message, string keyword, float value, float top, float down)
        {
            this.type = type;
            this.message = message;
            this.messageLong = "";
            this.keyword = keyword;
            this.value = value;

            this.top = top;
            this.down = down;
        }

        public StyledMessageDrawer(string type, string message, string messageLong)
        {
            this.type = type;
            this.message = message;
            this.messageLong = messageLong;
            keyword = null;

            this.top = 0;
            this.down = 0;
        }

        public StyledMessageDrawer(string type, string message, string messageLong, float top, float down)
        {
            this.type = type;
            this.message = message;
            this.messageLong = messageLong;
            keyword = null;

            this.top = top;
            this.down = down;
        }

        public StyledMessageDrawer(string type, string message, string messageLong, string keyword, float value, float top, float down)
        {
            this.type = type;
            this.message = message;
            this.messageLong = messageLong;
            this.keyword = keyword;
            this.value = value;

            this.top = top;
            this.down = down;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {
            Material material = materialEditor.target as Material;

            if (type == "None")
            {
                messageType = MessageType.None;
            }
            else if (type == "Info")
            {
                messageType = MessageType.Info;
            }
            else if (type == "Warning")
            {
                messageType = MessageType.Warning;
            }
            else if (type == "Error")
            {
                messageType = MessageType.Error;
            }

            if (keyword != null)
            {
                if (material.HasProperty(keyword))
                {
                    if (material.GetFloat(keyword) == value)
                    {
                        DrawMessage(position, prop);
                    }
                }
            }
            else
            {
                DrawMessage(position, prop);
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            Material material = editor.target as Material;

            if (keyword != null)
            {
                if (!material.HasProperty(keyword) || material.GetFloat(keyword) != value)
                {
                    return 0;
                }
            }

            string text;

            if (messageLong == "")
            {
                text = BoxoUtils.FormatMessage(message);
            }
            else
            {
                if (!useMessageLong)
                {
                    text = BoxoUtils.FormatMessage(message);
                }
                else
                {
                    text = BoxoUtils.FormatMessage(messageLong);
                }
            }

            float height = Mathf.Max(36, EditorStyles.helpBox.CalcHeight(new GUIContent(text), EditorGUIUtility.currentViewWidth - 30));

            return top + height + down;
        }

        void DrawMessage(Rect position, MaterialProperty prop)
        {
            string text;

            if (messageLong == "")
            {
                text = BoxoUtils.FormatMessage(message);
            }
            else
            {
                if (!useMessageLong)
                {
                    text = BoxoUtils.FormatMessage(message);
                }
                else
                {
                    text = BoxoUtils.FormatMessage(messageLong);
                }
            }

            float helpHeight = Mathf.Max(36, EditorStyles.helpBox.CalcHeight(new GUIContent(text), position.width - 30f));

            Rect messageRect = new Rect(position.x, position.y + top, position.width, helpHeight);

            EditorGUI.HelpBox(messageRect, text, messageType);

            if (messageLong != "")
            {
                if (GUI.Button(messageRect, GUIContent.none, GUIStyle.none))
                {
                    useMessageLong = !useMessageLong;
                }
            }
        }
    }
}