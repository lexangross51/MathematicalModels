namespace BicubicHermite.Core.SplineCalculator.Mathematics;

public static class HermiteBasis2D
{
    public static int BasisSize => HermiteBasis1D.BasisSize * HermiteBasis1D.BasisSize;

    public static double Phi(int func, double x, double y, double hx, double hy)
    {
        int i = 2 * (func / 4 % 2) + func % 2;
        int j = 2 * (func / 8) + func / 2 % 2;

        return HermiteBasis1D.Phi(i, x, hx) * HermiteBasis1D.Phi(j, y, hy);
    }

    public static int[][] MakeBasis(Graphics.Objects.Mesh.Mesh mesh)
    {
        var basisInfo = new int[mesh.Elements.Length][];
        int nx = mesh.Elements[0].Nodes[2];

        for (int ielem = 0; ielem < mesh.Elements.Length; ielem++)
        {
            basisInfo[ielem] = new int[BasisSize];
            
            int node = 4 * nx * (ielem / (nx - 1)) + 4 * (ielem % (nx - 1));

            for (int i = 0; i < BasisSize; i++)
            {
                basisInfo[ielem][i] = node + i / 8 * nx * 4 + i % 8;
            }
        }
        
        return basisInfo;
    }

    public static int[] GetBasisForElement(Graphics.Objects.Mesh.Mesh mesh, int ielem)
    {
        var basisInfo = new int[BasisSize];
        int nx = mesh.Elements[0].Nodes[2];
        int node = 4 * nx * (ielem / (nx - 1)) + 4 * (ielem % (nx - 1));

        for (int i = 0; i < BasisSize; i++)
        {
            basisInfo[i] = node + i / 8 * nx * 4 + i % 8;
        }

        return basisInfo;
    }
}