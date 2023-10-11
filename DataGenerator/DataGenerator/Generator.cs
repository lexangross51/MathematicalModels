namespace DataGenerator;

public static class Generator
{
    public static List<(double X, double Y)> GeneratePoints(double x0, double x1, double y0, double y1, int nx, int ny)
    {
        var points = new List<(double X, double Y)>();

        var hx = (x1 - x0) / (nx - 1);
        var hy = (y1 - y0) / (ny - 1);

        for (int i = 0; i < ny; i++)
        {
            for (int j = 0; j < nx; j++)
            {
                points.Add((x0 + j * hx, y0 + i * hy));
            }
        }
        
        return points;
    }

    public static void WriteData(string filename, List<(double X, double Y)> points, Func<double, double, double> f)
    {
        var sw = new StreamWriter(filename);
        
        foreach (var p in points)
        {
            sw.WriteLine($"{p.X} {p.Y} {f(p.X, p.Y)}");
        }

        sw.Close();
    }
}