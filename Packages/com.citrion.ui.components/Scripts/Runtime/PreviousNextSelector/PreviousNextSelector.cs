using CitrioN.Common;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CitrioN.UI
{
  [AddTooltips]
  [HeaderInfo("A UI element representing a list of options. " +
              "The options can be selected with the previous and next buttons or UI navigation for left and right. " +
              "More convenient than a dropdown for games that focus on controller usage.")]
  public class PreviousNextSelector : Selectable
  {
    [Header("Previous Next Selector")]

    [SerializeField]
    [Tooltip("Should the options be possible to\n" +
             "cycle through continuously?")]
    protected bool allowCycle = true;

    [SerializeField]
    [Tooltip("Should the previous and next buttons\n" +
             "be representing if cycling is possible?")]
    protected bool representNoCycleOnButtons = false;

    [SerializeField]
    [Tooltip("The button to select the previous option")]
    protected Button previousButton;
    [SerializeField]
    [Tooltip("The button to select the next option")]
    protected Button nextButton;

    [SerializeField]
    [Tooltip("The text component used to display the current option")]
    protected TextMeshProUGUI textComponent;

    /// <summary>
    /// Values that can be selected
    /// </summary>
    [SerializeField]
    [Tooltip("A list of values for the selector to have")]
    protected List<string> values = new List<string>();

    //[SerializeField]
    private int currentIndex = 0;

    [Tooltip("Invoked with the new value as the parameter when the value of the selector changes")]
    public UnityEvent<string> onValueChanged = new UnityEvent<string>();

    [Tooltip("Invoked with the previous and new index as the parameters when the selected index changes")]
    public UnityEvent<int, int> onIndexChanged = new UnityEvent<int, int>();

    public UnityEvent onIndexIncreased = new UnityEvent();
    public UnityEvent onIndexDecreased = new UnityEvent();

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
            if (values.Count > 0)
            {
              currentIndex = values.Count + value;
            }
          }
          else { currentIndex = 0; }
        }
        else // value >= 0
        {
          if (AllowCycle)
          {
            if (values.Count > 0)
            {
              currentIndex = value % values.Count;
            }
          }
          else { currentIndex = Mathf.Clamp(value, 0, Mathf.Max(values.Count - 1, 0)); }
        }

        if (currentIndex != previousIndex)
        {
          if (values.Count > 0)
          {
            onValueChanged?.Invoke(values[currentIndex]);
            onIndexChanged?.Invoke(previousIndex, currentIndex);
            bool goToLast = previousIndex == 0 && currentIndex == values.Count - 1;
            bool goToFirst = previousIndex == values.Count - 1 && currentIndex == 0;
            if ((currentIndex < previousIndex && !goToFirst) || goToLast)
            {
              onIndexDecreased?.Invoke();
            }
            else
            {
              onIndexIncreased?.Invoke();
            }
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

    public List<string> Values { get => values; protected set => values = value; }

    protected override void Start()
    {
      base.Start();
      if (Application.isPlaying)
      {
        RegisterButtonCallbacks();
        UpdateVisuals();
      }
    }

    private void RegisterButtonCallbacks()
    {
      if (previousButton != null)
      {
        previousButton.onClick.RemoveListener(OnPreviousButtonClicked);
        previousButton.onClick.AddListener(OnPreviousButtonClicked);
      }
      if (nextButton != null)
      {
        nextButton.onClick.RemoveListener(OnNextButtonClicked);
        nextButton.onClick.AddListener(OnNextButtonClicked);
      }
    }

    private void OnPreviousButtonClicked()
    {
      // Select the previous next selector to ensure
      // keyboard/controller navigation works
      Select();

      SelectPrevious();
    }

    private void SelectPrevious() => CurrentIndex--;

    private void UpdateVisuals()
    {
      if (CurrentIndex < 0 || CurrentIndex >= values.Count)
      {
        if (textComponent != null)
        {
          textComponent.SetText(string.Empty);
        }
        return;
      }

      if (RepresentNoCycleOnButtons && !AllowCycle)
      {
        UpdateButtonVisuals();
      }

      if (textComponent != null)
      {
        textComponent.SetText(values[CurrentIndex]);
      }
    }

    private void UpdateButtonVisuals()
    {
      bool isLast = CurrentIndex == values.Count - 1;
      if (nextButton != null)
      {
        bool enabled = AllowCycle || !isLast;
        var images = nextButton.GetComponentsInChildren<Image>();
        foreach (var image in images)
        {
          image.enabled = enabled;
        }
        var textComponents_TMP = nextButton.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var textComponent in textComponents_TMP)
        {
          textComponent.enabled = enabled;
        }
        var textComponents = nextButton.GetComponentsInChildren<Text>();
        foreach (var textComponent in textComponents)
        {
          textComponent.enabled = enabled;
        }
      }

      bool isFirst = CurrentIndex == 0;
      if (previousButton != null)
      {
        bool enabled = AllowCycle || !isFirst;
        var images = previousButton.GetComponentsInChildren<Image>();
        foreach (var image in images)
        {
          image.enabled = enabled;
        }
        var textComponents_TMP = previousButton.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var textComponent in textComponents_TMP)
        {
          textComponent.enabled = enabled;
        }
        var textComponents = previousButton.GetComponentsInChildren<Text>();
        foreach (var textComponent in textComponents)
        {
          textComponent.enabled = enabled;
        }
      }
    }

    private void OnNextButtonClicked()
    {
      // Select the previous next selector to ensure
      // keyboard/controller navigation works
      Select();

      SelectNext();
    }

    private void SelectNext() => CurrentIndex++;

    public void SetValue(string value)
    {
      var index = values.IndexOf(value);
      CurrentIndex = Mathf.Clamp(index, 0, Mathf.Max(values.Count - 1, 0));
      UpdateVisuals();
      onValueDirty?.Invoke();
    }

    public void SetValueWithoutNotify(string value)
    {
      var index = values.IndexOf(value);
      currentIndex = Mathf.Clamp(index, 0, Mathf.Max(values.Count - 1, 0));
      UpdateVisuals();
      onValueDirty?.Invoke();
    }

    public string GetValue()
    {
      if (CurrentIndex >= 0 && CurrentIndex < values.Count)
      {
        return values[CurrentIndex];
      }
      return null;
    }

    public void ClearOptions()
    {
      values.Clear();
      CurrentIndex = 0;
    }

    public void AddOptions(List<string> list)
    {
      values.AddRange(list);
      UpdateVisuals();
    }

    //public override Selectable FindSelectableOnLeft()
    //{
    //  SelectPrevious();
    //  return null;
    //}

    //public override Selectable FindSelectableOnRight()
    //{
    //  SelectNext();
    //  return null; ;
    //}

    public override void OnMove(AxisEventData eventData)
    {
      if (!IsActive() || !IsInteractable())
      {
        base.OnMove(eventData);
        return;
      }

      switch (eventData.moveDir)
      {
        case MoveDirection.Left:
          SelectPrevious();
          break;
        case MoveDirection.Right:
          SelectNext();
          break;
        default:
          base.OnMove(eventData);
          break;
      }
    }
  }
}