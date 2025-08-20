using EstructurasDatosProyecto.Crawling;
using EstructurasDatosProyecto.Models;
using EstructurasDatosProyecto.Search;
using EstructurasDatosProyecto.Calculos;
using EstructurasDatosProyecto.Processing;

namespace EstructurasDatosProyecto.Strategies
{
    public class EstrategiaBusquedaCoseno : IEstrategiaBusqueda
    {
        private Normalizer normalizadorConsulta;        // Normalizar texto de consulta
        private Tokenizer tokenizadorConsulta;          // Tokenizar consulta normalizada

        // Constructor - inicializa procesadores de texto
        public EstrategiaBusquedaCoseno()
        {
            normalizadorConsulta = new Normalizer();    // Crear normalizador
            tokenizadorConsulta = new Tokenizer();      // Crear tokenizador
        }

        // Ejecuta búsqueda completa y retorna resultados ordenados por similitud coseno
        public ListaDobleCircular<ResultadoBusqueda> BuscarDocumentos(string consulta, ListaDobleCircular<TermInfo> terminos, ListaDobleCircular<Documento> documentos)
        {
            ListaDobleCircular<ResultadoBusqueda> resultados = new ListaDobleCircular<ResultadoBusqueda>();

            if (consulta == null || consulta == "" || terminos.EstaVacia() || documentos.EstaVacia())
            {
                return resultados;                      // Retornar lista vacia si parametros invalidos
            }

            // Paso 1: Crear vector TF-IDF de la consulta
            Vector vectorConsulta = CrearVectorConsulta(consulta, terminos);
            if (vectorConsulta.EstaVacio() || vectorConsulta.EsCero())
            {
                return resultados;                      // Si consulta no tiene terminos validos
            }

            // Paso 2: Evaluar cada documento y calcular similitud
            Iterator<Documento> iteradorDocs = documentos.ObtenerIterator();
            while (iteradorDocs.TieneSiguiente())
            {
                Documento documento = iteradorDocs.Siguiente();

                // Crear vector TF-IDF del documento
                Vector vectorDocumento = CrearVectorDocumento(documento, terminos);

                // Calcular similitud coseno entre consulta y documento
                double similitud = CalcularSimilitud(vectorConsulta, vectorDocumento);

                // Solo incluir resultados con similitud mayor a cero
                if (similitud > 0.0)
                {
                    ResultadoBusqueda resultado = new ResultadoBusqueda(documento, similitud, consulta);
                    resultados.Insertar(resultado);
                }
            }

            // Paso 3: Ordenar resultados por similitud (mayor a menor)
            return OrdenarResultadosPorSimilitud(resultados);
        }

        // Convierte consulta de texto en vector TF-IDF usando terminos del indice
        public Vector CrearVectorConsulta(string consulta, ListaDobleCircular<TermInfo> terminos)
        {
            if (consulta == null || consulta == "" || terminos.EstaVacia())
            {
                return new Vector();                    // Vector vacio si parametros invalidos
            }

            // Procesar consulta: normalizar y tokenizar
            string consultaNormalizada = normalizadorConsulta.NormalizarTexto(consulta);
            string[] tokensConsulta = tokenizadorConsulta.TokenizarTexto(consultaNormalizada);

            if (tokensConsulta.Length == 0)
            {
                return new Vector();                    // Vector vacio si no hay tokens
            }

            // Crear vector con dimension igual al numero de terminos en el indice
            int dimensionVector = terminos.Count();
            Vector vectorConsulta = new Vector(dimensionVector);

            // Llenar vector con valores TF-IDF de cada termino
            Iterator<TermInfo> iteradorTerminos = terminos.ObtenerIterator();
            int posicion = 0;

            while (iteradorTerminos.TieneSiguiente())
            {
                TermInfo termino = iteradorTerminos.Siguiente();

                // Contar frecuencia del termino en la consulta (TF)
                int frecuenciaEnConsulta = ContarFrecuenciaTermino(tokensConsulta, termino.termino);

                // Calcular TF-IDF: frecuencia * IDF del termino
                double tfIdf = frecuenciaEnConsulta * termino.ObtenerIdf();

                // Establecer valor en la posicion correspondiente del vector
                vectorConsulta.EstablecerElemento(posicion, tfIdf);
                posicion++;
            }

            return vectorConsulta;                      // Retornar vector de consulta completo
        }

        // Convierte documento en vector TF-IDF usando terminos del indice
        public Vector CrearVectorDocumento(Documento documento, ListaDobleCircular<TermInfo> terminos)
        {
            if (documento == null || terminos.EstaVacia())
            {
                return new Vector();                    // Vector vacio si parametros invalidos
            }

            // Crear vector con dimension igual al numero de terminos en el indice
            int dimensionVector = terminos.Count();
            Vector vectorDocumento = new Vector(dimensionVector);

            // Llenar vector con valores TF-IDF de cada termino del indice
            Iterator<TermInfo> iteradorTerminos = terminos.ObtenerIterator();
            int posicion = 0;

            while (iteradorTerminos.TieneSiguiente())
            {
                TermInfo termino = iteradorTerminos.Siguiente();

                // Buscar posting del documento en este termino
                Posting posting = termino.BuscarPostingPorDocumento(documento.getId());

                double tfIdf = 0.0;                     // Valor por defecto si documento no tiene termino
                if (posting != null)                    // Si documento contiene el termino
                {
                    tfIdf = posting.ObtenerTfIdf();     // Obtener valor TF-IDF calculado
                }

                // Establecer valor en la posicion correspondiente del vector
                vectorDocumento.EstablecerElemento(posicion, tfIdf);
                posicion++;
            }

            return vectorDocumento;                     // Retornar vector del documento completo
        }

