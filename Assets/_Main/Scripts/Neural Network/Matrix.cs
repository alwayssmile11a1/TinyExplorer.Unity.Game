using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class Matrix
{

    public int columnNb;
    public int rowNb;
    public VectorN[] rows;

    /*-$-$-$-$-$-$-$-$-CONSTRUCTORS & INDEXERS-$-$-$-$-$-$-$-$-*/

    public Matrix(int rowNb, int columnNb)
    {
        this.rowNb = rowNb;
        this.columnNb = columnNb;
        rows = new VectorN[rowNb];
        for (int i = 0; i < rowNb; i++)
        {
            rows[i] = new VectorN(columnNb);
        }
    }

    public Matrix(int size) : this(size, size) { }

    public VectorN this[int i] //Be careful this does not look for errors
    {
        get
        {
            return rows[i];
        }
        set
        {
            rows[i] = value;
        }
    }

    /*-$-$-$-$-$-$-$-$-METHODS-$-$-$-$-$-$-$-$-*/

    public void printSize()
    {
        Debug.Log("Matrix " + rows.Length + " x " + rows[0].Length);
    }

    public void print()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            string Line = "Line n°" + i + " :";
            for (int j = 0; j < columnNb; j++)
            {
                Line += " " + this[i][j];
            }
            Debug.Log(Line);
        }
    }

    public bool isSquare()
    {
        return columnNb == rowNb;
    }

    public Matrix transpose()
    { //Returns new matrice in case the input is not square
        Matrix m = new Matrix(this.columnNb, this.rowNb);
        for (int i = 0; i < this.rowNb; i++)
        {
            for (int j = 0; j < this.columnNb; j++)
            {
                m[j][i] = this[i][j];
            }
        }
        return m;
    }

    /*-$-$-$-$-$-$-$-$-OPERATORS-$-$-$-$-$-$-$-$-*/

    public static Matrix operator +(Matrix m1, Matrix m2)
    {
        if (m1.rowNb == m2.rowNb && m1.columnNb == m2.columnNb)
        {
            Matrix m = new Matrix(m1.rowNb, m1.columnNb); //Destination matrix
            for (int i = 0; i < m1.rowNb; i++)
            {
                m[i] = m1[i] + m2[i]; //CF Operations on VectorN
            }
            return m;
        }
        else
        {
            throw new System.Exception("MATRIX DIMENSIONS MUST AGREE");
        }
    }

    public static Matrix Identity(int n)
    {
        Matrix m = new Matrix(n);
        for (int i = 0; i < n; i++)
        {
            m[i][i] = 1;
        }
        return m;
    }

    public static Matrix operator -(Matrix m1, Matrix m2)
    {
        if (m1.rowNb == m2.rowNb && m1.columnNb == m2.columnNb)
        {
            Matrix m = new Matrix(m1.rowNb, m1.columnNb); //Destination matrix
            for (int i = 0; i < m1.rowNb; i++)
            {
                m[i] = m1[i] - m2[i]; //CF Operations on VectorN
            }
            return m;
        }
        else
        {
            throw new System.Exception("MATRIX DIMENSIONS MUST AGREE");
        }
    }

    public static Matrix operator *(Matrix m1, float f)
    {
        Matrix m = new Matrix(m1.rowNb, m1.columnNb); //Destination matrix
        for (int i = 0; i < m1.rowNb; i++)
        {
            m[i] = m1[i] * f; //CF Operations on VectorN
        }
        return m;
    }
    public static Matrix operator *(float f, Matrix m1)
    {
        Matrix m = new Matrix(m1.rowNb, m1.columnNb); //Destination matrix
        for (int i = 0; i < m1.rowNb; i++)
        {
            m[i] = m1[i] * f; //CF Operations on VectorN
        }
        return m;
    }

    public static Matrix operator *(Matrix m1, Matrix m2)
    {
        if (m1.columnNb == m2.rowNb)
        { //Ex 2X8 * 8x1 => 2x1
            Matrix m = new Matrix(m1.rowNb, m2.columnNb); //Destination matrix
            for (int i = 0; i < m.rowNb; i++)
            {
                for (int j = 0; j < m.columnNb; j++)
                {
                    float sum = 0;
                    for (int k = 0; k < m.rowNb; k++)
                    {
                        sum += m1[i][k] * m2[k][j]; // c(i,j) = a(i,k)b(k,j); and M = matrix of c(i,j)
                    }
                    m[i][j] = sum; //CF Operations on VectorN
                }
            }
            return m;
        }
        else
        {
            throw new System.Exception("MATRIX DIMENSIONS MUST AGREE");
        }
    }


    public static Matrix operator /(Matrix m1, float f)
    {
        if (f != 0)
        {
            Matrix m = new Matrix(m1.rowNb, m1.columnNb); //Destination matrix
            for (int i = 0; i < m1.rowNb; i++)
            {
                m[i] = m1[i] / f; //CF Operations on VectorN
            }
            return m;
        }
        else
        {
            throw new System.Exception("ARRGH, DIVISION BY ZERO");
        }
    }

    public static bool operator ==(Matrix m1, Matrix m2)
    {
        if (m1.rowNb == m2.rowNb && m1.columnNb == m2.columnNb)
        {
            for (int i = 0; i < m1.rowNb; i++)
            {
                if (m1[i] == m2[i])
                { //CF Operations on VectorN
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool operator !=(Matrix m1, Matrix m2)
    {
        if (m1.rowNb == m2.rowNb && m1.columnNb == m2.columnNb)
        {
            for (int i = 0; i < m1.rowNb; i++)
            {
                if (m1[i] == m2[i])
                { //CF Operations on VectorN
                    return true;
                }
            }
            return false;
        }
        else
        {
            return true;
        }
    }

    public override string ToString()
    {
        Debug.Log("Start Logging Matrix:");
        for (int i = 0; i < rowNb; i++)
        {
            Debug.Log($"{rows[i].ToString()}");
        }
        return "Finish Logging Matrix";
    }

    public void Randomize()
    {
        for (int i = 0; i < rowNb; i++)
        {
            for (int j = 0; j < columnNb; j++)
            {
                rows[i][j] = Random.Range(-1f, 1f);
            }
        }
    }

    public void Print()
    {
        for (int i = 0; i < rowNb; i++)
        {
            Debug.Log(rows[i].ToString());
        }
    }

    public static Matrix FromArray(float[] arr)
    {
        Matrix m = new Matrix(arr.Length, 1);
        for (int i = 0; i < arr.Length; i++)
        {
            m.rows[i][0] = arr[i];
        }
        return m;
    }

    public float[] ToArray()
    {
        float[] arr = new float[rowNb];
        for (int i = 0; i < rowNb; i++)
        {
            for (int j = 0; j < columnNb; j++)
            {
                arr[i] = rows[i][j];
            }
        }
        return arr;
    }

    public void map(Func<float, float> func)
    {
        for (int i = 0; i < rowNb; i++)
        {
            for (int j = 0; j < columnNb; j++)
            {
                rows[i][j] = func(rows[i][j]);
            }
        }
    }
}