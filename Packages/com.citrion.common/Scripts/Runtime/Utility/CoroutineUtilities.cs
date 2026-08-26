using System;
using System.Collections;
#if UNITY_EDITOR && EDITOR_COROUTINES
using Unity.EditorCoroutines.Editor;
#endif
using UnityEngine;

namespace CitrioN.Common
{
  public static class CoroutineUtilities
  {
    public static CoroutineHolder StartCoroutine(IEnumerator routine, MonoBehaviour owner)
    {
      var holder = new CoroutineHolder();
      holder.Owner = owner;
#if UNITY_EDITOR && EDITOR_COROUTINES
      if (Application.isPlaying)
      {
        holder.Coroutine = owner.StartCoroutine(routine);
      }
      else
      {
        if (owner == null)
        {
          holder.EditorCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(routine);
        }
        else
        {
          holder.EditorCoroutine = EditorCoroutineUtility.StartCoroutine(routine, owner);
        }
      }
#else
      holder.Coroutine = owner.StartCoroutine(routine);
#endif
      return holder;
    }

    public static CustomYieldInstruction GetWaitUntil(Func<bool> predicate)
    {
#if UNITY_EDITOR && EDITOR_COROUTINES
      if (Application.isPlaying)
      {
        return new WaitUntil(predicate);
      }
      else
      {
        return new EditorWaitUntil(predicate);
      }
#else
      return new WaitUntil(predicate);
#endif
    }
  }
}