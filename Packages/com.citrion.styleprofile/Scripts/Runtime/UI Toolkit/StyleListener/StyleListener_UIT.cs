using CitrioN.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.StyleProfileSystem.UIToolkit
{
  [HeaderInfo("\n\nApplies the style changes to all matching UI Toolkit VisualElement's that match the " +
              "specified query string. The query format is the same as the format used by UI Toolkit for style sheets.\n" +
              "Example: .unity-base-slider__text-field")]
  [AddComponentMenu("CitrioN/Style Profile/Style Listener/Style Listener (UI Toolkit)")]
  public class StyleListener_UIT : StyleListener
  {
    [SerializeField]
    [Tooltip("The UI Document to apply style changes to.")]
    protected UIDocument document;

    [SerializeField]
    [Tooltip("The style type to change.")]
    protected StyleType styleType = StyleType.None;

    [SerializeField]
    [TextArea(1, 99)]
    [Tooltip("A query string to find the matching visual elements.")]
    protected string queryString = string.Empty;

    protected static char[] separators = new char[] { '.', '#', ':' };

    protected override void OnEnable()
    {
      if (Application.isPlaying)
      {
        RegisterEvents();
        // We update the style next frame to allow the
        // UI Document to be populated with content
        CoroutineRunner.Instance.InvokeDelayedByFrames(UpdateStyleProfileIfActiveAndEnabled);
      }
    }

    protected void UpdateStyleProfileIfActiveAndEnabled()
    {
      if (isActiveAndEnabled)
      {
        UpdateStyleFromProfile();
      }
    }

    public override void ApplyChange(object value)
    {
      if (document == null) { return; }
      VisualElement root = document.rootVisualElement;
      if (root == null) { return; }

      ApplyChangeToMatchingElements(value);
    }

    protected virtual void ApplyChangeToMatchingElements(object value)
    {
      var elements = GetMatchingElements(queryString);

      // TODO Transform?
      //elements.ForEach(e => e.style.transformOrigin = new StyleTransformOrigin(new TransformOrigin()));
      //elements.ForEach(e => e.style.translate = new StyleTranslate(new Translate()));
      //elements.ForEach(e => e.style.scale = new StyleScale());
      //elements.ForEach(e => e.style.rotate = new StyleRotate(new Rotate()));

      switch (styleType)
      {
        case StyleType.None:
          break;
        case StyleType.Display_Opacity:
          if (value is float f1) { elements.ForEach(e => e.style.opacity = f1); }
          break;
        case StyleType.Display_Display:
          if (value is DisplayStyle d1) { elements.ForEach(e => e.style.display = d1); }
          break;
        case StyleType.Display_Visibility:
          if (value is Visibility v1) { elements.ForEach(e => e.style.visibility = v1); }
          break;
        case StyleType.Display_Overflow:
          if (value is Overflow o1) { elements.ForEach(e => e.style.overflow = o1); }
          break;
        case StyleType.Position_Position:
          if (value is Position p1) { elements.ForEach(e => e.style.position = p1); }
          break;
        case StyleType.Position_Left:
          if (value is StyleLength s1) { elements.ForEach(e => e.style.left = s1); }
          break;
        case StyleType.Position_Top:
          if (value is StyleLength s2) { elements.ForEach(e => e.style.top = s2); }
          break;
        case StyleType.Position_Right:
          if (value is StyleLength s3) { elements.ForEach(e => e.style.right = s3); }
          break;
        case StyleType.Position_Bottom:
          if (value is StyleLength s4) { elements.ForEach(e => e.style.bottom = s4); }
          break;
        case StyleType.Flex_Basis:
          if (value is StyleLength s5) { elements.ForEach(e => e.style.flexBasis = s5); }
          break;
        case StyleType.Flex_Shrink:
          if (value is StyleFloat f2) { elements.ForEach(e => e.style.flexShrink = f2); }
          break;
        case StyleType.Flex_Grow:
          if (value is StyleFloat f3) { elements.ForEach(e => e.style.flexGrow = f3); }
          break;
        case StyleType.Flex_Direction:
          if (value is FlexDirection fd1) { elements.ForEach(e => e.style.flexDirection = fd1); }
          break;
        case StyleType.Flex_Wrap:
          if (value is Wrap w1) { elements.ForEach(e => e.style.flexWrap = w1); }
          break;
        case StyleType.Align_Items:
          if (value is Align a1) { elements.ForEach(e => e.style.alignItems = a1); }
          break;
        case StyleType.Justify_Content:
          if (value is Justify a2) { elements.ForEach(e => e.style.justifyContent = a2); }
          break;
        case StyleType.Align_Self:
          if (value is Align a3) { elements.ForEach(e => e.style.alignSelf = a3); }
          break;
        case StyleType.Size_Width:
          if (value is StyleLength s6) { elements.ForEach(e => e.style.width = s6); }
          break;
        case StyleType.Size_Height:
          if (value is StyleLength s7) { elements.ForEach(e => e.style.height = s7); }
          break;
        case StyleType.Size_Width_Min:
          if (value is StyleLength s8) { elements.ForEach(e => e.style.minWidth = s8); }
          break;
        case StyleType.Size_Width_Max:
          if (value is StyleLength s9) { elements.ForEach(e => e.style.maxWidth = s9); }
          break;
        case StyleType.Size_Height_Min:
          if (value is StyleLength s10) { elements.ForEach(e => e.style.minHeight = s10); }
          break;
        case StyleType.Size_Height_Max:
          if (value is StyleLength s11) { elements.ForEach(e => e.style.maxHeight = s11); }
          break;
        case StyleType.Margin_All:
          if (value is StyleLength s12)
          {
            elements.ForEach(e =>
            {
              e.style.marginTop = s12;
              e.style.marginRight = s12;
              e.style.marginBottom = s12;
              e.style.marginLeft = s12;
            });
          }
          break;
        case StyleType.Margin_Left:
          if (value is StyleLength s13) { elements.ForEach(e => e.style.marginLeft = s13); }
          break;
        case StyleType.Margin_Right:
          if (value is StyleLength s14) { elements.ForEach(e => e.style.marginRight = s14); }
          break;
        case StyleType.Margin_Top:
          if (value is StyleLength s15) { elements.ForEach(e => e.style.marginTop = s15); }
          break;
        case StyleType.Margin_Bottom:
          if (value is StyleLength s16) { elements.ForEach(e => e.style.marginBottom = s16); }
          break;
        case StyleType.Padding_All:
          if (value is StyleLength s17)
          {
            elements.ForEach(e =>
            {
              e.style.paddingTop = s17;
              e.style.paddingRight = s17;
              e.style.paddingBottom = s17;
              e.style.paddingLeft = s17;
            });
          }
          break;
        case StyleType.Padding_Left:
          if (value is StyleLength s18) { elements.ForEach(e => e.style.paddingLeft = s18); }
          break;
        case StyleType.Padding_Right:
          if (value is StyleLength s19) { elements.ForEach(e => e.style.paddingRight = s19); }
          break;
        case StyleType.Padding_Top:
          if (value is StyleLength s20) { elements.ForEach(e => e.style.paddingTop = s20); }
          break;
        case StyleType.Padding_Bottom:
          if (value is StyleLength s21) { elements.ForEach(e => e.style.paddingBottom = s21); }
          break;
        case StyleType.Text_Font:
          if (value is StyleFont font) { elements.ForEach(e => e.style.unityFont = font); }
          break;
        case StyleType.Text_FontAsset:
          if (value is StyleFontDefinition fontDef) { elements.ForEach(e => e.style.unityFontDefinition = fontDef); }
          break;
        case StyleType.Text_Style:
          if (value is FontStyle fontStyle) { elements.ForEach(e => e.style.unityFontStyleAndWeight = fontStyle); }
          break;
        case StyleType.Text_Size:
          if (value is StyleLength s) { elements.ForEach(e => e.style.fontSize = s); }
          break;
        case StyleType.Text_Color:
          if (value is Color c2) { elements.ForEach(e => e.style.color = c2); }
          break;
        case StyleType.Text_Align:
          if (value is TextAnchor textAnchor) { elements.ForEach(e => e.style.unityTextAlign = textAnchor); }
          break;
        case StyleType.Text_Wrap:
          if (value is WhiteSpace w) { elements.ForEach(e => e.style.whiteSpace = w); }
          break;
        case StyleType.Text_Overflow:
          if (value is TextOverflow textOverflow) { elements.ForEach(e => e.style.textOverflow = textOverflow); }
          break;
        case StyleType.Text_Outline_Width:
          if (value is StyleFloat f4) { elements.ForEach(e => e.style.unityTextOutlineWidth = f4); }
          break;
        case StyleType.Text_Outline_Color:
          if (value is Color c3) { elements.ForEach(e => e.style.unityTextOutlineColor = c3); }
          break;
        case StyleType.Text_Shadow_Offset_Horizontal:
          if (value is float textShadowOffsetVertical)
          {
            elements.ForEach(e =>
            {
              var currentShadowStyle = e.style.textShadow;
              var currentShadow = currentShadowStyle.value;
              var currentOffset = currentShadow.offset;
              currentOffset.y = textShadowOffsetVertical;
              currentShadow.offset = currentOffset;
              currentShadowStyle.value = currentShadow;
              e.style.textShadow = currentShadowStyle;
            });
          }
          break;
        case StyleType.Text_Shadow_Offset_Vertical:
          if (value is float textShadowOffsetHorizontal)
          {
            elements.ForEach(e =>
            {
              var currentShadowStyle = e.style.textShadow;
              var currentShadow = currentShadowStyle.value;
              var currentOffset = currentShadow.offset;
              currentOffset.x = textShadowOffsetHorizontal;
              currentShadow.offset = currentOffset;
              currentShadowStyle.value = currentShadow;
              e.style.textShadow = currentShadowStyle;
            });
          }
          break;
        case StyleType.Text_Shadow_Blur_Radius:
          if (value is float textShadowBlurRadius)
          {
            elements.ForEach(e =>
            {
              var currentShadowStyle = e.style.textShadow;
              var currentShadow = currentShadowStyle.value;
              currentShadow.blurRadius = textShadowBlurRadius;
              currentShadowStyle.value = currentShadow;
              e.style.textShadow = currentShadowStyle;
            });
          }
          break;
        case StyleType.Text_Shadow_Color:
          if (value is Color textShadowColor)
          {
            elements.ForEach(e =>
            {
              var currentShadowStyle = e.style.textShadow;
              var currentShadow = currentShadowStyle.value;
              currentShadow.color = textShadowColor;
              currentShadowStyle.value = currentShadow;
              e.style.textShadow = currentShadowStyle;
            });
          }
          break;
        case StyleType.Text_Spacing_Letter:
          if (value is StyleLength s22) { elements.ForEach(e => e.style.letterSpacing = s22); }
          break;
        case StyleType.Text_Spacing_Word:
          if (value is StyleLength s23) { elements.ForEach(e => e.style.wordSpacing = s23); }
          break;
        case StyleType.Text_Spacing_Paragraph:
          if (value is StyleLength s24) { elements.ForEach(e => e.style.unityParagraphSpacing = s24); }
          break;
        case StyleType.Background_Color:
          if (value is Color c) { elements.ForEach(e => e.style.backgroundColor = c); }
          break;
        case StyleType.Background_Image:
          if (value is StyleBackground sb) { elements.ForEach(e => e.style.backgroundImage = sb); }
          break;
        case StyleType.Background_Image_Tint:
          if (value is Color c4) { elements.ForEach(e => e.style.unityBackgroundImageTintColor = c4); }
          break;
        case StyleType.Background_Scale_Mode:
#pragma warning disable CS0618
          if (value is ScaleMode scaleMode) { elements.ForEach(e => e.style.unityBackgroundScaleMode = scaleMode); }
#pragma warning restore CS0618
          break;
        case StyleType.Background_Slice_All:
          if (value is StyleInt i1)
          {
            elements.ForEach(e =>
            {
              e.style.unitySliceTop = i1;
              e.style.unitySliceRight = i1;
              e.style.unitySliceBottom = i1;
              e.style.unitySliceLeft = i1;
            });
          }
          break;
        case StyleType.Background_Slice_Left:
          if (value is StyleInt i2) { elements.ForEach(e => e.style.unitySliceLeft = i2); }
          break;
        case StyleType.Background_Slice_Top:
          if (value is StyleInt i3) { elements.ForEach(e => e.style.unitySliceTop = i3); }
          break;
        case StyleType.Background_Slice_Right:
          if (value is StyleInt i4) { elements.ForEach(e => e.style.unitySliceRight = i4); }
          break;
        case StyleType.Background_Slice_Bottom:
          if (value is StyleInt i5) { elements.ForEach(e => e.style.unitySliceBottom = i5); }
          break;
#if UNITY_2022_2_OR_NEWER
        case StyleType.Background_Slice_Scale:
          if (value is StyleFloat f5) { elements.ForEach(e => e.style.unitySliceScale = f5); }
          break; 
#endif
        case StyleType.Border_Color_All:
          if (value is Color c5)
          {
            elements.ForEach(e =>
            {
              e.style.borderTopColor = c5;
              e.style.borderRightColor = c5;
              e.style.borderBottomColor = c5;
              e.style.borderLeftColor = c5;
            });
          }
          break;
        case StyleType.Border_Color_Top:
          if (value is Color c6) { elements.ForEach(e => e.style.borderTopColor = c6); }
          break;
        case StyleType.Border_Color_Right:
          if (value is Color c7) { elements.ForEach(e => e.style.borderRightColor = c7); }
          break;
        case StyleType.Border_Color_Bottom:
          if (value is Color c8) { elements.ForEach(e => e.style.borderBottomColor = c8); }
          break;
        case StyleType.Border_Color_Left:
          if (value is Color c9) { elements.ForEach(e => e.style.borderLeftColor = c9); }
          break;
        case StyleType.Border_Width_All:
          if (value is StyleFloat sf1)
          {
            elements.ForEach(e =>
            {
              e.style.borderTopWidth = sf1;
              e.style.borderRightWidth = sf1;
              e.style.borderBottomWidth = sf1;
              e.style.borderLeftWidth = sf1;
            });
          }
          break;
        case StyleType.Border_Width_Top:
          if (value is StyleFloat sf2) { elements.ForEach(e => e.style.borderTopWidth = sf2); }
          break;
        case StyleType.Border_Width_Right:
          if (value is StyleFloat sf3) { elements.ForEach(e => e.style.borderRightWidth = sf3); }
          break;
        case StyleType.Border_Width_Bottom:
          if (value is StyleFloat sf4) { elements.ForEach(e => e.style.borderBottomWidth = sf4); }
          break;
        case StyleType.Border_Width_Left:
          if (value is StyleFloat sf5) { elements.ForEach(e => e.style.borderLeftWidth = sf5); }
          break;
        case StyleType.Border_Radius_All:
          if (value is StyleLength s25)
          {
            elements.ForEach(e =>
            {
              e.style.borderTopLeftRadius = s25;
              e.style.borderTopRightRadius = s25;
              e.style.borderBottomRightRadius = s25;
              e.style.borderBottomLeftRadius = s25;
            });
          }
          break;
        case StyleType.Border_Radius_Top_Left:
          if (value is StyleLength s26) { elements.ForEach(e => e.style.borderTopLeftRadius = s26); }
          break;
        case StyleType.Border_Radius_Top_Right:
          if (value is StyleLength s27) { elements.ForEach(e => e.style.borderTopRightRadius = s27); }
          break;
        case StyleType.Border_Radius_Bottom_Right:
          if (value is StyleLength s28) { elements.ForEach(e => e.style.borderBottomRightRadius = s28); }
          break;
        case StyleType.Border_Radius_Bottom_Left:
          if (value is StyleLength s29) { elements.ForEach(e => e.style.borderBottomLeftRadius = s29); }
          break;
        case StyleType.Cursor_Image:
          if (value is Texture2D cursorTexture)
          {
            elements.ForEach(e =>
            {
              var currentCursorStyle = e.style.cursor;
              var currentCursor = currentCursorStyle.value;
              currentCursor.texture = cursorTexture;
              currentCursorStyle.value = currentCursor;
              e.style.cursor = currentCursorStyle;
            });
          }
          break;
      }
    }

    protected override bool CanApply(StyleProfile styleProfile, string key, object value)
    {
      bool canApply = base.CanApply(styleProfile, key, value);

      if (canApply && document == null)
      {
        document = GetComponentInParent<UIDocument>();
        canApply = document != null;
      }

      return canApply;
    }

    protected List<VisualElement> GetMatchingElements(string queryString)
    {
      if (document == null) { return null; }
      VisualElement root = document.rootVisualElement;
      if (root == null) { return null; }

      UQueryBuilder<VisualElement> results = /*default; */new UQueryBuilder<VisualElement>(root);
      List<VisualElement> firstResults = new List<VisualElement>();
      queryString = queryString.Replace("\n", string.Empty);

      var queries = queryString.Split(' '/*, '#', '.', ':'*/);
      if (queries == null || queries.Length == 0) { return null; }

      //results = root.Query(className: queries[0].Substring(1)).Children<VisualElement>(className: queries[2].Substring(1));
      //results = root.Query(className: queries[0].Substring(1)).Descendents<VisualElement>(classname: queries[2].Substring(1));
      //return results.ToList();

      //var query = queries[0];

      //var results = new UQueryBuilder<VisualElement>();

      //// Check for class name
      //if (query.StartsWith("."))
      //{
      //  var className = query.Substring(1);
      //  results = root.Query(className: className);
      //}
      //// Check for name
      //if (query.StartsWith("#"))
      //{
      //  var name = query.Substring(1);
      //  results = root.Query(name: name);
      //}
      //else if (query == ">")
      //{
      //  // Check for direct childs
      //}

      //if (results != null)
      //{
      //var resultsList = results.ToList();
      //List<VisualElement> matchingElements = new List<VisualElement>();
      bool searchDirectChild = false;
      char[] separators = new char[] { '.', '#', ':' };
      bool isRoot = true;
      bool removeFirstResults = false;

      for (int i = 0; i < queries.Length; i++)
      {

        var query = queries[i];
        if (query == ">")
        {
          searchDirectChild = true;
          continue;
        }

        string elementName = null;
        string className = null;
        string pseudoClassName = null;

        elementName = GetSelector('#', query);
        className = GetSelector('.', query);
        pseudoClassName = GetSelector(':', query);

        if (isRoot)
        {
          GetMatchingElements(root, ref results, elementName, className, pseudoClassName);

          // If there is name of a class specified make it no longer the root
          if (elementName != null || className != null)
          {
            isRoot = false;
          }

          //foreach (var item in results.ToList())
          //{
          //  firstResults.Add(item);
          //}
          //results.Build();
        }
        else
        {
          GetMatchingElements(ref results, elementName, className, pseudoClassName, searchDirectChild);
          //results.Build();
          removeFirstResults = true;
        }

        searchDirectChild = false;
      }
      //String        
      //string pseudoClassName = resolvedQueries.Find(q => q.StartsWith(":"));

      //for (int i = 0; i < queries.Length; i++)
      //{
      //  var query = queries[i];
      //  var query = queries[i];
      //  List<int> indices = new List<int>();
      //  int lastIndex = -1;

      //  do
      //  {
      //    lastIndex = query.IndexOfAny(separators, lastIndex + 1);
      //    if (lastIndex != -1)
      //    {
      //      indices.Add(lastIndex);
      //    }
      //  } while (lastIndex != -1);

      //  var resolvedQueries = new List<string>();
      //  for (int j = 0; j < indices.Count; j++)
      //  {
      //    // Check if it is the last index
      //    if (j == indices.Count - 1)
      //    {
      //      resolvedQueries.Add(query.Substring(indices[j]));
      //    }
      //    else
      //    {
      //      resolvedQueries.Add(query.Substring(indices[j], indices[j + 1] - indices[j]));
      //    }
      //  }

      //var firstQuery = resolvedQueries[0];

      //if (resolvedQueries[0] == ">")
      //{
      //  searchDirectChild = true;
      //  continue;
      //}


      //string className = resolvedQueries.Find(q => q.StartsWith("."));
      //string elementName = resolvedQueries.Find(q => q.StartsWith("#"));
      //string pseudoClassName = resolvedQueries.Find(q => q.StartsWith(":"));

      //results = GetMatchingElements(results, className, elementName, pseudoClassName);

      //searchDirectChild = false;

      //foreach (var item in resultsList)
      //{
      //var elements = GetMatchingElements(item, className, elementName, pseudoClassName);

      //if (elements != null && elements.Count > 0)
      //{
      //  matchingElements.AddRange(elements);
      //}

      //// Name + ClassName + PseudoClassName
      //if (elementName != null && className != null && pseudoClassName != null)
      //{
      //  var r = item.Query(name = elementName, classes: new string[] { className, pseudoClassName });
      //  matchingElements.AddRange(r.ToList());
      //}
      //// Name + ClassName
      //else if (elementName != null && className != null && pseudoClassName == null)
      //{
      //  var r = item.Query(name = elementName, className: className);
      //  matchingElements.AddRange(r.ToList());
      //}
      //// Name
      //else if (elementName != null && className == null && pseudoClassName == null)
      //{
      //  var r = item.Query(name = elementName);
      //  matchingElements.AddRange(r.ToList());
      //}
      //// ClassName + PseudoClassName
      //else if (elementName == null && className != null && pseudoClassName != null)
      //{
      //  var r = item.Query(classes: new string[] { className, pseudoClassName });
      //  matchingElements.AddRange(r.ToList());
      //}
      //// ClassName
      //else if (elementName == null && className != null && pseudoClassName == null)
      //{
      //  var r = item.Query(className: className);
      //  matchingElements.AddRange(r.ToList());
      //}
      //}

      //foreach (var item in resultsList)
      //{
      //  var elements = GetMatchingElements(item, className, elementName, pseudoClassName);

      //  if (elements != null && elements.Count > 0)
      //  {
      //    matchingElements.AddRange(elements);
      //  }
      //}

      //// Check for class name
      //if (firstQuery.StartsWith("."))
      //{
      //  var className = query.Substring(1);
      //  if (searchDirectChild)
      //  {
      //    // TODO
      //  }
      //  else
      //  {


      //  }
      //}
      //// Check for name
      //if (firstQuery.StartsWith("#"))
      //{
      //  var name = query.Substring(1);
      //  results = root.Query(name: name);
      //}
      //else if (firstQuery == ">")
      //{
      //  // Check for direct childs
      //  searchDirectChild = true;
      //}


      //}
      //}
      if (results != null)
      {
        var list = results.ToList();
        if (removeFirstResults)
        {
          list.RemoveAll(e => firstResults.Contains(e));
        }
        return list;
      }

      return null;
    }

    protected static string GetSelector(char identifier, string input)
    {
      string selector = string.Empty;
      var startIndex = input.IndexOf(identifier);
      if (startIndex != -1)
      {
        var endIndex = input.IndexOfAny(separators, startIndex + 1);
        if (endIndex == -1)
        {
          selector = input.Substring(startIndex + 1);
        }
        else
        {
          selector = input.Substring(startIndex, endIndex - startIndex);
        }
      }

      return selector;
    }

    protected /*UQueryBuilder<VisualElement>*/void GetMatchingElements(ref UQueryBuilder<VisualElement> input, string elementName,
      string className, string pseudoClassName, bool directChildren = false)
    {
      //UQueryBuilder<VisualElement> r = new UQueryBuilder<VisualElement>();

      //input.ForEach(c => ConsoleLogger.Log(c.name));

      // Name + ClassName + PseudoClassName
      if (!string.IsNullOrEmpty(elementName) && !string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(pseudoClassName))
      {
        if (directChildren)
        {
          input = input.Children<VisualElement>(name = elementName, classes: new string[] { className, pseudoClassName });
        }
        else
        {
          input = input.Descendents<VisualElement>(name = elementName, classNames: new string[] { className, pseudoClassName });
        }
      }
      // Name + ClassName
      else if (!string.IsNullOrEmpty(elementName) && !string.IsNullOrEmpty(className) && string.IsNullOrEmpty(pseudoClassName))
      {
        if (directChildren)
        {
          input = input.Children<VisualElement>(name = elementName, className: className);
        }
        else
        {
          input = input.Descendents<VisualElement>(name = elementName, classname: className);
        }
      }
      // Name
      else if (!string.IsNullOrEmpty(elementName) && string.IsNullOrEmpty(className) && string.IsNullOrEmpty(pseudoClassName))
      {
        if (directChildren)
        {
          input = input.Children<VisualElement>(name = elementName);
        }
        else
        {
          input = input.Descendents<VisualElement>(name = elementName);
        }
      }
      // ClassName + PseudoClassName
      else if (string.IsNullOrEmpty(elementName) && !string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(pseudoClassName))
      {
        if (directChildren)
        {
          input = input.Children<VisualElement>(classes: new string[] { className, pseudoClassName });
        }
        else
        {
          input = input.Descendents<VisualElement>(classNames: new string[] { className, pseudoClassName });
        }
      }
      // ClassName
      else if (string.IsNullOrEmpty(elementName) && !string.IsNullOrEmpty(className) && string.IsNullOrEmpty(pseudoClassName))
      {
        if (directChildren)
        {
          //input = input.Children<VisualElement>().Where((e) => e.ClassListContains(className));
          input = input.Children<VisualElement>(className: className);
        }
        else
        {
          input = input.Descendents<VisualElement>(classname: className);
        }
      }

      //var list = r.ToList();
      //input.ForEach(i => list.Remove(i));
      ////r.ToList().ForEach(element => { ConsoleLogger.Log(element.name); });
      //UQueryBuilder<VisualElement> actualList = new UQueryBuilder<VisualElement>();
      //actualList.

      //return r;
    }

    protected /*UQueryBuilder<VisualElement>*/void GetMatchingElements(VisualElement input, ref UQueryBuilder<VisualElement> r, string elementName,
      string className, string pseudoClassName)
    {
      //UQueryBuilder<VisualElement> r = new UQueryBuilder<VisualElement>();

      // Name + ClassName + PseudoClassName
      if (!string.IsNullOrEmpty(elementName) && !string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(pseudoClassName))
      {
        r = input.Query(name: elementName, classes: new string[] { className, pseudoClassName });
      }
      // Name + ClassName
      else if (!string.IsNullOrEmpty(elementName) && !string.IsNullOrEmpty(className) && string.IsNullOrEmpty(pseudoClassName))
      {
        r = input.Query(name: elementName, className: className);
      }
      // Name
      else if (!string.IsNullOrEmpty(elementName) && string.IsNullOrEmpty(className) && string.IsNullOrEmpty(pseudoClassName))
      {
        r = input.Query(name: elementName);
      }
      // ClassName + PseudoClassName
      else if (string.IsNullOrEmpty(elementName) && !string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(pseudoClassName))
      {
        r = input.Query(classes: new string[] { className, pseudoClassName });

      }
      // ClassName
      else if (string.IsNullOrEmpty(elementName) && !string.IsNullOrEmpty(className) && string.IsNullOrEmpty(pseudoClassName))
      {
        //r = input.Query(className: className);
        r = input.Query(classes: new string[] { className });
      }

      //var list = r.ToList();
      //r.ToList().ForEach(element => { ConsoleLogger.Log(element.name); });
      //return r;
    }
  }
}