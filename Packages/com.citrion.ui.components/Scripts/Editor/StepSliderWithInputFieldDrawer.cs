using UnityEditor;
using UnityEngine;

namespace CitrioN.UI.Editor
{
  [CustomEditor(typeof(StepSliderWithInputField))]

  public class StepSliderWithInputFieldDrawer : StepSliderDrawer
  {
    SerializedProperty inputField;

    protected override void OnEnable()
    {
      base.OnEnable();
      inputField = serializedObject.FindProperty("inputField");
    }

#if TEXT_MESH_PRO
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();

      EditorGUI.BeginChangeCheck();

      var newInputField = EditorGUILayout.ObjectField(new GUIContent("Input Field", inputField.tooltip), 
                                                      inputField.objectReferenceValue,
                                                      typeof(TMPro.TMP_InputField), allowSceneObjects: true);

      if (EditorGUI.EndChangeCheck())
      {
        inputField.objectReferenceValue = newInputField;
      }

      serializedObject.ApplyModifiedProperties();
    }
#endif
  }
}