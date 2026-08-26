using CitrioN.Common.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator.Editor
{
  [CreateAssetMenu(fileName = "WelcomeController_",
                   menuName = "CitrioN/Settings Menu Creator/Editor/ScriptableObjects/VisualTreeAsset/Controller/Welcome")]
  public class WelcomeController_SettingsMenuCreator : WelcomeController
  {
    protected string customerPaidTextKey = "customer-paid";
    protected string customerFreeTextKey = "customer-free";

    public override void Setup(VisualElement root)
    {
      base.Setup(root);

      if (root == null) { return; }

      string packageVersion;
      var hasProVersion = PackageUtilities.IsPackageInstalled("com.citrion.settings-menu-creator.pro", out packageVersion);
      var hasInputRebind = PackageUtilities.IsPackageInstalled("com.citrion.settings-menu-creator.input", out packageVersion);
      var hasPostProcess = PackageUtilities.IsPackageInstalled("com.citrion.settings-menu-creator.post-processing", out packageVersion);
      var hasSrp = PackageUtilities.IsPackageInstalled("com.citrion.settings-menu-creator.srp", out packageVersion);
      var hasPaidVersion = hasProVersion || hasInputRebind || hasPostProcess || hasSrp;

      SetLabelText(root, customerLabelClass, hasPaidVersion ? customerPaidTextKey : customerFreeTextKey);
    }
  }
}