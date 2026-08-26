using UnityEngine;

namespace CitrioN.Common
{
  [System.AttributeUsage(System.AttributeTargets.Class)]
  [SkipObfuscationRename]
  public class ClassDescriptionAttribute : System.Attribute
  {
    private string resolveString = string.Empty;
    private DescriptionResolveMethod descriptionResolveMethod = DescriptionResolveMethod.Raw;

    public ClassDescriptionAttribute() { }

    public ClassDescriptionAttribute(string descriptionOrFieldName)
    {
      this.resolveString = descriptionOrFieldName;
    }

    public ClassDescriptionAttribute(string descriptionOrFieldName,
                                     DescriptionResolveMethod descriptionResolveMethod)
    {
      this.resolveString = descriptionOrFieldName;
      this.descriptionResolveMethod = descriptionResolveMethod;
    }

    public string GetDescription(object obj)
    {
      if (string.IsNullOrEmpty(resolveString))
      {
        return string.Empty;
      }

      switch (descriptionResolveMethod)
      {
        case DescriptionResolveMethod.Raw:
          return resolveString;

        case DescriptionResolveMethod.Field:
          if (obj == null)
          {
            Debug.LogError($"Object is null");
            return string.IsNullOrEmpty(resolveString) ? string.Empty : resolveString;
          }
          var field = obj.GetType().GetSerializableField(resolveString);
          if (field == null)
          {
            Debug.LogError($"No field found with the name {resolveString}");
            return string.IsNullOrEmpty(resolveString) ? string.Empty : resolveString;
          }
          var fieldType = field.FieldType;
          if (fieldType != typeof(string))
          {
            Debug.LogError($"The field with the name {resolveString} must be of " +
                           $"type string to be used for the class description");
            return string.IsNullOrEmpty(resolveString) ? string.Empty : resolveString;
          }
          return field.GetValue(obj) as string;

        case DescriptionResolveMethod.Property:
          if (obj == null)
          {
            Debug.LogError($"Object is null");
            return string.IsNullOrEmpty(resolveString) ? string.Empty : resolveString;
          }
          var property = obj.GetType().GetProperty(resolveString);
          if (property == null)
          {
            Debug.LogError($"No property found with the name {resolveString}");
            return string.IsNullOrEmpty(resolveString) ? string.Empty : resolveString;
          }
          var propertyType = property.PropertyType;
          if (propertyType != typeof(string))
          {
            Debug.LogError($"The property with the name {resolveString} must return" +
                           $"a string to be used for the class description");
            return string.IsNullOrEmpty(resolveString) ? string.Empty : resolveString;
          }
          return property.GetValue(obj) as string;

        case DescriptionResolveMethod.Method:
          if (obj == null)
          {
            Debug.LogError($"Object is null");
            return string.IsNullOrEmpty(resolveString) ? string.Empty : resolveString;
          }
          var method = obj.GetType().GetMethod(resolveString);
          if (method == null)
          {
            Debug.LogError($"No method found with the name {resolveString}.");
            return string.IsNullOrEmpty(resolveString) ? string.Empty : resolveString;
          }
          if (method.ReturnType != typeof(string))
          {
            Debug.LogError($"The method with the name {resolveString} must return a " +
                           $"string to be used for the class description");
            return string.IsNullOrEmpty(resolveString) ? string.Empty : resolveString;
          }
          return method.Invoke(obj, null) as string;

        default:
          return string.IsNullOrEmpty(resolveString) ? string.Empty : resolveString;
      }
    }
  }
}