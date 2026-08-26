using CitrioN.Common;
using UnityEngine;
using UnityEngine.UI;

namespace CitrioN.UI
{
  [AddTooltips]
  [HeaderInfo("Allows the updating of a GridLayoutGroup's cell size.")]
  [RequireComponent(typeof(GridLayoutGroup))]
  public class GridLayoutGroupUpdater : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("Whether the cell sizes of the GridLayoutGroup should be squared.")]
    private bool enforceSquared = false;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ReadOnly]
#endif
    [SerializeField]
    [Tooltip("The GridLayoutGroup to set the cell size for.")]
    private GridLayoutGroup gridLayoutGroup;

    private void Reset()
    {
      gridLayoutGroup = GetComponent<GridLayoutGroup>();
    }

    private void OnEnable()
    {
      UpdateGridLayoutGroupCellSize();
    }

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Button("Update GridLayoutGroup Settings")]
#endif
    [ContextMenu("Update GridLayoutGroup Settings")]
    [SkipObfuscationRename]
    public void UpdateGridLayoutGroupCellSize()
    {
      if (gridLayoutGroup == null)
      {
        Debug.LogError($"No {nameof(GridLayoutGroup)} found");
        return;
      }

      if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
      {
        // Get the grid width from the rect component of the layout group
        var gridWidth = gridLayoutGroup.GetComponent<RectTransform>().rect.width;
        var availableWidth = gridWidth - gridLayoutGroup.padding.left - gridLayoutGroup.padding.right -
                             ((gridLayoutGroup.constraintCount - 1) * gridLayoutGroup.spacing.x);
        var columnWidth = availableWidth / gridLayoutGroup.constraintCount;
        gridLayoutGroup.cellSize = new Vector2(columnWidth, enforceSquared ? columnWidth : gridLayoutGroup.cellSize.y);
      }
      else if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedRowCount)
      {
        // Get the grid height from the rect component of the layout group
        var gridHeight = gridLayoutGroup.GetComponent<RectTransform>().rect.height;
        var availableHeight = gridHeight - gridLayoutGroup.padding.top - gridLayoutGroup.padding.bottom -
                             ((gridLayoutGroup.constraintCount - 1) * gridLayoutGroup.spacing.y);
        var rowHeight = availableHeight / gridLayoutGroup.constraintCount;
        gridLayoutGroup.cellSize = new Vector2(enforceSquared ? rowHeight : gridLayoutGroup.cellSize.x, rowHeight);
      }
    }
  }
}