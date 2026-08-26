using UnityEditor;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  public abstract class PropertyDrawerFromVisualTreeAssetWithDebug : PropertyDrawerFromVisualTreeAsset
  {
    protected StringToStringRelationProfile debugTextProfile;

    protected virtual string ProfileFolderPath
              => "Packages/com.citrion.common/UI Toolkit/ScriptableObjects/Profiles/StringToString/";
    protected virtual string ProfileFileName => GetType().Name;
    protected virtual string ProfileLanguageExtension => "_English";

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
      base.CreatePropertyGUI(property);

      var debugTextProfilePath = $"{ProfileFolderPath}{ProfileFileName}{ProfileLanguageExtension}.asset";
      debugTextProfile = AssetDatabase.LoadAssetAtPath<StringToStringRelationProfile>(debugTextProfilePath);

      return root;
    }
  }
}