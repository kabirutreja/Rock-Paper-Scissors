using UnityEngine;
using UnityEditor;

namespace Boxophobic.Utility
{
    [CustomEditor(typeof(Notebox))]
    [CanEditMultipleObjects]
    public class NoteBoxInspector : Editor
    {
        const int MIN_SIZE = 11;
        const int MAX_SIZE = 25;

        SerializedProperty size;
        SerializedProperty color;
        SerializedProperty text;

        void OnEnable()
        {
            size = serializedObject.FindProperty("noteSize");
            color = serializedObject.FindProperty("noteColor");
            text = serializedObject.FindProperty("noteText");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUIStyle textStyle = new GUIStyle();
            textStyle.normal.textColor = color.colorValue;
            textStyle.fontSize = size.intValue;
            textStyle.wordWrap = true;

            GUILayout.BeginHorizontal();
            GUILayout.Label("");
            color.colorValue = EditorGUILayout.ColorField("", color.colorValue, GUILayout.Width(35));
            GUILayout.EndHorizontal();

            text.stringValue = EditorGUILayout.TextArea(text.stringValue, textStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("");
            size.intValue = Mathf.RoundToInt(GUILayout.HorizontalSlider(Mathf.RoundToInt(size.intValue), MIN_SIZE, MAX_SIZE, GUILayout.Width(35)));
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();

        }
    }
}