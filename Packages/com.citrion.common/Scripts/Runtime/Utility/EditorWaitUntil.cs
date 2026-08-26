using System;
using UnityEngine;

namespace CitrioN.Common
{
  public class EditorWaitUntil : CustomYieldInstruction
  {
    private Func<bool> m_Predicate;

    public override bool keepWaiting { get { return !m_Predicate(); } }

    public EditorWaitUntil(Func<bool> predicate) { m_Predicate = predicate; }
  }
}