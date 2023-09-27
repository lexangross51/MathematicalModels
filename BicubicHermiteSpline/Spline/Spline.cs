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

    public abstract double ValueAtPoint(double x, double y);

    public abstract void Save(string folderName = ".", string filename = "spline");
}

public class HermiteBicubicSpline : Spline
{
    public HermiteBicubicSpline(Mesh.Mesh mesh, double[] values) : base(mesh, values)
    {
    }

    public override double ValueAtPoint(double x, double y)
    {
        int elem = Mesh.FindElementByPoint(x, y);

        if (elem == -1) return double.MinValue;

        var functions = HermiteBasis2D.GetBasisForElement(Mesh, elem);
        var nodes = Mesh.Elements[elem].Nodes;
        var p1 = Mesh.Points[nodes.First()];
        var p2 = Mesh.Points[nodes.Last()];
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

    public override void Save(string folderName = ".", string filename = "spline")
    {
        if (!Directory.Exists(folderName))
            throw new Exception($"Folder {folderName} does not exists");

        double minX = Mesh.Points.Min(p => p.X);
        double maxX = Mesh.Points.Max(p => p.X);
        double minY = Mesh.Points.Min(p => p.Y);
        double maxY = Mesh.Points.Max(p => p.Y);

        int nx = Mesh.AbscissaPointsCount * 4;
        int ny = Mesh.OrdinatePointsCount * 4;

        double hx = (maxX - minX) / (nx - 1);
        double hy = (maxY - minY) / (ny - 1);
    
        var sw = new StreamWriter($"{folderName}/{filename}");

        for (int i = 0; i < ny; i++)
        {
            for (int j = 0; j < nx; j++)
            {
                double x = minX + j * hx;
                double y = minY + i * hy;
                double v = ValueAtPoint(x, y);
                
                if (Math.Abs(v - double.MinValue) < 1E-14) continue;
                
                sw.WriteLine($"{x} {y} {v}");
            }
        }
    
        sw.Close();
    }
}