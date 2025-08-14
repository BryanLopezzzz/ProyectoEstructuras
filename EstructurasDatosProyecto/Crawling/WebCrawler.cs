using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EstructurasDatosProyecto.Crawling;

public class WebCrawler
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly HashSet<string> _visited = new HashSet<string>();
        private readonly int _maxDepth = 6;
        private string _outputFolder;
        private int _docIdCounter = 1; 
        private List<Document> _docMap = new List<Document>();

        private string _rootDomain; // Nuevo: dominio raíz

        public async Task StartCrawlAsync(string outputFolder, List<string> rootUrls, string patron = "")
        {
            Directory.CreateDirectory(_outputFolder = outputFolder);

            // Tomar el dominio de la primera URL para limitar el rastreo
            _rootDomain = new Uri(rootUrls.First()).Host;

            foreach (var url in rootUrls)
            {
                await CrawlAsync(url, 0, patron);
            }

            // Guardar mapa de documentos
            string docMapPath = Path.Combine(_outputFolder, "docmap.json");
            File.WriteAllText(docMapPath, JsonConvert.SerializeObject(_docMap, Formatting.Indented));

            Console.WriteLine($"Crawling completado. Documentos guardados en {_outputFolder}");
        }

        private async Task CrawlAsync(string url, int depth, string patron)
        {
            try
            {
                if (depth > _maxDepth || _visited.Contains(url) || !url.StartsWith("http"))
                    return;

                // Limitar a mismo dominio
                if (new Uri(url).Host != _rootDomain)
                    return;

                if (string.IsNullOrEmpty(patron) || url.Contains(patron))
                {
                    Console.WriteLine($"[{depth}] Visitando: {url}");
                    _visited.Add(url);

                    string html = await _httpClient.GetStringAsync(url);
                    string safeName = ToSafeFilename(url);

                    string plainText = ExtractTextFromHtml(html);

                    string filePath = Path.Combine(_outputFolder, $"{safeName}.txt");
                    File.WriteAllText(filePath, plainText);

                    _docMap.Add(new Document()
                    {
                        DocId = _docIdCounter++,
                        Url = url,
                        FileName = $"{safeName}.txt"
                    });

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

        private string ToSafeFilename(string url)
        {
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(url));
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                base64 = base64.Replace(c, '_');
            }
            return base64;
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
    }