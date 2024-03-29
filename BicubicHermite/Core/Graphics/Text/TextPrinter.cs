﻿using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using BicubicHermite.Core.Graphics.RenderContext;
using BicubicHermite.Core.Graphics.Shaders;
using BicubicHermite.Core.Graphics.Shaders.Wrappers;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BicubicHermite.Core.Graphics.Text;

public enum TextOrientation : byte
{
    Horizontal,
    Vertical
}

public static class TextPrinter
{
    private static readonly ShaderProgram Shader;
    private static Texture.Texture? _texture;
    private static Font? _font;
    private static readonly SolidBrush Brush;
    private static readonly PointF StartPoint;
    private static readonly float[] TextPosition =
    {
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f,
        0.5f, -0.5f, 0.0f, 1.0f, 0.0f,
        0.5f,  0.5f, 0.0f, 1.0f, 1.0f,
        -0.5f,  0.5f, 0.0f, 0.0f, 1.0f,
    };
    private static readonly uint[] Indices =
    {
        0, 1, 3,
        1, 2, 3
    };

    private static readonly VertexArrayObject Vao;
    private static readonly VertexBufferObject<float> Vbo;

    static TextPrinter()
    {
        StartPoint = new PointF(0, 0);
        Brush = new SolidBrush(Color.Black);
        
        Vao = new VertexArrayObject();
        Vbo = new VertexBufferObject<float>(TextPosition, BufferUsageHint.DynamicDraw);
        _ = new ElementBufferObject(Indices);

        Shader = ShaderCollection.TextShader();
        Shader.Use();
        Shader.SetUniform("model", Matrix4.Identity);
        Shader.SetUniform("view", Matrix4.Identity);
        Shader.SetUniform("projection", Matrix4.Identity);
        Shader.GetAttribLocation("position", out var position);
        Shader.GetAttribLocation("texPosition", out var texPosition);
        Vao.SetAttributePointer(position, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        Vao.SetAttributePointer(texPosition, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
    }

    public static Size TextMeasure(string text, SharpPlotFont font)
        => TextRenderer.MeasureText(text, font.SystemFont);

    public static void DrawText(Viewport2DRenderer renderer, string text, double x, double y, 
        SharpPlotFont font, TextOrientation orientation = TextOrientation.Horizontal)
    {
        _font = font.SystemFont;
        Brush.Color = font.Color;
        
        var camera = renderer.GetCamera();
        var renderSettings = renderer.GetRenderSettings();
        
        var textSize = TextMeasure(text, font);
        Bitmap textImage = new(textSize.Width, textSize.Height);
        
        // Build texture
        using var graphics = System.Drawing.Graphics.FromImage(textImage);
        graphics.Clear(Color.Transparent);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        graphics.DrawString(text, _font, Brush, StartPoint);
        graphics.Dispose();

        if (orientation == TextOrientation.Vertical)
        {
            textImage.RotateFlip(RotateFlipType.Rotate90FlipXY);
        }

        var w = (textImage.Width + 1) / renderSettings.ScreenSize.Width * camera.GetProjection().Width;
        var h = (textImage.Height + 1) / renderSettings.ScreenSize.Height * camera.GetProjection().Height;
        _texture = new Texture.Texture(textImage);
        textImage.Dispose();
        
        TextPosition[0] = (float)x;
        TextPosition[1] = (float)y;
        TextPosition[5] = (float)(x + w);
        TextPosition[6] = (float)y;
        TextPosition[10] = (float)(x + w);
        TextPosition[11] = (float)(y + h);
        TextPosition[15] = (float)x;
        TextPosition[16] = (float)(y + h);

        Vbo.Bind();
        Vbo.UpdateData(TextPosition);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        Vao.Bind();
        _texture.Use();
        Shader.Use();
        Shader.SetUniform("projection", camera.GetProjectionMatrix());
        
        GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        
        Vbo.Unbind();
        Vao.Unbind();
        _texture.Dispose();
    }
}