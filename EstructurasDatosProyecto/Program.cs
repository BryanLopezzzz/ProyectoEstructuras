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
        // Carpeta donde estarán los documentos .txt
        string outputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documentos");
        Directory.CreateDirectory(outputFolder);

        // Si quieres usar crawler, solo se ejecuta si no hay documentos
        bool needCrawl = Directory.GetFiles(outputFolder, "*.txt").Length == 0;

        if (needCrawl)
        {
            Console.WriteLine("No hay documentos. Ejecutando crawler...");

            var crawler = new WebCrawler();
            var urls = new ListaDobleCircular<string>();
            urls.Insertar("https://www.revistas.una.ac.cr/index.php/uniciencia");
            urls.Insertar("https://www.una.ac.cr/");
            urls.Insertar("https://www.revistas.una.ac.cr/");

            await crawler.IniciarRastreoAsync(outputFolder, urls);
            Console.WriteLine($"Documentos generados por el crawler: {crawler.ObtenerMapaDocumentos().Count()}");
        }

        // Cargar documentos directamente desde la carpeta "Documentos"
        var docMap = new ListaDobleCircular<Document>();
        string[] archivos = Directory.GetFiles(outputFolder, "*.txt");

        int id = 1;
        foreach (var archivo in archivos)
        {
            docMap.Insertar(new Document
            {
                DocId = id++,
                Url = "Archivo local",
                FileName = Path.GetFileName(archivo)
            });
        }

        Console.WriteLine($"Documentos cargados: {docMap.Count()}");
        if (docMap.Count() == 0)
        {
            Console.WriteLine("No se puede construir el índice, no hay documentos.");
            return;
        }

        // Construir índice invertido
        Console.WriteLine("\nConstruyendo índice invertido...");
        var builder = new IndexBuilder();
        var index = builder.BuildIndex(outputFolder, docMap);

        Console.WriteLine("\nÍndice invertido generado correctamente.");
        Console.WriteLine($"Número de documentos indexados: {docMap.Count()}");
    }
}