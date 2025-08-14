using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using EstructurasDatosProyecto.Models;
using EstructurasDatosProyecto.Crawling;

namespace EstructurasDatosProyecto.Utils;

public class Persistence
{
    // Carga el mapa de documentos desde un archivo JSON (docmap.json).
    public static List<Document> LoadDocMap(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"⚠ No se encontró {filePath}, devolviendo lista vacía.");
            return new List<Document>();
        }

        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<List<Document>>(json) ?? new List<Document>();
    }
    
    // Guarda el índice invertido en formato JSON.
    public static void SaveIndex(string filePath, Dictionary<string, TermInfo> index)
    {
        // Convierte a JSON con identación
        string json = JsonConvert.SerializeObject(index, Formatting.Indented);

        File.WriteAllText(filePath, json);

        Console.WriteLine($"Índice guardado en: {filePath}");
    }
    
    // Carga un índice invertido desde archivo JSON.
    public static Dictionary<string, TermInfo> LoadIndex(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"No se encontró el archivo de índice: {filePath}");

        string json = File.ReadAllText(filePath);
        var index = JsonConvert.DeserializeObject<Dictionary<string, TermInfo>>(json);

        return index ?? new Dictionary<string, TermInfo>();
    }
}