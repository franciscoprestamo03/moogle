using System;
using System.IO;
using System.Text.Json;

namespace MoogleLibrary
{
	//Esta clase se encarga de obtener toda la informacion ya preprocesada y verificar si tiene que procesarse de nuevo
	//Ademas calcula el tfidf de las palabras y el tfidf de las palabras lematizadas
	//Ademas de obtener dicha informacion la almacena para pasarla a la clase query y solo procesar dicha informacion una vez
	
	public class Obteiner
	{
        private string contentPath = Path.Join("..", "Content");
        private string jsonFilesPath = Path.Join("..", "MoogleEngine", "jsonFiles");
        private string contentPathFinal = Path.Join("..", "ContentWithSpaces");
        public List<string> AllTxt { get; private set; }
        public List<string> FileNames { get; private set; }
        public Dictionary<string, string> filesDates { get; private set; }
        public List<string> AllTxtProcessed { get; private set; }
        public Matrix WordsOccurrences { get; private set; }
        public Matrix SteamedOccurrences { get; private set; }
        public Dictionary<string, string> SteamedWords { get; private set; }
        public Dictionary<string, int> DocumentsToChange { get; private set; }
        public Matrix TfIdf { get; private set; }
        public Matrix TfIdfSteamed { get; private set; }
        public Dictionary<string, float> idf { get; private set; }
        public Dictionary<string, float> idfSteamed { get; private set; }


        public Obteiner()
		{
            DocumentsToChange = new();

            if (!verifyJson())
            {
                Process a = new();
                deserialize(true);
            }
            else
            {
                deserialize(false);
            }

            verifyFiles();
        }
	//este metodo se encarga de obtener la informacion almacenada en los json
        private void deserialize(bool processTfidf)
        {
            Console.WriteLine("se esta deserealizando");

            string fileName1 = "SteamedWords.json";
            string jsonString1 = File.ReadAllText(Path.Join(jsonFilesPath, fileName1));
            SteamedWords = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString1)!;

            string fileName2 = "allTxt.json";
            string jsonString2 = File.ReadAllText(Path.Join(jsonFilesPath, fileName2));
            AllTxt = JsonSerializer.Deserialize<List<string>>(jsonString2)!;

            string fileName3 = "fileNames.json";
            string jsonString3 = File.ReadAllText(Path.Join(jsonFilesPath, fileName3));
            FileNames = JsonSerializer.Deserialize<List<string>>(jsonString3)!;

            string fileName4 = "allTxtProcessed.json";
            string jsonString4 = File.ReadAllText(Path.Join(jsonFilesPath, fileName4));
            AllTxtProcessed = JsonSerializer.Deserialize<List<string>>(jsonString4)!;

            string fileName5 = "filesDates.json";
            string jsonString5 = File.ReadAllText(Path.Join(jsonFilesPath, fileName5));
            filesDates = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString5)!;

            WordsOccurrences = new(AllTxt.Count);
            SteamedOccurrences = new(AllTxt.Count);

            WordsOccurrences.deserialize(Path.Join(jsonFilesPath, "wordsOccurrences.json"));
            SteamedOccurrences.deserialize(Path.Join(jsonFilesPath, "steamedOccurrences.json"));

