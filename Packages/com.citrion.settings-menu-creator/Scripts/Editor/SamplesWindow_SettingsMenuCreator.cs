using CitrioN.Common.Editor;
using UnityEditor;

namespace CitrioN.SettingsMenuCreator.Editor
{
  public class SamplesWindow_SettingsMenuCreator : SamplesWindowBase
  {
    protected override string SamplesGroupName => "SMC: Samples";

    //[MenuItem("Tools/CitrioN/Settings Menu Creator/Show Samples")]
    public static void ShowWindow()
    {
      ShowWindow(typeof(SamplesWindow_SettingsMenuCreator), "SMC: Samples");
    }
  }
}