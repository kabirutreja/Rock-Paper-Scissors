using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  public class UnityPackageBundleDataScrollViewList : ScrollViewListFromScriptableObjectItemDataCreator<IntegrationPackageData>
  {
    protected VisualTreeAsset tagTemplate;
    protected string packageImportButtonClass;
    protected string showAssetButtonClass;
    protected string integrationDetailsLabelClass;
    protected string integrationDetailsContainerClass;
    protected string tagContainerClass;

    protected const string assetImportedString = "Asset Imported";
    protected const string assetNotImportedString = "Asset Not Imported";
    protected const string integrationImportedString = "Integration Imported";
    protected const string integrationNotImportedString = "Integration Not Imported";

    protected Toggle hideNotImportedAssetsToggle;

    public UnityPackageBundleDataScrollViewList(string groupName, VisualTreeAsset itemTemplate, VisualTreeAsset tagTemplate, VisualElement root,
      string searchFieldClass = "toolbar-search-field",
      string itemDisplayNameLabelClass = "label__item-name", string itemDescriptionNameLabelClass = "label__item-description",
      string refreshButtonClass = "button__refresh-list", string packageImportButtonClass = "button__import",
      string showAssetButtonClass = "button__show-asset", string integrationDetailsContainerClass = "details-container",
      string integrationDetailsLabelClass = "label__integration-details", string tagContainerClass = "container__tags")
      : base(groupName, itemTemplate, root, searchFieldClass, itemDisplayNameLabelClass, itemDescriptionNameLabelClass, refreshButtonClass)
    {
      this.tagTemplate = tagTemplate;
      this.packageImportButtonClass = packageImportButtonClass;
      this.showAssetButtonClass = showAssetButtonClass;
      this.integrationDetailsContainerClass = integrationDetailsContainerClass;
      this.integrationDetailsLabelClass = integrationDetailsLabelClass;
      this.tagContainerClass = tagContainerClass;
    }

    protected override void Init()
    {
      base.Init();

      var toolbarExtras = root.Q(className: "toolbar__extras");
      if (toolbarExtras == null) { return; }

      hideNotImportedAssetsToggle = new Toggle("Hide not imported assets");
      hideNotImportedAssetsToggle.RegisterValueChangedCallback(OnHideIntegrationsToggleValueChanged);
      toolbarExtras.Add(hideNotImportedAssetsToggle);
    }

    protected override void BindListItem(VisualElement elem, int index)
    {
      base.BindListItem(elem, index);

      if (data.Count <= index) { return; }

      var item = data[index];

      if (item == null) { return; }

      var tagContainer = elem.Q(className: tagContainerClass);
      if (tagContainer != null && tagTemplate != null)
      {
        AddTagButtons(tagContainer, item.IsIntegrationAssetImported ? assetImportedString : assetNotImportedString
          /*item.IsIntegrationImported ? integrationImportedString : integrationNotImportedString*/);
      }

      var displayNameLabel = elem.Q<Label>(className: itemDisplayNameLabelClass);
      displayNameLabel?.SetText(item.DisplayName);

      var detailsLabel = elem.Q<Label>(className: integrationDetailsLabelClass);
      detailsLabel?.SetText(item.IntegrationDetails);

      bool hasDetails = !string.IsNullOrEmpty(item.IntegrationDetails);
      var integrationDetailsContainer = elem.Q(className: integrationDetailsContainerClass);
      integrationDetailsContainer?.Show(hasDetails);

      var containers = elem.Query(className: "container").ToList();
      foreach (var c in containers)
      {
        c?.SetBackgroundColor(UnityVariablesUtility.GetVariable<Color>(UnityVariableName.Toolbar_Background_Color, true));
      }

      UIToolkitUtilities.SetupButton(elem, item.IsIntegrationImported ? "Reimport Integration" : "Import Integration",
        () => ImportPackage(item), null, packageImportButtonClass);

      UIToolkitUtilities.SetupButton(elem, "Asset Store",
        () => ShowAsset(item), null, showAssetButtonClass);
    }

    protected override void UpdateSearch(string searchValue)
    {
      base.UpdateSearch(searchValue);

      if (!hideNotImportedAssetsToggle.value) { return; }
      HideIntegrationsForNotImportedAssets();
    }

    protected void OnHideIntegrationsToggleValueChanged(ChangeEvent<bool> evt)
    {
      if (evt.newValue != evt.previousValue && searchField != null)
      {
        UpdateSearch(searchField.value);
      }
    }

    protected void HideIntegrationsForNotImportedAssets()
    {
      for (int i = 0; i < data.Count; i++)
      {
        var item = data[i];

        if (item == null) { continue; }

        if (items.TryGetValue(item, out var elem))
        {
          if (!item.IsIntegrationAssetImported)
          {
            elem.Show(false);
          }
        }
      }
    }

    private void AddTagButtons(VisualElement tagContainer, params string[] tags)
    {
      for (int i = 0; i < tags.Length; i++)
      {
        var tagElement = tagTemplate.Instantiate();
        tagContainer.Add(tagElement);
        var button = tagElement.Q<Button>();
        if (button == null) { continue; }
        button.SetText(tags[i]);
        button.pickingMode = PickingMode.Ignore;
      }
    }

    private void ImportPackage(IntegrationPackageData data)
    {
      if (data == null) { return; }

      var package = data.Package;

      if (package == null) { return; }

      var assetPath = AssetDatabase.GetAssetPath(package);
      ProjectUtilities.ImportUnityPackageFromFilePathWithDialog(assetPath);
    }

    private void ShowAsset(IntegrationPackageData data)
    {
      if (data == null) { return; }

      var url = data.AssetUrl;

      if (url == null) { return; }

      Application.OpenURL(url);
    }
  }
}