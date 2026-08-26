namespace CitrioN.SettingsMenuCreator
{
  public static class SettingsCollectionUtility
  {
    public static void AddSettingToCollection(Setting setting, SettingsCollection settingsCollection)
    {
      if (setting == null) { return; }

      var settingsHolder = new SettingHolder();
      settingsHolder.Setting = setting;

      AddSettingToCollection(settingsHolder, settingsCollection);
    }

    public static void AddSettingToCollection(SettingHolder holder, SettingsCollection settingsCollection)
    {
      if (settingsCollection == null || holder == null) { return; }

#if UNITY_EDITOR
      var serializedObject = new UnityEditor.SerializedObject(settingsCollection);
      UnityEditor.Undo.RecordObject(serializedObject.targetObject, "Added setting to collection");
#endif

      settingsCollection.Settings.Add(holder);

#if UNITY_EDITOR
      serializedObject.ApplyModifiedProperties();
      serializedObject.Update();
      UnityEditor.EditorUtility.SetDirty(serializedObject.targetObject);
#endif
    }

    public static void RemoveSettingFromCollection(int settingIndex, SettingsCollection settingsCollection)
    {
      if (settingsCollection == null || settingsCollection.Settings?.Count <= settingIndex) { return; }

#if UNITY_EDITOR
      var serializedObject = new UnityEditor.SerializedObject(settingsCollection);
      UnityEditor.Undo.RecordObject(serializedObject.targetObject, "Remove setting from collection");
#endif

      settingsCollection.Settings.RemoveAt(settingIndex);

#if UNITY_EDITOR
      serializedObject.ApplyModifiedProperties();
      serializedObject.Update();
      UnityEditor.EditorUtility.SetDirty(serializedObject.targetObject);
#endif
    }

    public static SettingHolder DuplicateSetting(int settingIndex, SettingsCollection settingsCollection)
    {
      if (settingsCollection == null || settingsCollection.Settings?.Count <= settingIndex) { return null; }

      var settingHolder = settingsCollection.Settings[settingIndex];
      var newSettingHolder = SettingHolder.GetCopy(settingHolder);

      AddSettingToCollection(newSettingHolder, settingsCollection);

      return newSettingHolder;
    }
  }
}
