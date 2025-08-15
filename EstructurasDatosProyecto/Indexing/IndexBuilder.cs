namespace EstructurasDatosProyecto.Indexing;
using EstructurasDatosProyecto.Utils;
using EstructurasDatosProyecto.Crawling;
using EstructurasDatosProyecto.Models;

public class IndexBuilder
{
    public InvertedIndex BuildIndex(string docsFolder, ListaDobleCircular<Document> docMap)
    {
        var index = new InvertedIndex();

        docMap.Recorrer(doc =>
        {
            string filePath = Path.Combine(docsFolder, doc.FileName);
            if (!File.Exists(filePath))
                return;

            string[] tokens = Tokenizer.TokenizeFile(filePath);

            foreach (var token in tokens)
            {
                index.AddTerm(token, doc.DocId);
            }
        });

        return index;
    }
}