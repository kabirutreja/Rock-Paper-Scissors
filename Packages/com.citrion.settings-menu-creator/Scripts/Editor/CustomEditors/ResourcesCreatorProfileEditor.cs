using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator.Editor
{
  [CustomEditor(typeof(ResourcesCreatorProfile))]
  public class ResourcesCreatorProfileEditor : UnityEditor.Editor
  {
    public override VisualElement CreateInspectorGUI()
    {
      var root = new VisualElement();

      // Add the default inspector
      // InspectorElement.FillDefaultInspector(root, serializedObject, this);

#if !UNITY_2022_1_OR_NEWER
      // We need to manually insert the headers here
      // because Unity does not draw them in the default inspector in Unity 2021
      // Common.UIToolkitUtilities.InsertElementBefore(root, "PropertyField:settingsMenu_UGUI", Common.UIToolkitUtilities.CreateHeaderLabel("UGUI"));
      // Common.UIToolkitUtilities.InsertElementBefore(root, "PropertyField:settingsMenu_UIT", Common.UIToolkitUtilities.CreateHeaderLabel("UI Toolkit"));
#endif

      // Add some spacing
      var spacer = new VisualElement();
      spacer.style.height = 10;
      root.Add(spacer);

      //// Create and add a button for creating the resources
      //var createResourcesButton = new Button(CreateResources);
      //createResourcesButton.text = "Create Resources";
      //createResourcesButton.style.height = 30;
      //root.Add(createResourcesButton);

      // Create and add a button for creating the resources
      var editButton = new Button(OpenProfileEditor);
      editButton.text = "Open In Editor";
      editButton.style.height = 30;
      root.Add(editButton);

      return root;
    }

    private void OpenProfileEditor()
    {
      var managerWindow = ManagerWindow_SettingsMenuCreator.ShowManagerTab_ResourcesGenerator();
      if (managerWindow == null) { return; }

      var asset = managerWindow.tabContents.assets.Find(i => i.displayName == "Resources Generator");
      if (asset == null) { return; }
      var controller = asset.controller;
      if (controller == null) { return; }
      if (target != null && target is ResourcesCreatorProfile profile)
      {
        if (controller is ResourceGenerationController resourceGenerationController)
        {
          resourceGenerationController.Profile = profile;
        }
      }
    }

    //private void CreateResources()
    //{
    //  var targetObject = serializedObject.targetObject;
    //  if (targetObject != null && targetObject is ResourcesCreatorProfile creator)
    //  {
    //    ResourcesCreatorProfile.CreateResourcesFromProfile(creator);
    //  }
    //}
  }
}