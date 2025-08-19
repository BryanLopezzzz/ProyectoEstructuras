namespace EstructurasDatosProyecto.Indexing;
using EstructurasDatosProyecto.Utils;
using EstructurasDatosProyecto.Crawling;
using EstructurasDatosProyecto.Models;

public class IndexBuilder
{
    public InvertedIndex BuildIndex(string docsFolder, ListaDobleCircular<Documento> docMap)
    {
        var index = new InvertedIndex();

        docMap.Recorrer(doc =>
        {
            // Recuperar archivo
            string filePath = Path.Combine(docsFolder, doc.FileName);
            if (!File.Exists(filePath)) return;

            string[] tokens = Tokenizer.TokenizeFile(filePath);

            for (int i = 0; i < tokens.Length; i++)
            {
                index.AddTerm(tokens[i], doc.DocId);
            }
        });

        return index;
    }
}