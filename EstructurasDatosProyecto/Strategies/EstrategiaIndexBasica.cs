using EstructurasDatosProyecto.Crawling;
using EstructurasDatosProyecto.Models;

namespace EstructurasDatosProyecto.Strategies
{
    public class EstrategiaIndexBasica : IEstrategiaIndex
    {
        public void ProcesarDocumento(Documento documento, ListaDobleCircular<TermInfo> terminos, ListaDobleCircular<Documento> documentos)
        {
            if (documento == null || !documento.estaProcesado()) return;

            // Agregar documento a la colección si no existe
            if (!ExisteDocumento(documentos, documento))
            {
                documentos.Insertar(documento);
            }

            // Obtener términos únicos del documento
            string[] terminosDocumento = documento.getTerminosUnicos();

            // Procesar cada término único del documento
            for (int i = 0; i < terminosDocumento.Length; i++)
            {
                string termino = terminosDocumento[i];
                if (termino == null || termino == "") continue;

                // Buscar si el término ya existe en el índice
                TermInfo terminoExistente = BuscarTermino(terminos, termino);

                if (terminoExistente == null)
                {
                    // Crear nuevo TermInfo si no existe
                    terminoExistente = new TermInfo(termino);
                    terminos.Insertar(terminoExistente);
                }

                // Calcular frecuencia del término en este documento
                int frecuencia = documento.contarFrecuenciaTermino(termino);

                // Crear posting y agregarlo al término
                Posting nuevoPosting = new Posting(documento.getId(), frecuencia);
                terminoExistente.AgregarPosting(nuevoPosting);
            }
        }

        public void ProcesarDocumentos(ListaDobleCircular<Documento> documentos, ListaDobleCircular<TermInfo> terminos)
        {
            if (documentos.EstaVacia()) return;

            Iterator<Documento> iterador = documentos.ObtenerIterator();

            // Crear lista temporal para almacenar documentos durante el procesamiento
            ListaDobleCircular<Documento> documentosTemp = new ListaDobleCircular<Documento>();

            while (iterador.TieneSiguiente())
            {
                Documento documento = iterador.Siguiente();
                ProcesarDocumento(documento, terminos, documentosTemp);
            }
        }

        public void CalcularTfIdf(ListaDobleCircular<TermInfo> terminos, int totalDocumentos)
        {
            if (terminos.EstaVacia() || totalDocumentos <= 0) return;

            Iterator<TermInfo> iterador = terminos.ObtenerIterator();

            while (iterador.TieneSiguiente())
            {
                TermInfo termino = iterador.Siguiente();

                // Calcular IDF para el término
                termino.CalcularIdf(totalDocumentos);

                // Actualizar TF-IDF de todos los postings del término
                termino.ActualizarTfIdfPostings();
            }
        }

        public string ObtenerNombreEstrategia()
        {
            return "Estrategia de Indexación Básica TF-IDF";
        }

        // Método auxiliar para buscar un término en la lista
        private TermInfo BuscarTermino(ListaDobleCircular<TermInfo> terminos, string termino)
        {
            if (terminos.EstaVacia()) return null;

            Iterator<TermInfo> iterador = terminos.ObtenerIterator();

            while (iterador.TieneSiguiente())
            {
                TermInfo terminoActual = iterador.Siguiente();
                if (terminoActual.termino.Equals(termino))
                {
                    return terminoActual;
                }
            }

            return null;
        }

        // Método auxiliar para verificar si un documento ya existe en la lista
        private bool ExisteDocumento(ListaDobleCircular<Documento> documentos, Documento documento)
        {
            if (documentos.EstaVacia()) return false;

            Iterator<Documento> iterador = documentos.ObtenerIterator();

            while (iterador.TieneSiguiente())
            {
                Documento docActual = iterador.Siguiente();
                if (docActual.esIgual(documento))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
