using CitrioN.Common;
using CitrioN.Common.Editor;
using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator.Editor
{
  /// <summary>
  /// Base property drawer that adds a button to change a field of type <typeparamref name="T2"/>
  /// on an instance of <typeparamref name="T1"/>. <br/>
  /// The property drawer can only be used for a property of type <typeparamref name="T1"/>
  /// </summary>
  /// <typeparam name="T1">The type for the drawer and the instance to change the field for</typeparam>
  /// <typeparam name="T2">The field type to change on the instance</typeparam>
  public abstract class SettingMenuPropertyDrawerWithChangeButton<T1, T2> : PropertyDrawerFromTemplate_SettingsMenu
  {
    #region Properties
    protected virtual string ChangeButtonText => "Change";

    protected virtual string PropertyFieldClass => $"property-field";

    protected virtual string ChangeButtonClass
      => $"property-change-button";

    protected virtual string DropdownHeader => string.Empty;

    protected virtual string DropdownStringToReplace => string.Empty;

    protected virtual string DropdownReplacementString => string.Empty;

    protected virtual bool UpdateButtonBeforeElements => true;
    #endregion

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
      var root = base.CreatePropertyGUI(property);

      if (EditorUtilities.GetPropertyValue<T1>(property, out var instance))
      {
        Setup(instance, property, root);
      }

      return root;
    }

    private VisualElement GetDrawerParent(VisualElement elem)
    {
      VisualElement parent = elem;

      do
      {
        parent = parent.parent?.parent;
      } while (parent != null && parent.ClassListContains(RootClass));

      return parent ?? elem; // Is the same as 'parent != null ? parent : elem;'
    }

    protected virtual void Setup(T1 instance, SerializedProperty property,
                                 VisualElement root)
    {
      UpdateFields(instance, property, GetDrawerParent(root), updateButton: true);
    }

    protected virtual void UpdateFields(T1 instance, SerializedProperty property,
                              VisualElement root, bool updateButton)
    {
      if (updateButton && UpdateButtonBeforeElements)
      {
        SetupChangeButton(instance, property, root);
      }
      SetupVisualElements(property, root);
      if (updateButton && !UpdateButtonBeforeElements)
      {
        SetupChangeButton(instance, property, root);
      }
    }

    protected virtual void SetupVisualElements(SerializedProperty property, VisualElement root)
    {
      UIToolkitEditorExtensions.SetupPropertyField(property, root, PropertyFieldClass);
    }

    #region Change Button Methods - TODO Can this be moved into a more general class so other systems can use it too?
    protected virtual void SetupChangeButton(T1 instance, SerializedProperty property, VisualElement root)
    {
      var buttonClickable = GetChangeButtonClickable<T1, T2>(instance, property, root,
                              ChangeValueOnInstance, /*OnValueChanged*/null, UpdateFields,
                              DropdownHeader, DropdownStringToReplace, DropdownReplacementString);
      SetupChangeButton(root, ChangeButtonClass, ChangeButtonText, buttonClickable);
    }

    protected void SetupChangeButton(VisualElement root, string changeButtonClass,
                                     string changeButtonText, Clickable buttonClickable)
    {
      var button = root.Q<Button>(className: changeButtonClass);
      if (button == null)
      {
        button = new Button();
        button.AddToClassList(changeButtonClass);
        root.Add(button);
      }
      button.SetText(changeButtonText);
      button.clickable = buttonClickable;
    }

    private Clickable GetChangeButtonClickable(T1 instance,
      SerializedProperty property, VisualElement root)
      => new Clickable(() => OnChangeButtonClicked(instance, property, root));

    protected Clickable GetChangeButtonClickable<V1, V2>(V1 instance,
      SerializedProperty property, VisualElement root,
      Action<V1, V2> applyChange, Action<SerializedProperty, VisualElement, V2> valueChangedCallback,
      Action<V1, SerializedProperty, VisualElement, bool> visualsUpdateCallback,
      string dropdownHeader = "", string dropdownStringToReplace = "", string dropdownReplacementString = "")
      => new Clickable(() => OnChangeButtonClicked<V1, V2>(instance, property, root,
                             applyChange, valueChangedCallback, visualsUpdateCallback,
                             dropdownHeader, dropdownStringToReplace, dropdownReplacementString));

    private void OnChangeButtonClicked(T1 instance,
      SerializedProperty property, VisualElement root)
    {
      EditorUtilities.GetInstanceOfTypeFromAdvancedDropdown(
        findDerivedTypes: true, (i) => ChangeValue(i, instance, root, property),
        DropdownHeader, DropdownStringToReplace, DropdownReplacementString, typeof(T2));
    }

    private void OnChangeButtonClicked<V1, V2>(V1 instance,
      SerializedProperty property, VisualElement root,
      Action<V1, V2> applyChange, Action<SerializedProperty, VisualElement, V2> valueChangedCallback,
      Action<V1, SerializedProperty, VisualElement, bool> visualsUpdateCallback,
      string dropdownHeader = "", string dropdownStringToReplace = "", string dropdownReplacementString = "")
    {
      EditorUtilities.GetInstanceOfTypeFromAdvancedDropdown(
        findDerivedTypes: true, (i) =>
        ChangeValue<V1, V2>(i, instance, root, property, applyChange, valueChangedCallback, visualsUpdateCallback),
        dropdownHeader, dropdownStringToReplace, dropdownReplacementString, typeof(V2));
    }
    #endregion

    private void ChangeValue(object value, T1 instance, VisualElement root, SerializedProperty property)
    {
      if (instance == null) { return; }

      if (typeof(T2).IsAssignableFrom(value.GetType()))
      {
        var boxedValue = (T2)value;
        var serializedObject = property.serializedObject;
        Undo.RecordObject(serializedObject.targetObject, $"Changed {typeof(T2).Name}");

        ChangeValueOnInstance(instance, boxedValue);
        //OnValueChanged(property, root, boxedValue);

        EditorUtility.SetDirty(serializedObject.targetObject);
        serializedObject.Update();

        UpdateFields(instance, property, root, updateButton: false);
      }
      else
      {
        ConsoleLogger.LogWarning($"Value is not of type {typeof(T2).Name}");
      }
    }

    protected void ChangeValue<V1, V2>(object value, V1 instance, VisualElement root, SerializedProperty property,
      Action<V1, V2> applyChange, Action<SerializedProperty, VisualElement, V2> valueChangedCallback,
      Action<V1, SerializedProperty, VisualElement, bool> visualsUpdateCallback)
    {
      if (instance == null) { return; }

      if (typeof(V2).IsAssignableFrom(value.GetType()))
      {
        var boxedValue = (V2)value;
        var serializedObject = property.serializedObject;
        Undo.RecordObject(serializedObject.targetObject, $"Changed {typeof(V2).Name}");

        applyChange?.Invoke(instance, boxedValue);
        valueChangedCallback?.Invoke(property, root, boxedValue);

        EditorUtility.SetDirty(serializedObject.targetObject);
        serializedObject.Update();

        visualsUpdateCallback?.Invoke(instance, property, root, false);
      }
      else
      {
        ConsoleLogger.LogWarning($"Value is not of type {typeof(V2).Name}");
      }
    }

    protected abstract void ChangeValueOnInstance(T1 instance, T2 value);

    //protected virtual void OnValueChanged(SerializedProperty property, VisualElement root, T2 newValue) { }
  }
}