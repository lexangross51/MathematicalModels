using System;
using System.Collections.Generic;
using System.Linq;

namespace BicubicHermite.Core.SplineCalculator.Mathematics;

public static class EnumerableExtensions
{
    public static double Norm(this IEnumerable<double> collection)
        => Math.Sqrt(collection.Sum(item => item * item));
}