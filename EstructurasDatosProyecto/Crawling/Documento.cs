using EstructurasDatosProyecto.Models;
using EstructurasDatosProyecto.Processing;
using System.Text;

namespace EstructurasDatosProyecto.Crawling;

public class Documento
{
    private string id; // ID unico del documento
    private string rutaArchivo; // Ruta del archivo
    private string contenidoOriginal; // Contenido original
    private string contenidoNormalizado; // Contenido procesado
    private string[] tokens; // Palabras del documento
    private bool procesado; // Si ya fue procesado

    private Normalizer normalizer; // Para normalizar texto
    private Tokenizer tokenizer; // Para separar en palabras

    public Documento()
    {
        id = ""; // Inicializar ID vacio
        rutaArchivo = ""; // Inicializar ruta vacia
        contenidoOriginal = ""; // Inicializar contenido vacio
        contenidoNormalizado = ""; // Inicializar contenido normalizado vacio
        tokens = new string[0]; // Inicializar arreglo vacio
        procesado = false; // Marcar como no procesado
        normalizer = new Normalizer(); // Crear normalizador
        tokenizer = new Tokenizer(); // Crear tokenizador
    }

    public Documento(string rutaArchivo)
    {
        this.rutaArchivo = rutaArchivo; // Asignar ruta
        id = generarIdDesdeRuta(rutaArchivo); // Generar ID desde ruta
        contenidoOriginal = ""; // Inicializar contenido vacio
        contenidoNormalizado = ""; // Inicializar contenido normalizado vacio
        tokens = new string[0]; // Inicializar arreglo vacio
        procesado = false; // Marcar como no procesado
        normalizer = new Normalizer(); // Crear normalizador
        tokenizer = new Tokenizer(); // Crear tokenizador
    }

    public Documento(string id, string rutaArchivo)
    {
        this.id = id; // Asignar ID especifico
        this.rutaArchivo = rutaArchivo; // Asignar ruta
        contenidoOriginal = ""; // Inicializar contenido vacio
        contenidoNormalizado = ""; // Inicializar contenido normalizado vacio
        tokens = new string[0]; // Inicializar arreglo vacio
        procesado = false; // Marcar como no procesado
        normalizer = new Normalizer(); // Crear normalizador
        tokenizer = new Tokenizer(); // Crear tokenizador
    }

    private string generarIdDesdeRuta(string ruta)
    {
        if (ruta == null || ruta == "") return "DOC_" + getTimestampActual(); // Si no hay ruta usar timestamp

        string nombreArchivo = extraerNombreArchivo(ruta); // Obtener nombre del archivo
        if (nombreArchivo == "") // Si no se puede extraer nombre
        {
            return "DOC_" + getTimestampActual(); // Usar timestamp
        }

        return "DOC_" + nombreArchivo; // Retornar ID con prefijo
    }

    private string extraerNombreArchivo(string ruta)
    {
        if (ruta == null || ruta == "") return "";

        // Encontrar ultimo separador de directorio
        int ultimoSeparador = -1;
        for (int i = ruta.Length - 1; i >= 0; i--)
        {
            if (ruta[i] == '\\' || ruta[i] == '/') // Buscar separadores
            {
                ultimoSeparador = i; // Guardar posicion
                break;
            }
        }

        // Obtener nombre con extension
        string nombreConExtension;
        if (ultimoSeparador == -1)
        {
            nombreConExtension = ruta; // No hay separadores, toda la ruta es el nombre
        }
        else
        {
            nombreConExtension = ""; // Construir nombre manualmente
            for (int i = ultimoSeparador + 1; i < ruta.Length; i++)
            {
                nombreConExtension = nombreConExtension + ruta[i]; // Agregar cada caracter
            }
        }

        // Remover extension
        int ultimoPunto = -1;
        for (int i = nombreConExtension.Length - 1; i >= 0; i--)
        {
            if (nombreConExtension[i] == '.') // Buscar punto de extension
            {
                ultimoPunto = i; // Guardar posicion
                break;
            }
        }

        if (ultimoPunto == -1)
        {
            return nombreConExtension; // No hay extension
        }
        else
        {
            string resultado = ""; // Construir resultado sin extension
            for (int i = 0; i < ultimoPunto; i++)
            {
                resultado = resultado + nombreConExtension[i]; // Agregar cada caracter
            }
            return resultado;
        }
    }

    private string getTimestampActual()
    {
        DateTime ahora = DateTime.Now; // Obtener fecha actual
        return ahora.Ticks.ToString(); // Convertir a string
    }

    public bool cargarContenido()
    {
        try
        {
            if (rutaArchivo == null || rutaArchivo == "" || !File.Exists(rutaArchivo)) // Validar ruta
            {
                return false; // Archivo no existe
            }

            contenidoOriginal = File.ReadAllText(rutaArchivo, Encoding.UTF8); // Leer archivo
            return true; // Carga exitosa
        }
        catch (Exception)
        {
            contenidoOriginal = ""; // Limpiar contenido en caso de error
            return false; // Indicar fallo
        }
    }

    public void setContenido(string contenido)
    {
        if (contenido == null)
        {
            contenidoOriginal = ""; // Proteger contra null
        }
        else
        {
            contenidoOriginal = contenido; // Asignar contenido
        }
        procesado = false; // Marcar como no procesado
    }

