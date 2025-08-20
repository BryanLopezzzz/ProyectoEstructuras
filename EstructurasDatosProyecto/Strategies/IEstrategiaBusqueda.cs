using EstructurasDatosProyecto.Crawling;
using EstructurasDatosProyecto.Models;
using EstructurasDatosProyecto.Search;
using EstructurasDatosProyecto.Calculos;

namespace EstructurasDatosProyecto.Strategies
{
    public interface IEstrategiaBusqueda
    {
        // Ejecuta búsqueda y retorna documentos ordenados por relevancia
        ListaDobleCircular<ResultadoBusqueda> BuscarDocumentos(string consulta, ListaDobleCircular<TermInfo> terminos, ListaDobleCircular<Documento> documentos);

        // Convierte una consulta de texto en vector TF-IDF
        Vector CrearVectorConsulta(string consulta, ListaDobleCircular<TermInfo> terminos);

        // Convierte un documento en vector TF-IDF
        Vector CrearVectorDocumento(Documento documento, ListaDobleCircular<TermInfo> terminos);

        // Calcula similitud entre dos vectores
        double CalcularSimilitud(Vector vectorConsulta, Vector vectorDocumento);

        // Nombre de la estrategia para identificación
        string ObtenerNombreEstrategia();
    }
}
