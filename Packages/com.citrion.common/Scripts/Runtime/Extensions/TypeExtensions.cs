using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CitrioN.Common
{
  /// <summary>
  /// Contains <see cref="Type"/> extension methods.
  /// </summary>
  public static class TypeExtensions
  {
    /// <summary>
    /// Determines if type is a generic list.
    /// </summary>
    public static bool IsListType(this Type type)
    {
      return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }

    public static bool IsListOfType<T>(this Type type)
    {
      if (!type.IsListType()) { return false; }
      return type.GetGenericTypeDefinition() == typeof(List<T>);
    }

    [SkipObfuscation]
    public static FieldInfo GetSerializableField(this Type type, string name)
    {
      var field = type?.GetField(name, BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Instance);
      if (field != null &&
         (field.IsPublic || field.IsDefined(typeof(SerializeField))))
      {
        return field;
      }
      return null;
    }

    [SkipObfuscation]
    public static IEnumerable<FieldInfo> GetSerializableFields(this Type type)
    {
      return type?.GetFields(BindingFlags.Public | BindingFlags.NonPublic |
                             BindingFlags.Instance).Where(f => f.IsPublic ||
                                                          f.IsDefined(typeof(SerializeField)));
    }

    [SkipObfuscation]
    public static FieldInfo GetPrivateField(this Type type, string fieldName)
    {
      return type?.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public static FieldInfo GetFieldIncludingBaseClasses(this Type type, string fieldName)
    {
      if (type == null || string.IsNullOrEmpty(fieldName)) { return null; }
      FieldInfo fieldInfo;
      var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

      do
      {
        fieldInfo = type.GetField(fieldName, flags);
        type = type.BaseType;
      } while (fieldInfo == null && type != null);

      return fieldInfo;
    }

    [SkipObfuscation]
    public static IEnumerable<FieldInfo> GetPrivateFields(this Type type)
    {
      return type?.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
    }

    [SkipObfuscation]
    public static PropertyInfo GetPrivateProperty(this Type type, string propertyName)
    {
      return type?.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
    }

    [SkipObfuscation]
    public static IEnumerable<PropertyInfo> GetPrivateProperties(this Type type)
    {
      return type?.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
    }

    [SkipObfuscation]
    public static MethodInfo GetPrivateMethod(this Type type, string methodName)
    {
      return type?.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
    }

    [SkipObfuscation]
    public static IEnumerable<MethodInfo> GetPrivateMethods(this Type type)
    {
      return type?.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
    }

    [SkipObfuscation]
    public static void InvokePrivateMethodNoParams(this Type type, object obj, string methodName)
    {
      var method = type?.GetPrivateMethod(methodName);
      if (method != null && obj != null)
      {
        method.Invoke(obj, new object[] { });
      }
      else
      {
        ConsoleLogger.LogWarning($"Unable to invoke method '{methodName}'", LogType.Always);
      }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Finds all derived classed of the provided type.
    /// </summary>
    /// <param name="type">The type to find derived types for</param>
    /// <param name="includeSelf">Should the provided type be part of the returned list of types?</param>
    /// <returns>List of all types that derive from the provided type</returns>
    public static List<Type> GetDerivedTypesFromAllAssemblies(this Type type, bool includeSelf = true)
    {
      var derivedTypes = new List<Type>();
      if (type == null) { return derivedTypes; }

      // Get all assemblies
      var assemblies = AppDomain.CurrentDomain.GetAssemblies();

      // Search through all of the assemblies to find any types that derive from the provided type
      foreach (var assembly in assemblies)
      {
        // Get all types in the current assembly
        var assemblyTypes = assembly.GetTypes();
        // Iterate over all types in the current assembly
        foreach (var assemblyType in assemblyTypes)
        {
          // Check for invalid types
          if (assemblyType.IsAbstract || assemblyType.IsInterface ||
              type.IsAssignableFrom(assemblyType) == false || assemblyType.IsPublic == false)
          {
            continue;
          }
          if (includeSelf == false && assemblyType == type)
          {
            continue;
          }
          // Add the type to the list of derived types
          derivedTypes.AddIfNotContains(assemblyType);
        }
      }
      return derivedTypes;
    }

    public static List<Type> GetDerivedTypesForSelection(this Type t, bool includeSelf = true)
    {
      var validTypes = new List<Type>();
      if (t == null) { return validTypes; }

      var types = t.GetDerivedTypesFromAllAssemblies(includeSelf);
      foreach (var type in types)
      {
        var excludeAttribute = type.GetCustomAttribute(typeof(ExcludeFromMenuSelectionAttribute), false);
        if (excludeAttribute == null)
        {
          validTypes.AddIfNotContains(type);
        }
      }
      return validTypes;
    }

    public static List<Type> GetSerializableTypes()
    {
      List<Type> serializableTypes = new List<Type>();

      var assemblies1 = UnityEditor.Compilation.CompilationPipeline.GetAssemblies().ToList();
      var editorAssemblies = new List<string>();
      foreach (var item in assemblies1)
      {
        if (item.flags.HasFlag(UnityEditor.Compilation.AssemblyFlags.EditorAssembly))
        {
          editorAssemblies.Add(item.name);
        }
      }

      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
      foreach (var assembly in assemblies)
      {
        var name = assembly.GetName().Name;

        if (editorAssemblies.Contains(name) || name.Contains(".Editor"))
        {
          // ConsoleLogger.Log("Is Editor Only Type: " + name);
          continue;
        }

        //var flags = assembly.GetCustomAttribute<AssemblyFlagsAttribute>();
        //if (flags != null)
        {
          //ConsoleLogger.Log($"assembly:{flags.AssemblyFlags}");
        }
        //if ((assembly.flags & UnityEditor.Compilation.AssemblyFlags.EditorAssembly) == UnityEditor.Compilation.AssemblyFlags.EditorAssembly)
        //{

        //}

        //ConsoleLogger.Log(assembly.FullName);
        Type[] types;
        try
        {
          types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
          types = e.Types;
        }

        foreach (Type type in types)
        {
          if (type == null) continue;

          //if (type.IsEnum) { serializableTypes.Add(type); }

          if (type.IsSerializable || typeof(UnityEngine.Object).IsAssignableFrom(type))
          {
            serializableTypes.Add(type);
          }
        }
      }

      return serializableTypes;
    }
#endif
  }
}