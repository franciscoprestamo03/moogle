using System;
namespace Algebra_Library
{
    public class Vector
    {
        private double[] vector;

        public int size { get; private set; }

        public Vector(int psize)
        {
            this.vector = new double[psize];
            size = psize;
        }

        public void SetData(double[] pvector)
        {
            if (pvector.Length != this.vector.Length)
            {
                throw new ArgumentException("Data length does not match vector length.");
            }
            this.vector = pvector;
        }

        public double this[int i]
        {
            get { return vector[i]; }
            set { vector[i] = value; }
        }

        public static Vector operator +(Vector a, Vector b)
        {
            if (a.size != b.size)
            {
                throw new ArgumentException("Vector sizes are not compatible for addition.");
            }

            Vector c = new Vector(a.size);
            for (int i = 0; i < a.size; i++)
            {
                c[i] = a[i] + b[i];
            }

            return c;
        }

        public static double operator *(Vector a, Vector b)
        {
            if (a.size != b.size)
            {
                throw new ArgumentException("Vector sizes are not compatible for dot product.");
            }

            double sum = 0;
            for (int i = 0; i < a.size; i++)
            {
                sum += a[i] * b[i];
            }

            return sum;
        }

        public void Print()
        {
            for (int i = 0; i < vector.Length; i++)
            {
                Console.Write("{0} ", vector[i]);
            }
            Console.WriteLine();
        }
    }


}

