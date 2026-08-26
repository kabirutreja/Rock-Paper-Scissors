using CitrioN.Common;
using System.ComponentModel;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [MenuOrder(1)]
  [MenuPath("From Name")]
  [DisplayName("Custom (From Name)")]
  public class InputElementProvider_UIT_FromName_Custom : InputElementProvider_UIT_FromName
  {
    [SerializeField]
    protected string providerName = string.Empty;

    protected override string ProviderName => providerName;

    public override string Name => "From Name";
  }

  [System.Serializable]
  [MenuOrder(11)]
  [MenuPath("From Name")]
  [DisplayName("Slider Float (From Name)")]
  public class InputElementProvider_UIT_FromName_SliderFloat : InputElementProvider_UIT_FromName
  {
    protected override string ProviderName => "Slider Float";
  }

  [System.Serializable]
  [MenuOrder(10)]
  [MenuPath("From Name")]
  [DisplayName("Slider Integer (From Name)")]
  public class InputElementProvider_UIT_FromName_SliderInteger : InputElementProvider_UIT_FromName
  {
    protected override string ProviderName => "Slider Integer";
  }

  [System.Serializable]
  [MenuOrder(99)]
  [MenuPath("From Name")]
  [DisplayName("Dropdown (From Name)")]
  public class InputElementProvider_UIT_FromName_Dropdown : InputElementProvider_UIT_FromName
  {
    protected override string ProviderName => "Dropdown";
  }

  [System.Serializable]
  [MenuOrder(90)]
  [MenuPath("From Name")]
  [DisplayName("Previous/Next Selector (From Name)")]
  public class InputElementProvider_UIT_FromName_PreviousNextSelector : InputElementProvider_UIT_FromName
  {
    protected override string ProviderName => "Previous Next Selector";
  }

  [System.Serializable]
  [MenuOrder(89)]
  [MenuPath("From Name")]
  [DisplayName("Previous/Next Selector No Cycle (From Name)")]
  public class InputElementProvider_UIT_FromName_PreviousNextSelector_NoCycle : InputElementProvider_UIT_FromName
  {
    protected override string ProviderName => "Previous Next Selector No Cycle";
  }

  [System.Serializable]
  [MenuOrder(9)]
  [MenuPath("From Name")]
  [DisplayName("Toggle (From Name)")]
  public class InputElementProvider_UIT_FromName_Toggle : InputElementProvider_UIT_FromName
  {
    protected override string ProviderName => "Toggle";
  }

  [System.Serializable]
  [MenuOrder(8)]
  [MenuPath("From Name")]
  [DisplayName("Button (From Name)")]
  public class InputElementProvider_UIT_FromName_Button : InputElementProvider_UIT_FromName
  {
    protected override string ProviderName => "Button";
  }

  [System.Serializable]
  [MenuOrder(7)]
  [MenuPath("From Name")]
  [DisplayName("Integer Field (From Name)")]
  public class InputElementProvider_UIT_FromName_IntegerField : InputElementProvider_UIT_FromName
  {
    protected override string ProviderName => "Integer Field";
  }

  [System.Serializable]
  [MenuOrder(6)]
  [MenuPath("From Name")]
  [DisplayName("Float Field (From Name)")]
  public class InputElementProvider_UIT_FromName_FloatField : InputElementProvider_UIT_FromName
  {
    protected override string ProviderName => "Float Field";
  }

  [System.Serializable]
  [MenuOrder(2)]
  [MenuPath("From Name")]
  [DisplayName("Headline (From Name)")]
  public class InputElementProvider_UIT_FromName_Headline : InputElementProvider_UIT_FromName
  {
    protected override string ProviderName => "Headline";
  }

  [System.Serializable]
  [MenuOrder(1)]
  [MenuPath("From Name")]
  [DisplayName("Title (From Name)")]
  public class InputElementProvider_UIT_FromName_Title : InputElementProvider_UIT_FromName
  {
    protected override string ProviderName => "Title";
  }
}
