namespace BicubicHermiteSpline.Mesh;

public class Element
{
    public int[] Nodes { get; }

    public Element(int[] nodes)
        => Nodes = nodes;
}