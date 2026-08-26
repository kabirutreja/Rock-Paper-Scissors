using UnityEngine;

namespace CitrioN.Common
{
  [AddTooltips]
  [HeaderInfo("Helper script that allows the disabling of logs from the ConsoleLogger. " +
              "Warnings and errors will still be logged.")]
  public class ConsoleLoggerDisabler : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("Whether regular logs of the 'ConsoleLogger' should be disabled.")]
    private bool disableRegularLogs = true;

    private void OnEnable()
    {
      ConsoleLogger.DisableRegularLogs = disableRegularLogs;
    }
  } 
}