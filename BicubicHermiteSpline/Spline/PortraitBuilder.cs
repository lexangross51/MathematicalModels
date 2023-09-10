using BicubicHermiteSpline.Mesh;

namespace BicubicHermiteSpline.Spline;

public static class PortraitBuilder
{
    public static void PortraitByNodes(IEnumerable<Element> elementsCollection, IEnumerable<Point2D> pointsCollection, 
        out int[] ig, out int[] jg)
    {
        var points = pointsCollection.ToArray();
        var elements = elementsCollection.ToArray();
        
        var connectivityList = new List<SortedSet<int>>();

        for (int i = 0; i < points.Length; i++)
        {
            connectivityList.Add(new SortedSet<int>());
        }

        int localSize = elements[0].Nodes.Length;

        foreach (var element in elements)
        {
            for (int i = 0; i < localSize; i++)
            {
                int nodeToInsert = element.Nodes[i];

                for (int j = 0; j < localSize; j++)
                {
                    int posToInsert = element.Nodes[j];

                    if (nodeToInsert < posToInsert)
                    {
                        connectivityList[posToInsert].Add(nodeToInsert);
                    }
                }
            }
        }

        ig = new int[connectivityList.Count + 1];

        ig[0] = 0;
        ig[1] = 0;

        for (int i = 1; i < connectivityList.Count; i++)
        {
            ig[i + 1] = ig[i] + connectivityList[i].Count;
        }

        jg = new int[ig[^1]];

        for (int i = 1, j = 0; i < connectivityList.Count; i++)
        {
            foreach (var it in connectivityList[i])
            {
                jg[j++] = it;
            }
        }
    }
}