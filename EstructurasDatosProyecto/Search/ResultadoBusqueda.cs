using EstructurasDatosProyecto.Crawling;

namespace EstructurasDatosProyecto.Search;

public class ResultadoBusqueda
{
    public Documento documento;          // Documento encontrado en la búsqueda
    public double similitud;            // Puntuación de similitud coseno (0.0 - 1.0)
    public string consultaOriginal;     // Consulta que produjo este resultado

    // Constructor por defecto
    public ResultadoBusqueda()
    {
        documento = null;               // Inicializar documento como null
        similitud = 0.0;               // Inicializar similitud en 0
        consultaOriginal = "";         // Inicializar consulta vacía
    }

    // Constructor con documento y similitud
    public ResultadoBusqueda(Documento documento, double similitud)
    {
        this.documento = documento;     // Asignar documento recibido
        this.similitud = similitud;    // Asignar puntuación de similitud
        this.consultaOriginal = "";    // Inicializar consulta vacía
    }

    // Constructor completo con todos los parámetros
    public ResultadoBusqueda(Documento documento, double similitud, string consultaOriginal)
    {
        this.documento = documento;            // Asignar documento recibido
        this.similitud = similitud;           // Asignar puntuación de similitud
        this.consultaOriginal = consultaOriginal; // Asignar consulta original
    }

    // Obtiene el documento del resultado
    public Documento ObtenerDocumento()
    {
        return documento;              // Retornar documento almacenado
    }

    // Obtiene la puntuación de similitud
    public double ObtenerSimilitud()
    {
        return similitud;              // Retornar valor de similitud
    }

    // Obtiene la consulta original que generó este resultado
    public string ObtenerConsultaOriginal()
    {
        return consultaOriginal;       // Retornar consulta original
    }

    // Establece una nueva puntuación de similitud
    public void EstablecerSimilitud(double nuevaSimilitud)
    {
        if (nuevaSimilitud >= 0.0 && nuevaSimilitud <= 1.0) // Validar rango [0,1]
        {
            similitud = nuevaSimilitud; // Asignar nueva similitud si es válida
        }
    }

    // Compara dos resultados por similitud (mayor similitud = más relevante)
    public bool EsMasRelevante(ResultadoBusqueda otroResultado)
    {
        if (otroResultado == null) return true;          // Si es null, este es más relevante
        return similitud > otroResultado.similitud;      // Comparar puntuaciones
    }

    // Compara dos resultados por igualdad basada en el documento
    public bool EsIgual(ResultadoBusqueda otroResultado)
    {
        if (otroResultado == null) return false;         // Si es null, no son iguales
        if (documento == null && otroResultado.documento == null) return true; // Ambos null
        if (documento == null || otroResultado.documento == null) return false; // Solo uno null
        return documento.esIgual(otroResultado.documento); // Comparar documentos
    }

    // Sobrescribe método Equals para comparación estándar
    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is ResultadoBusqueda)) return false; // Validar tipo
        return EsIgual((ResultadoBusqueda)obj);          // Reutilizar método EsIgual
    }

    // Genera código hash basado en el documento
    public override int GetHashCode()
    {
        if (documento == null) return 0;                 // Si documento es null, hash = 0
        return documento.GetHashCode();                  // Delegar al hash del documento
    }

    // Crea una copia del resultado actual
    public ResultadoBusqueda Clonar()
    {
        return new ResultadoBusqueda(documento, similitud, consultaOriginal);
    }

    // Representación textual del resultado para debugging
    public override string ToString()
    {
        string resultado = "Similitud: " + similitud.ToString("F3"); // Similitud con 3 decimales

        if (documento != null)                           // Si hay documento
        {
            resultado += ", Doc: " + documento.getId();  // Agregar ID del documento
            resultado += ", Tokens: " + documento.getNumeroTokens(); // Agregar número de tokens
        }
        else
        {
            resultado += ", Doc: null";                  // Indicar documento nulo
        }

        if (consultaOriginal != "")                      // Si hay consulta original
        {
            resultado += ", Consulta: '" + consultaOriginal + "'"; // Agregar consulta
        }

        return resultado;                                // Retornar string completo
    }
}