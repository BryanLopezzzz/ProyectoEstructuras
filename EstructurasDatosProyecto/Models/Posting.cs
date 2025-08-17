namespace EstructurasDatosProyecto.Models
{
    /// Representa una entrada en el índice invertido
    /// Contiene la información de un término en un documento específico
    public class Posting
    {
        public string idDocumento;     // Identificador único del documento
        public int frecuencia;         // TF: número de veces que aparece el término en el documento
        public double tfIdf;           // Valor TF-IDF calculado: M(t,d) = log10(N/DF(t)) * TF(t,d)

        // Constructor por defecto
        public Posting()
        {
            idDocumento = "";          // Inicializar ID del documento como cadena vacía
            frecuencia = 0;            // Inicializar TF (frecuencia del término) en 0
            tfIdf = 0.0;               // Inicializar TF-IDF en 0.0
        }

        // Constructor con parámetros básicos
        public Posting(string idDocumento, int frecuencia)
        {
            this.idDocumento = idDocumento;  // Asignar el ID del documento recibido
            this.frecuencia = frecuencia;    // Asignar la frecuencia del término (TF)
            this.tfIdf = 0.0;               // TF-IDF se calculará posteriormente
        }

        // Constructor completo con todos los parámetros
        public Posting(string idDocumento, int frecuencia, double tfIdf)
        {
            this.idDocumento = idDocumento;  // Asignar el ID del documento recibido
            this.frecuencia = frecuencia;    // Asignar TF (frecuencia del término en el documento)
            this.tfIdf = tfIdf;             // Asignar valor TF-IDF ya calculado
        }

        // Incrementa la frecuencia del término en una unidad
        // Útil cuando se encuentra el mismo término múltiples veces durante la indexación
        public void IncrementarFrecuencia()
        {
            frecuencia++;               // Aumentar TF en 1
        }

        // Establece el valor TF-IDF calculado usando la fórmula del proyecto:
        // M(t,d) = log10(N/DF(t)) * TF(t,d)
        public void EstablecerTfIdf(double valor)
        {
            tfIdf = valor;              // Asignar el valor TF-IDF calculado
        }

        // Calcula y establece el TF-IDF usando la fórmula del proyecto
        // M(t,d) = IDF(t) * TF(t,d) donde IDF(t) = log10(N/DF(t))
        public void CalcularTfIdf(double idf)
        {
            tfIdf = frecuencia * idf;   // Multiplicar TF por IDF según la fórmula del proyecto
        }

        // Obtiene la frecuencia del término en este documento (TF)
        public int ObtenerFrecuencia()
        {
            return frecuencia;          // Retornar valor actual de TF
        }

        // Obtiene el valor TF-IDF calculado
        public double ObtenerTfIdf()
        {
            return tfIdf;               // Retornar valor actual de TF-IDF
        }

        // Obtiene el ID del documento
        public string ObtenerIdDocumento()
        {
            return idDocumento;         // Retornar ID del documento
        }

        // Compara dos postings por su identificador de documento
        // Necesario para operaciones de búsqueda y ordenamiento en listas
        public bool EsIgual(Posting otroPosting)
        {
            if (otroPosting == null) return false;           // Si es null, no son iguales
            return idDocumento.Equals(otroPosting.idDocumento); // Comparar IDs de documento
        }

        // Sobrescribe el método Equals para comparación estándar de objetos
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Posting)) return false; // Validar tipo y null
            Posting otroPosting = (Posting)obj;              // Hacer casting al tipo correcto
            return idDocumento.Equals(otroPosting.idDocumento); // Comparar IDs de documento
        }

        // Genera un código hash basado en el ID del documento
        // Necesario para uso en estructuras de datos hash
        public override int GetHashCode()
        {
            return idDocumento.GetHashCode();                // Delegar al GetHashCode del string
        }

        // Crea una copia exacta del posting actual
        // Útil para operaciones que requieren duplicar postings
        public Posting Clonar()
        {
            return new Posting(idDocumento, frecuencia, tfIdf); // Crear nuevo posting con mismos valores
        }

        // Representación en cadena del posting para depuración
        // Muestra información completa del posting en formato legible
        public override string ToString()
        {
            string resultado = "Doc: " + idDocumento;            // Comenzar con ID del documento
            resultado += ", TF: " + frecuencia.ToString();       // Agregar frecuencia del término
            resultado += ", TF-IDF: " + tfIdf.ToString("F3");    // Agregar TF-IDF con 3 decimales
            return resultado;                                    // Retornar cadena completa
        }
    }
}

/* Características principales de la clase:
Almacena el ID del documento, la frecuencia del término y el valor TF-IDF
Constructores múltiples: Para diferentes escenarios de creación
Métodos de utilidad: Para incrementar frecuencia, establecer TF-IDF y comparación
*/