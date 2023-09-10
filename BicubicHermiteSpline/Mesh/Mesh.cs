namespace BicubicHermiteSpline.Mesh;

public class Mesh
{
    public Element[] Elements { get; }
    public Point2D[] Points { get; }
    
    public int AbscissaPointsCount { get; }
    public int OrdinatePointsCount { get; }

    public Mesh(IEnumerable<Element> elements, IEnumerable<Point2D> points)
    {
        (Elements, Points) = (elements.ToArray(), points.ToArray());
        AbscissaPointsCount = Points.DistinctBy(p => p.X).Count();
        OrdinatePointsCount = Points.DistinctBy(p => p.Y).Count();
    }

    public int FindElementByPoint(double x, double y)
    {
        for (var ielem = 0; ielem < Elements.Length; ielem++)
        {
            var element = Elements[ielem];
            var p1 = Points[element.Nodes.First()];
            var p2 = Points[element.Nodes.Last()];

            if (p1.X <= x && x <= p2.X && p1.Y <= y && y <= p2.Y)
            {
                return ielem;
            }
        }

        return -1;
    }
    
    public void Save(string folderName = ".")
    {
        StreamWriter sw;
        
        if (!Directory.Exists(folderName))
            throw new Exception($"Directory {folderName} does not exists");
    
        var elementsPath = $"{folderName}/elements";
        var pointsPath = $"{folderName}/points";
        
        if (Elements.First().Nodes.Length > 4)
        {
            sw = new StreamWriter(elementsPath);
            foreach (var element in Elements)
            {
                sw.WriteLine(
                    $"{element.Nodes[0]} {element.Nodes[3]} {element.Nodes[12]} {element.Nodes[15]}");
            }
            sw.Close();
        }
        else
        {
            sw = new StreamWriter(elementsPath);
            foreach (var element in Elements)
            {
                sw.WriteLine(
                    $"{element.Nodes[0]} {element.Nodes[1]} {element.Nodes[2]} {element.Nodes[3]}");
            }
            sw.Close();
        }
        
        sw = new StreamWriter(pointsPath);
        foreach (var point in Points)
        {
            sw.WriteLine($"{point.X} {point.Y}");
        }
        sw.Close();   
    }
}