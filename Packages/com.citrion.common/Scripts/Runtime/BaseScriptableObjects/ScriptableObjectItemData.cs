using UnityEngine;

namespace CitrioN.Common
{
  [AddTooltips]
  [HeaderInfo("Base class which holds information about an item such as its display name & description, " +
              "an optional group to better organize items of the same group and a priority to allow things like item ordering.")]
  [CreateAssetMenu(fileName = "ScriptableObjectItemData_",
                   menuName = "CitrioN/Common/ScriptableObjects/ScriptableObjectItemDatas/ScriptableObjectItemData")]
  public class ScriptableObjectItemData : ScriptableObject
  {
    [SerializeField]
    [Tooltip("The display name for this item.")]
    private string displayName;

    [SerializeField]
    [Tooltip("A group this item belongs to (Optional).")]
    private string group;

    [TextArea(minLines: 4, maxLines: 20)]
    [SerializeField]
    [Tooltip("The description for this item.")]
    private string description;

    [SerializeField]
    [Tooltip("The priority for this item. " +
             "The priority can for example be used to display multiple items in a specific order.")]
    private int priority = 1;

    public string DisplayName { get => displayName; set => displayName = value; }
    public string Group { get => group; set => group = value; }
    public string Description { get => description; set => description = value; }
    public int Priority { get => priority; set => priority = value; }
  }
}