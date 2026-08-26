using UnityEditor;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  public class UnityPackageBundleDataList : ListFromScriptableObjectItemDataCreator<UnityPackageBundleData>
  {
    protected string bundleImportButtonClass;

    public UnityPackageBundleDataList(string groupName, VisualTreeAsset itemTemplate, VisualElement root,
      string itemDisplayNameLabelClass = "label__item-name", string itemDescriptionNameLabelClass = "label__item-description",
      string refreshButtonClass = "button__refresh-list", string bundleImportButtonClass = "bundle-list-item__import-button")
      : base(groupName, itemTemplate, root, itemDisplayNameLabelClass, itemDescriptionNameLabelClass, refreshButtonClass)
    {
      this.bundleImportButtonClass = bundleImportButtonClass;
    }

    protected override void BindListItem(VisualElement elem, int index)
    {
      base.BindListItem(elem, index);

      if (data.Count <= index) { return; }

      var item = data[index];

      if (item == null) { return; }

      UIToolkitUtilities.SetupButton(elem, "Import",
        () => ImportBundle(item), null, bundleImportButtonClass);
    }

    private void ImportBundle(UnityPackageBundleData bundle)
    {
      if (bundle == null) { return; }

      var package = bundle.Package;

      if (package == null) { return; }

      var assetPath = AssetDatabase.GetAssetPath(package);
      ProjectUtilities.ImportUnityPackageFromFilePathWithDialog(assetPath);
    }
  }
}