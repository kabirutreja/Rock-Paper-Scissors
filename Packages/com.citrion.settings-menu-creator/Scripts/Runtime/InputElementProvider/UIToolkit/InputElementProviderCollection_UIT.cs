using CitrioN.Common;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [HeaderInfo("A profile with information about UI Toolkit input element providers used by the settings menu system. " +
              "An input element provider will create and initialize an input element (dropdown, slider etc.) for a setting. " +
              "By default the settings menu system uses the setting's value type (Boolean, Enum etc.) to automatically " +
              "choose the input element provider to use.\n\nExample:\n" +
              "The provider named 'Boolean' will be used for all settings of type boolean which use the auto select provider option " +
              "(specified by default).\n\n\n" +
              "You can also assign a custom name to a provider and use it in combination " +
              "with the 'From Name' provider option in each setting's advanced tab on the SettingsCollection object.\n\nExample:\n" +
              "You can assign a name such as 'Title' to any provider in your collection and use the 'From Name' with 'Title' option on a setting to " +
              "have it use that specific provider for managing its input element (generation) and initialization.",
              OverrideExisting = true)]
  [CreateAssetMenu(fileName = "InputElementProviderCollection_UIT_",
                   menuName = "CitrioN/Settings Menu Creator/Provider Collection/UI Toolkit/Default",
                   order = 6)]
  public class InputElementProviderCollection_UIT : StringToGenericDataRelationProfile<ScriptableInputElementProvider_UIT>
  {

  }
}
