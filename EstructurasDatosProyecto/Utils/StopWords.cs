namespace EstructurasDatosProyecto.Utils;

public class StopWords
{
    private static string[] _stopWords = new string[]
    {
        "el","la","los","las","de","y","que","a","en","un","una","por","con",
        "no","es","al","lo","se","del","como","su","para","más","o","pero","algo","todos",
        "si","ya","este","esta","son","fue","ha","sus","le","mi","porque","cuando","muy",
        "sin","sobre","también","me","hasta","hay","donde","mientras","quien","cual","cuales","quienes",
        "nos","ni","tiene","tienen","uno","dos","tres","años","entre","ese","esa","esos",
        "esas","otra","otras","ese","eso","estos","estas","año","día","días","mes","meses"
    };

    public static bool IsStopword(string word)
    {
        for (int i = 0; i < _stopWords.Length; i++)
        {
            if (_stopWords[i] == word) return true;
        }
        return false;
    }

    // Cargar stopwords desde archivo opcional
    public static void LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath)) return;

        string[] lines = File.ReadAllLines(filePath);
        _stopWords = new string[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            _stopWords[i] = lines[i].Trim().ToLowerInvariant();
        }
    }
}