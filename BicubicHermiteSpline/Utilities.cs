using System.Globalization;
using BicubicHermiteSpline.Mesh;
using BicubicHermiteSpline.Spline;

namespace BicubicHermiteSpline;

public static class Utilities
{
    public static Mesh.Mesh ReadMesh(string pointsFile, string elementsFile)
    {
        var separators = new[] { ',', '\t', '\n', };
        var points = new List<Point2D>();
        var elements = new List<Element>();
        var lines = File.ReadAllLines(pointsFile);
        
        for (var i = 1; i < lines.Length; i++)
        {
            if (lines[i].Contains(','))
                lines[i] = lines[i].Replace(',', '.');

            var words = lines[i].Split(separators);
            
            points.Add(new Point2D
            {
                X = double.Parse(words[0], CultureInfo.InvariantCulture),
                Y = double.Parse(words[1], CultureInfo.InvariantCulture)
            });
        }

        lines = File.ReadAllLines(elementsFile);

        for (var i = 1; i < lines.Length; )
        {
            var line = lines[i];
            var words = line.Split(separators);
            var nodes = words.Where(node => node != "").Select(int.Parse).ToArray();
            elements.Add(new Element(nodes));
            i += 3;
        }

        return new Mesh.Mesh(elements, points);
    }

    public static void ReadPointsAndValues(string dataFile, out Point2D[] points, out double[] values)
    {
        var separators = new[] { ',', '.' };
        var pointsList = new List<Point2D>();
        var valuesList = new List<double>();
        var lines = File.ReadAllLines(dataFile);

        for (var i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains(','))
                lines[i] = lines[i].Replace(',', '.');

            var words = lines[i].Split(separators);

            pointsList.Add(new Point2D
            {
                X = double.Parse(words[0]),
                Y = double.Parse(words[1])
            });

            valuesList.Add(double.Parse(words[2]));
        }

        points = pointsList.ToArray();
        values = valuesList.ToArray();
    }

    // Shift in percents
    public static (Point2D LeftBottom, Point2D RightTop) MakeAreaForData(IEnumerable<PracticeData> dataCollection, double shift = 20)
    {
        var data = dataCollection.ToArray();
        var minX = data.MinBy(d => d.X).X;
        var minY = data.MinBy(d => d.Y).Y;
        var maxX = data.MaxBy(d => d.X).X;
        var maxY = data.MaxBy(d => d.Y).Y;
        var dx = maxX - minX;
        var dy = maxY - minY;
        
        double alpha = shift / 100.0;

        minX -= alpha * dx;
        maxX += alpha * dx;
        minY -= alpha * dy;
        maxY += alpha * dy;

        return (new Point2D(minX, minY), new Point2D(maxX, maxY));
    }

    public static void ReadData(string filename, out List<Point2D> points, out List<double> values)
    {
        var separators = new[] { ',', '\t', '\n', ' ' };
        points = new List<Point2D>();
        values = new List<double>();

        var lines = File.ReadAllLines(filename);

        foreach (var line in lines)
        {
            var lLine = line.Replace(',', '.');
            var words = lLine.Split(separators).Select(val => double.Parse(val, CultureInfo.InvariantCulture)).ToArray();
            points.Add(new Point2D
            {
                X = words[0],
                Y = words[1],
            });
            values.Add(words[2]);
        }
    }
}