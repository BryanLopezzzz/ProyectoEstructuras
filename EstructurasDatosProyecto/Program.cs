using EstructurasDatosProyecto.Utils;
using EstructurasDatosProyecto.Crawling;
using EstructurasDatosProyecto.Models;
using EstructurasDatosProyecto.Indexing;

class Program
{
    static async Task Main()
    {
        // Carpeta donde se guardan los documentos
        string outputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documentos");

        // Crear carpeta si no existe
        Directory.CreateDirectory(outputFolder);

        string docMapPath = Path.Combine(outputFolder, "docmap.json");

        // Verificar si necesitamos ejecutar el crawler
        bool needCrawl = !File.Exists(docMapPath) || Directory.GetFiles(outputFolder, "*.txt").Length == 0;

        if (needCrawl)
        {
            Console.WriteLine("No hay datos o falta docmap.json. Ejecutando crawler...");

            var crawler = new WebCrawler();
            await crawler.StartCrawlAsync(
                outputFolder,
                new List<string>
                {
                    "https://www.revistas.una.ac.cr/index.php/uniciencia",
                    "https://www.una.ac.cr/",
                    "https://www.revistas.una.ac.cr/"
                }
            );
        }
        else
        {
            int count = Directory.GetFiles(outputFolder, "*.txt").Length;
            Console.WriteLine($"Se encontraron {count} documentos en la carpeta.");
        }

        // Cargar DocumentMeta
        var docMap = Persistence.LoadDocMap(docMapPath);
        if (docMap.Count == 0)
        {
            Console.WriteLine("⚠ Advertencia: docmap.json está vacío. No se puede construir el índice.");
            return;
        }

        // Construir índice invertido
        Console.WriteLine("\nConstruyendo índice invertido...\n");
        var builder = new IndexBuilder();
        var invertedIndex = builder.BuildIndex(outputFolder, docMap);

        // Mostrar resultados de prueba
        foreach (var kvp in invertedIndex.GetIndex())
        {
            Console.WriteLine($"{kvp.Key} -> DF: {kvp.Value.Df}, Docs: {string.Join(", ", kvp.Value.Postings.Select(p => $"{p.DocId}({p.Tf})"))}");
        }

        // Guardar índice
        Persistence.SaveIndex(Path.Combine(outputFolder, "index.json"), invertedIndex.GetIndex());

        Console.WriteLine("\nÍndice invertido generado y guardado correctamente.");
    }
}