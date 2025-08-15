using EstructurasDatosProyecto.Utils;
using EstructurasDatosProyecto.Crawling;
using EstructurasDatosProyecto.Models;
using EstructurasDatosProyecto.Indexing;
using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        string outputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documentos");
        Directory.CreateDirectory(outputFolder);

        string docMapPath = Path.Combine(outputFolder, "docmap.json");

        bool needCrawl = !File.Exists(docMapPath) || Directory.GetFiles(outputFolder, "*.txt").Length == 0;

        if (needCrawl)
        {
            Console.WriteLine("No hay datos o falta docmap.json. Ejecutando crawler...");

            var crawler = new WebCrawler();

            var urls = new ListaDobleCircular<string>();
            urls.Insertar("https://www.revistas.una.ac.cr/index.php/uniciencia");
            urls.Insertar("https://www.una.ac.cr/");
            urls.Insertar("https://www.revistas.una.ac.cr/");

            await crawler.IniciarRastreoAsync(outputFolder, urls);

            // Guardar docmap
            Persistence.SaveDocMap(docMapPath, crawler.ObtenerMapaDocumentos());
        }

        // Cargar DocumentMeta en lista circular
        var docMap = new ListaDobleCircular<Document>();
        Persistence.LoadDocMap(docMapPath, docMap);

        if (docMap.Count() == 0)
        {
            Console.WriteLine("⚠ docmap.json está vacío. No se puede construir el índice.");
            return;
        }

        Console.WriteLine("\nConstruyendo índice invertido...\n");

        var builder = new IndexBuilder();
        builder.BuildIndex(outputFolder, docMap);

        Console.WriteLine("\nÍndice invertido generado correctamente.");
    }
}