            if (processTfidf)
            {
                Console.WriteLine("calculando tfidf");
                calculateTfidf();
            }
            else
            {
                TfIdf = new(AllTxtProcessed.Count);
                TfIdfSteamed = new(AllTxtProcessed.Count);
                Console.WriteLine("recuperando tfidf");
                TfIdf.deserialize(Path.Join(jsonFilesPath, "tfidf.json"));
                TfIdfSteamed.deserialize(Path.Join(jsonFilesPath, "tfidfSteamed.json"));


                string fileName6 = "idf.json";
                string jsonString6 = File.ReadAllText(Path.Join(jsonFilesPath, fileName6));
                idf = JsonSerializer.Deserialize<Dictionary<string, float>>(jsonString6)!;

                string fileName7 = "idfSteamed.json";
                string jsonString7 = File.ReadAllText(Path.Join(jsonFilesPath, fileName7));
                idfSteamed = JsonSerializer.Deserialize<Dictionary<string, float>>(jsonString7)!;

            }
            
        }
	//Este metodo se encarga de calcular el tfidf con la formula comun exceptuando que suma uno en la parte de cantidad de documentos para que el idf de una palabra que aparece muy repetida sea bajo pero no 0
	//Ademas se almacena si una palabra ya fue calculada la cantidad de apariciones en tenxtos para no calcular de nuevo
        private void calculateTfidf()
        {
            idf = new();
            idfSteamed = new();

            TfIdf = new(AllTxtProcessed.Count);

            for (int i = 0; i < AllTxtProcessed.Count; i++)
            {
                float n = WordsOccurrences.sumRow(i);
                foreach (var item in WordsOccurrences.GetRow(i))
                {
                    string word = item.Key;
                    float num = item.Value;
                    float t = 1;
                    if (idf.ContainsKey(word))
                    {
                        t = idf[word];
                    }
                    else
                    {
                        t = WordsOccurrences.sumColum(word);
                        idf.Add(word, t);
                    }

                    TfIdf[i,word]=(float)(((float)(num / (float)n)) * (Math.Log((AllTxtProcessed.Count+1) / (float)t)));
                    
                }
            }

            TfIdf.serialize(Path.Join(jsonFilesPath, "tfidf.json"));

            TfIdfSteamed = new(AllTxtProcessed.Count);
		//calculo del tfidf para las palabras lematizadas
            for (int i = 0; i < AllTxtProcessed.Count; i++)
            {
                float n = SteamedOccurrences.sumRow(i);
                foreach (var item in SteamedOccurrences.GetRow(i))
                {
                    string word = item.Key;
                    float num = item.Value;

                    float t = 1;
                    if (idfSteamed.ContainsKey(word))
                    {
                        t = idfSteamed[word];
                    }
                    else
                    {
                        t = SteamedOccurrences.sumColum(word);
                        idfSteamed.Add(word, t);
                    }

                    TfIdfSteamed[i, word] = (float)(((float)(num / (float)n)) * (Math.Log((AllTxtProcessed.Count + 1) / (float)t)));
                    //Console.WriteLine($"i {i} word {word} num {num} n {n} t{t} log {Math.Log(AllTxtProcessed.Count / (float)t)}");
                }
            }

            TfIdfSteamed.serialize(Path.Join(jsonFilesPath, "tfidfSteamed.json"));


            string dir8 = Path.Join(jsonFilesPath, "idf.json");
            string jsonString8 = JsonSerializer.Serialize(idf);
            File.WriteAllText(dir8, jsonString8);

            string dir9 = Path.Join(jsonFilesPath, "idfSteamed.json");
            string jsonString9 = JsonSerializer.Serialize(idfSteamed);
            File.WriteAllText(dir9, jsonString9);


		//aaaaaaaaaa
            float[] normaTfidf = new float[AllTxtProcessed.Count];

            for (int i = 0; i < AllTxtProcessed.Count; i++)
            {
                foreach (var item in idf)
                {
                    string word = item.Key;
                    normaTfidf[i] += TfIdf[i, word] * TfIdf[i, word];
                }
                normaTfidf[i] = (float) Math.Sqrt(normaTfidf[i]);
            }

            float[] normaSteamedTfidf = new float[AllTxtProcessed.Count];

            for (int i = 0; i < AllTxtProcessed.Count; i++)
            {
                foreach (var item in idf)
                {
                    string word = item.Key;
                    normaSteamedTfidf[i] += TfIdfSteamed[i, word] * TfIdfSteamed[i, word];
                }
                normaSteamedTfidf[i] = (float)Math.Sqrt(normaSteamedTfidf[i]);
            }


            string dir1 = Path.Join(jsonFilesPath, "normaTfidf.json");
            string jsonStringFilesNames = JsonSerializer.Serialize(normaTfidf);
            File.WriteAllText(dir1, jsonStringFilesNames);

            string dir2 = Path.Join(jsonFilesPath, "normaSteamedTfidf.json");
            string jsonStringFilesNames2 = JsonSerializer.Serialize(normaSteamedTfidf);
            File.WriteAllText(dir2, jsonStringFilesNames2);
            //aaaaaaaaaaaa

        }
	//verifica que esten los json
        public bool verifyJson()
        {
            bool a = true;

            a &= File.Exists(Path.Join(jsonFilesPath, "SteamedWords.json"));
            a &= File.Exists(Path.Join(jsonFilesPath, "allTxt.json"));
            a &= File.Exists(Path.Join(jsonFilesPath, "fileNames.json"));
            a &= File.Exists(Path.Join(jsonFilesPath, "allTxtProcessed.json"));
            a &= File.Exists(Path.Join(jsonFilesPath, "tfidf.json"));
            a &= File.Exists(Path.Join(jsonFilesPath, "filesDates.json"));
            a &= File.Exists(Path.Join(jsonFilesPath, "idf.json"));
            a &= File.Exists(Path.Join(jsonFilesPath, "idfSteamed.json"));

            return a; 
        }
	//verifica las fechas de los archivos
        public void verifyFiles()
        {
            List<string> actualicedFilesNames = new();
            var txtDirs = Directory.EnumerateFiles(contentPath, "*.txt");

            foreach (var dir in txtDirs)
            {
                string fileName = Path.GetFileName(dir);
                fileName = Path.ChangeExtension(fileName, null);

                actualicedFilesNames.Add(fileName);
            }

            foreach(var fileName in actualicedFilesNames)
            {
                if (!FileNames.Contains(fileName))
                {
                    DocumentsToChange.Add(fileName,1);
                }
            }

            foreach (var fileName in FileNames)
            {
                if (!actualicedFilesNames.Contains(fileName))
                {
                    DocumentsToChange.Add(fileName, -1);
                }
            }

            if (DocumentsToChange.Count > 0)
            {
                foreach(var item in DocumentsToChange)
                {
                    if(item.Value == -1)
                    {
                        File.Delete(Path.Join(contentPathFinal,item.Key+".txt"));
                    }

                }


                //Console.WriteLine(DocumentsToChange.ElementAt(0).Key);
                Console.WriteLine("hubo cambios");
                Process b = new();
                deserialize(true);
            }
            else
            {
                var txtDirs2 = Directory.EnumerateFiles(contentPath, "*.txt");

                foreach (var dir in txtDirs2)
                {

                    string fileName = Path.GetFileName(dir);

                    fileName = Path.ChangeExtension(fileName, null);

                    if (filesDates[fileName]!=File.GetLastWriteTime(dir).ToLongTimeString())
                    {
                        Console.WriteLine("hubo cambios en un documento");
                        Process c = new();
                        deserialize(true);
                    }
                }
                Console.WriteLine("no hubo cambios en ningun documento");
            }
            Console.WriteLine("no hubo cambios");
            
            DocumentsToChange.Clear();
        }
	} 
}

