// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace Boxophobic.StyledGUI
{
    [CustomPropertyDrawer(typeof(StyledTexturePreview))]
    public class StyledTexturePreviewAttributeDrawer : PropertyDrawer
    {
        int channel = 0;
        ColorWriteMask channelMask = ColorWriteMask.All;

        StyledTexturePreview a;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            a = (StyledTexturePreview)attribute;

            var tex = (Texture)property.objectReferenceValue;

            float y = position.y;

            if (a.displayName != "")
            {
                Rect objectRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                Rect labelRect = new Rect(objectRect.x, objectRect.y, EditorGUIUtility.labelWidth - 1, objectRect.height);
                Rect fieldRect = new Rect( objectRect.x + EditorGUIUtility.labelWidth, objectRect.y, objectRect.width - EditorGUIUtility.labelWidth, objectRect.height);

                EditorGUI.LabelField(labelRect, a.displayName);
                tex = (Texture)EditorGUI.ObjectField(fieldRect, tex, typeof(Texture), false);

                property.objectReferenceValue = tex;

                y += EditorGUIUtility.singleLineHeight + 10;
            }

            if (tex == null)
            {
                return;
            }

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

            float previewHeight = position.width;

            if (tex.width > 0 && tex.height > 0)
            {
                previewHeight = position.width * tex.height / tex.width;
            }

            Rect previewRect = new Rect(position.x, y, position.width, previewHeight);

            EditorGUI.DrawPreviewTexture(previewRect, tex, null, ScaleMode.ScaleAndCrop, 1, 0, channelMask);

            y += previewHeight + 2;

            Rect infoRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);

            float buttonWidth = (position.width - 60) / 3;

            Rect memoryRect = new Rect(infoRect.x, infoRect.y, buttonWidth, infoRect.height);
            Rect sizeRect = new Rect(memoryRect.xMax - 1, infoRect.y, buttonWidth, infoRect.height);
            Rect formatRect = new Rect(sizeRect.xMax - 1, infoRect.y, buttonWidth, infoRect.height);
            Rect popupRect = new Rect(infoRect.xMax - 60, infoRect.y, 60, infoRect.height);

            EditorGUI.LabelField(memoryRect, (UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(tex) / 1024f / 1024f).ToString("F2") + " mb", styledText);
            EditorGUI.LabelField(sizeRect, tex.width.ToString(), styledText);
            EditorGUI.LabelField(formatRect, tex.graphicsFormat.ToString(), styledText);

            channel = EditorGUI.Popup(popupRect, channel, new string[] { "RGB", "R", "G", "B", "A" }, styledPopup);

            if (channel == 0)
            {
                channelMask = ColorWriteMask.All;
            }
            else if (channel == 1)
            {
                channelMask = ColorWriteMask.Red;
            }
            else if (channel == 2)
            {
                channelMask = ColorWriteMask.Green;
            }
            else if (channel == 3)
            {
                channelMask = ColorWriteMask.Blue;
            }
            else if (channel == 4)
            {
                channelMask = ColorWriteMask.Alpha;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            a = (StyledTexturePreview)attribute;

            var tex = (Texture)property.objectReferenceValue;

            if (tex == null)
            {
                if (a.displayName != "")
                {
                    return EditorGUIUtility.singleLineHeight;
                }

                return 0;
            }

            float height = 0;

            if (a.displayName != "")
            {
                height += EditorGUIUtility.singleLineHeight + 10;
            }

            float previewHeight = EditorGUIUtility.currentViewWidth;

            if (tex.width > 0 && tex.height > 0)
            {
                previewHeight *= tex.height / (float)tex.width;
            }

            height += previewHeight;
            height += 2;
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}