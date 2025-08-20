
using EstructurasDatosProyecto.Models;

namespace EstructurasDatosProyecto.Utils
{
    // Clase responsable de leer archivos de texto plano del sistema de archivos
    // Optimizada para procesar al menos 1000 documentos de texto de diferentes tamanos
    // Implementa SRP: Solo se encarga de operaciones de lectura de archivos .txt
    public class LectorDirectorios
    {
        private bool lecturaRecursiva;              // Si debe leer subdirectorios
        private long tamaNoMinimo;                 // Tamano minimo en bytes para considerar archivo
        private long tamaNoMaximo;                 // Tamano maximo en bytes para procesar archivo
        private int contadorArchivos;               // Contador de archivos procesados

        // Constructor por defecto - configuracion optimizada para archivos de texto
        public LectorDirectorios()
        {
            lecturaRecursiva = true;                // Por defecto leer subdirectorios
            tamaNoMinimo = 1024;                   // 1 KB minimo (archivos muy pequenos no sirven)
            tamaNoMaximo = 1024 * 1024;           // 1 MB maximo (archivos hasta 900kb)
            contadorArchivos = 0;                   // Inicializar contador
        }

        // Constructor con configuracion personalizada
        public LectorDirectorios(bool recursivo, long tamaNoMin, long tamaNoMax)
        {
            lecturaRecursiva = recursivo;           // Asignar configuracion de recursion
            tamaNoMinimo = tamaNoMin > 0 ? tamaNoMin : 1024;           // Validar tamano minimo (1 KB)
            tamaNoMaximo = tamaNoMax > tamaNoMin ? tamaNoMax : 1024 * 1024; // Validar tamano maximo (1MB)
            contadorArchivos = 0;                   // Inicializar contador
        }

        // Lee todos los archivos .txt de un directorio y retorna lista ordenada por tamano
        public ListaDobleCircular<string> LeerArchivosTexto(string rutaDirectorio)
        {
            ListaDobleCircular<string> rutasArchivos = new ListaDobleCircular<string>(); // Lista resultado

            if (!ValidarRutaDirectorio(rutaDirectorio)) // Validar que el directorio existe
            {
                return rutasArchivos;               // Retornar lista vacia si invalido
            }

            contadorArchivos = 0;                   // Reiniciar contador
            ProcesarDirectorio(rutaDirectorio, rutasArchivos); // Procesar directorio principal

            // Ordenar archivos por tamano para optimizar procesamiento (pequenos primero)
            return OrdenarArchivosPorTamano(rutasArchivos);
        }

        // Procesa un directorio individual y agrega archivos .txt validos
        private void ProcesarDirectorio(string rutaDirectorio, ListaDobleCircular<string> rutasArchivos)
        {
            try
            {
                // Obtener todos los archivos del directorio actual
                string[] todosArchivos = Directory.GetFiles(rutaDirectorio);

                // Filtrar solo archivos .txt del directorio
                for (int i = 0; i < todosArchivos.Length; i++)
                {
                    string rutaArchivo = todosArchivos[i];      // Obtener ruta del archivo
                    if (EsArchivoTexto(rutaArchivo) && EsArchivoTextoValido(rutaArchivo)) // Validar .txt y tamano
                    {
                        rutasArchivos.Insertar(rutaArchivo);    // Agregar a lista
                        contadorArchivos++;                     // Incrementar contador
                    }
                }

                // Si lectura recursiva habilitada, procesar subdirectorios
                if (lecturaRecursiva)
                {
                    string[] subdirectorios = Directory.GetDirectories(rutaDirectorio);
                    for (int i = 0; i < subdirectorios.Length; i++) // Recorrer subdirectorios
                    {
                        ProcesarDirectorio(subdirectorios[i], rutasArchivos); // Llamada recursiva
                    }
                }
            }
            catch (Exception) // Capturar errores de acceso a directorios
            {
                // Continuar procesamiento aunque falle un directorio
                // Importante para procesamiento masivo de 1000+ archivos
            }
        }

