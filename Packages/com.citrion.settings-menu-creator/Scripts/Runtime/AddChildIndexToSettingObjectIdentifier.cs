using CitrioN.Common;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [HeaderInfo("Helper script that adds the child index to the SettingObject's identifier.", OverrideExisting = true)]
  public class AddChildIndexToSettingObjectIdentifier : SettingObjectIdentifierModifier
  {
    [SerializeField]
    [Tooltip("The prefix to add before the child index.")]
    protected string indexPrefix = "-";

    [SerializeField]
    [Tooltip("The suffix to add after the child index.")]
    protected string indexSuffix = string.Empty;

    public override void ApplyModifier()
    {
      if (!canApply) { return; }
      if (settingObject == null) { return; }

      var identifier = settingObject.Identifier;
      identifier = AddChildIndex(identifier);
      settingObject.Identifier = identifier;
      canApply = false;
    }

    protected string AddChildIndex(string input)
    {
      var index = transform.GetSiblingIndex();

      if (index > 0)
      {
        return $"{input}{indexPrefix}{index + 1}{indexSuffix}";
      }
      return input;
    }
  }
}