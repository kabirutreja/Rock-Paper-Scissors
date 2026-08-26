using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.Common
{
  public static class UIToolkitStyleUtilities
  {
    public const string SCRIPT_FIELD_NAME = "PropertyField:m_Script";
    public const string INSPECTOR_TOOLBAR_CLASS = "inspector_toolbar";
    public const string INSPECTOR_FOOTER_TOOLBAR_CLASS = "inspector_footer_toolbar";
    public const string TOGGLE_DESCRIPTIONS_BUTTON_CLASS = "button__toggle-descriptions";
    public const string DESCRIPTION_PARENT_CLASS = "description-parent";

    public static void ApplyGroupBoxStyle(VisualElement elem, bool includeLabel = true)
    {
      if (elem == null) { return; }

      elem.SetPaddingAll(5);

      elem.style.marginBottom = 5;
      elem.style.marginTop = 5;
      elem.style.marginRight = -2;

      elem.SetBorderWidthAll(1);
      elem.SetBorderRadiusAll(3);

      elem.style.backgroundColor = UnityVariablesUtility.GetVariable<Color>(UnityVariableName.InputField_Background_Color, true);
      elem.SetBorderColorAll(UnityVariablesUtility.GetVariable<Color>(UnityVariableName.InputField_Border_Color, true));

      if (!includeLabel) { return; }

      var label = elem.Q<Label>(className: "unity-group-box__label");
      if (label == null) { return; }

      label.SetMargin(5, true, false, false, true);
      label.SetTextWrapping(true);
    }

    public static void ApplyToolbarStyle(VisualElement elem)
    {
      if (elem == null) { return; }

      //elem.SetPadding(2, true, true, true, true);
      //elem.SetPadding(1, false, true, false, true);

      elem.style.marginBottom = 3;
      elem.style.marginRight = -5;

      //elem.SetBorderWidthAll(1);
      //elem.SetBorderRadiusAll(3);

      //elem.style.backgroundColor = UnityVariablesUtility.GetVariable<Color>(UnityVariableName.Toolbar_Background_Color, true);
      //elem.SetBorderColorAll(UnityVariablesUtility.GetVariable<Color>(UnityVariableName.Toolbar_Border_Color, true));
      elem.SetFlexDirection(FlexDirection.RowReverse);
      elem.SetFlexWrap(Wrap.Wrap);
      elem.SetAlignItems(Align.FlexEnd);
    }

    public static VisualElement AddHeaderToolbar(VisualElement root, bool applyStyle)
    {
      if (root == null) { return null; }

      var headerToolbar = root.Q<VisualElement>(className: INSPECTOR_TOOLBAR_CLASS);

      if (headerToolbar == null)
      {
        var scriptField = root.Q<VisualElement>(SCRIPT_FIELD_NAME);
        if (scriptField != null)
        {
          var parent = scriptField.parent;
          if (parent == null) { return null; }

          headerToolbar = new VisualElement();
          parent.Add(headerToolbar);

          headerToolbar.PlaceInFront(scriptField);
          headerToolbar.AddToClassList(INSPECTOR_TOOLBAR_CLASS);
        }
        else
        {
          var parent = root;

          headerToolbar = new VisualElement();
          parent.Add(headerToolbar);

          var firstElement = root.ElementAt(0);
          headerToolbar.PlaceInFront(firstElement);
          firstElement.PlaceInFront(headerToolbar);
          headerToolbar.AddToClassList(INSPECTOR_TOOLBAR_CLASS);
        }
      }

      if (applyStyle) { ApplyToolbarStyle(headerToolbar); }

      return headerToolbar;
    }

    public static VisualElement AddFooterToolbar(VisualElement root, bool applyStyle)
    {
      if (root == null) { return null; }

      var footerToolbar = root.Q<VisualElement>(className: INSPECTOR_FOOTER_TOOLBAR_CLASS);

      if (footerToolbar == null)
      {
        var scriptField = root.Q<VisualElement>(SCRIPT_FIELD_NAME);
        if (scriptField != null)
        {
          var parent = scriptField.parent;
          if (parent == null) { return null; }

          footerToolbar = new VisualElement();
          parent.Add(footerToolbar);

          footerToolbar.BringToFront();
          footerToolbar.AddToClassList(INSPECTOR_FOOTER_TOOLBAR_CLASS);
        }
        else
        {
          var parent = root;

          footerToolbar = new VisualElement();
          parent.Add(footerToolbar);
          footerToolbar.BringToFront();
          footerToolbar.AddToClassList(INSPECTOR_FOOTER_TOOLBAR_CLASS);
        }
      }

      footerToolbar.SetJustifyContent(Justify.SpaceBetween);
      if (applyStyle) { ApplyToolbarStyle(footerToolbar); }

      return footerToolbar;
    }

    public static Button AddToggleDescriptionsButton(VisualElement root, VisualElement parent)
    {
      if (root == null || parent == null) { return null; }

      var toggleButton = root.Q<Button>(className: TOGGLE_DESCRIPTIONS_BUTTON_CLASS);

      if (toggleButton == null)
      {
        toggleButton = new Button(() => ToggleTooltips(root));
        toggleButton.AddToClassList(TOGGLE_DESCRIPTIONS_BUTTON_CLASS);
        parent.Add(toggleButton);
      }
      else
      {
        toggleButton.clickable = new Clickable(() => ToggleTooltips(root));
      }

      toggleButton.tooltip = "Toggle descriptions";
      toggleButton.SetText("?");

      // TODO Should the reparenting also occur if the button already exists?
      return toggleButton;
    }

    public static Button AddMethodButton(VisualElement root, VisualElement parent, Action onClick, string buttonClassName, string label = null, string tooltip = null)
    {
      if (root == null || parent == null) { return null; }

      var button = root.Q<Button>(className: buttonClassName);

      if (button == null)
      {
        button = new Button(onClick);
        button.AddToClassList(buttonClassName);
        button.SetFlexShrink(1);
        parent.Add(button);
      }
      else
      {
        button.clickable = new Clickable(onClick);
      }

      if (!string.IsNullOrEmpty(tooltip))
      {
        button.tooltip = tooltip;
      }
      if (!string.IsNullOrEmpty(label))
      {
        button.SetText(label);
      }

      // TODO Should the reparenting also occur if the button already exists?
      return button;
    }

    public static void ToggleTooltips(VisualElement root)
    {
      if (root == null) { return; }

      var parents = root.Query<VisualElement>(className: DESCRIPTION_PARENT_CLASS).ToList();
      if (parents == null || parents.Count < 1) { return; }

      foreach (var elem in parents)
      {
        elem?.ToggleVisibility();
      }
    }
  }
}
