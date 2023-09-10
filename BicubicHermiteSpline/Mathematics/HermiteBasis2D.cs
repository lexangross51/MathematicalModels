namespace BicubicHermiteSpline.Mathematics;

public static class HermiteBasis2D
{
    public static int BasisSize => HermiteBasis1D.BasisSize * HermiteBasis1D.BasisSize;

    public static double Phi(int func, double x, double y, double hx, double hy)
    {
        int i = 2 * (func / 4 % 2) + func % 2;
        int j = 2 * (func / 8) + func / 2 % 2;

        return HermiteBasis1D.Phi(i, x, hx) * HermiteBasis1D.Phi(j, y, hy);
    }
}