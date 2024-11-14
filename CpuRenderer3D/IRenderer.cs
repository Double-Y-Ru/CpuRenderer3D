﻿namespace CpuRenderer3D
{
    public interface IRenderer
    {
        void Render(Entity entity, RenderingContext shaderContext, ShaderProgram shaderProgram);
    }
}