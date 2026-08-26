using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  public static class QualitySettingsUtility
  {
    public static int QualitySettingsCount
      => QualitySettings.names?.Length > 0 ? QualitySettings.names.Length : 0;
  }
}