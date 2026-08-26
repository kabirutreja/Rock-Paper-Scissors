using CitrioN.Common;
using CitrioN.Common.Editor;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CitrioN.UI.Editor
{
  [CustomEditor(typeof(TabMenu_UGUI))]
  public class TabMenu_UGUI_Editor : EditorFromVisualTreeAsset
  {
    protected const string DEFAULT_INSPECTOR_CLASS = "container__default-inspector";
    protected const string CONTAINER_BUTTONS_SELECT_TABS_CLASS = "container__buttons__select-tabs";
    protected const string INITIALIZE_BUTTON_CLASS = "button__initialize-everything";
    protected const string INITIALIZE_TABS_BUTTON_CLASS = "button__initialize-tabs";
    protected const string INITIALIZE_TAB_NAMES_BUTTON_CLASS = "button__initialize-tab-names";
    protected const string INITIALIZE_TAB_CONTENTS_CLASS = "button__initialize-tab-contents";

    protected VisualElement root;

    public override string UxmlPath => $"Packages/com.citrion.ui.components/UI Toolkit/UXML/Editors/{GetType().Name}.uxml";

    public override string StyleSheetPath => $"Packages/com.citrion.ui.components/UI Toolkit/USS/Editors/{GetType().Name}";

    public override VisualElement CreateInspectorGUI()
    {
      root = base.CreateInspectorGUI();
      var defaultInspectorContainer = root.Q(className: DEFAULT_INSPECTOR_CLASS);
      InspectorElement.FillDefaultInspector(defaultInspectorContainer ?? root, serializedObject, this);

      var contentSourcesPropertyField = root.Q<PropertyField>(name: "PropertyField:contentSources");
      if (contentSourcesPropertyField != null)
      {
        contentSourcesPropertyField.RegisterValueChangeCallback(OnContentSourcesChanged);
      }

      UpdateInitializeButtons();
      UpdateTabSelectionButtons();

      ClassAttribute.ApplyClassAttributesToHierarchy(root, serializedObject);
      return root;
    }

    private void UpdateInitializeButtons()
    {
      if (root == null) { return; }

      var tabMenuScript = target as TabMenu_UGUI;
      if (tabMenuScript == null) { return; }

      UIToolkitUtilities.SetupButton(root, null, tabMenuScript.Reinitialize, null, INITIALIZE_BUTTON_CLASS);
      UIToolkitUtilities.SetupButton(root, null, () => tabMenuScript.ReinitializeTabs(), null, INITIALIZE_TABS_BUTTON_CLASS);
      UIToolkitUtilities.SetupButton(root, null, () => tabMenuScript.ReinitializeTabNames(), null, INITIALIZE_TAB_NAMES_BUTTON_CLASS);
      UIToolkitUtilities.SetupButton(root, null, () => tabMenuScript.ReinitializeTabsContent(), null, INITIALIZE_TAB_CONTENTS_CLASS);
    }

    private void OnContentSourcesChanged(SerializedPropertyChangeEvent evt) => UpdateTabSelectionButtons();

    private void UpdateTabSelectionButtons()
    {
      if (root == null) { return; }
      var buttonsContainer = root.Q(className: CONTAINER_BUTTONS_SELECT_TABS_CLASS);
      if (buttonsContainer == null) { return; }

      var tabMenuScript = target as TabMenu_UGUI;
      if (tabMenuScript == null) { return; }

      buttonsContainer.RemoveAllChildren();

      var tabsCount = tabMenuScript.TabsCount;

      for (int i = 0; i < tabsCount; i++)
      {
        var tabIndex = i;
        var button = new Button(() => tabMenuScript.SelectTab(tabIndex));
        button.SetText($"Tab {i + 1}");
        button.SetFlexShrink(1);
        button.SetFlexGrow(1);
        buttonsContainer.Add(button);
      }
    }
  }
}