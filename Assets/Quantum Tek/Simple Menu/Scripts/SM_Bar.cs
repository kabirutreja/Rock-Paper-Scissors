using UnityEngine;
using UnityEngine.UI;

namespace QuantumTek.SimpleMenu
{
    /// <summary> Determines how the progress bar will fill. </summary>
    [System.Serializable]
    public enum SM_FillType
    {
        Width,
        Height,
        FillAmount
    }

    /// <summary> A progress bar/circle to display XP, health, loading, or something else.  </summary>
    [AddComponentMenu("Quantum Tek/Simple Menu/Bar")]
    [DisallowMultipleComponent]
    public class SM_Bar : MonoBehaviour
    {
        [Header("Object References")]
        [Tooltip("A reference to the actual bar transform.")]
        [SerializeField] protected RectTransform bar;
        [Tooltip("A reference to the fill transform.")]
        [SerializeField] protected RectTransform fill;
        [Tooltip("A reference to the fill image.")]
        [SerializeField] protected Image fillImage;
        [Space]
        [Header("Fill Variables")]
        [Tooltip("How the bar should be filled.")]
        [SerializeField] protected SM_FillType type;

        /// <summary> Sets the percentage filled. </summary>
        /// <param name="percent">The percent represented as a number between 0 and 1.</param>
        public void SetFill(float percent)
        {
            if (!bar || !fill || !fillImage) return;
            percent = Mathf.Clamp(percent, 0, 1);

            if (type == SM_FillType.Width) fill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bar.rect.width * percent);
            else if (type == SM_FillType.Height) fill.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bar.rect.height * percent);
            else if (type == SM_FillType.FillAmount) fillImage.fillAmount = percent;
        }
    }
}