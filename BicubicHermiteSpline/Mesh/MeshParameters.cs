namespace BicubicHermiteSpline.Mesh;

public struct MeshParameters
{
    public Point2D LeftBottom { get; set; }
    public Point2D RightTop { get; set; }
    public int XSplits { get; set; }
    public int YSplits { get; set; }
    public int Refinement { get; set; }
}