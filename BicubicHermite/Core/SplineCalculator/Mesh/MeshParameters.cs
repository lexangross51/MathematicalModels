using BicubicHermite.Core.Graphics.Objects;

namespace BicubicHermite.Core.SplineCalculator.Mesh;

public struct MeshParameters
{
    public Point LeftBottom { get; set; }
    public Point RightTop { get; set; }
    public int XSplits { get; set; }
    public int YSplits { get; set; }
    public int Refinement { get; set; }
}