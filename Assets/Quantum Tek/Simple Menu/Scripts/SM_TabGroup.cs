using System.Collections.Generic;
using UnityEngine;

namespace QuantumTek.SimpleMenu
{
    /// <summary> How the tabs are aligned at the top of a tab group. </summary>
    [System.Serializable]
    public enum SM_TabAlign
    {
        Center,
        Left,
        Right
    }

    /// <summary> Handles everything in a tab group, from the tab windows to which one is open. </summary>
    [AddComponentMenu("Quantum Tek/Simple Menu/Tab Group")]
    [DisallowMultipleComponent]
    public class SM_TabGroup : MonoBehaviour
    {
        [Header("Object References")]
        [Tooltip("A reference to the tab group's content.")]
        [SerializeField] protected Transform content;
        [Tooltip("A reference to the tab group's animator.")]
        [SerializeField] protected Animator animator;
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
        [Space]
        [Header("Tab Variables")]
        [Tooltip("How far from the left and right side the tabs will be during left and right align, and how far off the top of the window they will be.")]
        [SerializeField] protected Vector2 tabOffset;
        [Tooltip("The alignment that the tabs use in the AlignTabs function.")]
        [SerializeField] protected SM_TabAlign alignment;

        /// <summary> A list of the windows in the tab group. Don't change unless through the GetWindows function. </summary>
        protected List<SM_TabWindow> windows;
        /// <summary> A list of the tabs in the tab group. Don't change. </summary>
        protected List<SM_Tab> tabs;
        /// <summary> A reference to the currently selected tab window. Don't change, use the ChangeTab function. </summary>
        protected SM_TabWindow current;
        /// <summary> Whether or not the tab group is active or shown. Don't change this, as it's changed by the Toggle function. </summary>
        [HideInInspector] public bool active;

        protected void Start()
        {
            // Get active state from start
            if (content) active = content.gameObject.activeSelf;
            // Find the tab windows in this tab group
            GetWindows();

            // Get starting window
            int windowCount = windows.Count;
            for (int i = 0; i < windowCount; ++i)
            { if (windows[i].content.gameObject.activeSelf) current = windows[i]; }

            ChangeTab(current);
            
            Toggle(active);
        }

        protected void GetWindows()
        {
            SM_TabWindow[] tempWindows = content.GetComponentsInChildren<SM_TabWindow>();
            windows = new List<SM_TabWindow>(tempWindows);
            
            tabs = new List<SM_Tab>();
            int windowCount = windows.Count;
            for (int i = 0; i < windowCount; ++i)
            { tabs.Add(windows[i].tab); }
        }

        /// <summary> Toggles the active state of the tab group. </summary>
        /// <param name="shown">Whether or not the tab group should be shown.</param>
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

        /// <summary> Changes the current tab. </summary>
        /// <param name="tab">The tab window to change to.</param>
        public void ChangeTab(SM_TabWindow tab)
        {
            if (!tab) return;
            if (current) current.Toggle(false);
            current = tab;
            if (current) current.Toggle(true);
        }

        /// <summary> Aligns the tabs of the tab group with the tab group's alignment. </summary>
        public void AlignTabs()
        {
            if (windows.Count == 0 || tabs.Count == 0) GetWindows();

            float tabsWidth = 0;
            int tabCount = tabs.Count;
            RectTransform tabTransform;
            // Find the total width.
            for (int i = 0; i < tabCount; ++i)
            { tabTransform = tabs[i].GetComponent<RectTransform>(); if (!tabTransform) continue; tabsWidth += tabTransform.rect.width; }
            // Align the tabs using the total width.
            float currentTabWidth = 0;
            for (int i = 0; i < tabCount; ++i)
            {
                tabTransform = tabs[i].GetComponent<RectTransform>();
                if (!tabTransform) continue;
                float tabWidth = tabTransform.rect.width;
                float tabHeight = tabTransform.rect.height;

                if (alignment == SM_TabAlign.Center)
                {
                    tabTransform.anchorMin = new Vector2(0.5f, 1);
                    tabTransform.anchorMax = new Vector2(0.5f, 1);
                    tabTransform.pivot = new Vector2(0.5f, 1);
                    tabTransform.anchoredPosition = new Vector2(-tabsWidth / 2 + tabsWidth / 2 / tabCount + currentTabWidth, tabHeight + tabOffset.y);
                    currentTabWidth += tabWidth;
                }
                else if (alignment == SM_TabAlign.Left)
                {
                    tabTransform.anchorMin = new Vector2(0, 1);
                    tabTransform.anchorMax = new Vector2(0, 1);
                    tabTransform.pivot = new Vector2(0, 1);
                    tabTransform.anchoredPosition = new Vector2(currentTabWidth + tabOffset.x, tabHeight + tabOffset.y);
                    currentTabWidth += tabWidth;
                }
                else if (alignment == SM_TabAlign.Right)
                {
                    currentTabWidth += tabWidth;
                    tabTransform.anchorMin = new Vector2(1, 1);
                    tabTransform.anchorMax = new Vector2(1, 1);
                    tabTransform.pivot = new Vector2(1, 1);
                    tabTransform.anchoredPosition = new Vector2(-tabsWidth + currentTabWidth - tabOffset.x, tabHeight + tabOffset.y);
                }
            }
        }
    }
}
