using UnityEngine;

namespace CitrioN.Common.Editor
{
  [CreateAssetMenu(fileName = "DocumentationData_",
                   menuName = "CitrioN/Common/DocumentationData")]
  public class DocumentationData : ScriptableObjectItemData
	{
    [SerializeField]
    [Tooltip("The path to the offline documentation folder")]
    private string offlineDocumentationFolder;

    [SerializeField]
    [Tooltip("The link to the online documentation")]
    private string onlineDocumentationLink;

    public string OfflineDocumentationFolder { get => offlineDocumentationFolder; set => offlineDocumentationFolder = value; }

    public string OnlineDocumentationLink { get => onlineDocumentationLink; set => onlineDocumentationLink = value; }
  } 
}