﻿using BicubicHermite.Core.Graphics.Viewport;
using OpenTK.Mathematics;

namespace BicubicHermite.Core.Graphics.Camera;

public abstract class AbstractCamera
{
    protected readonly IProjection Projection;

    protected AbstractCamera(IProjection projection) => Projection = projection;

    public abstract Matrix4 GetModelMatrix();
    public abstract Matrix4 GetViewMatrix();
    public abstract Matrix4 GetProjectionMatrix();
    public IProjection GetProjection() => Projection;

    public abstract void Zoom(double xPivot, double yPivot, double delta);
    public abstract void Move(double dx, double dy);
}