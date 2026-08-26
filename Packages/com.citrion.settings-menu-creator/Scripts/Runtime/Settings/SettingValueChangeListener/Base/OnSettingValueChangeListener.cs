using CitrioN.Common;
using UnityEngine;
using UnityEngine.Events;

namespace CitrioN.SettingsMenuCreator
{
  [AddTooltips]
  [HeaderInfo("Allows to react to value changes of a setting. " +
              "A SettingsCollection reference or a collection identifier can be used to " +
              "ignore value changes of unwanted collections. A setting identifier needs to be " +
              "specified for the listener to know which setting to react to.")]
  public class OnSettingValueChangeListener<T> : MonoBehaviour
  {
    [Header("Setting")]
    [SerializeField]
    [Tooltip("The setting identifier to react to. " +
             "This identifier needs to match or no reaction will happen.")]
    protected string settingIdentifier = string.Empty;

    [Header("Settings Collection")]
    [SerializeField]
    [Tooltip("A reference to a SettingsCollection to react to. " +
             "Value changes from collections that are not the referenced will be ignored. " +
             "The 'Settings Collection Identifier' field will also be ignored if a collection is referenced.")]
    protected SettingsCollection settingsCollection;

    [SerializeField]
    [Tooltip("A settings collection identifier to react to. " +
             "If no direct SettingsCollection is referenced this identifier can be used to ignore value changes of " +
             "collections with a different identifier. This is more flexible than a direct reference because " +
             "multiple SettingsCollections could have the same identifier. This allows the replacement of collections " +
             "while allowing this listener to still react to their setting value changes.")]
    protected string settingsCollectionIdentifier = string.Empty;

    [Space(10)]

    [SerializeField]
    [Tooltip("The action(s) to invoke when a matching setting value was changed.")]
    protected UnityEvent<T> onSettingValueChanged = new UnityEvent<T>();

    [SerializeField]
    [Tooltip("The action(s) to invoke when a matching setting value was changed. " +
             "Includes the SettingsCollection as the first parameter.")]
    protected UnityEvent<SettingsCollection, T> onSettingValueChangedWithCollection = new UnityEvent<SettingsCollection, T>();

    private void Awake()
    {
      AddListener();
    }

    private void OnEnable()
    {
      AddListener();

      if (settingsCollection != null)
      {
        if (settingsCollection.GetCurrentSettingValue<T>(settingIdentifier, out var value))
        {
          OnSettingValueChanged(null, settingIdentifier, settingsCollection, value);
        }
      }
    }

    private void OnDisable()
    {
      RemoveListener();
    }

    private void AddListener()
    {
      RemoveListener();
      GlobalEventHandler.AddEventListener<Setting, string, SettingsCollection, object>
        (SettingsMenuVariables.SETTING_VALUE_CHANGED_EVENT_NAME, OnSettingValueChanged);
    }

    private void RemoveListener()
    {
      GlobalEventHandler.RemoveEventListener<Setting, string, SettingsCollection, object>
        (SettingsMenuVariables.SETTING_VALUE_CHANGED_EVENT_NAME, OnSettingValueChanged);
    }

    protected virtual bool CanReactToChange(Setting setting, string settingIdentifier, SettingsCollection collection, object value, out T newValue)
    {
      if (string.IsNullOrEmpty(this.settingIdentifier))
      {
        ConsoleLogger.LogWarning("A setting identifier needs to be specified " +
                                 "to react to a setting value change!");
        newValue = default(T);
        return false;
      }

      if (this.settingIdentifier == settingIdentifier && value is T actualValue)
      {
        bool isCollectionSpecified = settingsCollection != null;
        bool isCollectionIdentifierSpecified = !string.IsNullOrEmpty(settingsCollectionIdentifier);

        if ((!isCollectionSpecified && !isCollectionIdentifierSpecified) ||
           (isCollectionSpecified && settingsCollection == collection) ||
           (!isCollectionSpecified && isCollectionIdentifierSpecified &&
             settingsCollectionIdentifier == collection.Identifier))
        {
          newValue = actualValue;
          return true;
        }
      }

      newValue = default(T);
      return false;
    }

    private void OnSettingValueChanged(Setting setting, string settingIdentifier, SettingsCollection collection, object value)
    {
      if (CanReactToChange(setting, settingIdentifier, collection, value, out T newValue))
      {
        onSettingValueChanged?.Invoke(newValue);
        onSettingValueChangedWithCollection?.Invoke(collection, newValue);
        OnSettingValueChanged(newValue);
      }
    }

    protected virtual void OnSettingValueChanged(T newValue)
    {

    }
  }
}
