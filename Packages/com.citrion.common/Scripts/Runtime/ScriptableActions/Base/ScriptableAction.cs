using UnityEngine;

namespace CitrioN.Common
{
  [AddTooltips]
  [HeaderInfo("ScriptableObject based functionality. Can for example be invoked via UnityEvents.")]
  public abstract class ScriptableAction : ScriptableObject
	{
    public abstract void InvokeAction();
	}

  [AddTooltips]
  [HeaderInfo("ScriptableObject based functionality. Uses one parameter. Can for example be invoked via UnityEvents.")]
  public abstract class ScriptableAction<T> : ScriptableObject
  {
    public abstract void InvokeAction(T arg1);
  }

  [AddTooltips]
  [HeaderInfo("ScriptableObject based functionality. Uses two parameters. Can for example be invoked via UnityEvents.")]
  public abstract class ScriptableAction<T1, T2> : ScriptableObject
  {
    public abstract void InvokeAction(T1 arg1, T2 arg2);
  }
}
