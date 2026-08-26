using CitrioN.Common;
using System.ComponentModel;
using TMPro;

namespace CitrioN.StyleProfileSystem
{
  [System.Serializable]
	[MenuOrder(500)]
	[MenuPath("UGUI/Text Mesh Pro")]
	[DisplayName("Font (TextMeshPro)")]
	public class StyleProfileData_FontAsset_TMP : GenericStyleProfileData<TMP_FontAsset> { } 
}