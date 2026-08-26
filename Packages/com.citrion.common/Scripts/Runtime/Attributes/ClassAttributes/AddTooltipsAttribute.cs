using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace CitrioN.Common
{
  [SkipObfuscationRename]
  [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
  public class AddTooltipsAttribute : ClassAttribute
  {
    private bool addTooltips = true;
    private bool showScriptField = false;
    private bool placeBeforeField = false;

    protected const string SCRIPT_FIELD_NAME = "PropertyField:m_Script";
    protected const string SCRIPT_FIELD_BINDING_PATH = "m_Script";

    public bool AddTooltips { get => addTooltips; set => addTooltips = value; }
    public bool ShowScriptField { get => showScriptField; set => showScriptField = value; }
    public bool PlaceBeforeField { get => placeBeforeField; set => placeBeforeField = value; }

    public AddTooltipsAttribute() { }

#if UNITY_EDITOR
    public override void UpdateHierarchy(VisualElement root, SerializedObject serializedObject)
    {
      if (root == null || serializedObject == null) { return; }

      if (!ShowScriptField)
      {
        var scriptField = root.Q<PropertyField>(SCRIPT_FIELD_NAME);
        scriptField?.Show(false);
      }

      if (!AddTooltips) { return; }

      var propertyFields = root.Query<PropertyField>().ToList();
      if (propertyFields == null || propertyFields.Count == 0) { return; }

      bool hasPropertiesWithTooltips = false;

      var obj = serializedObject.targetObject;
      if (obj == null) { return; }

      PropertyField propertyField;
      string propertyName;
      VisualElement parent;
      FieldInfo field;
      TooltipAttribute tooltipAttribute;
      string tooltip;
      GroupBox tooltipParent;
      Label tooltipLabel;

      for (int i = 0; i < propertyFields.Count; i++)
      {
        propertyField = propertyFields[i];
        if (propertyField == null) { continue; }

        propertyName = propertyField.bindingPath;// propertyField.name;

        if (propertyName == SCRIPT_FIELD_BINDING_PATH) { continue; }

        // We remove 'PropertyField:' from the name to get the field name
        //propertyName = propertyName.Substring(14);
        //propertyName = propertyName.Replace("PropertyField:", "");

        field = obj.GetType().GetFieldIncludingBaseClasses(propertyName);
        if (field == null) { continue; }

        tooltipAttribute = field.GetCustomAttribute<TooltipAttribute>();
        if (tooltipAttribute == null) { continue; }

        tooltip = tooltipAttribute.tooltip;
        if (string.IsNullOrEmpty(tooltip)) { continue; }

        tooltipParent = new GroupBox();

        tooltipParent.AddToClassList(UIToolkitStyleUtilities.DESCRIPTION_PARENT_CLASS);

        // TODO Implement style sheet support and remove hardcoded values?
        tooltipParent.SetPaddingAll(5);

        tooltipParent.style.marginBottom = PlaceBeforeField ? 5 : 15;
        tooltipParent.style.marginTop = PlaceBeforeField ? i == 2 ? 15 : 0 : 3;
        tooltipParent.style.marginRight = -2;

        tooltipParent.SetBorderWidthAll(1);
        tooltipParent.SetBorderRadiusAll(3);

        tooltipParent.style.backgroundColor = UnityVariablesUtility.GetVariable<Color>(UnityVariableName.InputField_Background_Color, true);
        tooltipParent.SetBorderColorAll(UnityVariablesUtility.GetVariable<Color>(UnityVariableName.InputField_Border_Color, true));

        tooltipLabel = new Label(tooltip);
        tooltipLabel.SetTextWrapping(true);
        tooltipLabel.style.paddingLeft = 3;

        parent = propertyField.parent;
        parent.Add(tooltipParent);
        tooltipParent.PlaceInFront(propertyField);
        if (PlaceBeforeField) { propertyField.PlaceInFront(tooltipParent); }
        tooltipParent.Add(tooltipLabel);
        tooltipParent.ToggleVisibility();

        hasPropertiesWithTooltips = true;
      }

      if (hasPropertiesWithTooltips)
      {
        AddDescriptionsToggleButton(root);
      }
    }

    public static void AddTooltip(Type type, VisualElement elem, string propertyName, int parentOffset = 1, bool placeBeforeField = false)
    {
      if (elem == null) { return; }
      var parent = elem;
      var sibling = parent;
      for (int i = 0; i < parentOffset; i++)
      {
        sibling = parent;
        parent = parent.parent;
      }
      var tooltipParent = parent.Q<GroupBox>(classes: new string[] { UIToolkitStyleUtilities.DESCRIPTION_PARENT_CLASS, propertyName });
      if (tooltipParent != null) { return; }

      FieldInfo field = type.GetFieldIncludingBaseClasses(propertyName);
      if (field == null) { return; }

      var tooltipAttribute = field.GetCustomAttribute<TooltipAttribute>();
      if (tooltipAttribute == null) { return; }

      var tooltip = tooltipAttribute.tooltip;
      if (string.IsNullOrEmpty(tooltip)) { return; }

      tooltipParent = CreateTooltip(parent, sibling, tooltip, placeBeforeField, propertyName);
      //tooltipParent = new GroupBox();

      //tooltipParent.AddToClassList(UIToolkitStyleUtilities.DESCRIPTION_PARENT_CLASS, propertyName);

      //// TODO Implement style sheet support and remove hardcoded values?
      //tooltipParent.SetPaddingAll(5);

      //tooltipParent.style.marginBottom = placeBeforeField ? 5 : 15;
      //tooltipParent.style.marginTop = placeBeforeField ? 0 : 3;
      //tooltipParent.style.marginRight = -2;

      //tooltipParent.SetBorderWidthAll(1);
      //tooltipParent.SetBorderRadiusAll(3);

      //tooltipParent.style.backgroundColor = UnityVariablesUtility.GetVariable<Color>(UnityVariableName.InputField_Background_Color, true);
      //tooltipParent.SetBorderColorAll(UnityVariablesUtility.GetVariable<Color>(UnityVariableName.InputField_Border_Color, true));

      //var tooltipLabel = new Label(tooltip);
      //tooltipLabel.SetTextWrapping(true);
      //tooltipLabel.style.paddingLeft = 3;
      //tooltipParent.Add(tooltipLabel);

      //parent.Add(tooltipParent);
      //tooltipParent.PlaceInFront(sibling);
      //if (placeBeforeField) { sibling.PlaceInFront(tooltipParent); }
      //tooltipParent.ToggleVisibility();
    }

    public static GroupBox CreateTooltip(VisualElement parent, VisualElement sibling, string tooltip, bool placeBeforeField = false, params string[] classNames)
    {
      var tooltipParent = new GroupBox();

      tooltipParent.AddToClassList(UIToolkitStyleUtilities.DESCRIPTION_PARENT_CLASS);
      tooltipParent.AddToClassList(classNames);

      // TODO Implement style sheet support and remove hardcoded values?
      tooltipParent.SetPaddingAll(5);

      tooltipParent.style.marginBottom = placeBeforeField ? 5 : 15;
      tooltipParent.style.marginTop = placeBeforeField ? 0 : 3;
      tooltipParent.style.marginRight = -2;

      tooltipParent.SetBorderWidthAll(1);
      tooltipParent.SetBorderRadiusAll(3);

      tooltipParent.style.backgroundColor = UnityVariablesUtility.GetVariable<Color>(UnityVariableName.InputField_Background_Color, true);
      tooltipParent.SetBorderColorAll(UnityVariablesUtility.GetVariable<Color>(UnityVariableName.InputField_Border_Color, true));

      var tooltipLabel = new Label(tooltip);
      tooltipLabel.SetTextWrapping(true);
      tooltipLabel.style.paddingLeft = 3;
      tooltipParent.Add(tooltipLabel);

      parent.Add(tooltipParent);
      tooltipParent.PlaceInFront(sibling);
      if (placeBeforeField) { sibling.PlaceInFront(tooltipParent); }
      tooltipParent.ToggleVisibility();
      return tooltipParent;
    }

    private void AddDescriptionsToggleButton(VisualElement root)
    {
      var headerToolbar = UIToolkitStyleUtilities.AddHeaderToolbar(root, true);
      UIToolkitStyleUtilities.AddToggleDescriptionsButton(root, headerToolbar);
    }

    public static string GetTooltipForProperty(Type type, string fieldName)
    {
      FieldInfo field = type?.GetFieldIncludingBaseClasses(fieldName);
      if (field == null) { return null; }

      var tooltipAttribute = field.GetCustomAttribute<TooltipAttribute>();
      if (tooltipAttribute == null) { return null; }

      return tooltipAttribute.tooltip;
    }
#endif
  }
}