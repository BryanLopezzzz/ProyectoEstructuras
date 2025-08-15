using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using EstructurasDatosProyecto.Models;
using EstructurasDatosProyecto.Crawling;

namespace EstructurasDatosProyecto.Utils;

public class Persistence
{
    // Guarda documentos en JSON usando ListaDobleCircular
    public static void SaveDocMap(string filePath, ListaDobleCircular<Document> docs)
    {
        using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            sw.WriteLine("[");
            bool first = true;

            docs.Recorrer(doc =>
            {
                if (!first) sw.WriteLine(",");
                first = false;
                sw.Write(SerializeDocument(doc));
            });

            sw.WriteLine();
            sw.WriteLine("]");
        }
    }

    // Carga documentos desde JSON en ListaDobleCircular
    public static void LoadDocMap(string filePath, ListaDobleCircular<Document> docs)
    {
        if (!File.Exists(filePath)) return;

        string json = File.ReadAllText(filePath);
        var matches = Regex.Matches(json, @"\{[^\}]*\}");
        foreach (Match m in matches)
        {
            Document doc = ParseDocument(m.Value);
            docs.Insertar(doc);
        }
    }

    private static string SerializeDocument(Document doc)
    {
        return $"  {{ \"DocId\": {doc.DocId}, \"Url\": \"{doc.Url}\", \"FileName\": \"{doc.FileName}\" }}";
    }

    private static Document ParseDocument(string json)
    {
        Document doc = new Document();

        var idMatch = Regex.Match(json, @"""DocId"":\s*(\d+)");
        var urlMatch = Regex.Match(json, @"""Url"":\s*""([^""]+)""");
        var fileMatch = Regex.Match(json, @"""FileName"":\s*""([^""]+)""");

        if (idMatch.Success) doc.DocId = int.Parse(idMatch.Groups[1].Value);
        if (urlMatch.Success) doc.Url = urlMatch.Groups[1].Value;
        if (fileMatch.Success) doc.FileName = fileMatch.Groups[1].Value;

        return doc;
    }
}