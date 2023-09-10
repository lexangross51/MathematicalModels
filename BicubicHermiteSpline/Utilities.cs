using BicubicHermiteSpline.Mesh;
using BicubicHermiteSpline.Spline;

namespace BicubicHermiteSpline;

public static class Utilities
{
    public static Mesh.Mesh ReadMesh(string pointsFile, string elementsFile)
    {
        var separators = new char[] { ',', '.' };
        var points = new List<Point2D>();
        var elements = new List<Element>();
        var lines = File.ReadAllLines(pointsFile);
        
        for (var i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains(','))
                lines[i] = lines[i].Replace(',', '.');

            var words = lines[i].Split(separators);
            
            points.Add(new Point2D
            {
                X = double.Parse(words[0]),
                Y = double.Parse(words[1])
            });
        }

        lines = File.ReadAllLines(elementsFile);

        foreach (var line in lines)
        {
            var words = line.Split(separators);
            var nodes = words.Select(int.Parse).ToArray();
            elements.Add(new Element(nodes));
        }

        return new Mesh.Mesh(elements, points);
    }

    public static void ReadPointsAndValues(string dataFile, out Point2D[] points, out double[] values)
    {
        var separators = new char[] { ',', '.' };
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
}