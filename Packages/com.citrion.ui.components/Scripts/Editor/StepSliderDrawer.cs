using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace CitrioN.UI.Editor
{
  [CustomEditor(typeof(StepSlider))]
  public class StepSliderDrawer : SliderEditor
  {
    SerializedProperty stepSize;
    SerializedProperty stepCount;
    SerializedProperty valueVisualsMultiplier;
    SerializedProperty valueVisualsSuffix;
    SerializedProperty valueVisualsOffset;

    protected override void OnEnable()
    {
      base.OnEnable();
      stepSize = serializedObject.FindProperty("singleStepSize");
      stepCount = serializedObject.FindProperty("stepCount");
      valueVisualsOffset = serializedObject.FindProperty("valueVisualsOffset");
      valueVisualsMultiplier = serializedObject.FindProperty("valueVisualsMultiplier");
      valueVisualsSuffix = serializedObject.FindProperty("valueVisualsSuffix");
    }

    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();

      EditorGUI.BeginChangeCheck();

      float newStepSize = EditorGUILayout.FloatField(new GUIContent("Step Size", stepSize.tooltip), stepSize.floatValue);
      int newStepCount = EditorGUILayout.IntField(new GUIContent("Step Count", stepCount.tooltip), stepCount.intValue);

      var valueVisualsOffsetField = EditorGUILayout.FloatField(new GUIContent("Value Visuals Offset",
                                                               valueVisualsOffset.tooltip), valueVisualsOffset.floatValue);
      var valueVisualsMultiplierField = EditorGUILayout.FloatField(new GUIContent("Value Visuals Multiplier", 
                                                                   valueVisualsMultiplier.tooltip), valueVisualsMultiplier.floatValue);
      string valueVisualsSuffixField = EditorGUILayout.TextField(new GUIContent("Value Visuals Suffix", 
                                                                 valueVisualsSuffix.tooltip), valueVisualsSuffix.stringValue);

      if (EditorGUI.EndChangeCheck())
      {
        stepSize.floatValue = Mathf.Clamp(newStepSize, 0/*.0001f*/, float.MaxValue);
        stepCount.intValue = Mathf.Max(newStepCount, 0);
        valueVisualsOffset.floatValue = Mathf.Clamp(valueVisualsOffsetField, float.MinValue, float.MaxValue);
        valueVisualsMultiplier.floatValue = Mathf.Clamp(valueVisualsMultiplierField, 0, float.MaxValue);
        valueVisualsSuffix.stringValue = valueVisualsSuffixField;
      }

      serializedObject.ApplyModifiedProperties();
    }

    //public override VisualElement CreateInspectorGUI()
    //{
    //  //return base.CreateInspectorGUI();
    //  var root = new VisualElement();
    //  //var slider = (Slider)serializedObject.targetObject;
    //  //var editor = Editor.CreateEditor(serializedObject.context);

    //  //var script = MonoScript.FromScriptableObject(editor);
    //  //string path = AssetDatabase.GetAssetPath(script);
    //  //if (path == string.Empty)
    //  //  return;
    //  //var editorGui = editor.CreateInspectorGUI();
    //  //root.Add(editorGui);
    //  //root.Add(new InspectorElement(serializedObject));
    //  return root;
    //}
  }
}