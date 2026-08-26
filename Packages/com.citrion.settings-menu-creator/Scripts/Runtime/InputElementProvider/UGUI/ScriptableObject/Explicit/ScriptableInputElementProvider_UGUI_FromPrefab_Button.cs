using CitrioN.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [CreateAssetMenu(fileName = "Provider_UGUI_Button_",
                   menuName = "CitrioN/Settings Menu Creator/Input Element Provider/UGUI/Button",
                   order = 58)]
  public class ScriptableInputElementProvider_UGUI_FromPrefab_Button : ScriptableInputElementProvider_UGUI_FromPrefab
  {
    private static Dictionary<Button, UnityEngine.Events.UnityAction> onClickListeners = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
      onClickListeners.Clear();
    }

    public override Type GetInputFieldType(SettingsCollection settings) => typeof(Button);

    public override bool UpdateInputElement(RectTransform elem, string settingIdentifier,
                                                string labelText, SettingsCollection settings,
                                                List<object> values, bool initialize)
    {
      var success = base.UpdateInputElement(elem, settingIdentifier, labelText, settings, values, initialize);

      if (initialize && Application.isPlaying)
      {
        var button = elem.GetComponentInChildren<Button>(true);
        if (button == null) { return false; }
        //button.SetOnClickListener(() => settings?.ApplySettingChange(settingIdentifier, forceApply: false, false, null));

        bool listenerExists = onClickListeners.TryGetValue(button, out var listener);

        if (listenerExists && listener != null)
        {
          button.onClick.RemoveListener(listener);
        }

        listener = () =>
        {
          if (settings != null)
          {
            settings.ApplySettingChange(settingIdentifier, forceApply: false, false, null);
          }
        };

        onClickListeners.AddOrUpdateDictionaryItem(button, listener);
        button.onClick.AddListener(listener);

        // TODO Do we need the return true here?
        return true;
      }
      return success;
    }
  }
}
