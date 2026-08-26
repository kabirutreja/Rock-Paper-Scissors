using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;

namespace CitrioN.Common.Editor
{
  [CreateAssetMenu(fileName = "PresetDropdownData_",
                   menuName = "CitrioN/Common/Presets/PresetDropdownData")]
  public class PresetDropdownData : ScriptableObject
  {
    [SerializeField]
    [Tooltip("The preset to use")]
    private Preset preset;
    [SerializeField]
    [Tooltip("The name to display in the advanced dropdown window")]
    private string displayName;
    [SerializeField]
    [Tooltip("The path for the preset in the advanced dropdown")]
    private string dropdownPath;
    [SerializeField]
    [Tooltip("A list of groups to allow including/excluding the preset")]
    private List<string> groups = new List<string>();
    [SerializeField]
    [Tooltip("The preset priority.\n\n" +
             "A higher priority will place the preset higher in the list.")]
    private int priority = -1;

    public Preset Preset { get => preset; set => preset = value; }
    public int Priority { get => priority; set => priority = value; }
    public string DropdownPath { get => dropdownPath; set => dropdownPath = value; }
    public string DisplayName { get => displayName; set => displayName = value; }
    public List<string> Groups { get => groups; set => groups = value; }
  }
}
