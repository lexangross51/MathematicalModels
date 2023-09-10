namespace BicubicHermiteSpline.Mathematics;

public static class HermiteBasis1D
{
    public static int BasisSize => 4;
    public static double Phi(int func, double x, double h)
    {
        return func switch
        {
            0 => 1.0 - 3.0 * x * x + 2.0 * x * x * x,
            1 => h * (x - 2.0 * x * x + x * x * x),
            2 => 3.0 * x * x - 2.0 * x * x * x,
            3 => h * (-x * x + x * x * x),
            _ => throw new IndexOutOfRangeException($"unknown function with number {func}")
        };
    }
}