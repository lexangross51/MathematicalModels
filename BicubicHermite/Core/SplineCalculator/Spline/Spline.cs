using System;
using System.Collections.Generic;
using System.Linq;
using BicubicHermite.Core.Graphics.Objects;
using BicubicHermite.Core.SplineCalculator.Mathematics;
using BicubicHermiteSpline.Spline;

namespace BicubicHermite.Core.SplineCalculator.Spline;

public abstract class Spline
{
    protected readonly Graphics.Objects.Mesh.Mesh Mesh;
    protected readonly double[] Values;

    protected Spline(Graphics.Objects.Mesh.Mesh mesh, double[] values)
    {
        Mesh = mesh;
        Values = values;
    }

    public abstract double ValueAtPoint(double x, double y);

    public abstract IEnumerable<double> CalculateAtPoints(IEnumerable<Point> pointsCollection);

    public abstract double CalculateResidual(IEnumerable<PracticeData> dataCollection);
}

public class HermiteBicubicSpline : Spline
{
    public HermiteBicubicSpline(Graphics.Objects.Mesh.Mesh mesh, double[] values) : base(mesh, values)
    {
    }

    public override double ValueAtPoint(double x, double y)
    {
        int elem = Mesh.FindElementByPoint(x, y);

        if (elem == -1) return double.MinValue;

        var functions = HermiteBasis2D.GetBasisForElement(Mesh, elem);
        var nodes = Mesh.Elements[elem].Nodes;
        var p1 = Mesh.Points[nodes[0]];
        var p2 = Mesh.Points[nodes[^1]];
        double hx = p2.X - p1.X;
        double hy = p2.Y - p1.Y;

        var ksi = (x - p1.X) / hx;
        var eta = (y - p1.Y) / hy;
        
        double sum = 0.0;
        for (int i = 0; i < HermiteBasis2D.BasisSize; i++)
        {
            sum += Values[functions[i]] * HermiteBasis2D.Phi(i, ksi, eta, hx, hy);
        }
    
        return sum;
    }

    public override IEnumerable<double> CalculateAtPoints(IEnumerable<Point> pointsCollection)
    {
        var points = pointsCollection as Point[] ?? pointsCollection.ToArray();
        var values = points.Select(p => ValueAtPoint(p.X, p.Y))
            .Where(v => Math.Abs(v - double.MinValue) > 1E-14)
            .ToList();

        return values;
    }

    public override double CalculateResidual(IEnumerable<PracticeData> dataCollection)
    {
        double sqrMean = 0.0;
        var data = dataCollection.ToArray();
        
        foreach (var d in data)
        {
            var splineValue = ValueAtPoint(d.X, d.Y);
            sqrMean += (d.Value - splineValue) * (d.Value - splineValue);
        }

        return Math.Sqrt(sqrMean / data.Length);
    }
}