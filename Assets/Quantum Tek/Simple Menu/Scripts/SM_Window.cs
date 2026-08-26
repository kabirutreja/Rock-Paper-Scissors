using UnityEngine;

namespace QuantumTek.SimpleMenu
{
    /// <summary> Controls everything about a window in a menu. </summary>
    [AddComponentMenu("Quantum Tek/Simple Menu/Window")]
    [DisallowMultipleComponent]
    public class SM_Window : MonoBehaviour
    {
        [Header("Object References")]
        [Tooltip("A reference to the window's content.")]
        [SerializeField] protected Transform content;
        [Tooltip("A reference to the window's animator.")]
        [SerializeField] protected Animator animator;
        [Space]
        [Header("Animation Variables")]
        [Tooltip("How the animation will be handled for opening/closing the window.")]
        [SerializeField] protected SM_AnimationType animationType;
        [Tooltip("The name of the animator boolean used, if applicable.")]
        [SerializeField] protected string animatorBool = "Shown";
        [Tooltip("The name of the animator trigger used to show the window, if applicable.")]
        [SerializeField] protected string animatorShowTrigger = "Show";
        [Tooltip("The name of the animator trigger used to hide the window, if applicable.")]
        [SerializeField] protected string animatorHideTrigger = "Hide";

        /// <summary> Whether or not the window is active or shown. Don't change this, as it's changed by the Toggle function. </summary>
        [HideInInspector] public bool active;

        protected void Awake()
        {
            // Get active state from start
            if (content) active = content.gameObject.activeSelf;
            Toggle(active);
        }

        /// <summary> Toggles the active state of the window. </summary>
        /// <param name="shown">Whether or not the window should be shown.</param>
        public void Toggle(bool shown)
        {
            active = shown;

            if (animator)
            {
                if (animationType == SM_AnimationType.ActiveState) { if (content) content.gameObject.SetActive(shown); }
                else if (animationType == SM_AnimationType.AnimatorBool) animator.SetBool(animatorBool, shown);
                else if (animationType == SM_AnimationType.AnimatorTrigger) animator.SetBool(shown ? animatorShowTrigger : animatorHideTrigger, shown);
            }
        }
    }
}