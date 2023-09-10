namespace BicubicHermiteSpline.Mathematics;

public abstract class IterativeSolver
{
    protected SparseMatrix Matrix = default!;
    protected double[] RightPart = default!;
    protected readonly int MaxIters;
    protected readonly double Eps;
    public double[]? Solution { get; protected set; }

    protected IterativeSolver(int maxIters, double eps)
        => (MaxIters, Eps) = (maxIters, eps);

    public void SetSystem(SparseMatrix matrix, double[] rightPart)
        => (Matrix, RightPart) = (matrix, rightPart);

    public abstract void Compute();
}

public class LOS : IterativeSolver
{
    public LOS(int maxIters, double eps) : base(maxIters, eps)
    {
    }

    public override void Compute()
    {
        try
        {
            ArgumentNullException.ThrowIfNull(Matrix, $"{nameof(Matrix)} cannot be null, set the matrix");
            ArgumentNullException.ThrowIfNull(RightPart, $"{nameof(RightPart)} cannot be null, set the vector");

            Solution = new double[RightPart.Length];

            double[] z = new double[RightPart.Length];
            double[] r = new double[RightPart.Length];
            double[] p = new double[RightPart.Length];
            double[] product = new double[RightPart.Length];
            SparseMatrix.Dot(Matrix, Solution, product);

            for (int i = 0; i < product.Length; i++)
            {
                r[i] = RightPart[i] - product[i];
            }

            Array.Copy(r, z, r.Length);
            SparseMatrix.Dot(Matrix, z, p);

            var squareNorm = Vector.Dot(r, r);

            for (int index = 0; index < MaxIters && squareNorm > Eps; index++)
            {
                var alpha = Vector.Dot(p, r) / Vector.Dot(p, p);

                for (int i = 0; i < Solution.Length; i++)
                {
                    Solution[i] += alpha * z[i];
                }

                squareNorm = Vector.Dot(r, r) - (alpha * alpha * Vector.Dot(p, p));

                for (int i = 0; i < Solution.Length; i++)
                {
                    r[i] -= alpha * p[i];
                }

                SparseMatrix.Dot(Matrix, r, product);

                var beta = -Vector.Dot(p, product) / Vector.Dot(p, p);

                for (int i = 0; i < Solution.Length; i++)
                {
                    z[i] = r[i] + beta * z[i];
                    p[i] = product[i] + beta * p[i];
                }
            }
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"We had problem: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"We had problem: {ex.Message}");
        }
    }
}