using UnityEngine;
using UnityEngine.UI;

namespace QuantumTek.SimpleMenu
{
    /// <summary> A script to save the value of a slider. </summary>
    [AddComponentMenu("Quantum Tek/Simple Menu/Save Slider")]
    [DisallowMultipleComponent]
    public class SM_SaveSlider : MonoBehaviour
    {
        [Tooltip("The name to store the data under.")]
        [SerializeField] protected string settingName;
        [Tooltip("The script to get the data from.")]
        [SerializeField] protected Slider reference;

        protected void Awake()
        {
            if (!reference || settingName.Length == 0) return;
            reference.value = PlayerPrefs.GetFloat(settingName);
        }

        /// <summary> Saves the data from the corresponding component to the PlayerPrefs. </summary>
        public void Save()
        {
            if (!reference || settingName.Length == 0) return;
            PlayerPrefs.SetFloat(settingName, reference.value);
            PlayerPrefs.Save();
        }
    }
}