    public void setId(string nuevoId)
    {
        if (nuevoId == null)
        {
            id = ""; // Proteger contra null
        }
        else
        {
            id = nuevoId; // Asignar nuevo ID
        }
    }

    public void procesarContenido()
    {
        if (contenidoOriginal == null || contenidoOriginal == "") // Si no hay contenido
        {
            contenidoNormalizado = ""; // Establecer contenido normalizado vacio
            tokens = new string[0]; // Establecer tokens vacio
            procesado = true; // Marcar como procesado
            return;
        }

        contenidoNormalizado = normalizer.NormalizarTexto(contenidoOriginal); // Normalizar contenido
        tokens = tokenizer.TokenizarTexto(contenidoNormalizado); // Tokenizar contenido
        procesado = true; // Marcar como procesado
    }

    public string getId()
    {
        return id; // Retornar ID
    }

    public string getRutaArchivo()
    {
        return rutaArchivo; // Retornar ruta
    }

    public string getContenidoOriginal()
    {
        return contenidoOriginal; // Retornar contenido original
    }

    public string getContenidoNormalizado()
    {
        return contenidoNormalizado; // Retornar contenido normalizado
    }

    public string[] getTokens()
    {
        return tokens; // Retornar tokens
    }

    public bool estaProcesado()
    {
        return procesado; // Retornar estado de procesamiento
    }

    public int getNumeroTokens()
    {
        return tokens.Length; // Retornar cantidad de tokens
    }

    public bool tieneTermino(string termino)
    {
        if (termino == null || termino == "" || tokens == null) return false; // Validar parametros

        for (int i = 0; i < tokens.Length; i++) // Recorrer todos los tokens
        {
            if (tokens[i].Equals(termino)) // Si encuentra el termino
            {
                return true; // Documento contiene el termino
            }
        }

        return false; // Termino no encontrado
    }

    public int contarFrecuenciaTermino(string termino)
    {
        if (termino == null || termino == "" || tokens == null) return 0; // Validar parametros

        int frecuencia = 0; // Contador de frecuencia
        for (int i = 0; i < tokens.Length; i++) // Recorrer todos los tokens
        {
            if (tokens[i].Equals(termino)) // Si token coincide con termino
            {
                frecuencia++; // Incrementar contador
            }
        }

        return frecuencia; // Retornar frecuencia total
    }

    public string[] getTerminosUnicos()
    {
        if (tokens == null || tokens.Length == 0) // Si no hay tokens
        {
            return new string[0]; // Retornar arreglo vacio
        }

        ListaDobleCircular<string> terminosUnicos = new ListaDobleCircular<string>(); // Lista para terminos unicos

        for (int i = 0; i < tokens.Length; i++) // Recorrer todos los tokens
        {
            string token = tokens[i]; // Obtener token actual
            if (!existeEnLista(terminosUnicos, token)) // Si no existe en la lista
            {
                terminosUnicos.Insertar(token); // Agregar termino unico
            }
        }

        return convertirListaAArreglo(terminosUnicos); // Convertir lista a arreglo
    }

    private bool existeEnLista(ListaDobleCircular<string> lista, string elemento)
    {
        if (lista.EstaVacia()) return false; // Si lista vacia, elemento no existe

        Iterator<string> iterador = lista.ObtenerIterator(); // Obtener iterador
        while (iterador.TieneSiguiente()) // Mientras haya elementos
        {
            string elementoActual = iterador.Siguiente(); // Obtener siguiente elemento
            if (elementoActual.Equals(elemento)) // Si coincide con el buscado
            {
                return true; // Elemento existe en la lista
            }
        }

        return false; // Elemento no encontrado
    }

    private string[] convertirListaAArreglo(ListaDobleCircular<string> lista)
    {
        if (lista.EstaVacia()) return new string[0]; // Si lista vacia, retornar arreglo vacio

        int tamano = lista.Count(); // Obtener tamano de la lista
        string[] arreglo = new string[tamano]; // Crear arreglo del tamano correcto
        Iterator<string> iterador = lista.ObtenerIterator(); // Obtener iterador

        for (int i = 0; i < tamano; i++) // Llenar arreglo
        {
            arreglo[i] = iterador.Siguiente(); // Asignar elemento siguiente
        }

        return arreglo; // Retornar arreglo completo
    }

    public bool esIgual(Documento otroDocumento)
    {
        if (otroDocumento == null) return false; // Si es null, no son iguales
        return id.Equals(otroDocumento.id); // Comparar IDs
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Documento)) return false; // Validar tipo
        return esIgual((Documento)obj); // Reutilizar metodo esIgual
    }

    public override int GetHashCode()
    {
        return id.GetHashCode(); // Delegar al GetHashCode del ID
    }

    public override string ToString()
    {
        string resultado = ""; // Usar concatenacion simple
        resultado = resultado + "ID: " + id; // Agregar ID
        resultado = resultado + ", Tokens: " + tokens.Length; // Agregar numero de tokens
        resultado = resultado + ", Procesado: " + procesado; // Agregar estado de procesamiento
        if (rutaArchivo != null && rutaArchivo != "") // Si hay ruta de archivo
        {
            string nombreArchivo = extraerNombreArchivo(rutaArchivo);
            resultado = resultado + ", Archivo: " + nombreArchivo; // Agregar nombre de archivo
        }
        return resultado; // Retornar string completo
    }
}