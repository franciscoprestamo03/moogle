﻿using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MoogleLibrary
{
	public class CosineSimilarityCalculator
	{
		private Matrix Tfidf;
		private Matrix TfidfSteamed;
		private Matrix WordsOccurrences;
		private Matrix SteamedOccurrences;


		Dictionary<string, string> SteamedWords;

		string query;
		public string processQuery { get; private set; }
        Dictionary<string, string> distanceToCalculate;
		Dictionary<string, int> asterDictionary;
		List<string> yesWords;
		List<string> noWords;

		List<string> AllTxtProcessed;
		Dictionary<string, float> idf;

        int numberQueryWors;
		int numberDocuments;

		Dictionary<string, float> queryWordsOccurrences;
		Dictionary<string, float> querySteamedOccurrences;

		Dictionary<string, float> queryTfidf;
		Dictionary<string, float> queryTfidfSteamed;

		public List<string> queryWords { get; private set; }
		public string resultQuery { get; private set; }

        public float[] points { get; private set; }


		public CosineSimilarityCalculator(Matrix pTfidf, Matrix pTfidfSteamed, string pquery, Dictionary<string, string> pSteamedWords, Matrix pWordsOccurrences, Matrix pSteamedOccurrences, int pnumberDocuments, List<string> pAllTxtProcessed, Dictionary<string,float> pidf)
        {
			numberQueryWors = 0;
			numberDocuments = pnumberDocuments;
			points = new float[numberDocuments];
			Tfidf = pTfidf;
			TfidfSteamed = pTfidfSteamed;
			SteamedWords = pSteamedWords;
			WordsOccurrences = pWordsOccurrences;
			SteamedOccurrences = pSteamedOccurrences;
			idf = pidf;
			queryWordsOccurrences = new();
			querySteamedOccurrences = new();
			queryTfidf = new();
			queryTfidfSteamed = new();
			distanceToCalculate = new();
			asterDictionary = new();
			yesWords = new();
			noWords = new();
			AllTxtProcessed = pAllTxtProcessed;


            query = pquery;

            calculateSymbols();

            processQuery = removePunctuations(query.ToLower());

			

			

            countOccurrences();

			calculateQueryTfidf();

			calculateSimilarity();

			symbols();
		}


		private void calculateSymbols()
		{
			string[] wordsOfQuery = query.Split(" ");

			for (int i = 0; i < wordsOfQuery.Length; i++)
			{
				string word = wordsOfQuery[i];

				if(word=="" || word==" ")
				{
					continue;
				}

                if (word[0] == '^')
                {
					
					Console.WriteLine("word " + word);



					yesWords.Add(removePunctuations(word));

                }
                if (word[0] == '*')
                {
					int count = 0;
					for (int j = 0; j < word.Length; j++)
					{
						if (word[j] == '*')
						{
							count++;
						}
						else if(word[j] == '^')
						{
							continue;
						}
						else
						{
							break;
						}
					}

                    Console.WriteLine("count " + count + " word " + removePunctuations(word));
                    asterDictionary.Add(removePunctuations(word), count);

                }
                if (word[0] == '!')
                {
					noWords.Add(removePunctuations(word));
					

					query = Regex.Replace(query, word,"");

                    

                    wordsOfQuery[i] = word;
                }

                if (word[0] == '~' && i > 0 && i < wordsOfQuery.Length - 1)
                {
                    distanceToCalculate.Add(removePunctuations(wordsOfQuery[i - 1]), removePunctuations(wordsOfQuery[i + 1]));
					//Console.WriteLine(wordsOfQuery[i - 1] +"   ----   "+ wordsOfQuery[i + 1]);
                    wordsOfQuery[i] = "";
                }

            }
		}

		private void symbols()
		{
            foreach (var item in distanceToCalculate)
			{
				string word1 = item.Key;
				string word2 = item.Value;

				for (int i = 0; i < AllTxtProcessed.Count; i++)
				{
					string txt = AllTxtProcessed[i];





					string[] allWords = txt.Split(" ");

					int minDistance = int.MaxValue;

					int index = 0;

					int whatWordIs = 0;

					for (int j = 0; j < allWords.Length; j++)
					{
						string word = allWords[j];

						if (word == word1 && whatWordIs == 2)
						{
							//Console.WriteLine("aaaaa");

							if (minDistance > (j - index)) minDistance = (j - index);

							//Console.WriteLine($"distance {j - index}");
							whatWordIs = 1;
							index = j;
							continue;
						}
						if (word == word2 && whatWordIs == 1)
						{
							
							if (minDistance > (j - index)) minDistance = (j - index);

							
							whatWordIs = 2;
							index = j;
							continue;
						}
						if (word == word1 && whatWordIs == 0)
						{
							
							index = j;
							whatWordIs = 1;
							continue;
						}
						if (word == word2 && whatWordIs == 0)
						{
							
							index = j;
							whatWordIs = 2;
							continue;
						}
					}

					
					if (minDistance == 0 || minDistance == int.MaxValue)
					{
						points[i] = 0;
					}
					else
					{
                        points[i] += (float)(0.1 /(float) minDistance);
					}
				}
			}

			for (int i = 0; i < AllTxtProcessed.Count; i++)
			{
				string txt = AllTxtProcessed[i];

				foreach (var noword in noWords)
				{
                    

                    if (Regex.IsMatch(txt, " " + noword + " "))
					{
						points[i] = 0;
					}
					if (SteamedWords.ContainsKey(noword))
					{

                        if (Regex.IsMatch(txt, " " + SteamedWords[noword] + " "))
                        {
                            
                            points[i] = 0;
                        }
                    }
				}

				

                foreach (var yesword in yesWords)
                {
                    Console.WriteLine($"yesword {yesword} document {i}");
                    if (!Regex.IsMatch(txt, " " + yesword + " "))
                    {
						points[i] = 0;
                    }
                    

                }
                
                
            }

        }


        private string removePunctuations(string input)
		{
			return Regex.Replace(input.Normalize(NormalizationForm.FormD), @"[^a-z0-9 ]+", "");
		}

        private int LevenshteinDistance(string string1, string string2)
        {
            int n = string1.Length;
            int m = string2.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            for (int i = 0; i <= n; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 0; j <= m; j++)
            {
                d[0, j] = j;
            }

            for (int j = 1; j <= m; j++)
            {
                for (int i = 1; i <= n; i++)
                {
                    int cost = (string1[i - 1] == string2[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }

        private void countOccurrences()
		{

            int minLDistance = int.MaxValue;
            string newQuery = "";

            string wordWithMinLDistance = "";

            foreach (string queryWord in processQuery.Split(" "))
            {
                if (queryWord == "" || queryWord == " ")
                {
                    continue;
                }
                foreach (var item in idf)
                {
                    string word = item.Key;
                    int n = LevenshteinDistance(word, queryWord);

                    if (n < minLDistance)
                    {
                        minLDistance = n;
                        wordWithMinLDistance = word;
                    }

                }

                newQuery += $"{wordWithMinLDistance} ";
                wordWithMinLDistance = "";
                minLDistance = int.MaxValue;
            }

			newQuery = newQuery.Trim();

			resultQuery = newQuery;

			

            queryWords = newQuery.Split(" ").ToList();



			numberQueryWors = queryWords.Count;
            
            foreach (var word in queryWords)
			{
                if (word == "" || word == " ")
                {
                    continue;
                }
                
                if (queryWordsOccurrences.ContainsKey(word))
				{
                    
                    queryWordsOccurrences[word] += 1;
				}
				else
				{
					queryWordsOccurrences.Add(word, 1);
				}

				if (SteamedWords.ContainsKey(word))
				{
					string steamedWord = SteamedWords[word];
                    
                    if (querySteamedOccurrences.ContainsKey(steamedWord))
					{
						querySteamedOccurrences[steamedWord] += 1;
					}
					else
					{
						querySteamedOccurrences.Add(steamedWord, 1);
					}

				}
			}
		}


		private void calculateQueryTfidf()
		{
            
            foreach (var item in queryWordsOccurrences)
			{
				string word = item.Key;
				float num = item.Value;
				float t = WordsOccurrences.sumColum(word);

				if (t != 0)
				{
                    queryTfidf.Add(word, (float)(((float)(num / (float)numberQueryWors)) * (Math.Log((numberDocuments+1) / (float)t))));
				}
				else
				{
                    queryTfidf.Add(word, 0);
                }
				
			}

			foreach (var item in querySteamedOccurrences)
			{
				string word = item.Key;
				float num = item.Value;
				float t = SteamedOccurrences.sumColum(word);

				if (t!=0)
				{
                    queryTfidfSteamed.Add(word, (float)(((float)(num / (float)numberQueryWors)) * (Math.Log((numberDocuments + 1) / (float)t))));
				}
				else
				{
					queryTfidfSteamed.Add(word, 0);

                }
				
			}
			
			foreach (var item in asterDictionary)
			{

				if (queryTfidf.ContainsKey(item.Key))
				{
                    queryTfidf[item.Key] = queryTfidf[item.Key] * (item.Value+1);
                }

				if (SteamedWords.ContainsKey(item.Key))
				{
					if (queryTfidfSteamed.ContainsKey(item.Key))
					{
                        queryTfidfSteamed[item.Key] = queryTfidfSteamed[item.Key] * (item.Value+1);
                    }
                }
			}


		}

		private void calculateSimilarity()
		{

            for (int i = 0; i < numberDocuments; i++)   
			{

                float mult = 0;
                float sum1 = 0;
                float sum2 = 0;
				float sum = 0;

                foreach (var item in queryTfidf)
                {
                    
                    string word = item.Key;


                    mult += queryTfidf[word] * Tfidf[i, word];

					sum1 += queryTfidf[word] * queryTfidf[word];

					sum2 += Tfidf[i, word] * Tfidf[i, word];
                }

                sum = (float)(Math.Sqrt(sum1) * Math.Sqrt(sum2));

				if (sum != 0)
				{
                    points[i] += mult / sum;
                }
            }

            for (int i = 0; i < numberDocuments; i++)
            {

                float mult = 0;
                float sum1 = 0;
                float sum2 = 0;
                float sum = 0;

                foreach (var item in queryTfidfSteamed)
                {
                    string word = item.Key;

                    mult += queryTfidfSteamed[word] * TfidfSteamed[i, word];

                    sum1 += queryTfidfSteamed[word] * queryTfidfSteamed[word];

                    sum2 += TfidfSteamed[i, word] * TfidfSteamed[i, word];
                }

                sum = (float)(Math.Sqrt(sum1) * Math.Sqrt(sum2));

                if (sum != 0)
                {
                    points[i] += (mult / sum)/(float)(2);
                }
            }

        }
	}
}