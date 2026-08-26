using System;

namespace CitrioN.Common
{
  public class CustomAction : CustomActionBase
  {
    protected event Action action;

    public CustomAction() { }

    public CustomAction(Action action)
    {
      this.action = action;
    }

    public Action Action => action;

    public void Invoke()
    {
      action.Invoke();
    }
  }

  public class GenericAction<T> : CustomActionBase
  {
    protected event Action<T> action;

    public GenericAction() { }

    public GenericAction(Action<T> action)
    {
      this.action = action;
    }

    public Action<T> Action => action;

    public void Invoke(T arg1)
    {
      action.Invoke(arg1);
    }
  }

  public class GenericAction<T1, T2> : CustomActionBase
  {
    protected event Action<T1, T2> action;

    public GenericAction() { }

    public GenericAction(Action<T1, T2> action)
    {
      this.action = action;
    }

    public Action<T1, T2> Action => action;

    public void Invoke(T1 arg1, T2 arg2)
    {
      action.Invoke(arg1, arg2);
    }
  }

  public class GenericAction<T1, T2, T3> : CustomActionBase
  {
    protected event Action<T1, T2, T3> action;

    public GenericAction() { }

    public GenericAction(Action<T1, T2, T3> action)
    {
      this.action = action;
    }

    public Action<T1, T2, T3> Action => action;

    public void Invoke(T1 arg1, T2 arg2, T3 arg3)
    {
      action.Invoke(arg1, arg2, arg3);
    }
  }

  public class GenericAction<T1, T2, T3, T4> : CustomActionBase
  {
    protected event Action<T1, T2, T3, T4> action;

    public GenericAction() { }

    public GenericAction(Action<T1, T2, T3, T4> action)
    {
      this.action = action;
    }

    public Action<T1, T2, T3, T4> Action => action;

    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
      action.Invoke(arg1, arg2, arg3, arg4);
    }
  }
}