using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using EstructurasDatosProyecto.Models;
using EstructurasDatosProyecto.Crawling;

namespace EstructurasDatosProyecto.Utils;

public class Persistence
{
    public static void SaveDocMap(string filePath, ListaDobleCircular<Document> docs)
    {
        using StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8);
        docs.Recorrer(doc =>
        {
            sw.WriteLine($"{doc.DocId}|{doc.Url}|{doc.FileName}");
        });
    }

    public static void LoadDocMap(string filePath, ListaDobleCircular<Document> lista)
    {
        if (!File.Exists(filePath)) return;

        foreach (var linea in File.ReadAllLines(filePath, Encoding.UTF8))
        {
            if (string.IsNullOrWhiteSpace(linea)) continue;
            string[] partes = linea.Split('|');
            if (partes.Length != 3) continue;

            lista.Insertar(new Document
            {
                DocId = int.Parse(partes[0]),
                Url = partes[1],
                FileName = partes[2]
            });
        }
    }
}