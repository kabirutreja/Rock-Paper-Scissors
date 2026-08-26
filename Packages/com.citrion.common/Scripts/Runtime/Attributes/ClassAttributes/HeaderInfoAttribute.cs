using UnityEngine.UIElements;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CitrioN.Common
{
  [SkipObfuscationRename]
  [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
  public class HeaderInfoAttribute : ClassAttribute
  {
    private string description = string.Empty;

    private bool overrideExisting = false;

    protected const string SCRIPT_FIELD_NAME = "PropertyField:m_Script";
    protected const string INFO_LABEL_CLASS = "label__header-info";

    public string Description { get => description; set => description = value; }

    public bool OverrideExisting { get => overrideExisting; set => overrideExisting = value; }

    public HeaderInfoAttribute(string description)
    {
      this.description = description;
    }

#if UNITY_EDITOR
    public override void UpdateHierarchy(VisualElement root, SerializedObject serializedObject)
    {
      if (root == null || serializedObject == null) { return; }

      if (string.IsNullOrEmpty(Description)) { return; }

      bool addContainer = true;

      var descriptionLabel = root.Q<Label>(className: INFO_LABEL_CLASS);

      if (descriptionLabel == null)
      {
        descriptionLabel = new Label(Description);
        descriptionLabel.AddToClassList(INFO_LABEL_CLASS);
      }
      else
      {
        addContainer = false;
        descriptionLabel.text = OverrideExisting ? Description : $"{descriptionLabel.text}{Description}";
      }

      descriptionLabel.SetTextWrapping(true);
      descriptionLabel.style.paddingLeft = 3;

      if (addContainer)
      {
        var headerToolbar = AddDescriptionsToggleButton(root);
        var parent = headerToolbar?.parent;

        var descriptionParent = new GroupBox();

        descriptionParent.AddToClassList(UIToolkitStyleUtilities.DESCRIPTION_PARENT_CLASS);

        descriptionParent.SetPaddingAll(5);

        //descriptionParent.style.marginBottom = PlaceBeforeField ? 5 : 15;
        //descriptionParent.style.marginTop = PlaceBeforeField ? i == 2 ? 15 : 0 : 3;
        descriptionParent.style.marginTop = 2;
        descriptionParent.style.marginRight = -2;
        descriptionParent.style.marginBottom = 20;

        descriptionParent.SetBorderWidthAll(1);
        descriptionParent.SetBorderRadiusAll(3);

        descriptionParent.style.backgroundColor = UnityVariablesUtility.GetVariable<Color>(UnityVariableName.InputField_Background_Color, true);
        descriptionParent.SetBorderColorAll(UnityVariablesUtility.GetVariable<Color>(UnityVariableName.InputField_Border_Color, true));

        var descriptionHeaderLabel = new Label($"{"Overview".Bold()}");
        descriptionHeaderLabel.SetTextWrapping(true);
        descriptionHeaderLabel.style.paddingLeft = 3;
        descriptionHeaderLabel.SetTextAlign(TextAnchor.MiddleCenter);
        descriptionParent.Add(descriptionHeaderLabel);

        descriptionParent.Add(descriptionLabel);

        parent.Add(descriptionParent);
        descriptionParent.PlaceInFront(headerToolbar);
        descriptionParent.ToggleVisibility();
      }
    }

    private VisualElement AddDescriptionsToggleButton(VisualElement root)
    {
      var headerToolbar = UIToolkitStyleUtilities.AddHeaderToolbar(root, true);
      UIToolkitStyleUtilities.AddToggleDescriptionsButton(root, headerToolbar);
      return headerToolbar;
    }
#endif
  }
}