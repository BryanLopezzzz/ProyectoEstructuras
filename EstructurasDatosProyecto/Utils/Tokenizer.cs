using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

namespace EstructurasDatosProyecto.Utils;

public class Tokenizer
{
    public static string[] Tokenize(string text)
    {
        if (string.IsNullOrEmpty(text)) return new string[0];

        string lower = text.ToLowerInvariant();
        string noAccents = RemoveAccents(lower);
        string clean = Regex.Replace(noAccents, @"[^\p{L}\p{Nd}]+", " ");

        string[] words = clean.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        // filtrar stopwords
        string[] result = new string[words.Length];
        int count = 0;
        for (int i = 0; i < words.Length; i++)
        {
            if (!StopWords.IsStopword(words[i]))
            {
                result[count++] = words[i];
            }
        }

        string[] tokens = new string[count];
        for (int i = 0; i < count; i++) tokens[i] = result[i];
        return tokens;
    }

    public static string[] TokenizeFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Archivo no encontrado: {filePath}");

        string content = File.ReadAllText(filePath);
        return Tokenize(content);
    }

    private static string RemoveAccents(string input)
    {
        var normalized = input.Normalize(NormalizationForm.FormD);
        char[] buffer = new char[normalized.Length];
        int idx = 0;

        foreach (char c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                buffer[idx++] = c;
            }
        }

        return new string(buffer, 0, idx).Normalize(NormalizationForm.FormC);
    }
}