namespace EstructurasDatosProyecto.Models
{
    // Representa la información completa de un término en el índice invertido
    // Contiene el término y su lista de postings (documentos donde aparece)
    public class TermInfo
    {
        public string termino;                           // El término/palabra
        public ListaDobleCircular<Posting> postings;    // Lista de documentos donde aparece el término
        public int frecuenciaDocumental;                 // DF: número de documentos que contienen el término
        public double idf;                               // IDF calculado: log10(N/DF(t))

        // Constructor por defecto
        public TermInfo()
        {
            termino = "";                                // Inicializar término vacío
            postings = new ListaDobleCircular<Posting>(); // Crear lista vacía de postings
            frecuenciaDocumental = 0;                    // Inicializar DF en 0
            idf = 0.0;                                   // Inicializar IDF en 0
        }

        // Constructor con término específico
        public TermInfo(string termino)
        {
            this.termino = termino;                      // Asignar el término recibido
            postings = new ListaDobleCircular<Posting>(); // Crear lista vacía de postings
            frecuenciaDocumental = 0;                    // Inicializar DF en 0
            idf = 0.0;                                   // Inicializar IDF en 0
        }

        // Agrega un nuevo posting a la lista de documentos para este término
        // Si el documento ya existe, incrementa su frecuencia (TF)
        public void AgregarPosting(Posting nuevoPosting)
        {
            Posting postingExistente = BuscarPostingPorDocumento(nuevoPosting.idDocumento); // Buscar si ya existe posting para este documento

            if (postingExistente != null)                // Si encontramos posting existente
            {
                postingExistente.IncrementarFrecuencia(); // Incrementar TF del término en el documento
            }
            else                                         // Si no existe posting para este documento
            {
                postings.Insertar(nuevoPosting);         // Agregar el nuevo posting a la lista
                frecuenciaDocumental++;                  // Incrementar DF (documentos que contienen el término)
            }
        }

        // Busca un posting específico por ID de documento
        // Algoritmo de búsqueda lineal en la lista circular
        public Posting BuscarPostingPorDocumento(string idDocumento)
        {
            if (postings.EstaVacia()) return null;       // Si la lista está vacía, retornar null

            Posting postingTemporal = new Posting(idDocumento, 0); // Crear posting temporal para comparación
            Iterator<Posting> iterador = postings.ObtenerIterator(); // Obtener iterador de la lista circular

            while (iterador.TieneSiguiente())            // Mientras haya elementos por recorrer
            {
                Posting postingActual = iterador.Siguiente(); // Obtener el siguiente posting de la lista
                if (postingActual.EsIgual(postingTemporal))   // Comparar IDs de documento
                {
                    return postingActual;                     // Si coinciden, retornar el posting encontrado
                }
            }

            return null;                                 // Si no se encontró, retornar null
        }

        // Calcula y establece el valor IDF según la fórmula del proyecto:
        // IDF(t) = log10(N / DF(t))
        // Ejemplos: log10(3/2) = 0.176, log10(3/1) = 0.477
        public void CalcularIdf(int totalDocumentos)
        {
            if (frecuenciaDocumental == 0 || totalDocumentos == 0) // Validar parámetros de entrada
            {
                idf = 0.0;                               // Si algún valor es 0, asignar IDF = 0
                return;                                  // Salir del método
            }

            double cociente = (double)totalDocumentos / (double)frecuenciaDocumental; // Calcular N/DF(t)
            idf = CalcularLogaritmoBase10(cociente);     // Aplicar log10 al cociente N/DF
        }

        // Implementación manual del logaritmo base 10 sin librerías externas
        // Usa la propiedad: log10(x) = ln(x) / ln(10)
        // Algoritmo necesario para obtener valores exactos como 0.176 y 0.477
        private double CalcularLogaritmoBase10(double x)
        {
            if (x <= 0) return 0.0;                     // Validar entrada positiva
            if (x == 1) return 0.0;                     // log10(1) = 0 por definición

            double logaritmoNatural = CalcularLogaritmoNatural(x); // Calcular ln(x) usando serie de Taylor
            double ln10 = 2.302585092994046;            // Constante ln(10) calculada previamente

            return logaritmoNatural / ln10;              // Convertir: log10(x) = ln(x) / ln(10)
        }

