using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MoogleLibrary
{
	public class Matrix
	{
		Dictionary<string, float>[] matrix;

		public Matrix(int row)
		{
			matrix = new Dictionary<string, float>[row];

			
            for (int i = 0; i < row; i++)
            {
                matrix[i] = new Dictionary<string, float>();
            }
        }

		public void serialize(string path)
		{
            string jsonString = JsonSerializer.Serialize(matrix);
            File.WriteAllText(path, jsonString);
        }

        public void deserialize(string path)
        {
            string jsonString = File.ReadAllText(path);
            matrix = JsonSerializer.Deserialize<Dictionary<string, float>[]>(jsonString)!;
        }

		public float sumRow(int i)
		{
			float sum = 0;
			foreach (var item in matrix[i])
			{
				sum += item.Value;
			}
			return sum;
		}

        public float sumColum(string word)
        {
            float sum = 0;
            for (int i = 0; i<matrix.Length;i++)
            {
				if(matrix[i].ContainsKey(word)) sum++;
            }
            return sum;
        }

		public Dictionary<string,float> GetRow(int i)
		{
			return matrix[i];
		}

        public float this[int i,string j]
		{
			get {
				//Console.WriteLine("i"+i);
				if (matrix[i].ContainsKey(j))
				{
                    return matrix[i][j];
                }
				return 0;	
			}
			set {
                if (matrix[i].ContainsKey(j))
                {
                    matrix[i][j] = value;
				}
				else
				{
					matrix[i].Add(j, value);
				}
				//Console.WriteLine($"i {i} word {j} value{matrix[i][j]}");
            }
		}


	}
}

