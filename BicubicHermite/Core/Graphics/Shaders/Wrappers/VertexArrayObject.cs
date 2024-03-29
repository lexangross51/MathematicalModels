﻿using System;
using OpenTK.Graphics.OpenGL4;

namespace BicubicHermite.Core.Graphics.Shaders.Wrappers;

public class VertexArrayObject : IBindable, IDisposable
{
    private readonly int _handle;

    public VertexArrayObject()
    {
        _handle = GL.GenVertexArray();
        GL.BindVertexArray(_handle);
    }

    public void Bind() => GL.BindVertexArray(_handle);

    public void Unbind() => GL.BindVertexArray(0);

    public void SetAttributePointer(
        int location, 
        int size, 
        VertexAttribPointerType type, 
        bool normalize, 
        int stride, 
        int offset)
    {
        GL.EnableVertexAttribArray(location);
        GL.VertexAttribPointer(location, size, type, normalize, stride, offset);
    }
    
    public void Dispose() => GL.DeleteVertexArray(_handle);
}