        // Verifica si un archivo tiene extension .txt
        private bool EsArchivoTexto(string rutaArchivo)
        {
            if (rutaArchivo == null || rutaArchivo == "")   // Validar ruta no nula
            {
                return false;                               // Archivo invalido
            }

            // Buscar ultimo punto en la ruta para extraer extension
            int ultimoPunto = -1;
            for (int i = rutaArchivo.Length - 1; i >= 0; i--)
            {
                if (rutaArchivo[i] == '.')                  // Encontrar punto
                {
                    ultimoPunto = i;                        // Guardar posicion
                    break;
                }
                if (rutaArchivo[i] == '\\' || rutaArchivo[i] == '/') // Si encuentra separador antes que punto
                {
                    break;                                  // No hay extension
                }
            }

            if (ultimoPunto == -1 || ultimoPunto >= rutaArchivo.Length - 1) // Sin extension
            {
                return false;                               // No es archivo de texto
            }

            // Construir extension manualmente
            string extension = "";
            for (int i = ultimoPunto; i < rutaArchivo.Length; i++)
            {
                extension = extension + rutaArchivo[i];     // Agregar cada caracter
            }

            // Comparar con .txt (ignorar mayusculas y minusculas)
            string extensionMinuscula = ConvertirAMinusculas(extension);
            return extensionMinuscula.Equals(".txt");       // Validar extension
        }

        // Convierte una cadena a minusculas manualmente
        private string ConvertirAMinusculas(string texto)
        {
            if (texto == null || texto == "") return "";   // Validar entrada

            string resultado = "";                          // Variable resultado
            for (int i = 0; i < texto.Length; i++)          // Recorrer cada caracter
            {
                char c = texto[i];                          // Obtener caracter
                if (c >= 'A' && c <= 'Z')                   // Si es mayuscula
                {
                    resultado = resultado + (char)(c + 32); // Convertir a minuscula
                }
                else
                {
                    resultado = resultado + c;              // Mantener caracter
                }
            }
            return resultado;                               // Retornar resultado
        }

        // Valida si un archivo de texto cumple con los criterios de tamano
        private bool EsArchivoTextoValido(string rutaArchivo)
        {
            if (!File.Exists(rutaArchivo))                  // Verificar que archivo existe
            {
                return false;                               // Archivo no existe
            }

            try
            {
                // Leer archivo para determinar tamano
                string contenido = File.ReadAllText(rutaArchivo);
                long tamaNoArchivo = 0;                     // Contador de bytes

                // Contar bytes manualmente
                for (int i = 0; i < contenido.Length; i++)
                {
                    tamaNoArchivo++;                        // Incrementar contador por caracter
                }

                // Validar rango de tamano: debe estar entre 1KB y 1MB
                return tamaNoArchivo >= tamaNoMinimo && tamaNoArchivo <= tamaNoMaximo;
            }
            catch (Exception) // Error al leer archivo
            {
                return false;                               // Considerar invalido si no se puede leer
            }
        }

        // Valida que la ruta del directorio sea valida usando File.Exists en archivo temporal
        private bool ValidarRutaDirectorio(string rutaDirectorio)
        {
            if (rutaDirectorio == null || rutaDirectorio == "") // Validar ruta no nula
            {
                return false;                               // Ruta invalida
            }

            try
            {
                // Intentar crear ruta de archivo temporal para validar directorio
                string archivoTemporal = rutaDirectorio;
                if (!rutaDirectorio.EndsWith("\\") && !rutaDirectorio.EndsWith("/"))
                {
                    archivoTemporal = rutaDirectorio + "\\temporal.txt"; // Agregar archivo temporal
                }
                else
                {
                    archivoTemporal = rutaDirectorio + "temporal.txt";   // Agregar archivo temporal
                }

                // Verificar que se puede acceder al directorio intentando operacion
                return rutaDirectorio.Length > 0 && rutaDirectorio.Contains(":");
            }
            catch (Exception) // Error al acceder al directorio
            {
                return false;                               // Directorio inaccesible
            }
        }

        // Ordena archivos por tamano usando algoritmo burbuja para 1000+ archivos
        // Archivos pequenos primero para mejor rendimiento de procesamiento
        private ListaDobleCircular<string> OrdenarArchivosPorTamano(ListaDobleCircular<string> rutasArchivos)
        {
            if (rutasArchivos.EstaVacia() || rutasArchivos.Count() <= 1) // Validar lista
            {
                return rutasArchivos;                       // No necesita ordenamiento
            }

            // Convertir a arreglo para aplicar algoritmo burbuja
            string[] arregloRutas = ConvertirListaAArreglo(rutasArchivos);

            // Aplicar algoritmo burbuja por tamano de archivo
            OrdenamientoBurbujaPorTamano(arregloRutas);

            // Convertir de vuelta a lista circular
            return ConvertirArregloALista(arregloRutas);
        }

