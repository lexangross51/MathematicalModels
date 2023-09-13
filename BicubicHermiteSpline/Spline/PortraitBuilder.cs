using BicubicHermiteSpline.Mesh;

namespace BicubicHermiteSpline.Spline;

public static class PortraitBuilder
{
    public static void PortraitByNodes(int[][] basisInfo, out int[] ig, out int[] jg)
    {
        var funcCount = basisInfo[^1][^1] + 1;
        var connectivityList = new List<SortedSet<int>>();

        for (int i = 0; i < funcCount; i++)
        {
            connectivityList.Add(new SortedSet<int>());
        }

        foreach (var nodes in basisInfo)
        {
            foreach (var nodeToInsert in nodes)
            {
                foreach (var posToInsert in nodes)
                {
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