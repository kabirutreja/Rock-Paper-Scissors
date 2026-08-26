using UnityEngine;
using UnityEngine.Events;

namespace CitrioN.Common
{
  /// <summary>
  /// (GameObject) UnityEvent subclass so the event will appear in the inspector.
  /// </summary>
  [System.Serializable] public class UnityGameObjectEvent : UnityEvent<GameObject> { }

  /// <summary>
  /// (Collider) UnityEvent subclass so the event will appear in the inspector.
  /// </summary>
  [System.Serializable] public class UnityColliderEvent : UnityEvent<Collider> { }

  /// <summary>
  /// (string) UnityEvent subclass so the event will appear in the inspector.
  /// </summary>
  [System.Serializable] public class UnityStringEvent : UnityEvent<string> { }

  /// <summary>
  /// (string, Color) UnityEvent subclass so the event will appear in the inspector.
  /// </summary>
  [System.Serializable] public class UnityStringColorEvent : UnityEvent<string, Color> { }

  /// <summary>
  /// (float) UnityEvent subclass so the event will appear in the inspector.
  /// </summary>
  [System.Serializable] public class UnityFloatEvent : UnityEvent<float> { }

}