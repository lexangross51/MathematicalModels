using BicubicHermiteSpline;
using BicubicHermiteSpline.Mesh;
using BicubicHermiteSpline.Spline;

// double Function(double x, double y) => x*x*x + y*y*y;
//
// double s = -200;
// double e = 200;
// double n = 4;
// var v = new List<double>();
// double h = (e - s) / n;
// for (int i = 0; i < n + 1; i++) v.Add(s + i * h);
//
// var points = new List<Point2D>();
// foreach (var t in v)
//     foreach (var t1 in v)
//         points.Add(new Point2D(t1, t));
//
// var sw = new StreamWriter("exact");
// foreach (var p in points)
// {
//     sw.WriteLine($"Function({p.X}, {p.Y}) = {Function(p.X, p.Y)}");
// }
// sw.Close();
//
// var practiceData = new List<PracticeData>();
// for (int i = 0; i < points.Count; i++)
// {
//     practiceData.Add(new PracticeData
//     {
//         X = points[i].X,
//         Y = points[i].Y,
//         Value = Function(points[i].X, points[i].Y)
//     });
// }




Utilities.ReadData("HTop.dat", out var points, out var values);
var practiceData = new List<PracticeData>();
var fp = points.First();
for (int i = 0; i < points.Count; i++)
{
    practiceData.Add(new PracticeData
    {
        X = points[i].X - fp.X,
        Y = points[i].Y - fp.Y,
        Value = values[i]
    });
}

var area = Utilities.MakeAreaForData(practiceData, 0);
var meshParameters = new MeshParameters
{
    LeftBottom = area.LeftBottom,
    RightTop = area.RightTop,
    XSplits = 50,
    YSplits = 50
};

var meshBuilder = new MeshBuilder(meshParameters);
var mesh = meshBuilder.Build();
var splineBuilder = new SplineBuilder(mesh, practiceData);
var spline = splineBuilder.Build();
spline.Save(@"C:\Users\lexan\source\repos\SharpPlot\SharpPlot\bin\Release\net7.0-windows");
// spline.SaveInPoints(points);