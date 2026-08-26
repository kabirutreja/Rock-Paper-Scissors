using UnityEngine;

namespace QuantumTek.SimpleMenu
{
    /// <summary> A tab in a tab window. </summary>
    [AddComponentMenu("Quantum Tek/Simple Menu/Tab")]
    [DisallowMultipleComponent]
    public class SM_Tab : MonoBehaviour
    {
        [Header("Object References")]
        [Tooltip("A reference to the tab's animator.")]
        [SerializeField] protected Animator animator;
        [Space]
        [Header("Animation Variables")]
        [Tooltip("How the animation will be handled for activating/deactivating the tab.")]
        [SerializeField] protected SM_AnimationType animationType;
        [Tooltip("The name of the animator boolean used, if applicable.")]
        [SerializeField] protected string animatorBool = "Shown";
        [Tooltip("The name of the animator trigger used to show the tab as active, if applicable.")]
        [SerializeField] protected string animatorShowTrigger = "Show";
        [Tooltip("The name of the animator trigger used to hide the tab, if applicable.")]
        [SerializeField] protected string animatorHideTrigger = "Hide";

        /// <summary> Whether or not the tab is active. </summary>
        [HideInInspector] public bool active;

        /// <summary> Toggles the active state of the tab. </summary>
        /// <param name="shown">Whether or not the tab should be shown.</param>
        public void Toggle(bool shown)
        {
            active = shown;
            
            if (animator)
            {
                if (animationType == SM_AnimationType.AnimatorBool) animator.SetBool(animatorBool, shown);
                else if (animationType == SM_AnimationType.AnimatorTrigger) animator.SetBool(shown ? animatorShowTrigger : animatorHideTrigger, shown);
            }
        }
    }
}