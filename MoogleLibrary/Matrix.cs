using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MoogleLibrary
{
	//Es una abstraccion de matrix solo que esta es un array de diccionarios por tanto indexa por entero y palabra lo cual es bastante util y eficiente
	//Permite que si quieres obtener un valor en un indice y dicha palabra en dicho documento representado por un entero no existe devuelve 0
	//Ademas contiene funciones para saber la cantidad de veces documentos que contienen a una palabra y otro par de metodos para serializarse y deserializarse automaticamente
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

