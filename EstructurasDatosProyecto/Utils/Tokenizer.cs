using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

namespace EstructurasDatosProyecto.Utils;

public class Tokenizer
{
    /// Esta clase procesa el texto que antes hacia parte de WebCrawler: minúsculas, elimina tildes, signos y stopwords.
    public static List<string> Tokenize(string text)
    {
        // minúsculas
        string lower = text.ToLowerInvariant();

        // eliminar tildes y caracteres especiales
        string noAccents = RemoveAccents(lower);

        // sustituir todo lo que no sea letra o número por espacios
        string clean = Regex.Replace(noAccents, @"[^\p{L}\p{Nd}]+", " ");

        // divide en palabras y filtrar stopwords
        var tokens = clean
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(word => !StopWords.IsStopword(word))
            .ToList();

        return tokens;
    }
    
    // procesa un archivo de texto y devuelve tokens listos para indexar.
    
    public static List<string> TokenizeFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Archivo no encontrado: {filePath}");

        string content = File.ReadAllText(filePath);
        return Tokenize(content);
    }
    
    // quita acentos.
    private static string RemoveAccents(string input)
    {
        var normalized = input.Normalize(NormalizationForm.FormD);
        var chars = normalized.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark);
        return new string(chars.ToArray()).Normalize(NormalizationForm.FormC);
    }
}