namespace BicubicHermiteSpline.Mathematics;

public static class BicubicHermiteShapes
{
    private static readonly Matrix<double> StiffnessMatrix2D = new(16, 16);

    private static Matrix<double> MakeStiffnessMatrix1D(double h)
    {
        var stiffness = new Matrix<double>(4, 4);
        
        double a11 = 36.0 / (30.0 * h);
        double a12 = 3.0 / 30.0;
        double a24 = h * h;

        stiffness[0, 0] = stiffness[2, 2] = a11;
        stiffness[0, 2] = stiffness[2, 0] = -a11;
        stiffness[0, 1] = stiffness[0, 3] = stiffness[1, 0] = stiffness[3, 0] = a12;
        stiffness[1, 2] = stiffness[2, 1] = stiffness[2, 3] = stiffness[3, 2] = -a12;
        stiffness[1, 1] = stiffness[3, 3] = 4.0 * a24;
        stiffness[1, 3] = stiffness[3, 1] = -a24;

        return stiffness;
    }

    private static Matrix<double> MakeMassMatrix1D(double h)
    {
        var mass = new Matrix<double>(4, 4);
        
        double a11 = 156.0 * h / 420.0;
        double a12 = 22.0 * h * h / 420.0;
        double a13 = 54.0 * h / 420.0;
        double a14 = 13.0 * h * h / 420.0;
        double a22 = h * h * h / 420.0;

        mass[0, 0] = mass[2, 2] = a11;
        mass[0, 1] = mass[1, 0] = a12;
        mass[0, 2] = mass[2, 0] = a13;
        mass[0, 3] = mass[3, 0] = -a14;
        mass[1, 1] = mass[3, 3] = 4.0 * a22;
        mass[1, 2] = mass[2, 1] = a14;
        mass[2, 3] = mass[3, 2] = -a12;
        mass[1, 3] = mass[3, 1] = -3.0 * a22;

        return mass;
    }

    public static Matrix<double> MakeStiffnessMatrix2D(double hx, double hy)
    {
        var gx = MakeStiffnessMatrix1D(hx);
        var gy = MakeStiffnessMatrix1D(hy);
        var mx = MakeMassMatrix1D(hx);
        var my = MakeMassMatrix1D(hy);
        
        for (int i = 0; i < HermiteBasis2D.BasisSize; i++)
        {
            int muI = 2 * (i / 4 % 2) + i % 2;
            int nuI = 2 * (i / 8) + i / 2 % 2;
            
            for (int j = 0; j < HermiteBasis2D.BasisSize; j++)
            {
                int muJ = 2 * (j / 4 % 2) + j % 2;
                int nuJ = 2 * (j / 8) + j / 2 % 2;

                StiffnessMatrix2D[i, j] = gx[muI, muJ] * my[nuI, nuJ] + mx[muI, muJ] * gy[nuI, nuJ];
            }
        }
        
        return StiffnessMatrix2D;
    }
}