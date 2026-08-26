#if UNITY_EDITOR && EDITOR_COROUTINES
using Unity.EditorCoroutines.Editor;
#endif
using UnityEngine;

namespace CitrioN.Common
{
  public class CoroutineHolder
  {
    [SerializeField]
    private MonoBehaviour owner = null;

    public MonoBehaviour Owner { get { return owner; } set { owner = value; } }
    public Coroutine Coroutine { get; set; } = null;

#if UNITY_EDITOR && EDITOR_COROUTINES
    public EditorCoroutine EditorCoroutine { get; set; } = null;
#endif

    public CoroutineHolder() { }

    public void StopCoroutine()
    {
      if (Coroutine != null && Owner != null)
      {
        Owner.StopCoroutine(Coroutine);
      }
#if UNITY_EDITOR && EDITOR_COROUTINES
      if (EditorCoroutine != null)
      {
        EditorCoroutineUtility.StopCoroutine(EditorCoroutine);
      }
#endif
    }
  }
}