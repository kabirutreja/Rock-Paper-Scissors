using UnityEngine;

namespace CitrioN.Common
{
  [SkipObfuscationRename]
  [AddTooltips]
  [HeaderInfo("Helper script that allows the sending/broadcasting of a message via Unity's 'SendMessage' or 'BroadcastMessage' methods.\n\n" +
              "Sending a message will invoke the method with the specified name if found on this object while broadcasting will include child objects.")]
  public class SendMessage : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("The method name to send/broadcast.")]
    protected string methodName = string.Empty;

    [SerializeField]
    [Tooltip("Whether the message should be broadcast to child objects too. " +
             "If disabled it will only include this object to find the matching method.")]
    protected bool broadcast = false;

    [Button]
    [ContextMenu("Send Message")]
    public void Send()
    {
      Send(methodName, broadcast);
    }

    [Button]
    public void Send(string methodName)
    {
      Send(methodName, broadcast);
    }

    [Button]
    [ContextMenu("Broadcast Message")]
    public void Broadcast()
    {
      Send(methodName, true);
    }

    [Button]
    public void Broadcast(string methodName)
    {
      Send(methodName, true);
    }

    [Button]
    public void Send(string methodName, bool broadcast)
    {
      if (string.IsNullOrEmpty(methodName)) { return; }
      if (broadcast)
      {
        BroadcastMessage(methodName, SendMessageOptions.DontRequireReceiver);
      }
      else
      {
        SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
      }
    }
  }
}