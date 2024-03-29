﻿using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace BicubicHermite.Core.Graphics.Shaders.Wrappers;

public class VertexBufferObject<T> : IBindable, IDisposable where T : struct
{
    private readonly int _handle;
    
    public VertexBufferObject(T[] data, BufferUsageHint usageHint = BufferUsageHint.StaticDraw)
    {
        _handle = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _handle);
        GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Marshal.SizeOf<T>(), data, usageHint);
    }

    public void Bind() => GL.BindBuffer(BufferTarget.ArrayBuffer, _handle);

    public void Unbind() => GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

    public void UpdateData(T[] newData)
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _handle);
        GL.BufferSubData(BufferTarget.ArrayBuffer,0, newData.Length * Marshal.SizeOf<T>(), newData);
    }

    public void Dispose() => GL.DeleteBuffer(_handle);
}