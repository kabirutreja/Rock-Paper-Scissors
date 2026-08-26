using CitrioN.Common;
using System.ComponentModel;
using UnityEngine;

namespace CitrioN.StyleProfileSystem
{
  [System.Serializable]
  [MenuOrder(2000)]
  [MenuPath("Basic/")]
  [DisplayName("Boolean")]
  public class StyleProfileData_Boolean : GenericStyleProfileData<bool> { }

  [System.Serializable]
  [MenuOrder(2000)]
  [MenuPath("Basic/")]
  [DisplayName("Integer")]
  public class StyleProfileData_Integer : GenericStyleProfileData<int> { }

  [System.Serializable]
  [MenuOrder(2000)]
  [MenuPath("Basic/")]
  [DisplayName("Float")]
  public class StyleProfileData_Float : GenericStyleProfileData<float> { }

  [System.Serializable]
  [MenuOrder(2000)]
  [MenuPath("Basic/")]
  [DisplayName("String")]
  public class StyleProfileData_String : GenericStyleProfileData<string> { }

  [System.Serializable]
  [MenuOrder(2000)]
  [MenuPath("Basic/")]
  [DisplayName("Color")]
  public class StyleProfileData_Color : GenericStyleProfileData<Color> { }

  [System.Serializable]
  [MenuOrder(2000)]
  [MenuPath("Basic/")]
  [DisplayName("Sprite")]
  public class StyleProfileData_Sprite : GenericStyleProfileData<Sprite> { }

  [System.Serializable]
  [MenuOrder(2000)]
  [MenuPath("Basic/")]
  [DisplayName("Vector2")]
  public class StyleProfileData_Vector2 : GenericStyleProfileData<Vector2> { }

  [System.Serializable]
  [MenuOrder(2000)]
  [MenuPath("Basic/")]
  [DisplayName("Vector3")]
  public class StyleProfileData_Vector3 : GenericStyleProfileData<Vector3> { }
}