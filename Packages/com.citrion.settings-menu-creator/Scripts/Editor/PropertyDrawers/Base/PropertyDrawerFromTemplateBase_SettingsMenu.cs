using CitrioN.Common.Editor;

namespace CitrioN.SettingsMenuCreator.Editor
{
  public class PropertyDrawerFromTemplateBase_SettingsMenu : PropertyDrawerFromTemplateBase
  {
    public override string UxmlPath
      => $"Packages/com.citrion.settings-menu-creator/Content/UI Toolkit/UXML/PropertyDrawers/{GetType().Name}.uxml";
    public override string StyleSheetPath
      => $"Packages/com.citrion.settings-menu-creator/Content/UI Toolkit/USS/PropertyDrawers/{GetType().Name}";
  }
}