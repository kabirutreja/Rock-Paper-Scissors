using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  [CreateAssetMenu(fileName = "WelcomeController_",
                   menuName = "CitrioN/Common/ScriptableObjects/VisualTreeAsset/Controller/Welcome")]
  public class WelcomeController : ScriptableVisualTreeAssetController
  {
    public StringToStringRelationProfile texts;

    public string assetComparisonImageClass = "image__asset-comparison";

    public Sprite assetComparisonSpriteDarkSkin;

    public Sprite assetComparisonSpriteLightSkin;

    protected string introLabelClass = "label__intro";
    protected string customerLabelClass = "label__customer";
    protected string documentationLabelClass = "label__documentation";
    protected string samplesLabelClass = "label__samples";
    protected string getStartedLabelClass = "label__getstarted";
    protected string supportLabelClass = "label__support";
    protected string uninstallLabelClass = "label__uninstall";
    protected string thanksLabelClass = "label__thanks";

    protected string introTextKey = "intro";
    protected string documentationTextKey = "documentation";
    protected string samplesTextKey = "samples";
    protected string getStartedTextKey = "get-started";
    protected string supportTextKey = "support";
    protected string uninstallTextKey = "uninstall";
    protected string thanksTextKey = "thanks";

    public override void Setup(VisualElement root)
    {
      if (root == null) { return; }

      SetLabelText(root, introLabelClass, introTextKey);
      SetLabelText(root, documentationLabelClass, documentationTextKey);
      SetLabelText(root, samplesLabelClass, samplesTextKey);
      SetLabelText(root, getStartedLabelClass, getStartedTextKey);
      SetLabelText(root, supportLabelClass, supportTextKey);
      SetLabelText(root, uninstallLabelClass, uninstallTextKey);
      SetLabelText(root, thanksLabelClass, thanksTextKey);
    }

    protected void SetLabelText(VisualElement root, string className, string key)
    {
      if (root == null) { return; }

      var label = root.Q<Label>(className: className);

      root.SetFlexGrow(1);

      if (label != null)
      {
        var text = texts != null ? texts.GetValue(key) : string.Empty;

        label.SetText(text);
      }

      var assetComparisonImage = root.Q<VisualElement>(className: assetComparisonImageClass);

      if (assetComparisonImage != null)
      {
        bool isDarkSkin = EditorUtilities.IsDarkSkin;
        assetComparisonImage.style.backgroundImage = new StyleBackground(isDarkSkin ? assetComparisonSpriteDarkSkin : assetComparisonSpriteLightSkin);
      }
    }
  }
}