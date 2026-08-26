using System;

namespace CitrioN.SettingsMenuCreator
{
  public abstract class ScriptableInputElementProvider_UGUI_FromPrefab_Generic<T> : ScriptableInputElementProvider_UGUI_FromPrefab
  {
    public override Type GetInputFieldParameterType(SettingsCollection settings) => typeof(T);

    //protected bool IsCorrectInputElement(RectTransform elem, SettingsCollection settings)
    //{
    //  if (elem == null) { return false; }
    //  var inputFieldType = GetInputFieldType(settings);
    //  var field = inputFieldType != null ? elem.GetComponentInChildren(inputFieldType, true) : null;
    //  if (field == null)
    //  {
    //    ConsoleLogger.LogWarning($"Input field element is of type {elem.GetType()}.\n" +
    //                             $"It should be of type {GetInputFieldType(settings)}");
    //    //DestroyImmediate(elem.gameObject);
    //    return false;
    //  }
    //  return true;
    //}
  }
}
