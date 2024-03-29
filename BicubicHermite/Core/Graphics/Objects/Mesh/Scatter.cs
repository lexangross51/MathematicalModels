﻿using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BicubicHermite.Core.Graphics.Objects.Mesh;

public class Scatter : IBaseObject
{
    public PrimitiveType ObjectType { get; }
    public int PointSize { get; }
    public Point[] Points { get; }
    public Color4[] Colors { get; }
    public uint[]? Indices { get; }

    public Scatter(IEnumerable<Point> pointCollection, int size)
    {
        ObjectType = PrimitiveType.Points;
        PointSize = size;
        Points = pointCollection.ToArray();
        Colors = new[] { Color4.Black };
        Indices = null;
    }
    
    public void BoundingBox(out Point leftBottom, out Point rightTop)
    {
        var minX = Points.MinBy(p => p.X).X;
        var maxX = Points.MaxBy(p => p.X).X;
        var minY = Points.MinBy(p => p.Y).Y;
        var maxY = Points.MaxBy(p => p.Y).Y;

        leftBottom = new Point(minX, minY);
        rightTop = new Point(maxX, maxY);
    }
}

