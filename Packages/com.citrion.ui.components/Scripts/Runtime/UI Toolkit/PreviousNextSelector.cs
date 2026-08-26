using CitrioN.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace CitrioN.UI.UIToolkit
{
  [SkipObfuscation]
#if UNITY_2023_2_OR_NEWER
  [UxmlElement]
#endif
  public partial class PreviousNextSelector : /*BaseField<string>*/VisualElement
  {
#if !UNITY_2023_2_OR_NEWER
    public new class UxmlFactory : UxmlFactory<PreviousNextSelector, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
      UxmlStringAttributeDescription labelText =
        new UxmlStringAttributeDescription { name = "label-text", defaultValue = "Previous Next Selector" };
      UxmlBoolAttributeDescription allowCycle =
        new UxmlBoolAttributeDescription { name = "allow-cycle", defaultValue = true };
      UxmlBoolAttributeDescription representNoCycleOnButtons =
        new UxmlBoolAttributeDescription { name = "represent-no-cycle-buttons", defaultValue = true };
      UxmlStringAttributeDescription values =
        new UxmlStringAttributeDescription { name = "values", defaultValue = string.Empty };

      public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
      {
        get { yield break; }
      }

      public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
      {
        base.Init(ve, bag, cc);
        //ConsoleLogger.Log("Initializing PreviousNextSelector");
        var prevNextSelector = ve as PreviousNextSelector;
        prevNextSelector.allowCycle = allowCycle.GetValueFromBag(bag, cc);
        prevNextSelector.representNoCycleOnButtons = representNoCycleOnButtons.GetValueFromBag(bag, cc);
        prevNextSelector.values = values.GetValueFromBag(bag, cc)?.Split(',')?.ToList();
        prevNextSelector.labelText = labelText.GetValueFromBag(bag, cc);
        //prevNextSelector.UpdateVisuals();
      }
    }
#endif

    private const string resourcesSubFolders = "UI Toolkit/USS/";
    private const string styleSheetResourceName = "PreviousNextSelector";

    private const string rootClassName = "prev-next-selector";

    // TO Move
    // Base Field
    private const string baseFieldClassName = "unity-base-field";

    // Label
    private const string unityBaseFieldLabelClassName = "unity-base-field__label";
    private const string prevNextSelectorLabelClassName = "prev-next-selector__label";
    private const string prevNextSelectorLabelName = "prev-next-selector__label";

    // Input
    private const string unityBaseFieldInputClassName = "unity-base-field__input";
    private const string prevNextSelectorInputClassName = "prev-next-selector__input";
    private const string prevNextSelectorInputName = "prev-next-selector__input";

    // Buttons
    private const string prevButtonClassName = "prev-next-selector__prev-button";
    private const string prevButtonName = "prev-next-selector__prev-button";
    private const string prevButtonImageClassName = "prev-next-selector__prev-button__image";
    private const string prevButtonImageName = "prev-next-selector__prev-button__image";

    private const string nextButtonClassName = "prev-next-selector__next-button";
    private const string nextButtonName = "prev-next-selector__next-button";
    private const string nextButtonImageClassName = "prev-next-selector__next-button__image";
    private const string nextButtonImageName = "prev-next-selector__next-button__image";

    private const string prevNextSelectorItemLabelClassName = "prev-next-selector__item-label";
    private const string prevNextSelectorItemLabelName = "prev-next-selector__item-label";

    private StyleSheet styleSheet;

    [SerializeField]
#if UNITY_2023_2_OR_NEWER
    [UxmlAttribute]
#endif
    [Tooltip("Should the options be possible to\n" +
             "cycle through continuously?")]
    public bool allowCycle = true;

    [SerializeField]
#if UNITY_2023_2_OR_NEWER
    [UxmlAttribute]
#endif
    [Tooltip("Should the previous and next buttons\n" +
             "be representing if cycling is possible?")]
    public bool representNoCycleOnButtons = false;

    [SerializeField]
    [Tooltip("The button to select the previous option")]
    protected Button previousButton;
    [SerializeField]
    [Tooltip("The button to select the next option")]
    protected Button nextButton;

    [SerializeField]
    protected VisualElement previousButtonImage;
    [SerializeField]
    protected VisualElement nextButtonImage;

    [SerializeField]
#if UNITY_2023_2_OR_NEWER
    [UxmlAttribute()]
#endif
    public string labelText = "Previous Next Selector";

    [SerializeField]
    public Label label;

    [SerializeField]
    protected Label itemLabel;

    /// <summary>
    /// Values that can be selected
    /// </summary>
    [SerializeField]
#if UNITY_2023_2_OR_NEWER
    [UxmlAttribute]
#endif
    public List<string> values = new List<string>();

    protected string stringValues;

    [SerializeField]
    private int currentIndex = 0;

    // TODO Use the base field callback instead
    public UnityEvent<string> onValueChanged = new UnityEvent<string>();

    [Tooltip("Invoked when the value is either changed or set with no change notification." +
         "This event can be useful to register callbacks that update visuals.")]
    public UnityEvent onValueDirty = new UnityEvent();

    public int CurrentIndex
    {
      get => currentIndex;
      set
      {
        //if (value < 0)
        //{
        //  currentIndex = value;
        //  UpdateTextComponent();
        //  return;
        //}

        var previousIndex = currentIndex;

        if (value < 0)
        {
          if (AllowCycle)
          {
            if (Values.Count > 0)
            {
              currentIndex = Values.Count + value;
            }
          }
          else { currentIndex = 0; }
        }
        else // value >= 0
        {
          if (AllowCycle)
          {
            if (Values.Count > 0)
            {
              currentIndex = value % Values.Count;
            }
          }
          else { currentIndex = Mathf.Clamp(value, 0, (Values.Count - 1).ClampLowerTo0()); }
        }

        if (currentIndex != previousIndex)
        {
          if (Values.Count > 0)
          {
            onValueChanged?.Invoke(Values[currentIndex]);
            // TODO Will the internal event be invoked when the value is set?
            //this.value
            UpdateVisuals();
          }
          //ConsoleLogger.Log("Index: " + CurrentIndex);
        }

        onValueDirty?.Invoke();
      }
    }

    public bool AllowCycle { get => allowCycle; set => allowCycle = value; }

    public bool RepresentNoCycleOnButtons
    {
      get => representNoCycleOnButtons;
      set => representNoCycleOnButtons = value;
    }
    //public Label Label { get => label; protected set => label = value; }

    public List<string> Values { get => values; protected set => values = value; }

    //public PreviousNextSelector(string label) : base(label,)
    //{

    //}

    public PreviousNextSelector()/* : this(null)*/
    {
      focusable = true;
      SetupHierarchy();
      UpdateVisuals();
      RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
      RegisterCallback<NavigationMoveEvent>(OnNavigationMove);
      ScheduleUtility.InvokeDelayedByFrames(UpdateVisuals);
    }

    private void OnNavigationMove(NavigationMoveEvent evt)
    {
      switch (evt.direction)
      {
        case NavigationMoveEvent.Direction.None:
          break;
        case NavigationMoveEvent.Direction.Left:
          //ConsoleLogger.Log("PNS: left");
          SelectPrevious();
          break;
        case NavigationMoveEvent.Direction.Up:
          break;
        case NavigationMoveEvent.Direction.Right:
          //ConsoleLogger.Log("PNS: right");
          SelectNext();
          break;
        case NavigationMoveEvent.Direction.Down:
          break;
#if UNITY_2022_2_OR_NEWER
        case NavigationMoveEvent.Direction.Next:
          ConsoleLogger.Log("PNS: next", Common.LogType.Debug);
          break;
        case NavigationMoveEvent.Direction.Previous:
          ConsoleLogger.Log("PNS: prev", Common.LogType.Debug);
          break;
#endif
      }
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
      //ConsoleLogger.Log("Geometry was changed");

      UpdateAspectRatio(previousButton);
      UpdateAspectRatio(nextButton);
    }

    // TODO Move this to a common script?
    private void UpdateAspectRatio(VisualElement elem, bool matchWidthToHeight = true, float aspect = 1.0f)
    {
      if (elem == null) { return; }

      var value = (matchWidthToHeight ? elem.layout.height : elem.layout.width) * aspect;

      if (matchWidthToHeight)
      {
        elem.style.minWidth = value;
        elem.style.maxWidth = value;
      }
      else
      {
        elem.style.minHeight = value;
        elem.style.maxHeight = value;
      }
    }

    protected virtual void SetupHierarchy()
    {
      if (styleSheet == null)
      {
        styleSheet = Resources.Load<StyleSheet>(resourcesSubFolders + styleSheetResourceName);
        this.AddStyleSheet(styleSheet);
      }
      //ConsoleLogger.Log("Creating PreviousNextSelector");
      AddToClassList(rootClassName);
      AddToClassList(baseFieldClassName);

      #region Label
      if (label == null)
      {
        label = new Label();
        label.text = labelText;
        Add(label);

        label.AddToClassList(unityBaseFieldLabelClassName);
        label.AddToClassList(prevNextSelectorLabelClassName);
        label.name = prevNextSelectorLabelName;
      }
      #endregion

      #region Input
      VisualElement inputContainer = new VisualElement();
      Add(inputContainer);
      inputContainer.AddToClassList(unityBaseFieldInputClassName);
      inputContainer.AddToClassList(prevNextSelectorInputClassName);
      inputContainer.name = prevNextSelectorInputName;

      if (previousButton == null)
      {
        previousButton = new Button(OnPreviousButtonClicked);
        inputContainer.Add(previousButton);
        previousButton.AddToClassList(prevButtonClassName);
        previousButton.name = prevButtonName;

        if (previousButtonImage == null)
        {
          previousButtonImage = new VisualElement();
          previousButton.Add(previousButtonImage);
          previousButtonImage.AddToClassList(prevButtonImageClassName);
          previousButtonImage.name = prevButtonImageName;
        }
      }

      if (itemLabel == null)
      {
        itemLabel = new Label();
        inputContainer.Add(itemLabel);
        itemLabel.text = "Selected Item";
        itemLabel.AddToClassList(prevNextSelectorItemLabelClassName);
        itemLabel.name = prevNextSelectorItemLabelName;
      }

      if (nextButton == null)
      {
        nextButton = new Button(OnNextButtonClicked);
        inputContainer.Add(nextButton);
        nextButton.AddToClassList(nextButtonClassName);
        nextButton.name = nextButtonName;

        if (nextButtonImage == null)
        {
          nextButtonImage = new VisualElement();
          nextButton.Add(nextButtonImage);
          nextButtonImage.AddToClassList(nextButtonImageClassName);
          nextButtonImage.name = nextButtonImageName;
        }
      }
      #endregion
    }

    private void OnPreviousButtonClicked() => SelectPrevious();

    private void SelectPrevious() => CurrentIndex--;

    private void UpdateVisuals()
    {
      if (CurrentIndex < 0 || CurrentIndex >= Values.Count)
      {
        itemLabel?.SetText(string.Empty);
        return;
      }

      if (RepresentNoCycleOnButtons && !AllowCycle)
      {
        UpdateButtonVisuals();
      }

      itemLabel?.SetText(Values[CurrentIndex]);

      label?.SetText(labelText);
    }

    private void UpdateButtonVisuals()
    {
      bool isFirst = CurrentIndex == 0;
      if (previousButton != null)
      {
        bool enabled = AllowCycle || !isFirst;
        previousButton.visible = enabled;
      }

      bool isLast = CurrentIndex == Values.Count - 1;
      if (nextButton != null)
      {
        bool enabled = AllowCycle || !isLast;
        nextButton.visible = enabled;
      }
    }

    private void OnNextButtonClicked() => SelectNext();

    private void SelectNext() => CurrentIndex++;

    public void SetValue(string value)
    {
      var index = Values.IndexOf(value);
      CurrentIndex = Mathf.Clamp(index, 0, (Values.Count - 1).ClampLowerTo0());
      UpdateVisuals();
      onValueDirty?.Invoke();
    }

    public void SetValueWithoutNotify(string value)
    {
      var index = Values.IndexOf(value);
      currentIndex = Mathf.Clamp(index, 0, (Values.Count - 1).ClampLowerTo0());
      UpdateVisuals();
      onValueDirty?.Invoke();
    }

    public string GetValue()
    {
      if (CurrentIndex >= 0 && CurrentIndex < Values.Count)
      {
        return Values[CurrentIndex];
      }
      return null;
    }

    public void ClearOptions()
    {
      Values.Clear();
      CurrentIndex = 0;
    }

    public void AddOptions(List<string> list)
    {
      Values.AddRange(list);
      UpdateVisuals();
    }
  }
}