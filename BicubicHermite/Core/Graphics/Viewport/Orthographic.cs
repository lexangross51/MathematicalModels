﻿using System;
using BicubicHermite.Core.Graphics.Objects.RenderContext;

namespace BicubicHermite.Core.Graphics.Viewport;

public class OrthographicProjection : IProjection
{
    private readonly bool _isEqualScale;
    private double _oldHorizontalCenter, _oldVerticalCenter;
    private double _oldWidth, _oldHeight;
    private double _zCenter;
    private double _zBuffer;
    private double[] _projection;
    private double DHorizontal => Width / 2.0;
    private double DVertical => _isEqualScale ? DHorizontal * Ratio : Height / 2.0;
    public double Ratio { get; set; }
    public double HorizontalCenter { get; set; }
    public double VerticalCenter { get; set; }
    public double Width { get; private set; }
    public double Height { get; private set; }
    public double ZBuffer { get; set; }

    public OrthographicProjection(double[] orthographic, double ratio, bool isEqualScale = false)
    {
        Ratio = ratio;
        _isEqualScale = isEqualScale;
        _projection = orthographic;
        
        SetProjection(orthographic);
    }

    public void SetProjection(double[] projection)
    {
        _projection = projection;
        HorizontalCenter = (projection[0] + projection[1]) * 0.5;
        VerticalCenter = (projection[2] + projection[3]) * 0.5;
        Width = projection[1] - projection[0];
        Height = projection[3] - projection[2];

        _oldHorizontalCenter = HorizontalCenter;
        _oldVerticalCenter = VerticalCenter;
        _oldWidth = Width;
        _oldHeight = Height;

        _zCenter = (projection[4] + projection[5]) * 0.5;
        _zBuffer = (projection[5] - projection[4]) * 0.5;
    }
    public double[] GetProjection()
    {
        _projection[0] = HorizontalCenter - DHorizontal;
        _projection[1] = HorizontalCenter + DHorizontal;
        _projection[2] = VerticalCenter - DVertical;
        _projection[3] = VerticalCenter + DVertical;
        _projection[4] = _zCenter - _zBuffer;
        _projection[5] = _zCenter + _zBuffer;
        
        return _projection;
    }
    
    public void FromProjectionToWorld(double x, double y, RenderSettings renderSettings, out double resX, out double resY)
    {
        var screenSize = renderSettings.ScreenSize;
        var indent = renderSettings.Indent;
        
        var dx = x - (HorizontalCenter - DHorizontal);
        var dy = y - (VerticalCenter - DVertical);
        var coefficient = dx / Width;
        resX = coefficient * (screenSize.Width - indent.Left) + indent.Left + indent.Right;

        coefficient = dy / Height;
        resY = coefficient * (screenSize.Height - indent.Bottom) + indent.Bottom + indent.Top;
    }
    public void FromWorldToProjection(double x, double y, RenderSettings renderSettings, out double resX, out double resY)
    {
        var screenSize = renderSettings.ScreenSize;
        var indent = renderSettings.Indent;
        
        if (x < indent.Left + indent.Right)
        {
            resX = HorizontalCenter - DHorizontal;
        }
        else if (x > screenSize.Width + indent.Left + indent.Right)
        {
            resX = HorizontalCenter + DHorizontal;
        }
        else
        {
            double coefficient = (x - indent.Left - indent.Right) / (screenSize.Width - indent.Left - indent.Right);
            resX = HorizontalCenter + (2 * coefficient - 1) * DHorizontal;
        }

        if (y < 0.0)
        {
            resY = VerticalCenter + DVertical;
        }
        else if (y > screenSize.Height)
        {
            resY = VerticalCenter - DVertical;
        }
        else
        {
            double coefficient = (screenSize.Height - indent.Bottom - y) / (screenSize.Height - indent.Bottom);
            resY = VerticalCenter + (2 * coefficient - 1) * DVertical;
        }
    }
    public void Scale(double x, double y, double delta)
    {
        var scale = delta < 1.05 ? 1.05 : 1.0 / 1.05;
        var left = x + scale * (HorizontalCenter - DHorizontal - x);
        var right = x + scale * (HorizontalCenter - DHorizontal + 2.0 * DHorizontal - x);
        var bottom = y + scale * (VerticalCenter - DVertical - y);
        var top = y + scale * (VerticalCenter - DVertical + 2.0 * DVertical - y);
        var newCenterX = (left + right) / 2.0;
        var newCenterY = (bottom + top) / 2.0;
        var newDHorizontal = newCenterX - left;
        var newDVertical = newCenterY - bottom;

        if (!(Math.Abs(2 * newDHorizontal) >
              Math.Max(Math.Abs(newCenterX - DHorizontal), Math.Abs(newCenterX + DHorizontal)) * 1E-05) ||
            !(Math.Abs(2 * newDVertical) >
              Math.Max(Math.Abs(newCenterY - DVertical), Math.Abs(newCenterY + DVertical)) * 1E-05)) return;
        
        HorizontalCenter = newCenterX;
        VerticalCenter = newCenterY;
        Width = 2.0 * newDHorizontal;
        Height = 2.0 * newDVertical;

        _oldHorizontalCenter = HorizontalCenter;
        _oldVerticalCenter = VerticalCenter;
        _oldWidth = Width;
        _oldHeight = Height;
    }
    public void Translate(double h, double v)
    {
        HorizontalCenter += h;
        VerticalCenter += v;
    }

    public void Reset()
    {
        HorizontalCenter = _oldHorizontalCenter;
        VerticalCenter = _oldVerticalCenter;
        Width = _oldWidth;
        Height = _oldHeight;
    }
}