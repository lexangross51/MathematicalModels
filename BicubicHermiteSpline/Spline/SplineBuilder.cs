using BicubicHermiteSpline.Mathematics;

namespace BicubicHermiteSpline.Spline;

public class SplineBuilder
{
    private readonly List<PracticeData>[] _dataInside;
    private readonly Mesh.Mesh _mesh;
    private readonly SparseMatrix _matrix;
    private readonly double[] _vector;
    private double[] _solution;
    private double _weight = 1.0;
    private double _alpha = 1E-06;
    private readonly int[][] _basisInfo;

    public SplineBuilder(Mesh.Mesh mesh, IEnumerable<PracticeData> data)
    { 
        _mesh = mesh;
        _basisInfo = HermiteBasis2D.MakeBasis(mesh);
        
        PortraitBuilder.PortraitByNodes(_basisInfo, out var ig, out var jg);
        
        _matrix = new SparseMatrix(ig, jg);
        _vector = new double[ig.Length - 1];
        _solution = new double[ig.Length - 1];
        _dataInside = new List<PracticeData>[_mesh.Elements.Length].Select(_ => new List<PracticeData>()).ToArray();
        
        DistributeData(data);
    }

    private void DistributeData(IEnumerable<PracticeData> dataCollection)
    {
        var data = dataCollection.ToArray();
        var usedPoints = new bool[data.Length];

        for (var i = 0; i < data.Length; i++)
        {
            if (usedPoints[i]) continue;
            
            var d = data[i];
            var ielem = _mesh.FindElementByPoint(d.X, d.Y);
            if (ielem == -1) continue;

            _dataInside[ielem].Add(d);
            usedPoints[i] = true;
        }
    }
    
    public HermiteBicubicSpline Build()
    {
        var localMatrix = new Matrix<double>(HermiteBasis2D.BasisSize, HermiteBasis2D.BasisSize);
        var localVector = new double[HermiteBasis2D.BasisSize];

        for (var ielem = 0; ielem < _mesh.Elements.Length; ielem++)
        {
            var element = _mesh.Elements[ielem];
            var nodes = element.Nodes;
            var p1 = _mesh.Points[nodes[0]];
            var p2 = _mesh.Points[nodes[^1]];
            var hx = p2.X - p1.X;
            var hy = p2.Y - p1.Y;
            var functions = _basisInfo[ielem];

            Array.Fill(localVector, 0.0);
            localMatrix.Fill(0.0);

            foreach (var d in _dataInside[ielem])
            {
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
                    _matrix.Add(functions[i], functions[j], localMatrix[i, j] + _alpha * stiffnessMatrix[i, j]);
                }

                _vector[functions[i]] += localVector[i];
            }
        }
        
        var solver = new CGMCholesky(7000, 1E-15);
        solver.SetSystem(_matrix, _vector);
        solver.Compute();
        _solution = solver.Solution!;

        return new HermiteBicubicSpline(_mesh, _solution);
    }
}