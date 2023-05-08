using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using MoogleLibrary;

namespace MoogleLibrary
{
	public class Process
	{
        private string contentPath = Path.Join("..","Content");
        private string jsonFilesPath = Path.Join("..", "MoogleEngine","jsonFiles");
        private string contentPathFinal = Path.Join("..", "ContentWithSpaces");
        List<string> AllTxt;
		List<string> FileNames;
        Dictionary<string,string> filesDates;
        List<string> AllTxtProcessed;
        Matrix WordsOccurrences;
        //Estan separados porque hay palabras que son su propio lema
        Matrix SteamedOccurrences;
        Dictionary<string, string> SteamedWords;


        public Process()
		{
            string fileName = "SteamedWords.json";
            string jsonString = File.ReadAllText(Path.Join(jsonFilesPath, fileName));
            SteamedWords = JsonSerializer.Deserialize<Dictionary<string,string>>(jsonString)!;

            AllTxt = new();
            FileNames = new();
            filesDates = new();
            AllTxtProcessed = new();
            

            addSpacesToAllTxt();
            obteinAllTxt();
            processAllTxt();

            WordsOccurrences = new(AllTxtProcessed.Count);
            SteamedOccurrences = new(AllTxtProcessed.Count);
            obteinAllTxtWordsOccurrences();

            serialize();
        }

        public void addSpacesToAllTxt()
        {
            string inputFolder = contentPath;
            string outputFolder = Path.Join("..", "ContentWithSpaces");

            // Crear la carpeta de salida si no existe
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            // Obtener una lista de todos los archivos de texto en la carpeta de entrada
            string[] inputFiles = Directory.GetFiles(inputFolder, "*.txt");

            // Iterar sobre cada archivo de texto en la carpeta de entrada
            foreach (string inputFile in inputFiles)
            {
                // Construir la ruta de salida para el archivo actual
                string outputFile = Path.Combine(outputFolder, Path.GetFileName(inputFile));

                // Abrir el archivo de entrada para leer
                using (StreamReader reader = new StreamReader(inputFile))
                {
                    // Abrir el archivo de salida para escribir
                    using (StreamWriter writer = new StreamWriter(outputFile))
                    {
                        // Leer cada línea del archivo de entrada
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();

                            // Agregar un espacio al final de la línea
                            line += " ";

                            // Escribir la línea en el archivo de salida
                            writer.WriteLine(line);
                        }
                    }
                }
            }
        }

        private void obteinAllTxt()
        {
            var txtDirs = Directory.EnumerateFiles(contentPathFinal, "*.txt");

            foreach (var dir in txtDirs)
            {
                string fileName = Path.GetFileName(dir);
                fileName = Path.ChangeExtension(fileName, null);

                FileNames.Add(fileName);
                string txt = File.ReadAllText(dir);
                AllTxt.Add(txt);
            }

            var txtDirsPath = Directory.EnumerateFiles(contentPath, "*.txt");

            foreach (var dir in txtDirsPath)
            {
                string fileName = Path.GetFileName(dir);
                fileName = Path.ChangeExtension(fileName, null);
                filesDates.Add(fileName, File.GetLastWriteTime(dir).ToLongTimeString());
                
            }
        }

        private void processAllTxt()
        {
            foreach (var ptext in AllTxt)
            {
                string text = removePunctuations(ptext);
                text = text.ToLower();
                text = " " + text + " ";
                AllTxtProcessed.Add(text);
            }
        }

        private string removePunctuations(string input)
        {
            return Regex.Replace(input.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");
        }

        private void obteinAllTxtWordsOccurrences()
        {
            for (int i = 0; i < AllTxtProcessed.Count; i++)
            {
                string[] words = AllTxtProcessed[i].Split(" ");

                for (int j = 0; j < words.Length; j++)
                {
                    string word = words[j];
                    if(word!="" && word!=" ")
                    {
                        if (SteamedWords.ContainsKey(word))
                        {
                            SteamedOccurrences[i, SteamedWords[word]] += 1;
                        }
                        WordsOccurrences[i, word] += 1;
                    }
                }
            }
        }

        private void serialize()
        {
            string dir1 = Path.Join(jsonFilesPath,"fileNames.json");
            string jsonStringFilesNames = JsonSerializer.Serialize(FileNames);
            File.WriteAllText(dir1, jsonStringFilesNames);

            string dir2 = Path.Join(jsonFilesPath, "allTxt.json");
            string jsonStringFilesNames2 = JsonSerializer.Serialize(AllTxt);
            File.WriteAllText(dir2, jsonStringFilesNames2);

            string dir3 = Path.Join(jsonFilesPath, "allTxtProcessed.json");
            string jsonStringFilesNames3 = JsonSerializer.Serialize(AllTxtProcessed);
            File.WriteAllText(dir3, jsonStringFilesNames3);

            string dir4 = Path.Join(jsonFilesPath, "filesDates.json");
            string jsonStringFilesNames4 = JsonSerializer.Serialize(filesDates);
            File.WriteAllText(dir4, jsonStringFilesNames4);

            WordsOccurrences.serialize(Path.Join(jsonFilesPath, "wordsOccurrences.json"));

            SteamedOccurrences.serialize(Path.Join(jsonFilesPath, "steamedOccurrences.json"));
            
        }
    }
}

