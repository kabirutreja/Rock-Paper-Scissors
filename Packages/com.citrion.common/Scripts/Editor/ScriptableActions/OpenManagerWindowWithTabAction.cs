using System;
using UnityEngine;

namespace CitrioN.Common.Editor
{
  [HeaderInfo("Opens the (editor) manager window of the specified type. " +
              "The window will use the specified title and open the specified tab if possible. " +
              "The type needs to be the fully qualified class name.", 
              OverrideExisting = true)]
  [CreateAssetMenu(fileName = "ScriptableAction_OpenManagerWindowWithTab",
                   menuName = "CitrioN/Common/ScriptableObjects/ScriptableActions/Open Manager Window With Tab")]
  public class OpenManagerWindowWithTabAction : ScriptableAction
  {
    [SerializeField]
    [Tooltip("The 'Manager Window' type to open.")]
    protected string managerWindowTypeName;
    [SerializeField]
    [Tooltip("The title for the manager window.")]
    protected string managerWindowTitle;
    [SerializeField]
    [Tooltip("The name of the manager window tab to open.")]
    protected string managerWindowTabName;

    public override void InvokeAction()
    {
      if (string.IsNullOrEmpty(managerWindowTypeName) ||
          string.IsNullOrEmpty(managerWindowTitle) ||
          string.IsNullOrEmpty(managerWindowTabName))
      {
        return;
      }
      var type = Type.GetType(managerWindowTypeName);
      if (type == null) { return; }
      ManagerWindow.ShowWindow(managerWindowTitle, managerWindowTabName, type);
    }
  }
}