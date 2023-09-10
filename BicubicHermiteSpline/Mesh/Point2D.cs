namespace BicubicHermiteSpline.Mesh;

public struct Point2D
{
    public double X { get; set; }
    public double Y { get; set; }

    public Point2D(double x, double y)
        => (X, Y) = (x, y);
}