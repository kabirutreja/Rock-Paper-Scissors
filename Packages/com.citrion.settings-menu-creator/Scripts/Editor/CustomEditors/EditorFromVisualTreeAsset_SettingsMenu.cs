using CitrioN.Common.Editor;

namespace CitrioN.SettingsMenuCreator.Editor
{
  public class EditorFromVisualTreeAsset_SettingsMenu : EditorFromVisualTreeAsset
  {
    public override string UxmlPath
      => $"Packages/com.citrion.settings-menu-creator/UI Toolkit/UXML/Editors/{GetType().Name}.uxml";
    public override string StyleSheetPath
      => $"Packages/com.citrion.settings-menu-creator/UI Toolkit/USS/Editors/{GetType().Name}";
  }
}