namespace EstructurasDatosProyecto.Models
{

    /// Representa una entrada en el índice invertido
    /// Contiene la información de un término en un documento específico
  
    public class Posting
    {
        public string idDocumento;     // Identificador único del documento
        public int frecuencia;         // Número de veces que aparece el término en el documento
        public double tfIdf;           // Valor TF-IDF calculado para este término en este documento

     
        /// Constructor por defecto
        public Posting()
        {
            idDocumento = "";
            frecuencia = 0;
            tfIdf = 0.0;
        }

        public Posting(string idDocumento, int frecuencia)
        {
            this.idDocumento = idDocumento;//Identificador del documento</param>
            this.frecuencia = frecuencia; //Frecuencia del término en el documento
            this.tfIdf = 0.0; // Se calculará posteriormente
        }

    
        /// Constructor completo con todos los parámetros
        public Posting(string idDocumento, int frecuencia, double tfIdf)
        {
            this.idDocumento = idDocumento;
            this.frecuencia = frecuencia;// Frecuencia del término en el document
            this.tfIdf = tfIdf;//Valor TF-IDF calculado
        }


        /// Incrementa la frecuencia del término en una unidad
        /// cuando se encuentra el mismo término múltiples veces

        public void IncrementarFrecuencia()
        {
            frecuencia++;
        }
        public void EstablecerTfIdf(double valor)
        {
            tfIdf = valor;//Nuevo valor TF-IDF
        }

        public bool EsIgual(Posting otroPosting)//Posting a comparar
        {
            if (otroPosting == null) return false;
            return idDocumento.Equals(otroPosting.idDocumento);// true si tienen el mismo ID de documento
        }


        /// Sobrescribe el método Equals para comparación de objetos
        public override bool Equals(object obj)//Objeto a comparar
        {
            if (obj == null || !(obj is Posting)) return false;
            Posting otroPosting = (Posting)obj;
            return idDocumento.Equals(otroPosting.idDocumento); //true si los objetos son iguales
        }



        public override int GetHashCode() /// Genera un código hash basado en el ID del documento
        {
            return idDocumento.GetHashCode();///Código hash del objeto
        }

      
        public override string ToString()  /// Representación en cadena del posting
                                           /// Útil para debugging y logging
                                           /// Representación textual del posting
        {
            return "Doc: " + idDocumento + ", Freq: " + frecuencia + ", TF-IDF: " + tfIdf;
        }
        
        public Posting Clonar()        /// Crea una copia exacta del posting actual
        {
            return new Posting(idDocumento, frecuencia, tfIdf); /// retorna nueva instancia de Posting con los mismos valores
        }
    }
}

/* Características principales de la clase:
Almacena el ID del documento, la frecuencia del término y el valor TF-IDF
Constructores múltiples: Para diferentes escenarios de creación
Métodos de utilidad: Para incrementar frecuencia, establecer TF-IDF y comparación
*/