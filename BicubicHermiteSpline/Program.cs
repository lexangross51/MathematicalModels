using BicubicHermiteSpline;
using BicubicHermiteSpline.Mesh;
using BicubicHermiteSpline.Spline;

double Function(double x, double y) => x * x * x + y * y * y;

var xArray = new[] { 0.0, 1.0 / 3.0, 2.0 / 3.0, 1.0 };

var points = new Point2D[16];
int index = 0;
foreach (var x1 in xArray)
{
    foreach (var x2 in xArray)
    {
        points[index++] = new Point2D(x2, x1);
    }
}

var practiceData = points.Select(p => new PracticeData { X = p.X, Y = p.Y, Value = Function(p.X, p.Y) }).ToList();

// var random = new Random();
//
// for (int i = 0; i < 16; i++)
// {
//     double x = random.NextDouble() * 20.0 - 10.0;
//     double y = random.NextDouble() * 20.0 - 10.0;
//     double v = Function(x, y);
//     
//     practiceData.Add(new PracticeData
//     {
//         X = x,
//         Y = y,
//         Value = v
//     });
// }

var area = Utilities.MakeAreaForData(practiceData, 0);
var meshParameters = new MeshParameters
{
    LeftBottom = area.LeftBottom,
    RightTop = area.RightTop,
    XSplits = 100,
    YSplits = 100
};
var meshBuilder = new MeshBuilder(meshParameters);
var mesh = meshBuilder.Build();
var newMesh = MeshTransformer.AddPointsForBicubicBasis(mesh);
var splineBuilder = new SplineBuilder(newMesh, practiceData);
var spline = splineBuilder.Build();

foreach (var p in newMesh.Points)
{
    Console.WriteLine($"Spline({p.X}, {p.Y}) = {Function(p.X, p.Y)}");
}


spline.Save(@"C:\Users\lexan\source\repos\SharpPlot\SharpPlot\bin\Release\net7.0-windows");