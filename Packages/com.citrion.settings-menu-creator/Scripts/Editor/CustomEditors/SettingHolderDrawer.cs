using CitrioN.Common;
using CitrioN.Common.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator.Editor
{
  [CustomPropertyDrawer(typeof(SettingHolder))]
  public class SettingHolderDrawer : SettingHolderDrawerBase<SettingHolder>
  {
    protected const string APPLY_IMMEDIATELY_SYNC_BUTTON_CLASS = "apply-immediately__button";
    protected const string OVERRIDE_IDENTIFIER_WHEN_COPIED_SYNC_BUTTON_CLASS = "override-identifier-when-copied__button";

    protected override void SetupVisualElements(SerializedProperty property, VisualElement root)
    {
      base.SetupVisualElements(property, root);
      SettingHolder holder = null;

      var syncButton_ApplyImmediately = root.Q<Button>(className: APPLY_IMMEDIATELY_SYNC_BUTTON_CLASS);
      if (syncButton_ApplyImmediately != null)
      {
        syncButton_ApplyImmediately.tooltip = "Apply this value to all\ncurrently selected settings.";
        if (holder != null || EditorUtilities.GetPropertyValue(property, out holder))
        {
          syncButton_ApplyImmediately.clicked += () => OnSyncSelectedButtonClicked_ApplyImmediately(holder);
        }
      }

      var syncButton_OverrideIdentifierWhenCopied = root.Q<Button>(className: OVERRIDE_IDENTIFIER_WHEN_COPIED_SYNC_BUTTON_CLASS);
      if (syncButton_OverrideIdentifierWhenCopied != null)
      {
        syncButton_OverrideIdentifierWhenCopied.tooltip = "Apply this value to all\ncurrently selected settings.";
        if (holder != null || EditorUtilities.GetPropertyValue(property, out holder))
        {
          syncButton_OverrideIdentifierWhenCopied.clicked += () => OnSyncSelectedButtonClicked_OverrideIdentifierWhenCopied(holder);
        }
      }
    }

    protected void OnSyncSelectedButtonClicked_ApplyImmediately(SettingHolder holder)
    {
      if (holder != null)
      {
        GlobalEventHandler.InvokeEvent("Sync ApplyImmediately", holder.ApplyImmediately);
      }
    }

    protected void OnSyncSelectedButtonClicked_OverrideIdentifierWhenCopied(SettingHolder holder)
    {
      if (holder != null)
      {
        GlobalEventHandler.InvokeEvent("Sync OverrideIdentifierWhenCopied", holder.OverrideIdentifierWhenCopied);
      }
    }
  }
}