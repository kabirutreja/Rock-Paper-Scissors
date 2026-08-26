using CitrioN.Common.Editor;
using UnityEditor;

namespace CitrioN.SettingsMenuCreator.Editor
{
  [CustomPropertyDrawer(typeof(InputElementProvider_UGUI))]
  public class InputElementProviderDrawer_UGUI : PropertyDrawerFromTemplateBase
  {
    protected override string PropertyFieldClass => "property-field__input-elements-provider-ugui";
  }
}