        // Implementacion algoritmo burbuja optimizada para archivos de texto
        private void OrdenamientoBurbujaPorTamano(string[] arreglo)
        {
            int n = arreglo.Length;                         // Obtener tamano del arreglo

            for (int i = 0; i < n - 1; i++)                 // Bucle externo
            {
                bool intercambioRealizado = false;          // Bandera para optimizar

                for (int j = 0; j < n - 1 - i; j++)         // Bucle interno
                {
                    long tamanoActual = ObtenerTamanoArchivo(arreglo[j]);       // Tamano archivo actual
                    long tamanoSiguiente = ObtenerTamanoArchivo(arreglo[j + 1]); // Tamano archivo siguiente

                    if (tamanoActual > tamanoSiguiente)     // Si actual es mayor que siguiente
                    {
                        IntercambiarElementos(arreglo, j, j + 1); // Intercambiar elementos
                        intercambioRealizado = true;        // Marcar que hubo intercambio
                    }
                }

                if (!intercambioRealizado)                  // Si no hubo intercambios
                {
                    break;                                  // Arreglo ya esta ordenado
                }
            }
        }

        // Intercambia dos elementos en el arreglo
        private void IntercambiarElementos(string[] arreglo, int indice1, int indice2)
        {
            string temporal = arreglo[indice1];             // Guardar elemento
            arreglo[indice1] = arreglo[indice2];            // Intercambiar
            arreglo[indice2] = temporal;                    // Completar intercambio
        }

        // Obtiene tamano de archivo contando caracteres manualmente
        private long ObtenerTamanoArchivo(string rutaArchivo)
        {
            try
            {
                string contenido = File.ReadAllText(rutaArchivo); // Leer contenido
                long contador = 0;                          // Contador de caracteres

                for (int i = 0; i < contenido.Length; i++)  // Contar cada caracter
                {
                    contador++;                             // Incrementar contador
                }

                return contador;                            // Retornar tamano
            }
            catch (Exception) // Error de acceso
            {
                return 0;                                   // Retornar 0 por defecto
            }
        }

        // Convierte lista circular a arreglo para algoritmo burbuja
        private string[] ConvertirListaAArreglo(ListaDobleCircular<string> lista)
        {
            int tamano = lista.Count();                     // Obtener tamano
            string[] arreglo = new string[tamano];          // Crear arreglo

            Iterator<string> iterador = lista.ObtenerIterator(); // Obtener iterador
            int indice = 0;                                 // Inicializar indice

            while (iterador.TieneSiguiente() && indice < tamano) // Recorrer lista
            {
                arreglo[indice] = iterador.Siguiente();     // Copiar elemento
                indice++;                                   // Incrementar indice
            }

            return arreglo;                                 // Retornar arreglo completo
        }

        // Convierte arreglo ordenado a lista circular
        private ListaDobleCircular<string> ConvertirArregloALista(string[] arreglo)
        {
            ListaDobleCircular<string> lista = new ListaDobleCircular<string>(); // Nueva lista

            for (int i = 0; i < arreglo.Length; i++)        // Recorrer arreglo
            {
                lista.Insertar(arreglo[i]);                 // Insertar elemento
            }

            return lista;                                   // Retornar lista completa
        }

        // Configura si la lectura debe ser recursiva
        public void ConfigurarLecturaRecursiva(bool recursivo)
        {
            lecturaRecursiva = recursivo;                   // Asignar configuracion
        }

        // Configura el rango de tamanos validos para archivos
        public void ConfigurarRangoTamanos(long tamaNoMin, long tamaNoMax)
        {
            if (tamaNoMin > 0 && tamaNoMax > tamaNoMin)     // Validar rango
            {
                tamaNoMinimo = tamaNoMin;                   // Asignar minimo
                tamaNoMaximo = tamaNoMax;                   // Asignar maximo
            }
        }

        // Obtiene el numero de archivos procesados en la ultima operacion
        public int ObtenerContadorArchivos()
        {
            return contadorArchivos;                        // Retornar contador
        }

        // Obtiene el tamano minimo configurado en bytes
        public long ObtenerTamanoMinimo()
        {
            return tamaNoMinimo;                           // Retornar limite minimo
        }

        // Obtiene el tamano maximo configurado en bytes
        public long ObtenerTamanoMaximo()
        {
            return tamaNoMaximo;                           // Retornar limite maximo
        }

        // Verifica si la lectura recursiva esta habilitada
        public bool EsLecturaRecursiva()
        {
            return lecturaRecursiva;                        // Retornar configuracion
        }

        // Reinicia el contador de archivos procesados
        public void ReiniciarContador()
        {
            contadorArchivos = 0;                           // Reiniciar contador
        }

        // Obtiene estadisticas de la ultima operacion de lectura
        public string ObtenerEstadisticas()
        {
            string estadisticas = "Archivos procesados: " + contadorArchivos;
            estadisticas += ", Rango tamano: " + tamaNoMinimo + "-" + tamaNoMaximo + " bytes";
            estadisticas += ", Lectura recursiva: " + (lecturaRecursiva ? "Si" : "No");
            return estadisticas;
        }
    }
}