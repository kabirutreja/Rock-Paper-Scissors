using System;

namespace CitrioN.Common
{
  public class SerializedAction
  {
    private Action action;

    public SerializedAction(Action action)
    {
      this.action = action;
    }

    public void Invoke()
    {
      action?.Invoke();
    }
  }
}