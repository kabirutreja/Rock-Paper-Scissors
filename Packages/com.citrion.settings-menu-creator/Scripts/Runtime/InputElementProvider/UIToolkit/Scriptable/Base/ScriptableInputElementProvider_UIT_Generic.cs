using CitrioN.Common;
using System;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  public abstract class ScriptableInputElementProvider_UIT_Generic<T> : ScriptableInputElementProvider_UIT_WithStyleSheet_ClassList
  {
    public override Type GetInputFieldParameterType(SettingsCollection settings) => typeof(T);

    // TODO Remove later
    //protected bool IsCorrectInputElement(VisualElement elem, SettingsCollection settings)
    //{
    //  var inputFieldType = GetInputFieldType(settings);
    //  bool isCorrectType = inputFieldType == elem.GetType();
    //  if (!isCorrectType)
    //  {
    //    ConsoleLogger.LogWarning($"Input field element is of type {elem.GetType()}.\n" +
    //                             $"It should be of type {inputFieldType}");
    //    elem.RemoveFromHierarchy();
    //    return false;
    //  }
    //  return true;
    //}
  }
}
