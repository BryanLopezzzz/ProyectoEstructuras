namespace EstructurasDatosProyecto.Indexing;
using EstructurasDatosProyecto.Utils;
using EstructurasDatosProyecto.Crawling;
using EstructurasDatosProyecto.Models;

public class IndexBuilder
{
    public InvertedIndex BuildIndex(string docsFolder, List<Document> docMap)
    {
        var index = new InvertedIndex();

        foreach (var doc in docMap)
        {
            string filePath = Path.Combine(docsFolder, doc.FileName);
            if (!File.Exists(filePath))
                continue;

            // Tokenizar directamente usando la clase estática
            var tokens = Tokenizer.TokenizeFile(filePath);

            foreach (var token in tokens)
            {
                index.AddTerm(token, doc.DocId);
            }
        }

        return index;
    }
}