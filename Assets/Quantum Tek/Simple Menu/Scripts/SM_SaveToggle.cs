using UnityEngine;
using UnityEngine.UI;

namespace QuantumTek.SimpleMenu
{
    /// <summary> A script to save the value of a toggle. </summary>
    [AddComponentMenu("Quantum Tek/Simple Menu/Save Toggle")]
    [DisallowMultipleComponent]
    public class SM_SaveToggle : MonoBehaviour
    {
        [Tooltip("The name to store the data under.")]
        [SerializeField] protected string settingName;
        [Tooltip("The script to get the data from.")]
        [SerializeField] protected Toggle reference;

        protected void Awake()
        {
            if (!reference || settingName.Length == 0) return;
            reference.isOn = PlayerPrefs.GetInt(settingName) == 1;
        }

        /// <summary> Saves the data from the corresponding component to the PlayerPrefs. </summary>
        public void Save()
        {
            if (!reference || settingName.Length == 0) return;
            PlayerPrefs.SetInt(settingName, reference.isOn ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}