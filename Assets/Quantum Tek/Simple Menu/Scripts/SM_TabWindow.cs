using UnityEngine;

namespace QuantumTek.SimpleMenu
{
    /// <summary> A window in a tab group. </summary>
    [AddComponentMenu("Quantum Tek/Simple Menu/Tab Window")]
    [DisallowMultipleComponent]
    public class SM_TabWindow : MonoBehaviour
    {
        [Header("Object References")]
        [Tooltip("A reference to the tab group's content.")]
        public Transform content;
        [Tooltip("A reference to the tab group's animator.")]
        [SerializeField] protected Animator animator;
        [Tooltip("A reference to the tab window's tab.")]
        public SM_Tab tab;
        [Space]
        [Header("Animation Variables")]
        [Tooltip("How the animation will be handled for opening/closing the tab group.")]
        [SerializeField] protected SM_AnimationType animationType;
        [Tooltip("The name of the animator boolean used, if applicable.")]
        [SerializeField] protected string animatorBool = "Shown";
        [Tooltip("The name of the animator trigger used to show the tab group, if applicable.")]
        [SerializeField] protected string animatorShowTrigger = "Show";
        [Tooltip("The name of the animator trigger used to hide the tab group, if applicable.")]
        [SerializeField] protected string animatorHideTrigger = "Hide";

        /// <summary> Whether or not the tab window is active or shown. Don't change this, as it's changed by the Toggle function. </summary>
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
            if (tab && transform.parent.gameObject.activeSelf) tab.Toggle(shown);
            active = shown;

            if (animator && transform.parent.gameObject.activeSelf)
            {
                if (animationType == SM_AnimationType.ActiveState) { if (content) content.gameObject.SetActive(shown); }
                else if (animationType == SM_AnimationType.AnimatorBool) animator.SetBool(animatorBool, shown);
                else if (animationType == SM_AnimationType.AnimatorTrigger) animator.SetBool(shown ? animatorShowTrigger : animatorHideTrigger, shown);
            }
        }
    }
}