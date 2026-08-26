using UnityEngine;
using TMPro;

namespace QuantumTek.SimpleMenu
{
    /// <summary> A script to save the value of a dropdown. </summary>
    [AddComponentMenu("Quantum Tek/Simple Menu/Save Dropdown")]
    [DisallowMultipleComponent]
    public class SM_SaveDropdown : MonoBehaviour
    {
        [Tooltip("The name to store the data under.")]
        [SerializeField] protected string settingName;
        [Tooltip("The script to get the data from.")]
        [SerializeField] protected TMP_Dropdown reference;

        protected void Awake()
        {
            if (!reference || settingName.Length == 0) return;
            reference.value = PlayerPrefs.GetInt(settingName);
        }

        /// <summary> Saves the data from the corresponding component to the PlayerPrefs. </summary>
        public void Save()
        {
            if (!reference || settingName.Length == 0) return;
            PlayerPrefs.SetInt(settingName, reference.value);
            PlayerPrefs.Save();
        }
    }
}