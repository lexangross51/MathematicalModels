namespace BicubicHermiteSpline.Mesh;

public struct MeshParameters
{
    public Point2D LeftBottom { get; init; }
    public Point2D RightTop { get; init; }
    public int XSplits { get; init; }
    public int YSplits { get; init; }
}