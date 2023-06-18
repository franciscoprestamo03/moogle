namespace MoogleEngine;
using MoogleLibrary;
using System;
using System.Text.RegularExpressions;

public static class Moogle
{
    //este metodo solamente busca un fragmento en cada texto por cada aparicion de cada palabra y devuelve un conjunto de fragmentos del texto
    public static string SnippetCalculator(string processQuery, string txt, int txtNumber)
    {
        string[] splitQuery = processQuery.Split(" ");

        string result = "";

        foreach(var word in splitQuery)
        {
            if (word == "" || word == " ")
            {
                continue;
            }
            
            string word1 = " " + word + " ";
            if (txt.Contains(word1))
            { 
                var a = Regex.Match(txt, word1);

                int index = a.Index;
                if (index == -1) continue;
                result += $"...{txt.Substring(Math.Max(0, index - 80), Math.Min(160, (txt.Length - 2)-index))}...  ";
                
            }
        }
        if (result == "")
        {
            return txt.Substring(0, Math.Min(txt.Length - 1, 250));
        }
        return result;
    }

    
    //aqui mediante el obteiner se obtienen todos los datos necesarios y se pasan a ser procesados por el cosine similarity calculator , se optienen las puntuaciones de cada txt , se ordenan y se pasa obtener el snippet de cada uno
    public static SearchResult Query(string query,Obteiner obteiner) {

        obteiner.verifyFiles();

        List<string> AllTxt = obteiner.AllTxt;
        List<string> FileNames = obteiner.FileNames;
        List<string> AllTxtProcessed = obteiner.AllTxtProcessed;
        Matrix WordsOccurrences = obteiner.WordsOccurrences;
        Matrix SteamedOccurrences = obteiner.SteamedOccurrences;
        Dictionary<string, string> SteamedWords = obteiner.SteamedWords;
        Matrix TfIdf = obteiner.TfIdf;
        Matrix TfidfSteamed = obteiner.TfIdfSteamed;
        Dictionary<string, float> idf = obteiner.idf;

        
        CosineSimilarityCalculator cosineSimilarityCalculator = new(TfIdf, TfidfSteamed, query, SteamedWords, WordsOccurrences, SteamedOccurrences, AllTxt.Count,AllTxtProcessed,idf);
        

        List<string> queryWords = cosineSimilarityCalculator.queryWords;

        string processQuery = cosineSimilarityCalculator.processQuery;

        float[] points = cosineSimilarityCalculator.points;

        SortedList<float, int> documents = new();


        string newQuery = cosineSimilarityCalculator.resultQuery;


        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] != 0)
            {
                while (documents.ContainsKey(points.Max() - points[i]))
                {
                    points[i] = (float)(points[i] - 0.00001);
                }
                documents.Add(points.Max() - points[i], i);
            }

        }

        SearchItem[] items = new SearchItem[Math.Min(documents.Count, 8)];

        for (int i = 0; i < Math.Min(documents.Count,8) ; i++)
        {
            items[i] = new SearchItem(FileNames[documents.ElementAt(i).Value], SnippetCalculator(newQuery, AllTxtProcessed[documents.ElementAt(i).Value], documents.ElementAt(i).Value),(float)( 0.1));
        }


        return new SearchResult(items, newQuery);
    }
}
