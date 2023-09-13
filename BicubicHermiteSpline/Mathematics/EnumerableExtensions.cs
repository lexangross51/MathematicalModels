namespace BicubicHermiteSpline.Mathematics;

public static class EnumerableExtensions
{
    public static double Norm(this IEnumerable<double> collection)
        => Math.Sqrt(collection.Sum(item => item * item));
}