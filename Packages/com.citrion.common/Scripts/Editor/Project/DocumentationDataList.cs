using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  public class DocumentationDataList : ListFromScriptableObjectItemDataCreator<DocumentationData>
  {
    protected const string SHOW_OFFLINE_DOCUMENTATION_BUTTON_CLASS = "button__offline-documentation";
    protected const string SHOW_ONLINE_DOCUMENTATION_BUTTON_CLASS = "button__online-documentation";

    public DocumentationDataList(string groupName, VisualTreeAsset itemTemplate, VisualElement root,
      string itemDisplayNameLabelClass = "label__item-name", string itemDescriptionNameLabelClass = "label__item-description",
      string refreshButtonClass = "button__refresh-list")
      : base(groupName, itemTemplate, root, itemDisplayNameLabelClass, itemDescriptionNameLabelClass, refreshButtonClass)
    { }

    protected override void BindListItem(VisualElement elem, int index)
    {
      base.BindListItem(elem, index);

      if (data.Count <= index) { return; }

      var item = data[index];

      string folderPath = item.OfflineDocumentationFolder;
      UIToolkitUtilities.SetupButton(elem, "Offline", 
        () => ShowFolder(folderPath), null, SHOW_OFFLINE_DOCUMENTATION_BUTTON_CLASS);
      
      string url = item.OnlineDocumentationLink;
      UIToolkitUtilities.SetupButton(elem, "Online", 
        () => OpenLink(url), null, SHOW_ONLINE_DOCUMENTATION_BUTTON_CLASS);
    }

    private void ShowFolder(string path)
    {
#if UNITY_EDITOR
      ProjectUtilities.ExpandFolder(path);
#endif
    }

    private void OpenLink(string link)
    {
      if (string.IsNullOrEmpty(link)) { return; }
      Application.OpenURL(link);
    }
  }
}