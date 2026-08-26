using CitrioN.Common;
using CitrioN.Common.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator.Editor
{
  [CustomPropertyDrawer(typeof(InputElementProviderSettings))]
  public class InputElementProviderSettingsDrawer
    : SettingMenuPropertyDrawerWithChangeButton
     <InputElementProviderSettings, InputElementProvider_UIT>
  {
    protected const string INPUT_ELEMENT_PROVIDER_LABEL_CLASS = "property-field__input-element-provider-label";
    protected const string INPUT_ELEMENT_PROVIDER_PARENT_CLASS = "property-field__input-element-provider-parent";
    protected const string ADD_SPACER_CLASS = "property-field__add-spacer";
    protected const string SPACER_ELEMENT_CLASS = "property-field__spacer-element";
    protected const string INPUT_ELEMENT_PROVIDER_UGUI_PROPERTY_FIELD_CLASS = "property-field__input-element-provider-ugui";
    protected const string TYPE_LABEL_UIT_CLASS = "type-label_uit";
    protected const string TYPE_LABEL_UGUI_CLASS = "type-label-ugui";

    protected const string ChangeButtonClass_UGUI = "property-change-button__ugui";

    protected override string DropdownStringToReplace => "InputElementProvider_";

    protected override void ChangeValueOnInstance(InputElementProviderSettings instance,
                                                  InputElementProvider_UIT value)
    {
      instance.InputElementProvider_UIToolkit = value;
    }

    protected void ChangeValueOnInstance(InputElementProviderSettings instance,
                                         InputElementProvider_UGUI value)
    {
      instance.InputElementProvider_UGUI = value;
    }

    protected override void SetupChangeButton(InputElementProviderSettings instance, SerializedProperty property, VisualElement root)
    {
      base.SetupChangeButton(instance, property, root);

      var buttonClickable = GetChangeButtonClickable<InputElementProviderSettings, InputElementProvider_UGUI>
                              (instance, property, root,
                              ChangeValueOnInstance, /*OnValueChanged*/null, UpdateFields,
                              dropdownHeader: "Header");
      SetupChangeButton(root, ChangeButtonClass_UGUI, ChangeButtonText, buttonClickable);
    }

    protected override void SetupVisualElements(SerializedProperty property, VisualElement root)
    {
      // Input Element Label
      var inputElementLabelClassProperty = property.FindPropertyRelative("customLabel");
      UIToolkitEditorExtensions.SetupPropertyField(inputElementLabelClassProperty, root, INPUT_ELEMENT_PROVIDER_LABEL_CLASS);

      // Parent Class Field
      var parentIdentifierProperty = property.FindPropertyRelative("parentIdentifier");
      UIToolkitEditorExtensions.SetupPropertyField(parentIdentifierProperty, root, INPUT_ELEMENT_PROVIDER_PARENT_CLASS);

      // Add Spacer Element Field
      var addSpacerProperty = property.FindPropertyRelative("addSpacer");
      UIToolkitEditorExtensions.SetupPropertyField(addSpacerProperty, root, ADD_SPACER_CLASS);

      // Spacer Element Class Field
      var spacerElementClassProperty = property.FindPropertyRelative("spacerElementClass");
      UIToolkitEditorExtensions.SetupPropertyField(spacerElementClassProperty, root, SPACER_ELEMENT_CLASS);

      #region Type Labels
      string labelText_UIT = "";
      string labelText_UGUI = "";
      if (EditorUtilities.GetPropertyValue<InputElementProviderSettings>(property, out var settings))
      {
        if (settings != null)
        {
          labelText_UIT = settings?.InputElementProvider_UIToolkit?.Name;
          labelText_UGUI = settings?.InputElementProvider_UGUI?.Name;
        }
      }
      UIToolkitExtensions.SetupLabel(root, TYPE_LABEL_UIT_CLASS, labelText_UIT);
      UIToolkitExtensions.SetupLabel(root, TYPE_LABEL_UGUI_CLASS, labelText_UGUI);
      #endregion

      // Element Provider Property (UI Toolkit)
      var inputElementProviderProperty_UIToolkit = property.FindPropertyRelative("inputElementProvider_UIToolkit");
      UIToolkitEditorExtensions.SetupPropertyField(inputElementProviderProperty_UIToolkit, root, PropertyFieldClass);

      // Element Provider Property (uGUI)
      var inputElementProviderProperty_UGUI = property.FindPropertyRelative("inputElementProvider_UGUI");
      UIToolkitEditorExtensions.SetupPropertyField(inputElementProviderProperty_UGUI, root,
                                                   INPUT_ELEMENT_PROVIDER_UGUI_PROPERTY_FIELD_CLASS);

      var syncSelectedButton_InputElementParent = root.Q<Button>(className: "button__sync__input-element-parent");
      if (syncSelectedButton_InputElementParent != null)
      {
        syncSelectedButton_InputElementParent.tooltip = "Apply this value to all\ncurrently selected settings.";
        syncSelectedButton_InputElementParent.clicked += () => OnSyncSelectedButtonClicked_InputElementParent(settings);
      }
    }

    private void OnSyncSelectedButtonClicked_InputElementParent(InputElementProviderSettings settings)
    {
      if (settings != null)
      {
        GlobalEventHandler.InvokeEvent("Sync InputElementProviderParent", settings.ParentIdentifier);
      }
    }
  }
}