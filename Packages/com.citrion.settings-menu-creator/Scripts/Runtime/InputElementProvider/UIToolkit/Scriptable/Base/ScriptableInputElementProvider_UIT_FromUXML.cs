using CitrioN.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [HeaderInfo("An input element provider that creates the input element based on a list of UXML files. " +
              "Using a list allows combining multiple UXML files to create more complex input elements while " +
              "keeping the separate files more modular and maintainable. " +
              "An example would be to have a UXML file for the input element with label and " +
              "a container dedicated for the actual element to interact with such as a dropdown or slider. " +
              "During the generation process those files will be instantiated and parented appropriately " +
              "in the order they appear in the list of templates. If you don't want them to be parented to " +
              "the root element of the previous element the '.provider-anchor' class can be used to mark any element " +
              "in its hierarchy to be used for parenting.")]
  //[CreateAssetMenu(fileName = "Provider_UIT_FromUXML_",
  //                 menuName = "CitrioN/Settings Menu Creator/Input Element Provider/UI Toolkit/Custom",
  //                 order = 139)]
  public class ScriptableInputElementProvider_UIT_FromUXML<T> : ScriptableInputElementProvider_UIT_WithStyleSheet_ClassList
    where T : VisualElement
  {
    //[SerializeField]
    //[Tooltip("The UXML to create when providing the element")]
    //protected VisualTreeAsset template;

    [SerializeField]
    [Tooltip("The templates to instantiate as the element to provide. " +
             "If more than one template is provided a .provider-anchor class " +
             "is required to specify where to attach the next template to. " +
             "If no .provider-anchor class is found it will be attached directly to " +
             "the previous element which may or may not be desired.")]
    protected List<VisualTreeAsset> templates = new List<VisualTreeAsset>();

    public override VisualElement GetInputElement(string settingIdentifier, SettingsCollection settings)
    {
      return ProviderUtility_UIT.GetInputElementBase<T>(settingIdentifier, settings, templates);
      //if (templates == null || templates.Count < 1) { return null; }

      //VisualElement root = null;
      //VisualElement previousInstanceRoot = null;

      //for (int i = 0; i < templates.Count; i++)
      //{
      //  // Create a new instance of the current template
      //  var templateContainer = templates[i].Instantiate();
      //  VisualElement instance = templateContainer;

      //  if (templateContainer.childCount > 0)
      //  {
      //    instance = templateContainer.ElementAt(0);

      //    // Add the style sheets from the template container
      //    // in case the template container will be removed.
      //    // This will ensure that any style sheets attached to the
      //    // original UXML file will not be lost.
      //    var styleSheetsCount = templateContainer.styleSheets.count;
      //    for (int j = 0; j < styleSheetsCount; j++)
      //    {
      //      instance.AddStyleSheet(templateContainer.styleSheets[j]);
      //    }
      //  }

      //  if (i == 0)
      //  {
      //    root = instance;
      //    root.AddToClassList(settingIdentifier);
      //  }
      //  else
      //  {
      //    VisualElement parent = previousInstanceRoot;
      //    if (previousInstanceRoot != null)
      //    {
      //      var anchor = previousInstanceRoot.Q(className: ProviderUtility_UIT.INPUT_ELEMENT_PROVIDER_ANCHOR_CLASS);
      //      if (anchor != null)
      //      {
      //        parent = anchor;
      //      }
      //    }

      //    parent?.Add(instance);
      //  }

      //  previousInstanceRoot = instance;
      //}

      //return root;

      #region OLD
      // TODO Remove later

      //if (template == null) { return null; }
      //var templateContainer = template.Instantiate();

      //if (templateContainer.childCount > 0)
      //{
      //  var instance = templateContainer.ElementAt(0);
      //  instance.AddToClassList(settingIdentifier);

      //  // Add the style sheets from the template container
      //  // in case the template container will be removed.
      //  // This will ensure that any style sheets attached to the
      //  // original UXML file will not be lost.
      //  var styleSheetsCount = templateContainer.styleSheets.count;
      //  for (int i = 0; i < styleSheetsCount; i++)
      //  {
      //    instance.AddStyleSheet(templateContainer.styleSheets[i]);
      //  }

      //  return instance;
      //}
      //return null; 
      #endregion
    }

    public override Type GetInputFieldParameterType(SettingsCollection settings) => null;

    public override Type GetInputFieldType(SettingsCollection settings) => null;
  }
}
