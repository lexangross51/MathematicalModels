using BicubicHermiteSpline.Mathematics;

namespace BicubicHermiteSpline.Spline;

public class SplineBuilder
{
    private readonly Mesh.Mesh _mesh;
    private readonly PracticeData[] _data;
    private readonly SparseMatrix _matrix;
    private readonly double[] _vector;
    private double[] _solution;
    private readonly double _weight = 1000.0;

    public SplineBuilder(Mesh.Mesh mesh, IEnumerable<PracticeData> data)
    {
        _data = data.ToArray();
        _mesh = mesh;

        PortraitBuilder.PortraitByNodes(_mesh.Elements, _mesh.Points, out var ig, out var jg);
        
        _matrix = new SparseMatrix(ig, jg);
        _vector = new double[_mesh.Points.Length];
        _solution = new double[_mesh.Points.Length];
    }
    
    public HermiteBicubicSpline Build()
    {
        var localMatrix = new Matrix<double>(HermiteBasis2D.BasisSize, HermiteBasis2D.BasisSize);
        var localVector = new double[HermiteBasis2D.BasisSize];

        for (var ielem = 0; ielem < _mesh.Elements.Length; ielem++)
        {
            var element = _mesh.Elements[ielem];
            var nodes = element.Nodes;
            var p1 = _mesh.Points[nodes.First()];
            var p2 = _mesh.Points[nodes.Last()];
            var hx = p2.X - p1.X;
            var hy = p2.Y - p1.Y;

            Array.Fill(localVector, 0.0);
            localMatrix.Fill(0.0);

            foreach (var d in _data)
            {
                var jelem = _mesh.FindElementByPoint(d.X, d.Y);
                if (jelem == -1) continue;
                if (jelem != ielem) continue;

                double ksi = (d.X - p1.X) / hx;
                double eta = (d.Y - p1.Y) / hy;

                for (int i = 0; i < HermiteBasis2D.BasisSize; i++)
                {
                    double phiI = HermiteBasis2D.Phi(i, ksi, eta, hx, hy);

                    for (int j = 0; j < HermiteBasis2D.BasisSize; j++)
                    {
                        localMatrix[i, j] += _weight * phiI * HermiteBasis2D.Phi(j, ksi, eta, hx, hy);
                    }

                    localVector[i] += _weight * d.Value * phiI;
                }
            }

            var stiffnessMatrix = BicubicHermiteShapes.MakeStiffnessMatrix2D(hx, hy);

            for (int i = 0; i < HermiteBasis2D.BasisSize; i++)
            {
                for (int j = 0; j < HermiteBasis2D.BasisSize; j++)
                {
                    _matrix.Add(nodes[i], nodes[j], localMatrix[i, j] + 1E-07 * stiffnessMatrix[i, j]);
                }

                _vector[nodes[i]] += localVector[i];
            }
        }

        var solver = new LOS(100, 1E-20);
        solver.SetSystem(_matrix, _vector);
        solver.Compute();
        _solution = solver.Solution!;

        return new HermiteBicubicSpline(_mesh, _solution);
    }
}