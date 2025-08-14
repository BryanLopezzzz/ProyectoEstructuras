namespace EstructurasDatosProyecto.Utils;

public class StopWords
{
    private static readonly HashSet<string> _stopWords = new HashSet<string>
    {
        "el", "la", "los", "las", "de", "y", "que", "a", "en", "un", "una", "por", "con", 
        "no", "es", "al", "lo", "se", "del", "como", "su", "para", "más", "o", "pero"
    };

    public static bool IsStopword(string word)
    {
        return _stopWords.Contains(word);
    }

    // Cargar stopwords desde un archivo opcional
    public static void LoadFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            _stopWords.Clear();
            foreach (var line in File.ReadAllLines(filePath))
            {
                string word = line.Trim().ToLowerInvariant();
                if (!string.IsNullOrWhiteSpace(word))
                    _stopWords.Add(word);
            }
        }
    }
}