using UnityEngine.Rendering;

namespace CitrioN.Common
{
  public static class RenderPipelineUtility
  {
    public static bool GetCurrentRenderPipelineAsset<T>(out T rp) where T : RenderPipelineAsset
    {
      var currentRp = GraphicsSettings.currentRenderPipeline;
      if (currentRp == null || currentRp is not T)
      {
        rp = null;
        return false;
      }

      rp = currentRp as T;
      return true;
    }
  }
}