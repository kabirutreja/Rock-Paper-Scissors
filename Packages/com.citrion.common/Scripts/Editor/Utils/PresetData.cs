using UnityEditor.Presets;
using UnityEngine;

namespace CitrioN.Common.Editor
{
  [CreateAssetMenu(fileName = "PresetData_",
                   menuName = "CitrioN/Common/Presets/PresetData")]
  public class PresetData : ScriptableObject
  {
    [SerializeField]
    [Tooltip("The preset to use")]
    private Preset preset;
    [SerializeField]
    [Tooltip("The preset filter to apply")]
    private string filter;
    [SerializeField]
    [Tooltip("The preset priority.\n\n" +
             "A higher priority will place the preset higher\n" +
             "in the Preset Manager list for its type.")]
    private int priority = 0;

    public Preset Preset { get => preset; set => preset = value; }
    public string Filter { get => filter; set => filter = value; }
    public int Priority { get => priority; set => priority = value; }
  }
}