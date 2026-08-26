using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CitrioN.Common
{
  /// <summary>
  /// Extension methods for the <see cref="UnityEngine.UI.Button"/> class
  /// </summary>
  public static class ButtonExtensions
  {
    /// <summary>
    /// Removes and set the action provided as the action to be executed when the button is clicked.
    /// </summary>
    /// <param name="action">The action to be invoked when the button is clicked</param>
    /// <param name="buttonDisplayText">An optional text if a text element is part of the hierarchy</param>
    public static void SetButtonFunctionality(this Button button, UnityAction action, string buttonDisplayText = "")
    {
      button.SetText(buttonDisplayText);
      button.SetOnClickListener(action);
    }

    public static void SetText(this Button button, string buttonDisplayText)
    {
      if (button == null) { return; }
#if TEXT_MESH_PRO
      var text_TMP = button.GetComponentInChildren<TMPro.TMP_Text>();
      if (text_TMP != null && text_TMP.enabled) text_TMP.SetText(buttonDisplayText);
#endif

      var text = button.GetComponentInChildren<Text>();
      if (text != null && text.enabled) text.text = buttonDisplayText;
    }

    public static void SetOnClickListener(this Button button, UnityAction action)
    {
      if (button == null)
      {
        Debug.LogWarning("Unable to add action to Button because Button is null.");
        return;
      }
      button.onClick.RemoveAllListeners();
      button.onClick.AddListener(action);
    }

    /// <summary>
    /// Adds the provided action to the onClick button event.
    /// Note: Only use real methods and not lambdas or anonymous methods.
    /// </summary>
    /// <param name="button">The button to add the action to</param>
    /// <param name="action">The action to add to the button</param>
    public static void AddOnClickListener(this Button button, UnityAction action)
    {
      if (button == null)
      {
        Debug.LogWarning("Unable to add action to Button because Button is null.");
        return;
      }
      button.onClick.RemoveListener(action);
      button.onClick.AddListener(action);
      //Debug.Log($"{button.name} has {button.onClick.GetPersistentEventCount()} listener(s)");
    }

    /// <summary>
    /// Removes the provided action from the onClick button event.
    /// Note: Only use real methods and not lambdas or anonymous methods.
    /// </summary>
    /// <param name="button">The button to remove the action from</param>
    /// <param name="action">The action to remove from the button</param>
    public static void RemoveOnClickListener(this Button button, UnityAction action)
    {
      if (button == null)
      {
        Debug.LogWarning("Unable to remove action from Button because Button is null.");
        return;
      }
      button.onClick.RemoveListener(action);
      //Debug.Log($"{button.name} has {button.onClick.GetPersistentEventCount()} listener(s)");
    }
  }
}