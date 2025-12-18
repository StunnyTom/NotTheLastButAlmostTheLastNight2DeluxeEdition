using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace WaterSystem
{
    public class WaterSystemFeature : ScriptableRendererFeature
    {
        #region Water Effects Pass

        class WaterFxPass : ScriptableRenderPass
        {
            private const string k_RenderWaterFXTag = "Render Water FX";
            private ProfilingSampler m_WaterFX_Profile = new ProfilingSampler(k_RenderWaterFXTag);
            private readonly ShaderTagId m_WaterFXShaderTag = new ShaderTagId("WaterFX");
            private readonly Color m_ClearColor = new Color(0.0f, 0.5f, 0.5f, 0.5f);

            private FilteringSettings m_FilteringSettings;
            private RTHandle m_WaterFX;

            public WaterFxPass()
            {
                // Transparent only
                m_FilteringSettings = new FilteringSettings(RenderQueueRange.transparent);
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                // No depth
                cameraTextureDescriptor.depthBufferBits = 0;

                // Half resolution
                cameraTextureDescriptor.width /= 2;
                cameraTextureDescriptor.height /= 2;

                // Default format
                cameraTextureDescriptor.colorFormat = RenderTextureFormat.Default;

                // Alloc RT via RTHandles
                RenderingUtils.ReAllocateIfNeeded(
                    ref m_WaterFX,
                    cameraTextureDescriptor,
                    FilterMode.Bilinear,
                    name: "_WaterFXMap"
                );

                ConfigureTarget(m_WaterFX);
                ConfigureClear(ClearFlag.Color, m_ClearColor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get(k_RenderWaterFXTag);
                using (new ProfilingScope(cmd, m_WaterFX_Profile))
                {
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();

                    // Drawing settings
                    var drawSettings = CreateDrawingSettings(
                        m_WaterFXShaderTag,
                        ref renderingData,
                        SortingCriteria.CommonTransparent
                    );

                    // Draw everything using "WaterFX" pass
                    context.DrawRenderers(
                        renderingData.cullResults,
                        ref drawSettings,
                        ref m_FilteringSettings
                    );
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void OnCameraCleanup(CommandBuffer cmd)
            {
                // Only release for this camera
                if (m_WaterFX != null)
                {
                    RTHandles.Release(m_WaterFX);
                    m_WaterFX = null;
                }
            }
        }

        #endregion


        #region Caustics Pass

        class WaterCausticsPass : ScriptableRenderPass
        {
            private const string k_RenderWaterCausticsTag = "Render Water Caustics";
            private ProfilingSampler m_WaterCaustics_Profile = new ProfilingSampler(k_RenderWaterCausticsTag);

            public Material WaterCausticMaterial;
            private static Mesh m_mesh;

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var cam = renderingData.cameraData.camera;

                // Skip preview
                if (cam.cameraType == CameraType.Preview || !WaterCausticMaterial)
                    return;

                CommandBuffer cmd = CommandBufferPool.Get(k_RenderWaterCausticsTag);

                using (new ProfilingScope(cmd, m_WaterCaustics_Profile))
                {
                    var sunMatrix = RenderSettings.sun != null
                        ? RenderSettings.sun.transform.localToWorldMatrix
                        : Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(-45f, 45f, 0f), Vector3.one);

                    WaterCausticMaterial.SetMatrix("_MainLightDir", sunMatrix);

                    // Create mesh if missing
                    if (!m_mesh)
                        m_mesh = GenerateCausticsMesh(1000f);

                    Vector3 position = cam.transform.position;
                    position.y = 0f;

                    var matrix = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);

                    cmd.DrawMesh(m_mesh, matrix, WaterCausticMaterial, 0, 0);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }

        #endregion



        WaterFxPass m_WaterFxPass;
        WaterCausticsPass m_CausticsPass;

        public WaterSystemSettings settings = new WaterSystemSettings();

        [HideInInspector][SerializeField] private Shader causticShader;
        [HideInInspector][SerializeField] private Texture2D causticTexture;

        private Material _causticMaterial;

        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int Size = Shader.PropertyToID("_Size");
        private static readonly int CausticTexture = Shader.PropertyToID("_CausticMap");

        public override void Create()
        {
            // Pass setup
            m_WaterFxPass = new WaterFxPass
            {
                renderPassEvent = RenderPassEvent.BeforeRenderingOpaques
            };

            m_CausticsPass = new WaterCausticsPass();

            // Load caustic material
            causticShader = causticShader ? causticShader : Shader.Find("Hidden/BoatAttack/Caustics");
            if (causticShader == null)
                return;

            if (_causticMaterial)
                CoreUtils.Destroy(_causticMaterial);

            _causticMaterial = CoreUtils.CreateEngineMaterial(causticShader);
            _causticMaterial.SetFloat("_BlendDistance", settings.causticBlendDistance);

            if (!causticTexture)
            {
#if UNITY_EDITOR
                causticTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(
                    "Packages/com.verasl.water-system/Textures/WaterSurface_single.tif"
                );
#endif
            }

            _causticMaterial.SetTexture(CausticTexture, causticTexture);

            switch (settings.debug)
            {
                case WaterSystemSettings.DebugMode.Caustics:
                    _causticMaterial.SetFloat(SrcBlend, 1f);
                    _causticMaterial.SetFloat(DstBlend, 0f);
                    _causticMaterial.EnableKeyword("_DEBUG");
                    m_CausticsPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
                    break;

                case WaterSystemSettings.DebugMode.Disabled:
                    _causticMaterial.SetFloat(SrcBlend, 2f);
                    _causticMaterial.SetFloat(DstBlend, 0f);
                    _causticMaterial.DisableKeyword("_DEBUG");
                    m_CausticsPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox + 1;
                    break;
            }

            _causticMaterial.SetFloat(Size, settings.causticScale);
            m_CausticsPass.WaterCausticMaterial = _causticMaterial;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(m_WaterFxPass);
            renderer.EnqueuePass(m_CausticsPass);
        }

        private static Mesh GenerateCausticsMesh(float size)
        {
            Mesh m = new Mesh();
            size *= 0.5f;

            Vector3[] verts =
            {
                new Vector3(-size, 0f, -size),
                new Vector3(size, 0f, -size),
                new Vector3(-size, 0f, size),
                new Vector3(size, 0f, size)
            };

            int[] tris =
            {
                0, 2, 1,
                2, 3, 1
            };

            m.vertices = verts;
            m.triangles = tris;

            return m;
        }

        [System.Serializable]
        public class WaterSystemSettings
        {
            [Header("Caustics Settings"), Range(0.1f, 1f)]
            public float causticScale = 0.25f;

            public float causticBlendDistance = 3f;

            [Header("Advanced Settings")]
            public DebugMode debug = DebugMode.Disabled;

            public enum DebugMode
            {
                Disabled,
                WaterEffects,
                Caustics
            }
        }
    }
}