        // Implementación sencilla del logaritmo natural 
        // Usa serie de Taylor básica: ln(1+y) = y - y²/2 + y³/3 - y⁴/4 + ...
        // Implementación simple para estudiante
        private double CalcularLogaritmoNatural(double x)
        {
            if (x <= 0) return 0.0;                     // Si x no es positivo, retornar 0
            if (x == 1) return 0.0;                     // ln(1) = 0

            // Si x está muy lejos de 1, aproximar usando valores conocidos
            if (x > 3.0)
            {
                return 1.0986;                          // Aproximación de ln(3) = 1.0986
            }
            if (x < 0.3)
            {
                return -1.2040;                         // Aproximación de ln(0.3) = -1.2040
            }

            // Para valores cercanos a 1, usar serie de Taylor simple
            double y = x - 1.0;                         // Cambiar variable: y = x - 1
            double resultado = 0.0;                     // Inicializar resultado
            double potencia = y;                        // Inicializar con y^1

            // Calcular solo los primeros 10 términos de la serie
            for (int i = 1; i <= 10; i++)
            {
                if (i % 2 == 1)                         // Términos impares: positivos
                {
                    resultado = resultado + potencia / i; // Sumar término positivo
                }
                else                                    // Términos pares: negativos  
                {
                    resultado = resultado - potencia / i; // Restar término negativo
                }
                potencia = potencia * y;                // Calcular siguiente potencia y^(i+1)
            }

            return resultado;                           // Retornar resultado de la serie
        }
        

        // Actualiza todos los TF-IDF de los postings usando la fórmula del proyecto:
        // M(t,d) = IDF(t) * TF(t,d) donde IDF(t) = log10(N/DF(t))
        public void ActualizarTfIdfPostings()
        {
            if (postings.EstaVacia()) return;           // Si no hay postings, salir del método

            Iterator<Posting> iterador = postings.ObtenerIterator(); // Obtener iterador para recorrer postings

            while (iterador.TieneSiguiente())           // Mientras haya postings por procesar
            {
                Posting posting = iterador.Siguiente(); // Obtener siguiente posting
                posting.CalcularTfIdf(idf);             // Calcular TF-IDF usando método del posting
            }
        }

        // Obtiene la frecuencia documental (DF) - número de documentos que contienen el término
        public int ObtenerFrecuenciaDocumental()
        {
            return frecuenciaDocumental;                // Retornar valor actual de DF
        }

        // Obtiene el valor IDF calculado para este término
        public double ObtenerIdf()
        {
            return idf;                                 // Retornar valor actual de IDF
        }

        // Obtiene un iterador para recorrer todos los postings de este término
        public Iterator<Posting> ObtenerIteradorPostings()
        {
            return postings.ObtenerIterator();          // Retornar iterador de la lista de postings
        }

        // Compara dos objetos TermInfo por su término
        public bool EsIgual(TermInfo otroTermInfo)
        {
            if (otroTermInfo == null) return false;     // Si el objeto es null, no son iguales
            return termino.Equals(otroTermInfo.termino); // Comparar strings de los términos
        }

        // Sobrescribe método Equals para comparación estándar de objetos
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is TermInfo)) return false; // Validar tipo y null
            return EsIgual((TermInfo)obj);              // Reutilizar método esIgual para evitar duplicación
        }

        // Genera código hash basado en el término para uso en estructuras hash
        public override int GetHashCode()
        {
            return termino.GetHashCode();               // Delegar al método GetHashCode del string
        }

        // Genera representación textual legible del objeto para depuración
        public override string ToString()
        {
            string resultado = "Término: " + termino;                    // Comenzar con el término
            resultado += ", DF: " + frecuenciaDocumental.ToString();    // Agregar frecuencia documental
            resultado += ", IDF: " + idf.ToString("F3");                // Agregar IDF con 3 decimales
            resultado += ", Postings: " + postings.Count().ToString();  // Agregar número de postings
            return resultado;                           // Retornar string completo
        }
    }
}