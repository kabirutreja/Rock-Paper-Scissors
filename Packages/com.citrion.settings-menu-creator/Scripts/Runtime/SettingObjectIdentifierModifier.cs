using CitrioN.Common;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [AddTooltips]
  [HeaderInfo("Helper script that allows the modification of the SettingObject.")]
  public abstract class SettingObjectIdentifierModifier : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The SettingObject to modify the identifier for.")]
    protected SettingObject settingObject;

    [Tooltip("Whether the modifier can be applied.")]
    public bool canApply = true;

    protected virtual void Awake()
    {
      if (canApply)
      {
        ApplyModifier();
      }
    }

    public virtual void ApplyModifier()
    {
      if (!canApply) { return; }
      canApply = false;
    }
  }
}