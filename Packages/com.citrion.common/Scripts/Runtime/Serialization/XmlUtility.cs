using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CitrioN.Common
{
  public static class XmlUtility
  {
    public static string ObjectToXml(object obj)
    {
      XmlSerializer serializer = new XmlSerializer(obj.GetType());
      StringBuilder result = new StringBuilder();
      using (var writer = XmlWriter.Create(result))
      {
        serializer.Serialize(writer, obj);
      }
      return result.ToString();
    }

    public static object XmlStringToObject(string xml, Type type)
    {
      if (type == null) { return null; }
      object value = null;

      var serializer = new XmlSerializer(type);

      using (var strReader = new StringReader(xml))
      {
        using (var reader = XmlReader.Create(strReader))
        {
          value = serializer.Deserialize(reader);
        }
      }

      return value;
    }

    public static object XmlStringToObject<T>(string xml)
    {
      return XmlStringToObject(xml, typeof(T));
    }
  }
}