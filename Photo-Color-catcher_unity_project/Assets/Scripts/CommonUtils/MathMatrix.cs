using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Math functions for matrices.
/// </summary>
public static class MathMatrix
{
    /// <summary>
    /// Multiplicates two matrices.
    /// </summary>
    /// <returns>Result matrix.</returns>
    /// <param name="i_m1">First matrix.</param>
    /// <param name="i_m2">Second matrix.</param>
    public static float[][] MatrixMultiplication(float[][] i_m1, float[][] i_m2)
    {
        //Gets number of rows and columns of both matrices.
        int m1Rows = i_m1.Length;
        int m2Rows = i_m2.Length;
        int m1Columns = i_m1[0].Length;
        int m2Columns = i_m2[0].Length;

        //Checks if one of the matrices has any row with a different number of elements.
        foreach (float[] row in i_m1)
            if (row.Length != m1Columns)
                return null;
        foreach (float[] row in i_m2)
            if (row.Length != m2Columns)
                return null;

        //Checks if matrices can be multiplicated.
        if (m1Columns != m2Rows)
            return null;
        
        //Base case.
        if (m1Rows == 1 && m2Columns == 1)
        {
            float aux = 0;
            for (int i = 0; i < m1Columns; i++)
            {
                aux += i_m1[0][i] * i_m2[i][0];
            }
            return new float[][]{ new float[] { aux } };
        }

        //Recursive case.
        else
        {
            float[][] aux = new float[m1Rows][];
            for (int i = 0; i < aux.Length; i++)
            {
                aux[i] = new float[m2Columns];
                for (int j = 0; j < aux[i].Length; j++)
                {

                    //Gets ith row from first matrix.
                    float[][] row = new float[1][];
                    row[0] = new float[m1Columns];
                    for (int k = 0; k < row[0].Length; k++)
                        row[0][k] = i_m1[i][k];

                    //Gets jth column from second matrix.
                    float[][] column = new float[m2Rows][];
                    for (int k = 0; k < column.Length; k++)
                    {
                        column[k] = new float[]{ i_m2[k][j] };
                    }

                    //Gets element ijth of the result matrix.
                    aux[i][j] = MatrixMultiplication(row, column)[0][0];
                }
            }
            return aux;
        }
    }
}