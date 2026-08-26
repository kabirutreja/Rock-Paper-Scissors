using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuantumTek.SimpleMenu
{
    /// <summary> Decides how windows, tab groups, tab windows, and tabs are animated. </summary>
    [System.Serializable]
    public enum SM_AnimationType
    {
        /// <summary> Will toggle the content game object's active state. </summary>
        ActiveState,
        /// <summary> Will set an animation parameter that is a boolean. </summary>
        AnimatorBool,
        /// <summary> Will set an animation parameter that is a trigger. </summary>
        AnimatorTrigger
    }

    /// <summary> The backbone of the menu system that serves as a parent to all the windows and tab groups. </summary>
    [AddComponentMenu("Quantum Tek/Simple Menu/Menu")]
    [DisallowMultipleComponent]
    public class SM_Menu : MonoBehaviour
    {
        [Header("Object References")]
        [Tooltip("A reference to the menu's background, if there is one.")]
        [SerializeField] protected Transform background;
        [Tooltip("A reference to the loading bar, if there is one.")]
        [SerializeField] protected SM_Bar loadingProgress;
        /// <summary> The list of windows in this menu. This shouldn't be changed, as it's set by the GetWindows function. </summary>
        protected List<SM_Window> windows;
        /// <summary> The list of tab groups in this menu. This shouldn't be changed, as it's set by the GetTabGroups function. </summary>
        protected List<SM_TabGroup> tabGroups;

        protected void Awake()
        {
            // Find the windows and tab groups in this menu
            GetWindows();
            GetTabGroups();
        }

        protected void GetWindows()
        {
            SM_Window[] tempWindows = GetComponentsInChildren<SM_Window>();
            windows = new List<SM_Window>(tempWindows);
        }
        protected void GetTabGroups()
        {
            SM_TabGroup[] tempTabGroups = GetComponentsInChildren<SM_TabGroup>();
            tabGroups = new List<SM_TabGroup>(tempTabGroups);
        }

        /// <summary> Quits the application. </summary>
        public void Quit()
        { Application.Quit(); }

        /// <summary> Loads a scene while showing a loading screen. </summary>
        /// <param name="buildIndex">The build index of the scene.</param>
        public void LoadScene(int buildIndex)
        {
            if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings) { Debug.LogWarning("Tried to load the scene with build index " + buildIndex + ", but the build index was out of range."); return; }
            StartCoroutine(LoadSceneAsync(buildIndex));
        }
        /// <summary> Loads a scene while showing a loading screen. </summary>
        /// <param name="sceneName">The name of the scene.</param>
        public void LoadScene(string sceneName)
        {
            if (sceneName.Length == 0) { Debug.LogWarning("Tried to load a scene with no name."); return; }
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        protected IEnumerator LoadSceneAsync(int buildIndex)
        {
            // Start loading the scene.
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(buildIndex);

            // Update loading graphic.
            while (!loadOperation.isDone)
            {
                // Get load progress, dividing by 0.9 because that is where it stops.
                float loadProgress = Mathf.Clamp01(loadOperation.progress / 0.9f);

                if (loadingProgress) loadingProgress.SetFill(loadProgress);

                yield return null;
            }
        }
        protected IEnumerator LoadSceneAsync(string sceneName)
        {
            // Start loading the scene.
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);

            // Update loading graphic.
            while (!loadOperation.isDone)
            {
                // Get load progress, dividing by 0.9 because that is where it stops.
                float loadProgress = Mathf.Clamp01(loadOperation.progress / 0.9f);

                if (loadingProgress) loadingProgress.SetFill(loadProgress);

                yield return null;
            }
        }

        /// <summary> Toggles the active state of the background. </summary>
        /// <param name="shown">Whether or not the background is shown.</param>
        public void ToggleBackground(bool shown)
        { if (background) background.gameObject.SetActive(shown); }
        /// <summary> Toggles the active state of all windows in the menu. </summary>
        /// <param name="shown">Whether or not the windows are shown.</param>
        public void ToggleWindows(bool shown)
        {
            int windowCount = windows.Count;
            for (int i = 0; i < windowCount; ++i)
            { windows[i].Toggle(shown); }
        }
        /// <summary> Toggles the active state of all tab groups in the menu. </summary>
        /// <param name="shown">Whether or not the tab groups are shown.</param>
        public void ToggleTabGroups(bool shown)
        {
            int tabGroupCount = tabGroups.Count;
            for (int i = 0; i < tabGroupCount; ++i)
            { tabGroups[i].Toggle(shown); }
        }
        /// <summary> Toggles the active state of everything in the menu. </summary>
        /// <param name="shown">Whether or not the menu is shown.</param>
        public void ToggleMenu(bool shown)
        {
            ToggleBackground(shown);
            ToggleWindows(shown);
            ToggleTabGroups(shown);
        }
    }
}