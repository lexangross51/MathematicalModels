namespace BicubicHermiteSpline.Mesh;

public static class MeshTransformer
{
    public static Mesh AddPointsForBicubicBasis(Mesh mesh)
    {
        int nx = mesh.AbscissaPointsCount;
        int ny = mesh.OrdinatePointsCount;
        int newPointsCount = (3 * nx - 2) * (3 * ny - 2);

        var newElements = new List<Element>();
        var newPoints = new Point2D[newPointsCount];

        var x = new double[4];
        var y = new double[4];

        for (int ielem = 0; ielem < mesh.Elements.Length; ielem++)
        {
            var nodes = new int[16];
            
            x[0] = mesh.Points[mesh.Elements[ielem].Nodes.First()].X;
            x[3] = mesh.Points[mesh.Elements[ielem].Nodes.Last()].X;
            x[1] = x[0] + (x[3] - x[0]) / 3.0;
            x[2] = x[0] + (x[3] - x[0]) * 2.0 / 3.0;
            
            y[0] = mesh.Points[mesh.Elements[ielem].Nodes.First()].Y;
            y[3] = mesh.Points[mesh.Elements[ielem].Nodes.Last()].Y;
            y[1] = y[0] + (y[3] - y[0]) / 3.0;
            y[2] = y[0] + (y[3] - y[0]) * 2.0 / 3.0;
            
            int node = 3 * (ielem / (nx - 1)) * (3 * nx - 2) + 3 * (ielem % (nx - 1));

            for (int i = 0; i < 4; i++)
                nodes[i] = node + i;

            for (int i = 4, j = 0; i < 8; i++, j++)
                nodes[i] = node + 3 * nx - 2 + j;

            for (int i = 8, j = 0; i < 12; i++, j++)
                nodes[i] = node + 6 * nx - 4 + j;

            for (int i = 12, j = 0; i < 16; i++, j++)
                nodes[i] = node + 9 * nx - 6 + j;
            
            for (int i = 0, index = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    newPoints[nodes[index++]] = new Point2D(x[j], y[i]);
                }
            }
            
            newElements.Add(new Element(nodes));
        }

        return new Mesh(newElements, newPoints);
    }
}