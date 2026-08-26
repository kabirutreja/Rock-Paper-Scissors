using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace QuantumTek.SimpleMenu
{
    /// <summary> A list of options to choose from. </summary>
    [AddComponentMenu("Quantum Tek/Simple Menu/Option List")]
    [DisallowMultipleComponent]
    public class SM_OptionList : MonoBehaviour
    {
        [Header("Object References")]
        [Tooltip("A reference to the text displaying the selected option.")]
        [SerializeField] protected TextMeshProUGUI optionText;
        [Space]
        [Header("Option Variables")]
        [Tooltip("The list of options to choose from.")]
        [SerializeField] protected List<string> options;
        [Tooltip("The index of the current option.")]
        public int current;
        [Tooltip("An event that triggers when changing the option.")]
        [SerializeField] protected UnityEvent onOptionChange;
        /// <summary> Returns the string of the current option. </summary>
        public string currentOption { get { return (current >= 0 && current < options.Count) ? options[current] : ""; } }

        protected void Awake()
        {
            // Ensure correct text from the start
            SetOption(current);
        }

        /// <summary> Changes the currently selected option by a certain amount. </summary>
        /// <param name="amount">The amount to change by.</param>
        public void ChangeOption(int amount)
        {
            current += amount;
            if (current >= options.Count) current = 0;
            else if (current < 0) current = options.Count - 1;
            SetOption(current);
        }

        /// <summary> Sets the currently selected option to a certain option. </summary>
        /// <param name="option">The new selected option.</param>
        public void SetOption(int option)
        {
            current = Mathf.Clamp(option, 0, options.Count);
            if (optionText && options.Count > 0 && current < options.Count) optionText.text = options[current];
            onOptionChange.Invoke();
        }
    }
}