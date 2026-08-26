using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  [CreateAssetMenu(fileName = "DocumentationController_",
                   menuName = "CitrioN/Common/ScriptableObjects/VisualTreeAsset/Controller/Documentation")]
  public class DocumentationController : ScriptableVisualTreeAssetController
  {
    public string groupName;

    public VisualTreeAsset listItemTemplate;

    public string headerLabelClass = "manager__tab__title";

    public string descriptionLabelClass = "label__description";

    public string headerText = "Documentation";

    public string classDescriptionsImageClass = "image__class-descriptions";

    public Sprite classDescriptionSpriteDarkSkin;

    public Sprite classDescriptionSpriteLightSkin;

    [TextArea(2, 99)]
    public string description;

    public override void Setup(VisualElement root)
    {
      if (!string.IsNullOrEmpty(groupName) && listItemTemplate != null)
      {
        new DocumentationDataList(groupName, listItemTemplate, root);
      }

      if (root == null) { return; }

      // Set the root to flex grow so it takes up the entire tab content space.
      // This allows for the image to use all remaining content space.
      root.SetFlexGrow(1);

      root.Q<Label>(className: headerLabelClass)?.SetText(headerText);
      root.Q<Label>(className: descriptionLabelClass)?.SetText(description);

      var classDescriptionsImage = root.Q<VisualElement>(className: classDescriptionsImageClass);

      if (classDescriptionsImage != null)
      {
        bool isDarkSkin = EditorUtilities.IsDarkSkin;
        classDescriptionsImage.style.backgroundImage = new StyleBackground(isDarkSkin ? classDescriptionSpriteDarkSkin : classDescriptionSpriteLightSkin);
      }
    }
  }
}