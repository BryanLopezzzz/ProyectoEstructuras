using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EstructurasDatosProyecto.Models;

namespace EstructurasDatosProyecto.Crawling
{
    public class WebCrawler
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly HashSet<string> _visited = new HashSet<string>();
        private readonly ListaDobleCircular<Documento> _documentos; // Lista de documentos procesados
        private readonly int _maxDepth = 6;
        private string _outputFolder;
        private readonly HashSet<string> _stopWords = new HashSet<string> {   
        "el","la","los","las","de","y","que","a","en","un","una","por","con",
        "no","es","al","lo","se","del","como","su","para","más","o","pero","algo","todos",
        "si","ya","este","esta","son","fue","ha","sus","le","mi","porque","cuando","muy",
        "sin","sobre","también","me","hasta","hay","donde","mientras","quien","cual","cuales","quienes",
        "nos","ni","tiene","tienen","uno","dos","tres","años","entre","ese","esa","esos",
        "esas","otra","otras","ese","eso","estos","estas","año","día","días","mes","meses" };

        // Constructor
        public WebCrawler()
        {
            _documentos = new ListaDobleCircular<Documento>();
        }

        // Constructor con profundidad personalizada
        public WebCrawler(int maxDepth)
        {
            _documentos = new ListaDobleCircular<Documento>();
            _maxDepth = maxDepth;
        }

        public async Task StartCrawlAsync(string ruta, List<string> rootUrls, string patron = "")
        {
            Directory.CreateDirectory(_outputFolder = ruta);

            foreach (var url in rootUrls)
            {
                await CrawlAsync(url, 0, patron);
            }
        }

        private async Task CrawlAsync(string url, int depth, string patron)
        {
            try
            {
                if (depth > _maxDepth || _visited.Contains(url) || !url.StartsWith("http"))
                {
                    return;
                }
                if (string.IsNullOrEmpty(patron) || url.Contains(patron))
                {
                    Console.WriteLine($"[{depth}] Visitando: {url}");
                    _visited.Add(url);
                    string html = await _httpClient.GetStringAsync(url);

                    // Guardar HTML (opcional)
                    // string safeName = ToSafeFilename(url);
                    // File.WriteAllText(Path.Combine(_outputFolder, $"{safeName}.html"), html);

                    // Extraer texto y crear documento usando nuestras clases
                    string plainText = ExtractTextFromHtml(html);
                    Documento documento = CrearDocumentoDesdeUrl(url, plainText);

                    if (documento != null)
                    {
                        _documentos.Insertar(documento);

                        // Guardar documento procesado usando nuestra clase
                        GuardarDocumentoPropio(documento);
                    }

                    // Parsear enlaces y continuar
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    var links = doc.DocumentNode
                        .SelectNodes("//a[@href]")
                        ?.Select(a => a.GetAttributeValue("href", null))
                        ?? new List<string>();

                    foreach (var link in links)
                    {
                        string fullUrl = ResolveUrl(url, link);
                        await CrawlAsync(fullUrl, depth + 1, patron);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en {url}: {ex.Message}");
            }
        }

        private string ExtractTextFromHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return WebUtility.HtmlDecode(doc.DocumentNode.InnerText);
        }

        // Método adaptado para usar nuestras clases
        private Documento CrearDocumentoDesdeUrl(string url, string contenidoHtml)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(contenidoHtml))
                return null;

            // Crear documento con ID basado en URL
            string safeName = ToSafeFilename(url);
            string rutaArchivo = Path.Combine(_outputFolder, $"{safeName}.txt");

            Documento documento = new Documento(rutaArchivo);
            documento.setId("WEB_" + safeName);

            // Normalizar texto usando el método original
            string normalizedText = NormalizeText(contenidoHtml);
            documento.setContenido(normalizedText);

            // Procesar contenido para generar tokens (esto aplicará nuestro filtrado de stopwords)
            documento.procesarContenido();

            return documento;
        }

        // Guarda documento usando nuestra clase Documento
        private void GuardarDocumentoPropio(Documento documento)
        {
            try
            {
                string nombreArchivo = documento.getId() + ".txt";
                string rutaCompleta = Path.Combine(_outputFolder, nombreArchivo);

                // Reconstruir texto desde tokens procesados
                string[] tokens = documento.getTokens();
                string contenidoFinal = string.Join(" ", tokens);

                File.WriteAllText(rutaCompleta, contenidoFinal);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando documento {documento.getId()}: {ex.Message}");
            }
        }

        // Método original de normalización (mantener funcionalidad)
        private string NormalizeText(string text)
        {
            string lower = text.ToLowerInvariant();
            string clean = Regex.Replace(lower, @"[^\p{L}\p{Nd}]+", " "); // solo letras y números

            // Usar nuestras stopwords en lugar de las del HashSet original
            string noStop = string.Join(" ", clean
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(w => !StopWords.IsStopword(w))); // Usar nuestra clase StopWords

            return noStop;
        }

        private string ToSafeFilename(string url)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(url));
        }

        private string ResolveUrl(string baseUrl, string href)
        {
            try
            {
                var baseUri = new Uri(baseUrl);
                var fullUri = new Uri(baseUri, href);
                return fullUri.ToString();
            }
            catch
            {
                return "";
            }
        }

        // Métodos adicionales para integración con el proyecto
        public ListaDobleCircular<Documento> ObtenerDocumentos()
        {
            return _documentos;
        }

        public int ContarDocumentos()
        {
            return _documentos.Count();
        }

        public int ContarUrlsVisitadas()
        {
            return _visited.Count;
        }

        // Método para obtener documentos como lista para compatibilidad
        public List<Documento> ObtenerDocumentosComoLista()
        {
            List<Documento> lista = new List<Documento>();

            if (!_documentos.EstaVacia())
            {
                Iterator<Documento> iterador = _documentos.ObtenerIterator();
                while (iterador.TieneSiguiente())
                {
                    lista.Add(iterador.Siguiente());
                }
            }

            return lista;
        }

        // Método para limpiar datos
        public void LimpiarDatos()
        {
            _visited.Clear();
            // Para limpiar _documentos necesitaríamos un método Clear en ListaDobleCircular
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}