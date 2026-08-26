using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.Common
{
  public static class UIToolkitExtensions
  {
    #region Label
    public static void SetText(this Label label, string text)
    {
      if (label != null)
      {
        label.text = text.TODOLocalize();
      }
    }
    #endregion

    #region TextInputBaseField
    public static void SetLabelText<TValueType>(this TextInputBaseField<TValueType> field, string text)
    {
      field?.labelElement?.SetText(text.TODOLocalize());
    }

    public static void SetInputFieldText(this TextInputBaseField<string> field, string text)
    {
      if (field == null) { return; }
      var textField = field.GetType().GetProperty("text");
      textField?.SetValue(field, text.TODOLocalize());
    }
    #endregion

    #region Button
    public static void SetText(this Button button, string text)
    {
      if (button != null)
      {
        button.text = text.TODOLocalize();
      }
    }
    #endregion

    #region Visual Element
    public static void Show(this VisualElement element, bool show)
    {
      if (element == null) { return; }
      element.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public static bool IsShownSelf(this VisualElement element)
    {
      if (element == null) { return false; }
      return element.style.display == DisplayStyle.Flex;
    }

    public static void ToggleVisibility(this VisualElement elem)
    {
      if (elem == null) { return; }
      var currentVisibility = elem.style.display;
      elem.style.display = currentVisibility == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public static VisualElement GetFirstAncestorWithClass(this VisualElement element, string className)
    {
      if (element == null) { return null; }

      if (element.ClassListContains(className)) { return element; }

      return element.parent?.GetFirstAncestorWithClass(className);
    }

    public static void AddToClassList(this VisualElement element, string className, bool add)
    {
      if (element == null) { return; }
      if (add)
      {
        element.AddToClassList(className);
      }
      else
      {
        element.RemoveFromClassList(className);
      }
    }

    //public static void SetImage(this VisualElement element, Sprite sprite)
    //{
    //  if (element == null) { return; }
    //  element.style.backgroundImage = new StyleBackground(sprite);
    //}

    public static void SetImage(this VisualElement element, Texture2D texture2D)
    {
      if (element == null) { return; }
      element.style.backgroundImage = new StyleBackground(texture2D);
    }

    public static void SetBackgroundColor(this VisualElement element, Color color)
    {
      if (element == null) { return; }
      element.style.backgroundColor = new StyleColor(color);
    }

    public static void SetTextColor(this VisualElement element, Color color)
    {
      if (element == null) { return; }
      element.style.color = new StyleColor(color);
    }

    public static void SetHeight(this VisualElement element, float height)
    {
      if (element == null) { return; }
      element.style.height = height;
    }

    public static void SetMinHeight(this VisualElement element, float height)
    {
      if (element == null) { return; }
      element.style.minHeight = height;
    }

    public static void SetMaxHeight(this VisualElement element, float height)
    {
      if (element == null) { return; }
      element.style.maxHeight = height;
    }

    public static void SetWidth(this VisualElement element, float width)
    {
      if (element == null) { return; }
      element.style.width = width;
    }

    public static void SetMinWidth(this VisualElement element, float width)
    {
      if (element == null) { return; }
      element.style.minWidth = width;
    }

    public static void SetMaxWidth(this VisualElement element, float width)
    {
      if (element == null) { return; }
      element.style.maxWidth = width;
    }

    public static void SetBorderWidthAll(this VisualElement element, float width)
    {
      if (element == null) { return; }
      element.style.borderTopWidth = width;
      element.style.borderRightWidth = width;
      element.style.borderBottomWidth = width;
      element.style.borderLeftWidth = width;
    }

    public static void SetBorderRadiusAll(this VisualElement element, float radius)
    {
      if (element == null) { return; }
      element.style.borderTopRightRadius = radius;
      element.style.borderBottomRightRadius = radius;
      element.style.borderBottomLeftRadius = radius;
      element.style.borderTopLeftRadius = radius;
    }

    public static void SetBorderColorAll(this VisualElement element, Color color)
    {
      if (element == null) { return; }
      element.style.borderTopColor = color;
      element.style.borderRightColor = color;
      element.style.borderBottomColor = color;
      element.style.borderLeftColor = color;
    }

    public static void SetPaddingAll(this VisualElement element, float padding)
    {
      if (element == null) { return; }
      element.style.paddingTop = padding;
      element.style.paddingRight = padding;
      element.style.paddingBottom = padding;
      element.style.paddingLeft = padding;
    }

    public static void SetMarginAll(this VisualElement element, float margin)
    {
      if (element == null) { return; }
      element.style.marginTop = margin;
      element.style.marginRight = margin;
      element.style.marginBottom = margin;
      element.style.marginLeft = margin;
    }

    public static void SetPadding(this VisualElement element, float padding, 
                                  bool top, bool right, bool bottom, bool left)
    {
      if (element == null) { return; }
      if (top) { element.style.paddingTop = padding; }
      if (right) { element.style.paddingRight = padding; }
      if (bottom) { element.style.paddingBottom = padding; }
      if (left) { element.style.paddingLeft = padding; }
    }

    public static void SetMargin(this VisualElement element, float margin,
                                 bool top, bool right, bool bottom, bool left)
    {
      if (element == null) { return; }
      if (top) { element.style.marginTop = margin; }
      if (right) { element.style.marginRight = margin; }
      if (bottom) { element.style.marginBottom = margin; }
      if (left) { element.style.marginLeft = margin; }
    }

    public static void SetFlexShrink(this VisualElement element, float shrink)
    {
      if (element == null) { return; }
      element.style.flexShrink = shrink;
    }

    public static void SetFlexGrow(this VisualElement element, float grow)
    {
      if (element == null) { return; }
      element.style.flexGrow = grow;
    }

    public static void SetFlexDirection(this VisualElement element, FlexDirection direction)
    {
      if (element == null) { return; }
      element.style.flexDirection = direction;
    }

    public static void SetFlexWrap(this VisualElement element, Wrap wrap)
    {
      if (element == null) { return; }
      element.style.flexWrap = wrap;
    }

    public static void SetAlignItems(this VisualElement element, Align align)
    {
      if (element == null) { return; }
      element.style.alignItems = align;
    }

    public static void SetJustifyContent(this VisualElement element, Justify justify)
    {
      if (element == null) { return; }
      element.style.justifyContent = justify;
    }

    public static void SetTextAlign(this VisualElement element, TextAnchor align)
    {
      if (element == null) { return; }
      element.style.unityTextAlign = align;
    }

    public static void SetTextWrapping(this VisualElement element, bool wrapText)
    {
      if (element == null) { return; }
      element.style.whiteSpace = wrapText ? WhiteSpace.Normal : WhiteSpace.NoWrap;
    }

    public static void StretchToParent(this VisualElement element)
    {
      element.StretchToParentSize();
    }

    public static void UpdateAspectRatio(this VisualElement elem, bool matchWidthToHeight = true, float aspect = 1.0f)
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

    public static void AddToClassList(this VisualElement element, params string[] classes)
    {
      if (element == null || classes == null) { return; }
      foreach (var c in classes)
      {
        if (string.IsNullOrEmpty(c)) { continue; }
        element.AddToClassList(c);
      }
    }

    public static void AddStyleSheet(this VisualElement element, StyleSheet styleSheet)
    {
      if (element == null || styleSheet == null) { return; }
      // Not requires as Unity does a check internally too
      //if (element.styleSheets.Contains(styleSheet)) { return; }
      element.styleSheets.Add(styleSheet);
    }

    public static void AddStyleSheets(this VisualElement element, params StyleSheet[] styleSheets)
    {
      if (element == null || styleSheets == null) { return; }
      foreach (var s in styleSheets)
      {
        element.AddStyleSheet(s);
        //if (s == null) { continue; }
        //if (element.styleSheets.Contains(s)) { continue; }
        //element.styleSheets.Add(s);
      }
    }

    public static void AddStyleSheets(this VisualElement element, List<StyleSheet> styleSheets)
    {
      if (element == null || styleSheets == null) { return; }
      foreach (var s in styleSheets)
      {
        element.AddStyleSheet(s);
        //if (s == null) { continue; }
        //if (element.styleSheets.Contains(s)) { continue; }
        //element.styleSheets.Add(s);
      }
    }

    public static void RemoveStyleSheet(this VisualElement element, StyleSheet styleSheet)
    {
      if (element == null || styleSheet == null) { return; }
      element.styleSheets.Remove(styleSheet);
    }

    public static void RemoveStyleSheets(this VisualElement element, params StyleSheet[] styleSheets)
    {
      if (element == null || styleSheets == null) { return; }
      foreach (var s in styleSheets)
      {
        element.RemoveStyleSheet(s);
        //if (s == null) { continue; }
        //if (!element.styleSheets.Contains(s)) { continue; }
        //element.styleSheets.Remove(s);
      }
    }

    public static void RemoveStyleSheets(this VisualElement element, List<StyleSheet> styleSheets)
    {
      if (element == null || styleSheets == null) { return; }
      styleSheets.ForEach(s => element.RemoveStyleSheet(s));
    }

    public static void RemoveAllChildren(this VisualElement element)
    {
      if (element == null) { return; }
      var children = element.Children().ToList();
      for (int i = 0; i < children.Count; i++)
      {
        children[i].RemoveFromHierarchy();
      }
    }
    #endregion

    #region Foldout
    public static void SetText(this Foldout foldout, string text)
    {
      if (foldout != null)
      {
        foldout.text = text.TODOLocalize();
      }
    }
    #endregion

    #region ListView

    /// <summary>
    /// Overrides/Sets the action that is invoked when the add or remove button from
    /// a <see cref="ListView"/> is clicked. <br/>
    /// For example if the default add action of adding an
    /// instance of the element type should be customized.
    /// </summary>
    /// <param name="listView">The list view to modify the button action for</param>
    /// <param name="modifyAddButton">Should the add or remove button be modified? <br/>
    /// True: add / False: remove</param>
    /// <param name="onClickAction">The action to be invoked when 
    /// the add or remove button is clicked</param>
    public static void SetAddRemoveButtonAction(this ListView listView, bool modifyAddButton, Action onClickAction = null)
    {
      if (listView == null) { return; }
      string buttonClassName = modifyAddButton ?
                               UIToolkitClassNames.LISTVIEW_ADD_BUTTON :
                               UIToolkitClassNames.LISTVIEW_REMOVE_BUTTON;
      var button = listView.Q<Button>(buttonClassName);
      if (button == null) { return; }
      button.clickable = new Clickable(() =>
      {
        onClickAction?.Invoke();
      });
    }

    #endregion

    public static void SetVisualElementTooltipFromTooltipAttribute
      (this VisualElement elem, Type type, string fieldName, bool overrideExisting = false)
    {
      if (elem == null || (!overrideExisting && !string.IsNullOrEmpty(elem.tooltip))) { return; }
      var tooltipAttribute = type?.GetSerializableField(fieldName)?.GetCustomAttribute<TooltipAttribute>();
      if (tooltipAttribute != null)
      {
        elem.tooltip = tooltipAttribute.tooltip;
      }
    }

    public static Label SetupLabel(VisualElement root, string className, string text)
    {
      var typeLabel = root.Q<Label>(className: className);

      if (typeLabel == null)
      {
        typeLabel = new Label();
        typeLabel.AddToClassList(className);
        root.Add(typeLabel);
      }

      typeLabel.SetText(text);
      return typeLabel;
    }

    public static T SetupVisualElement<T>(VisualElement root, string className) where T : VisualElement
    {
      var elem = root.Q<T>(className: className);

      if (elem == null)
      {
        elem = Activator.CreateInstance<T>();
        elem.AddToClassList(className);
        root.Add(elem);
      }

      return elem;
    }

    //public static bool GetPropertyValue<T>(SerializedProperty property, out T value)
    //{
    //  object objValue = EditorUtilities.GetPropertyValue(property);

    //  // TODO Check if the null check is required
    //  if (/*objValue != null && */objValue is T)
    //  {
    //    value = (T)objValue;
    //    return true;
    //  }

    //  value = default(T);
    //  return false;
    //}

    private static string TODOLocalize(this string text)
    {
      if (string.IsNullOrEmpty(text)) { return string.Empty; }
      // TODO Move this in a different class and connect this to the loca system generically
      return text;
    }

    //listView.itemIndexChanged += (oldIndex, newIndex) =>_listView.Rebuild();
  }
}