// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using Boxophobic.StyledGUI;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Boxophobic.Utility
{
    [ExecuteInEditMode]
    public class SceneSwitch : StyledMonoBehaviour
    {
        [StyledBanner("Switch")]
        public bool styledBanner;

        public GameObject setupStandard;
        public GameObject setupUniversal;
        public GameObject setupHD;
        [HideInInspector]
        public GameObject objectStandard;
        [HideInInspector]
        public GameObject objectUniversal;
        [HideInInspector]
        public GameObject objectHD;

        [Space(10)]
        public bool setRenderSettings;

        [Space(10)]
        public Material skyboxMaterial;
        [Range(0, 8)]
        public float skyboxAmbient = 1;
        [Range(0, 1)]
        public float skyboxReflection = 1;

        [StyledSpace(5)]
        public bool styledSpace;

        void OnEnable()
        {
            if (Application.isPlaying)
            {
                return;
            }

            int pipeline = 0;

            if (GraphicsSettings.defaultRenderPipeline != null)
            {
                if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("Universal"))
                {
                    pipeline = 1;
                }

                if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("HD"))
                {
                    pipeline = 2;
                }
            }

            if (QualitySettings.renderPipeline != null)
            {
                if (QualitySettings.renderPipeline.GetType().ToString().Contains("Universal"))
                {
                    pipeline = 1;
                }

                if (QualitySettings.renderPipeline.GetType().ToString().Contains("HD"))
                {
                    pipeline = 2;
                }
            }

            if (pipeline == 0)
            {
                if (setupStandard != null && objectStandard == null)
                {
                    objectStandard = Instantiate(setupStandard, gameObject.transform);
                    objectStandard.name = objectStandard.name.Replace("(Clone)", "");
                }
            }

            if (pipeline == 1)
            {
                if (setupUniversal != null && objectUniversal == null)
                {
                    objectUniversal = Instantiate(setupUniversal, gameObject.transform);
                    objectUniversal.name = objectUniversal.name.Replace("(Clone)", "");
                }
            }

            if (pipeline == 2)
            {
                if (setupHD != null && objectHD == null)
                {
                    objectHD = Instantiate(setupHD, gameObject.transform);
                    objectHD.name = objectHD.name.Replace("(Clone)", "");
                }
            }

            if (setRenderSettings)
            {
                RenderSettings.skybox = skyboxMaterial;
                RenderSettings.ambientIntensity = skyboxAmbient;
                RenderSettings.reflectionIntensity = skyboxReflection;

                DynamicGI.UpdateEnvironment();
            }

#if UNITY_EDITOR
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
#endif
        }
    }
}



