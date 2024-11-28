﻿using Silk.NET.Maths;
using SkillQuest.API;

namespace SkillQuest.Client.Engine.Graphics.API;

public interface IGraphicsInstance : IDisposable {
    public void Update(DateTime now, TimeSpan delta);
    public void Render(DateTime now, TimeSpan delta);

    public delegate void DoQuit( IApplication application );
    public event DoQuit? Quit;

    public void Draw(RenderPacket packet);

    public ITexture CreateTexture(string imagepath);

    public IModule CreateModule(string vertexPath, string fragmentPath);

    public ISurface CreateSurface(string gltfPath);
}

public class RenderPacket {
    public ITexture Texture { get; set; }

    public ISurface Surface { get; set; }

    public IModule Shader { get; set; }

    public Matrix4X4<float> Model { get; set; } = Matrix4X4<float>.Identity;

    public Matrix4X4<float> View { get; set; } = Matrix4X4<float>.Identity;

    public Matrix4X4<float> Projection { get; set; } = Matrix4X4<float>.Identity;
}

public interface IModule : IDisposable{
    public void Bind();
    public void Unbind();
    public void Attribute(String name, int value);
    public void Attribute(String name, float value);
    public void Attribute(String name, bool value);
    public void Attribute(String name, Vector2D<int> value);
    public void Attribute(String name, Vector2D<float> value);
    public void Attribute(String name, Vector3D<int> value);
    public void Attribute(String name, Vector3D<float> value);
    public void Attribute(String name, Vector4D<int> value);
    public void Attribute(String name, Vector4D<float> value);
    public void Attribute(String name, Matrix3X3<float> value);
    public void Attribute(String name, Matrix4X4<float> value);
}

public interface ITexture : IDisposable {
    void Bind();
    void Unbind();
}

public interface ISurface : IDisposable{
    /// <summary>
    /// TODO: Come up with some parameters
    /// </summary>
    public void Draw();
}