        // Calcula similitud coseno entre dos vectores usando formula: cos(θ) = (A * B) / (||A|| * ||B||)
        public double CalcularSimilitud(Vector vectorConsulta, Vector vectorDocumento)
        {
            if (vectorConsulta.EstaVacio() || vectorDocumento.EstaVacio())
            {
                return 0.0;                             // Sin similitud si alguno esta vacio
            }

            if (vectorConsulta.ObtenerDimension() != vectorDocumento.ObtenerDimension())
            {
                return 0.0;                             // Sin similitud si dimensiones diferentes
            }

            // Calcular producto punto A * B (numerador)
            double productoPunto = vectorConsulta * vectorDocumento;

            // Calcular magnitudes ||A|| y ||B|| (denominador)
            double magnitudConsulta = vectorConsulta.CalcularMagnitud();
            double magnitudDocumento = vectorDocumento.CalcularMagnitud();

            // Validar que las magnitudes no sean cero para evitar division por cero
            if (magnitudConsulta == 0.0 || magnitudDocumento == 0.0)
            {
                return 0.0;                             // Sin similitud si alguna magnitud es cero
            }

            // Aplicar formula del coseno: cos(θ) = (A * B) / (||A|| * ||B||)
            double similitudCoseno = productoPunto / (magnitudConsulta * magnitudDocumento);

            return similitudCoseno;                     // Retornar valor de similitud [0.0, 1.0]
        }

        // Retorna nombre de la estrategia para identificacion
        public string ObtenerNombreEstrategia()
        {
            return "Estrategia de Busqueda por Similitud Coseno TF-IDF";
        }

        // Metodo auxiliar: cuenta frecuencia de un termino en arreglo de tokens
        private int ContarFrecuenciaTermino(string[] tokens, string termino)
        {
            if (tokens == null || termino == null || termino == "")
            {
                return 0;                               // Frecuencia cero si parametros invalidos
            }

            int frecuencia = 0;                         // Contador de apariciones
            for (int i = 0; i < tokens.Length; i++)     // Recorrer todos los tokens
            {
                if (tokens[i].Equals(termino))          // Si token coincide con termino buscado
                {
                    frecuencia++;                       // Incrementar contador
                }
            }

            return frecuencia;                          // Retornar frecuencia total
        }

        // Metodo auxiliar: ordena resultados por similitud usando algoritmo burbuja (mayor a menor)
        private ListaDobleCircular<ResultadoBusqueda> OrdenarResultadosPorSimilitud(ListaDobleCircular<ResultadoBusqueda> resultados)
        {
            if (resultados.EstaVacia() || resultados.Count() <= 1)
            {
                return resultados;                      // No necesita ordenamiento
            }

            // Convertir lista circular a arreglo para facilitar ordenamiento
            ResultadoBusqueda[] arregloResultados = ConvertirListaAArreglo(resultados);

            // Aplicar algoritmo burbuja para ordenar por similitud descendente
            for (int i = 0; i < arregloResultados.Length - 1; i++)
            {
                for (int j = 0; j < arregloResultados.Length - 1 - i; j++)
                {
                    // Comparar similitudes: si actual < siguiente, intercambiar
                    if (arregloResultados[j].ObtenerSimilitud() < arregloResultados[j + 1].ObtenerSimilitud())
                    {
                        ResultadoBusqueda temporal = arregloResultados[j];
                        arregloResultados[j] = arregloResultados[j + 1];
                        arregloResultados[j + 1] = temporal;
                    }
                }
            }

            // Convertir arreglo ordenado de vuelta a lista circular
            return ConvertirArregloALista(arregloResultados);
        }

        // Metodo auxiliar: convierte lista circular en arreglo
        private ResultadoBusqueda[] ConvertirListaAArreglo(ListaDobleCircular<ResultadoBusqueda> lista)
        {
            int tamano = lista.Count();                 // Obtener tamano de la lista
            ResultadoBusqueda[] arreglo = new ResultadoBusqueda[tamano];

            Iterator<ResultadoBusqueda> iterador = lista.ObtenerIterator();
            int indice = 0;

            while (iterador.TieneSiguiente() && indice < tamano)
            {
                arreglo[indice] = iterador.Siguiente(); // Copiar elemento a arreglo
                indice++;
            }

            return arreglo;                             // Retornar arreglo completo
        }

        // Metodo auxiliar: convierte arreglo en lista circular
        private ListaDobleCircular<ResultadoBusqueda> ConvertirArregloALista(ResultadoBusqueda[] arreglo)
        {
            ListaDobleCircular<ResultadoBusqueda> lista = new ListaDobleCircular<ResultadoBusqueda>();

            for (int i = 0; i < arreglo.Length; i++)    // Recorrer arreglo
            {
                lista.Insertar(arreglo[i]);             // Insertar cada elemento en lista
            }

            return lista;                               // Retornar lista circular completa
        }
    }
}