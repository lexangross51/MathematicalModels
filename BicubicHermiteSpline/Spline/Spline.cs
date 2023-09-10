using BicubicHermiteSpline.Mathematics;
using BicubicHermiteSpline.Mesh;

namespace BicubicHermiteSpline.Spline;

public abstract class Spline
{
    protected readonly Mesh.Mesh Mesh;
    protected readonly double[] Values;

    protected Spline(Mesh.Mesh mesh, double[] values)
    {
        Mesh = mesh;
        Values = values;
    }

    public abstract double ValueAtPoint(Point2D point);

    public abstract void Save(string folderName = ".", string filename = "spline");
}

public class HermiteBicubicSpline : Spline
{
    public HermiteBicubicSpline(Mesh.Mesh mesh, double[] values) : base(mesh, values)
    {
    }

    public override double ValueAtPoint(Point2D point)
    {
        int elem = Mesh.FindElementByPoint(point.X, point.Y);
        
        if (elem == -1) throw new Exception($"Cannot find element");
    
        var nodes = Mesh.Elements[elem].Nodes;
        var p1 = Mesh.Points[nodes.First()];
        var p2 = Mesh.Points[nodes.Last()];
        double hx = p2.X - p1.X;
        double hy = p2.Y - p1.Y;

        var ksi = (point.X - p1.X) / hx;
        var eta = (point.Y - p1.Y) / hy;
        
        double sum = 0.0;
        for (int i = 0; i < HermiteBasis2D.BasisSize; i++)
        {
            sum += Values[nodes[i]] * HermiteBasis2D.Phi(i, ksi, eta, hx, hy);
        }
    
        return sum;
    }

    public override void Save(string folderName = ".", string filename = "spline")
    {
        if (!Directory.Exists(folderName))
            throw new Exception($"Folder {folderName} does not exists");
    
        var sw = new StreamWriter($"{folderName}/{filename}");
        
        foreach (var p in Mesh.Points)
        {
            double x = p.X;
            double y = p.Y;
            double v = ValueAtPoint(p);
    
            sw.WriteLine($"{x} {y} {v}");
        }
    
        sw.Close();
    }
}