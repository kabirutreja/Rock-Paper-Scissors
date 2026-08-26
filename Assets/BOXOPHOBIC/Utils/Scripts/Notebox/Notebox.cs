using UnityEngine;

namespace Boxophobic.Utility
{
    public class Notebox : MonoBehaviour
    {
#if (UNITY_EDITOR)
        [HideInInspector]
        public int noteSize = 16;
        [HideInInspector]
        public Color noteColor = Color.white;
        [TextArea(3, 5)]
        [HideInInspector]
        public string noteText = "";
#endif
    }
}