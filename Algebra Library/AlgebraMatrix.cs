using System;
using System.Numerics;

namespace Algebra_Library
{
    public class Matrix
    {
        public int rows { get; private set; }
        public int cols { get; private set; }
        private readonly double[,] matrix;

        public Matrix(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            matrix = new double[rows, cols];
        }

        public Matrix(double[,] pmatrix)
        {
            this.rows = pmatrix.GetLength(0);
            this.cols = pmatrix.GetLength(1);
            this.matrix = pmatrix;
        }

        public double this[int row, int col]
        {
            get { return matrix[row, col]; }
            set { matrix[row, col] = value; }
        }


        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.rows != b.rows || a.cols != b.cols)
            {
                throw new ArgumentException("Matrices must have the same dimensions to be added.");
            }

            Matrix result = new Matrix(a.rows, a.cols);

            for (int i = 0; i < a.rows; i++)
            {
                for (int j = 0; j < a.cols; j++)
                {
                    result[i, j] = a[i, j] + b[i, j];
                }
            }

            return result;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.cols != b.rows)
            {
                throw new ArgumentException("The number of columns in the first matrix must be equal to the number of rows in the second matrix.");
            }

            Matrix result = new Matrix(a.rows, b.cols);

            for (int i = 0; i < a.rows; i++)
            {
                for (int j = 0; j < b.cols; j++)
                {
                    double sum = 0;

                    for (int k = 0; k < a.cols; k++)
                    {
                        sum += a[i, k] * b[k, j];
                    }

                    result[i, j] = sum;
                }
            }

            return result;
        }

        public static Matrix operator *(Matrix a, double b)
        {
            Matrix result = new Matrix(a.rows, a.cols);

            for (int i = 0; i < a.rows; i++)
            {
                for (int j = 0; j < a.cols; j++)
                {
                    result[i, j] = a[i, j] * b;
                }
            }

            return result;
        }

        public static Matrix operator *(double a, Matrix b)
        {
            return b * a;
        }

        public static Vector operator *(Vector b,Matrix a)
        {
            if (a.cols != b.size)
            {
                throw new ArgumentException("The number of columns in the matrix must be equal to the size of the vector.");
            }

            Vector result = new Vector(a.rows);

            for (int i = 0; i < a.rows; i++)
            {
                double sum = 0;

                for (int j = 0; j < a.cols; j++)
                {
                    sum += a[i, j] * b[j];
                }

                result[i] = sum;
            }

            return result;
        }

        public static Matrix Sum(Matrix a, Matrix b)
        {
            if (a.rows != b.rows || a.cols != b.cols)
            {
                throw new ArgumentException("Matrices must have the same dimensions to be added.");
            }

            Matrix result = new Matrix(a.rows, a.cols);

            for (int i = 0; i < a.rows; i++)
            {
                for (int j = 0; j < a.cols; j++)
                {
                    result[i, j] = a[i, j] + b[i, j];
                }
            }

            return result;
        }
    }

}

