using UnityEngine;

namespace QuantumTek.SimpleMenu
{
    /// <summary> A script to save the value of an option list. </summary>
    [AddComponentMenu("Quantum Tek/Simple Menu/Option List")]
    [DisallowMultipleComponent]
    public class SM_SaveOptionList : MonoBehaviour
    {
        [Tooltip("The name to store the data under.")]
        [SerializeField] protected string settingName;
        [Tooltip("The script to get the data from.")]
        [SerializeField] protected SM_OptionList reference;

        protected void Awake()
        {
            if (!reference || settingName.Length == 0) return;
            reference.SetOption(PlayerPrefs.GetInt(settingName));
        }

        /// <summary> Saves the data from the corresponding component to the PlayerPrefs. </summary>
        public void Save()
        {
            if (!reference || settingName.Length == 0) return;
            PlayerPrefs.SetInt(settingName, reference.current);
            PlayerPrefs.Save();
        }
    }
}