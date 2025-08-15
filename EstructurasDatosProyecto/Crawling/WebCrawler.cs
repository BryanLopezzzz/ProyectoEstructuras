using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EstructurasDatosProyecto.Models;

namespace EstructurasDatosProyecto.Crawling;

public class WebCrawler
    {
        private const int PUERTO_HTTP = 80;
        private const int PROFUNDIDAD_MAXIMA = 6;

        private readonly HashSet<string> urlsVisitadas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private string directorioSalida = "salida";
        private string dominioPrincipal = "";

        private int contadorIdDocumentos = 1;
        private readonly ListaDobleCircular<Document> listaDocumentos = new ListaDobleCircular<Document>();

        public async Task IniciarRastreoAsync(string carpetaSalida, ListaDobleCircular<string> urlsIniciales, string filtroUrl = "")
        {
            Directory.CreateDirectory(directorioSalida = carpetaSalida);

            // Obtener primer elemento para dominio principal
            string primeraUrl = urlsIniciales.Buscar(u => true); // devuelve el primer elemento
            dominioPrincipal = new Uri(primeraUrl).Host;

            urlsIniciales.Recorrer(async url =>
            {
                await RastrearPaginaAsync(url, 0, filtroUrl);
            });
        }

        private async Task RastrearPaginaAsync(string url, int nivel, string filtroUrl)
        {
            if (!EsUrlValida(url, nivel, filtroUrl))
                return;

            Console.WriteLine("[" + nivel + "] Visitando: " + url);
            urlsVisitadas.Add(url);

            string html = await DescargarHtmlAsync(url);
            if (string.IsNullOrEmpty(html))
                return;

            string nombreArchivo = ConvertirUrlASegura(url) + ".html";
            GuardarHtml(nombreArchivo, html);

            listaDocumentos.Insertar(new Document
            {
                DocId = contadorIdDocumentos++,
                Url = url,
                FileName = nombreArchivo
            });

            foreach (string enlace in ExtraerEnlaces(html))
            {
                string enlaceAbsoluto = ResolverUrl(url, enlace);
                if (!string.IsNullOrEmpty(enlaceAbsoluto))
                    await RastrearPaginaAsync(enlaceAbsoluto, nivel + 1, filtroUrl);
            }
        }

        private bool EsUrlValida(string url, int nivel, string filtroUrl)
        {
            if (nivel > PROFUNDIDAD_MAXIMA) return false;
            if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return false;
            url = NormalizarUrl(url);
            if (urlsVisitadas.Contains(url)) return false;
            if (new Uri(url).Host != dominioPrincipal) return false;
            if (!string.IsNullOrEmpty(filtroUrl) && !url.Contains(filtroUrl, StringComparison.OrdinalIgnoreCase)) return false;
            return true;
        }

        private async Task<string> DescargarHtmlAsync(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                int puerto = uri.Port > 0 ? uri.Port : PUERTO_HTTP;

                TcpClient cliente = new TcpClient();
                await cliente.ConnectAsync(uri.Host, puerto);

                NetworkStream flujo = cliente.GetStream();
                StreamWriter escritor = new StreamWriter(flujo, Encoding.ASCII) { NewLine = "\r\n", AutoFlush = true };
                StreamReader lector = new StreamReader(flujo, Encoding.UTF8);

                await escritor.WriteLineAsync("GET " + uri.PathAndQuery + " HTTP/1.1");
                await escritor.WriteLineAsync("Host: " + uri.Host);
                await escritor.WriteLineAsync("Connection: close");
                await escritor.WriteLineAsync("");
                await escritor.FlushAsync();

                string respuesta = await lector.ReadToEndAsync();

                lector.Close();
                escritor.Close();
                flujo.Close();
                cliente.Close();

                int indiceHtml = respuesta.IndexOf("\r\n\r\n", StringComparison.Ordinal);
                return indiceHtml >= 0 ? respuesta.Substring(indiceHtml + 4) : respuesta;
            }
            catch
            {
                return "";
            }
        }

        private void GuardarHtml(string nombreArchivo, string contenido)
        {
            string ruta = Path.Combine(directorioSalida, nombreArchivo);
            File.WriteAllText(ruta, contenido);
        }

        private static IEnumerable<string> ExtraerEnlaces(string html)
        {
            if (string.IsNullOrEmpty(html)) yield break;

            Regex regex = new Regex(@"(?is)<a\s+[^>]*?href\s*=\s*(?:""([^""]*)""|'([^']*)'|([^\s>]+))");
            foreach (Match m in regex.Matches(html))
            {
                string href = m.Groups[1].Success ? m.Groups[1].Value :
                              m.Groups[2].Success ? m.Groups[2].Value :
                              m.Groups[3].Value;

                if (string.IsNullOrWhiteSpace(href)) continue;
                if (href.StartsWith("#") || href.StartsWith("javascript:") ||
                    href.StartsWith("mailto:") || href.StartsWith("tel:") || href.StartsWith("data:"))
                    continue;

                yield return href.Trim();
            }
        }

        private static string ResolverUrl(string baseUrl, string href)
        {
            try
            {
                Uri baseUri = new Uri(baseUrl);
                Uri absoluta = new Uri(baseUri, href);
                UriBuilder builder = new UriBuilder(absoluta) { Fragment = "" };
                return builder.Uri.ToString();
            }
            catch
            {
                return "";
            }
        }

        private static string NormalizarUrl(string url)
        {
            try
            {
                Uri u = new Uri(url);
                UriBuilder builder = new UriBuilder(u) { Fragment = "" };
                builder.Host = builder.Host.ToLowerInvariant();
                return builder.Uri.ToString();
            }
            catch
            {
                return url;
            }
        }

        private static string ConvertirUrlASegura(string url)
        {
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(url));
            foreach (char c in Path.GetInvalidFileNameChars())
                base64 = base64.Replace(c, '_');
            return base64;
        }

        public ListaDobleCircular<Document> ObtenerMapaDocumentos()
        {
            return listaDocumentos;
        }
    }