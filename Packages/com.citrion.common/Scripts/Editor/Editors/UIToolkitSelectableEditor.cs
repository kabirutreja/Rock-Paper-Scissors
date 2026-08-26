using UnityEditor;

namespace CitrioN.Common.Editor
{
  [CanEditMultipleObjects]
  //#if !UNITY_2022_1_OR_NEWER
  [CustomEditor(typeof(UnityEngine.UI.Selectable), editorForChildClasses: true, isFallback = true)]
  //#endif
  public class UIToolkitSelectableEditor : UIToolkitInspectorWindowEditor { }
}