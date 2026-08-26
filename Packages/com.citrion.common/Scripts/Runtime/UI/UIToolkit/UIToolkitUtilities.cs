using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.Common
{
  public static class UIToolkitUtilities
  {
    public static VisualElement CreateVisualElementWithClasses(this VisualTreeAsset template, VisualElement parent = null,
                                                       params string[] classes)
    {
      if (template == null) { return null; }
      var instance = template.Instantiate();
      if (parent != null)
      {
        parent.Add(instance);
      }
      instance.AddToClassList(classes);
      return instance;
    }

    /// <summary>
    /// Creates a label with style overrides of an inspector header.
    /// Mainly used for creating headers in Unity 2021 which are not
    /// drawn by Unity.
    /// </summary>
    /// <param name="labelText">The text for the label/header</param>
    public static Label CreateHeaderLabel(string labelText)
    {
      var label = new Label(labelText);
      label.AddToClassList("unity-header-drawer__label");
      label.style.unityTextAlign = TextAnchor.LowerLeft;
      label.style.unityFontStyleAndWeight = FontStyle.Bold;
      label.style.marginTop = new StyleLength(new Length(13, LengthUnit.Pixel));
      return label;
    }

    /// <summary>
    /// Inserts an element before another one in it's parent hierarchy.
    /// </summary>
    /// <param name="root">The root of the elements</param>
    /// <param name="nextElementsName">The name of the element to insert the new element before</param>
    /// <param name="elem">The new element to insert</param>
    public static void InsertElementBefore(VisualElement root, string nextElementsName, VisualElement elem)
    {
      var nextElement = root.Q(nextElementsName);
      if (nextElement != null)
      {
        InsertElementBefore(nextElement, elem);
      }
    }

    /// <summary>
    /// Inserts an element before another one in it's parent hierarchy.
    /// </summary>
    /// <param name="nextElement">The element to insert the new element before</param>
    /// <param name="elem">The new element to insert</param>
    /// <param name="deleteIfNotSuccessful">If the element should be deleted if inserting was not possible</param>
    public static void InsertElementBefore(VisualElement nextElement, VisualElement elem, bool deleteIfNotSuccessful = true)
    {
      if (nextElement == null)
      {
        elem.RemoveFromHierarchy();
        return;
      }

      var parent = nextElement.parent;

      if (parent == null)
      {
        elem.RemoveFromHierarchy();
        return;
      }

      var index = parent.IndexOf(nextElement);
      if (index != -1)
      {
        parent.Insert(index, elem);
      }
    }

    public static Button SetupButton(VisualElement root, string label, Action onClick, string elementName, params string[] classNames)
    {
      if (root == null) { return null; }

      Button button = null;

      bool hasName = !string.IsNullOrEmpty(elementName);
      if (!hasName) { elementName = null; }
      bool hasClass = classNames != null && classNames.Length > 0;

      button = root.Q<Button>(elementName, classNames);

      if (button == null) { return null; }

      if (label != null)
      {
        button.SetText(label);
      }

      if (onClick != null)
      {
        button.clickable = new Clickable(() => onClick?.Invoke());
      }

      return button;
    }

    public static Toggle SetupToggle(VisualElement root, string label, EventCallback<ChangeEvent<bool>> onValueChanged, 
                                     string elementName, params string[] classNames)
    {
      if (root == null) { return null; }

      Toggle toggle = null;

      bool hasName = !string.IsNullOrEmpty(elementName);
      if (!hasName) { elementName = null; }
      bool hasClass = classNames != null && classNames.Length > 0;

      toggle = root.Q<Toggle>(elementName, classNames);

      if (toggle == null) { return null; }

      if (label != null)
      {
        toggle.label = label;
      }

      if (onValueChanged != null)
      {
        toggle.RegisterCallback(onValueChanged);
      }

      return toggle;
    }
  }
}