﻿using System.Numerics;

namespace BicubicHermiteSpline.Mathematics;

public class SparseMatrix
{
    public int[] Ig { get; init; }
    public int[] Jg { get; init; }
    public double[] Di { get; }
    public double[] Gg { get; }

    public int Size => Di.Length;
    public SparseMatrix(int[] ig, int[] jg)
    {
        Ig = ig;
        Jg = jg;
        Di = new double[Ig.Length - 1];
        Gg = new double[Jg.Length];
    }
    
    public void Add(int i, int j, double value)
    {
        if (i == j)
        {
            Di[i] += value;
        }
        else if (i > j)
        {
            for (int idx = Ig[i]; idx < Ig[i + 1]; idx++)
            {
                if (Jg[idx] != j) continue;
                Gg[idx] += value;
            }
        }
    }
    
    public static void Dot(SparseMatrix matrix, double[] vector, double[]? product)
    {
        if (matrix.Size != vector.Length)
        {
            throw new Exception("Size of matrix not equal to size of vector");
        }

        product ??= new double[vector.Length];
        Array.Fill(product, 0.0);
        int[] ig = matrix.Ig;
        int[] jg = matrix.Jg;
        double[] di = matrix.Di;
        double[] ggl = matrix.Gg;
        double[] ggu = matrix.Gg;

        for (int i = 0; i < vector.Length; i++)
        {
            product[i] = di[i] * vector[i];

            for (int j = ig[i]; j < ig[i + 1]; j++)
            {
                product[i] += ggl[j] * vector[jg[j]];
                product[jg[j]] += ggu[j] * vector[i];
            }
        }
    }
}