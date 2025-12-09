using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineRenderFeature : ScriptableRendererFeature
{
    class OutlinePass : ScriptableRenderPass
    {
        private Material outlineMaterial;

        public OutlinePass(Material mat)
        {
            outlineMaterial = mat;
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (outlineMaterial == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("Outline Pass");
            cmd.Blit(null, BuiltinRenderTextureType.CameraTarget, outlineMaterial);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    public Material outlineMaterial;
    OutlinePass pass;

    public override void Create()
    {
        pass = new OutlinePass(outlineMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}
