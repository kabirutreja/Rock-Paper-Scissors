using CitrioN.Common;
using CitrioN.Common.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator.Editor
{
  public class SettingHolderDrawerBase<T1> : SettingMenuPropertyDrawerWithChangeButton<T1, Setting> where T1 : ISettingHolder
  {
    protected static int selectedTabIndex = 0;

    protected const string SETTING_TYPE_LABEL_CLASS = "setting-type-label";
    protected const string TAB_MENU_CLASS = "tab-menu";
    protected const string TAB_MENU_TAB_CLASS = "setting-tab";
    protected const string APPLY_IMMEDIATELY_TOGGLE_CLASS = "apply-immediately-toggle";
    protected const string IDENTIFIER_TEXT_FIELD_CLASS = "identifier-text-field";
    protected const string OVERRIDE_IDENTIFIER_WHEN_COPIED_TOGGLE_CLASS = "override-identifier-when-copied-toggle";
    protected const string INPUT_ELEMENT_PROVIDER_SETTINGS_PROPERTY_FIELD_CLASS = "input-element-provider-settings__property-field";
    protected const string INPUT_ELEMENT_LABEL_PROPERTY_FIELD_CLASS = "input-element-label__property-field";

    protected override string DropdownStringToReplace => "Setting_";

    protected override string DropdownReplacementString => "";

    protected override void ChangeValueOnInstance(T1 instance, Setting value)
    {
      instance.Setting = value;
    }

    protected override void Setup(T1 instance, SerializedProperty property,
                                  VisualElement root)
    {
      SetupTabMenu(root, property);
      base.Setup(instance, property, root);
    }

    private void SetupTabMenu(VisualElement root, SerializedProperty property)
    {
      if (root == null) { return; }

      var tabMenu = root.Q<TabMenu>(className: TAB_MENU_CLASS);

      if (tabMenu == null) { return; }

      if (!EditorUtilities.GetPropertyValue(property, out T1 holder))
      {
        ConsoleLogger.LogWarning($"Unable to setup tab menu because " +
                                 $"the property is not of type {nameof(ISettingHolder)}");
        return;
      }
      tabMenu.RegisterTabChangedCallback((i) => OnTabMenuTabChanged(i, holder));

      var tabElements = root.Query(className: TAB_MENU_TAB_CLASS).ToList();

      //if (tabElements?.Count > 0)
      //{
      //  for (int i = 0; i < tabElements.Count; i++)
      //  {
      //    var elem = tabElements[i];
      //    tabMenu.AddTabElement(elem, $"{elem.name}");
      //  }
      //  tabMenu.SelectTab(holder != null ? holder.CurrentTabMenuIndex : 0);
      //}

      // Delay the tab selection because the tab menu might
      // otherwise override the selected tab.
      //ScheduleUtility.InvokeDelayedByFrames(() => tabMenu.SelectTab(holder != null ? holder.CurrentTabMenuIndex : 0));
      
      ScheduleUtility.InvokeDelayedByFrames(() => tabMenu.SelectTab(selectedTabIndex));
    }

    private void OnTabMenuTabChanged(int tabIndex, T1 holder)
    {
      if (holder == null) { return; }
      //holder.CurrentTabMenuIndex = tabIndex;
      selectedTabIndex = tabIndex;
    }

    protected override void SetupVisualElements(SerializedProperty property, VisualElement root)
    {
      var prop = property.FindPropertyRelative("setting");
      var propertyField = UIToolkitEditorExtensions.SetupPropertyField(prop, root, PropertyFieldClass);

      // For some reason the property foldout is not expanded and not available this frame.
      // Force expand it next frame instead.
      ScheduleUtility.InvokeDelayedByFrames(() => { propertyField?.OpenPropertyFieldFoldout(); });

      // setting-type-label
      var settingTypeLabel = root.Q<Label>(className: SETTING_TYPE_LABEL_CLASS);

      bool isValidHolder = EditorUtilities.GetPropertyValue(property, out T1 holder);

      //settingTypeLabel?.SetText(holder != null ? holder.setting?.DisplayName : "Unknown Setting");
      settingTypeLabel?.SetText(holder != null && isValidHolder ? holder.Setting?.GetType().Name : "Unknown Setting");

      // apply-immediately-toggle
      var applyImmediatelyToggle = root.Q<Toggle>(className: APPLY_IMMEDIATELY_TOGGLE_CLASS);
      if (applyImmediatelyToggle != null)
      {
        var applyImmediatelyProperty = property.FindPropertyRelative("applyImmediately");
        applyImmediatelyToggle.label = "Apply Immediately";
        applyImmediatelyToggle.BindProperty(applyImmediatelyProperty);
        applyImmediatelyToggle.SetVisualElementTooltipFromTooltipAttribute(typeof(SettingHolder), "applyImmediately");
      }

      // identifier-text-field
      var identifierTextField = root.Q<TextField>(className: IDENTIFIER_TEXT_FIELD_CLASS);
      if (identifierTextField != null)
      {
        var identifierProperty = property.FindPropertyRelative("identifier");
        identifierTextField.SetLabelText("Identifier");
        identifierTextField.BindProperty(identifierProperty);
        identifierTextField.SetVisualElementTooltipFromTooltipAttribute(typeof(SettingHolder), "identifier");
      }

      // override-identifier-when-copied-toggle
      var overrideIdentifierWhenCopiedToggle = root.Q<Toggle>(className: OVERRIDE_IDENTIFIER_WHEN_COPIED_TOGGLE_CLASS);
      if (overrideIdentifierWhenCopiedToggle != null)
      {
        var overrideIdentifierProperty = property.FindPropertyRelative("overrideIdentifierWhenCopied");
        overrideIdentifierWhenCopiedToggle.label = "Override Identifier When Copied";
        overrideIdentifierWhenCopiedToggle.BindProperty(overrideIdentifierProperty);
        overrideIdentifierWhenCopiedToggle.SetVisualElementTooltipFromTooltipAttribute(typeof(SettingHolder), "overrideIdentifierWhenCopied");
      }

      // input-element-provider-settings__property-field
      var inputElementProviderSettingsProperty = property.FindPropertyRelative("inputElementProviderSettings");
      UIToolkitEditorExtensions.SetupPropertyField(inputElementProviderSettingsProperty, root, INPUT_ELEMENT_PROVIDER_SETTINGS_PROPERTY_FIELD_CLASS);


      // input-element-label__property-field
      var inputElementProviderSettingsLabelProperty = property.FindPropertyRelative("inputElementProviderSettings").FindPropertyRelative("customLabel");
      UIToolkitEditorExtensions.SetupPropertyField(inputElementProviderSettingsLabelProperty, root, INPUT_ELEMENT_LABEL_PROPERTY_FIELD_CLASS);
    }
  }
}