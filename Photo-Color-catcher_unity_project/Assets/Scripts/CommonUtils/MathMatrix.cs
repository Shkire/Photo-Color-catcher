using System.Collections;
using System.Collections.Generic;

public static class MathMatrix
{
	public static float[][] MatrixMultiplication(float[][] i_m1, float[][] i_m2)
	{
		int m1Rows = i_m1.Length;
		int m2Rows = i_m2.Length;
		int m1Columns = i_m1[0].Length;
		int m2Columns = i_m2[0].Length;
		foreach (float[] row in i_m1)
			if (row.Length != m1Columns)
				return null;
		foreach (float[] row in i_m2)
			if (row.Length != m2Columns)
				return null;
		if (m1Columns != m2Rows)
			return null;
		//Base case
		if (m1Rows == 1 && m2Columns == 1)
		{
			float aux = 0;
			for (int i = 0; i < m1Columns; i++)
			{
				aux += i_m1[1][i] * i_m2[i][1];
			}
			return new float[][]{ new float[] { aux } };

		}
		else
		{
			float[][] aux = new float[m1Rows][];
			for (int i = 0; i < aux.Length; i++)
			{
				aux[i] = new float[m2Columns];
				for (int j = 0; j < aux[i].Length; j++)
				{
					float[][] row = new float[1][];
					row[0] = new float[m1Columns];
					for (int k = 0; k < row[0].Length; k++)
						row[0][k] = i_m1[i][k];
					float[][] column = new float[m2Rows][];
					for (int k = 0; k < column.Length; k++)
					{
						column[k] = new float[]{ i_m2[k][j] };
					}
					aux[i][j] = MatrixMultiplication(row, column)[0][0];
				}
			}
			return aux;
		}
	}
}
