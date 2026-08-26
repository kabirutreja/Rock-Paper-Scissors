using CitrioN.Common;
using UnityEngine;

namespace CitrioN.SettingsMenuCreator
{
  [System.Serializable]
  [MenuOrder(600)]
  [MenuPath("Camera")]
  [ExcludeFromMenuSelection]
  public abstract class Setting_Camera<T1> : Setting_Generic_Reflection_Property_Unity<T1, Camera>
  {
    public override string EditorNamePrefix => "[Camera]";

    [SerializeField]
    [Tooltip("Should the camera reference be cached for future access?\n\n" +
             "While better for performance it will not work when\n" +
             "the main camera is changed.\n\n" +
             "Default: false")]
    protected bool cacheCamera = false;

    protected bool isCached = false;

    // TODO Should some initialization method reset this?
    // like on game start?
    protected Camera camera;

    public override bool StoreValueInternally => true;

    public virtual Camera Cam
    {
      get
      {
        if (isCached) { return camera; }
        else
        {
          if (cacheCamera)
          {
            CacheCamera();
            if (camera == null)
            {
              ConsoleLogger.LogWarning($"No camera object found for {GetType().Name}");
            }
            return camera;
          }
          return Camera.main;
        }
      }
    }

    protected void CacheCamera()
    {
      if (!isCached)
      {
        camera = Camera.main;
        isCached = true;
      }
    }

    public override object GetObject(SettingsCollection settings) => Cam;

    //protected override object ApplySettingChangeWithValue(SettingsCollection settings, T1 value)
    //{
    //  var cam = Cam;
    //  if (cam != null)
    //  {
    //    return ApplySettingChangeWithValueInternal(cam, settings, value);
    //  }
    //  return base.ApplySettingChangeWithValue(settings, value);
    //}

    //protected abstract object ApplySettingChangeWithValueInternal(Camera cam, SettingsCollection settings, T1 value);

    //public override List<object> GetCurrentValues(SettingsCollection settings)
    //{
    //  var cam = Cam;
    //  if (cam != null)
    //  {
    //    return GetCurrentValuesInternal(cam, settings);
    //  }
    //  return base.GetCurrentValues(settings);
    //}

    //protected abstract List<object> GetCurrentValuesInternal(Camera camera, SettingsCollection settings);
  }
}
