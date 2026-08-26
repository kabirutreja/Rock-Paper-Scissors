using CitrioN.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator.Editor
{
  [CanEditMultipleObjects]
  [CustomEditor(typeof(MenuWithInputElementsPrefabCreator))]
  public class MenuWithInputElementsPrefabCreatorEditor : UnityEditor.Editor
  {
    public override VisualElement CreateInspectorGUI()
    {
      var root = new VisualElement();

      // Add the default inspector
      InspectorElement.FillDefaultInspector(root, serializedObject, this);

      // Add some spacing
      var spacer = new VisualElement();
      spacer.style.height = 10;
      root.Add(spacer);

      // Create and add a button for creating/updating the prefab
      var createResourcesButton = new Button(CreatePrefab);
      createResourcesButton.tooltip = "Creates a new layout prefab variant or modifies an existing prefab and adds/updates " +
                                      "the input elements for the settings of the referenced SettingsCollection. " +
                                      "Preexisting input elements on the prefab will NOT be removed so if you have any input elements from before " +
                                      "you'll need to remove them if you want them to be regenerated.";
      createResourcesButton.text = "Create/Update Prefab";
      createResourcesButton.style.height = 30;
      root.Add(createResourcesButton);

      ClassAttribute.ApplyClassAttributesToHierarchy(root, serializedObject);

      return root;
    }

    private void CreatePrefab()
    {
      var targetObject = serializedObject.targetObject;
      if (targetObject != null && targetObject is MenuWithInputElementsPrefabCreator creator)
      {
        creator.CreatePrefab();
      }
    }
  }
}