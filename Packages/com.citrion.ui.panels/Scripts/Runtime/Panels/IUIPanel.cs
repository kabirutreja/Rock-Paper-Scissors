namespace CitrioN.UI
{
  public interface IUIPanel
  {
    string PanelName { get; set; }

    bool PreventOpening { get; set; }

    bool CanOpen { get; }

    void Open(params object[] input);

    void OpenNoParams();

    void Close(params object[] input);

    void CloseNoParams();

    void Toggle();
  }
}