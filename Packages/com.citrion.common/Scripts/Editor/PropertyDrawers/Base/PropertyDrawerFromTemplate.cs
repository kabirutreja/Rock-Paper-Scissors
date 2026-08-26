using UnityEditor;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  public class PropertyDrawerFromTemplate : PropertyDrawer
  {
    protected virtual string RootClass => $"property-drawer__{GetType().Name}";

    public virtual string UxmlPath
      => $"Packages/com.citrion.common/UI Toolkit/UXML/PropertyDrawers/{GetType().Name}.uxml";

    public virtual string StyleSheetPath
      => $"Packages/com.citrion.common/UI Toolkit/USS/PropertyDrawers/{GetType().Name}";

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
      return UIToolkitEditorExtensions.CreateVisualElementFromTemplate(UxmlPath, StyleSheetPath, RootClass);
    }
  }
}