namespace BicubicHermiteSpline.Mesh;

public class MeshBuilder
{
    private MeshParameters _parameters;

    public MeshBuilder(MeshParameters parameters)
    {
        _parameters = parameters;

        PrepareRefinement();
    }

    private void PrepareRefinement()
    {
        if (_parameters.Refinement == 0) return;

        _parameters.XSplits *= (int)Math.Pow(2, _parameters.Refinement);
        _parameters.YSplits *= (int)Math.Pow(2, _parameters.Refinement);
    }
    
    public Mesh Build()
    {
        double x0 = _parameters.LeftBottom.X;
        double y0 = _parameters.LeftBottom.Y;
        double x1 = _parameters.RightTop.X;
        double y1 = _parameters.RightTop.Y;
        int nx = _parameters.XSplits;
        int ny = _parameters.YSplits;
        double hx = (x1 - x0) / nx;
        double hy = (y1 - y0) / ny;

        int pointsCount = (nx + 1) * (ny + 1);
        int elementsCount = nx * ny;

        var points = new Point2D[pointsCount];
        var elements = new Element[elementsCount];

        for (int i = 0; i < ny + 1; i++)
        {
            for (int j = 0; j < nx + 1; j++)
            {
                double x = x0 + j * hx;
                double y = y0 + i * hy;

                points[i * (nx + 1) + j] = new Point2D(x, y);
            }
        }

        for (int i = 0; i < ny; i++)
        {
            for (int j = 0; j < nx; j++)
            {
                var nodes = new[]
                {
                    i * (nx + 1) + j,
                    i * (nx + 1) + j + 1,
                    i * (nx + 1) + j + nx + 1,
                    i * (nx + 1) + j + nx + 2,
                };

                elements[i * nx + j] = new Element(nodes);
            }
        }

        return new Mesh(elements, points);
    }
}