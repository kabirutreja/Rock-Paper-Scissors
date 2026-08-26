using CitrioN.Common.Editor;

namespace CitrioN.SettingsMenuCreator.Editor
{
  public class PropertyDrawerFromTemplate_SettingsMenu : PropertyDrawerFromTemplate
  {
    public override string UxmlPath
      => $"Packages/com.citrion.settings-menu-creator/UI Toolkit/UXML/PropertyDrawers/{GetType().Name}.uxml";
    public override string StyleSheetPath
      => $"Packages/com.citrion.settings-menu-creator/UI Toolkit/USS/PropertyDrawers/{GetType().Name}";
  }
}