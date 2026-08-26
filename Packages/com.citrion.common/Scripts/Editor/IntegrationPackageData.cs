using UnityEngine;

namespace CitrioN.Common.Editor
{
  [CreateAssetMenu(fileName = "IntegrationPackageData_",
                   menuName = "CitrioN/Common/IntegrationPackageData")]
  public class IntegrationPackageData : ScriptableObjectItemData
  {
    [SerializeField]
    [Tooltip("The asset's publisher name.")]
    private string publisherName;

    [SerializeField]
    [Tooltip("The url to the asset.")]
    private string assetUrl;

    [SerializeField]
    [TextArea(minLines: 4, maxLines: 20)]
    [Tooltip("The details about the integration.")]
    private string integrationDetails;

    [SerializeField]
    [UnityPackageObject]
    [Tooltip("The Unity package file to manage.")]
    private Object packageAsset;

    [SerializeField]
    [Tooltip("An asset reference for checking if the required " +
         "asset for the integration is in the project.")]
    private Object assetReferenceAsset;

    [SerializeField]
    [Tooltip("A package identifier for checking if the required " +
             "package for the integration is in the project.")]
    private string packageId;

    [SerializeField]
    [Tooltip("An asset reference for checking if the " +
             "integration is in the project.")]
    private Object integrationReferenceAsset;

    public string PublisherName { get => publisherName; set => publisherName = value; }

    public Object Package { get => packageAsset; protected set => packageAsset = value; }

    public string AssetUrl { get => assetUrl; set => assetUrl = value; }

    public string IntegrationDetails { get => integrationDetails; set => integrationDetails = value; }

    public bool IsIntegrationAssetImported
    {
      get
      {
        if (assetReferenceAsset != null) { return true; }
        if (!string.IsNullOrEmpty(packageId))
        {
          return PackageUtilities.IsPackageInstalled(packageId, out _);
        }
        return false;
      }
    }

    public bool IsIntegrationImported => integrationReferenceAsset != null;
  }
}