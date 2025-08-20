using EstructurasDatosProyecto.Crawling;
using EstructurasDatosProyecto.Models;

namespace EstructurasDatosProyecto.Strategies
{
    public interface IEstrategiaIndex
    {
        // Procesa un documento individual y actualiza la lista de términos
        void ProcesarDocumento(Documento documento, ListaDobleCircular<TermInfo> terminos, ListaDobleCircular<Documento> documentos);

        // Procesa múltiples documentos y construye el índice completo
        void ProcesarDocumentos(ListaDobleCircular<Documento> documentos, ListaDobleCircular<TermInfo> terminos);

        // Calcula TF-IDF para todos los términos - ejecutar después de procesar documentos
        void CalcularTfIdf(ListaDobleCircular<TermInfo> terminos, int totalDocumentos);

        // Nombre de la estrategia para identificación
        string ObtenerNombreEstrategia();
    }
}
