using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  public class EditorDragAndDropManipulator : PointerManipulator
  {
    protected Action<VisualElement, object> onDragEnter;
    protected Action<VisualElement, object> onDragLeave;
    protected Action<VisualElement, object> onDragPerform;
    protected Type validDragType;

    protected object DraggedObject
    {
      get
      {
        if (DragAndDrop.objectReferences?.Length > 0)
        {
          return DragAndDrop.objectReferences[0];
        }
        return null;
      }
    }

    public EditorDragAndDropManipulator(VisualElement root)
    {
      target = root;
    }

    public EditorDragAndDropManipulator(VisualElement root, Type validDragType,
                                        Action<VisualElement, object> onDragEnter,
                                        Action<VisualElement, object> onDragLeave,
                                        Action<VisualElement, object> onDragPerform)
      : this(root)
    {
      this.validDragType = validDragType;
      if (onDragEnter != null) { this.onDragEnter += onDragEnter; }
      if (onDragLeave != null) { this.onDragLeave += onDragLeave; }
      if (onDragPerform != null) { this.onDragPerform += onDragPerform; }
    }

    protected override void RegisterCallbacksOnTarget()
    {
      target.RegisterCallback<DragEnterEvent>(OnDragEnter);
      target.RegisterCallback<DragLeaveEvent>(OnDragLeave);
      target.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
      target.RegisterCallback<DragPerformEvent>(OnDragPerform);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
      target.UnregisterCallback<DragEnterEvent>(OnDragEnter);
      target.UnregisterCallback<DragLeaveEvent>(OnDragLeave);
      target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
      target.UnregisterCallback<DragPerformEvent>(OnDragPerform);
    }

    protected virtual void OnDragEnter(DragEnterEvent e)
    {
      //ConsoleLogger.Log("Entered drag area");
      onDragEnter?.Invoke(target, DraggedObject);
    }

    protected virtual void OnDragLeave(DragLeaveEvent e)
    {
      // TODO Should the target be taken from the event instead?
      // e.target
      // e.currentTarget

      //ConsoleLogger.Log("Left drag area");
      onDragLeave?.Invoke(target, DraggedObject);
    }

    /// <summary>
    /// Runs every frame while a drag is in progress
    /// </summary>
    protected virtual void OnDragUpdate(DragUpdatedEvent e)
    {
      DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
      //if (DraggedObject != null)
      //{
      //  bool isCorrectType = DraggedObject.GetType().IsAssignableFrom(validDragType);
      //  DragAndDrop.visualMode = isCorrectType ?
      //                           DragAndDropVisualMode.Generic : DragAndDropVisualMode.None;
      //}
    }

    protected virtual void OnDragPerform(DragPerformEvent e)
    {
      //ConsoleLogger.Log($"Dropped {DraggedObject}");
      onDragLeave?.Invoke(target, DraggedObject);
      onDragPerform?.Invoke(target, DraggedObject);
    }
  }
}