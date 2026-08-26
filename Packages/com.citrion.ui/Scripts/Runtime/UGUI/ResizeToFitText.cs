using CitrioN.Common;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CitrioN.UI
{
  // TODO Check for remaining overrides
  // TODO Test overrides for them being required?
  [AddTooltips]
  [HeaderInfo("Allows the resizing of a 'RectTransform' component's size to match the size of a text component.")]
  [RequireComponent(typeof(RectTransform))]
  public class ResizeToFitText : UIBehaviour, ILayoutElement
  {
    [SerializeField]
    [Tooltip("The RectTransform component to match the size for.")]
    protected RectTransform rectTransform;
    [SerializeField]
    [Tooltip("The text component to query the required size from.")]
    protected TextMeshProUGUI textComponent;

    public float minWidth 
      => textComponent != null ? ((ILayoutElement)textComponent).minWidth : 0;

    public float preferredWidth 
      => textComponent != null ? ((ILayoutElement)textComponent).preferredWidth : 0;

    public float flexibleWidth 
      => textComponent != null ? ((ILayoutElement)textComponent).flexibleWidth : -1;

    public float minHeight 
      => textComponent != null ? ((ILayoutElement)textComponent).minHeight : 0;

    public float preferredHeight 
      => textComponent != null ? ((ILayoutElement)textComponent).preferredHeight : 0;

    public float flexibleHeight 
      => textComponent != null ? ((ILayoutElement)textComponent).flexibleHeight : -1;

    public int layoutPriority 
      => textComponent != null ? ((ILayoutElement)textComponent).layoutPriority : 1;

#if UNITY_EDITOR
    protected override void Reset()
    {
      base.Reset();
      CacheComponents();
    }
#else
    protected void Reset()
    {
      CacheComponents();
    }
#endif

    protected override void Awake()
    {
      base.Awake();
      CacheComponents();
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
      //textComponent?.RegisterDirtyLayoutCallback(OnDirtyLayout);
      RebuildLayout();
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
      // We use the dirty layout callback because it is an object
      // specific callback and the text changed event requires a comparison check
      // which doesn't scale well with a larger amounts of this component in the scene
      // because each component will have to do the comparison.
      //textComponent.UnregisterDirtyLayoutCallback(OnDirtyLayout);
      RebuildLayout();
    }

    private void CacheComponents()
    {
      if (rectTransform == null)
      {
        rectTransform = GetComponent<RectTransform>();
      }
      if (textComponent == null)
      {
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
      }
    }

    private void OnTextChanged(UnityEngine.Object obj)
    {
      if (obj != null && obj == textComponent)
      {
        RebuildLayout();
      }
    }

    protected void RebuildLayout()
    {
      if (!IsActive()) { return; }
      LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }

    protected override void OnTransformParentChanged()
    {
      base.OnTransformParentChanged();
      RebuildLayout();
    }

    protected override void OnDidApplyAnimationProperties()
    {
      base.OnDidApplyAnimationProperties();
      RebuildLayout();
    }

    protected override void OnBeforeTransformParentChanged()
    {
      base.OnBeforeTransformParentChanged();
      RebuildLayout();
    }

    public void CalculateLayoutInputHorizontal()
    {
      if (textComponent == null) { return; }
      ((ILayoutElement)textComponent).CalculateLayoutInputHorizontal();
    }

    public void CalculateLayoutInputVertical()
    {
      if (textComponent == null) { return; }
      ((ILayoutElement)textComponent).CalculateLayoutInputVertical();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
      base.OnValidate();
      if (!Application.isPlaying)
      {
        RebuildLayout();
      }
    }
#endif
  }
}