using EstructurasDatosProyecto.Models;

namespace EstructurasDatosProyecto.Processing
{
    public class Normalizer
    {
        public Normalizer()
        {
        }

        public string NormalizarTexto(string texto)
        {
            if (texto == null || texto == "") return "";               // Verificar si el texto esta vacio

            string resultado = ConvertirAMinusculas(texto);            // Paso 1: convertir a minusculas
            resultado = EliminarTildes(resultado);                     // Paso 2: quitar tildes
            resultado = LimpiarPuntuacion(resultado);                   // Paso 3: limpiar puntuacion
            resultado = EliminarStopwords(resultado);                   // Paso 4: eliminar stopwords

            return resultado;                                   // Devolver el texto completamente normalizado
        }

        private string ConvertirAMinusculas(string texto)
        {
            string resultado = "";                               // Variable para almacenar el texto en minusculas

            for (int i = 0; i < texto.Length; i++)                  // Recorrer cada caracter del texto
            {
                char c = texto[i];                                  // Obtener el caracter en la posicion i
                if (c >= 'A' && c <= 'Z')                           // Verificar si es mayuscula
                {
                    resultado = resultado + (char)(c + 32);         // Convertir a minuscula sumando 32
                }
                else
                {
                    resultado = resultado + c; // Mantener el caracter original
                }
            }

            return resultado; // Devolver el texto convertido a minusculas
        }

        private string EliminarTildes(string texto)
        {
            string resultado = "";                          // Variable para almacenar el texto sin tildes

            for (int i = 0; i < texto.Length; i++)          // Recorrer cada caracter del texto
            {
                char c = texto[i];                          // Obtener el caracter en la posicion i
                if (c == 'á') resultado = resultado + 'a';          // Reemplazar a con tilde
                else if (c == 'é') resultado = resultado + 'e'; 
                else if (c == 'í') resultado = resultado + 'i'; 
                else if (c == 'ó') resultado = resultado + 'o'; 
                else if (c == 'ú') resultado = resultado + 'u'; 
                else if (c == 'ñ') resultado = resultado + 'n'; 
                else resultado = resultado + c; 
            }

            return resultado; // Devolver el texto sin tildes
        }

        private string LimpiarPuntuacion(string texto)
        {
            string resultado = "";                                    // Variable para almacenar el texto limpio

            for (int i = 0; i < texto.Length; i++)                      // Recorrer cada caracter del texto
            {
                char c = texto[i];                                      // Obtener el caracter en la posicion i
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')) // Verificar si es letra o numero
                {
                    resultado = resultado + c;                   // Agregar el caracter valido al resultado
                }
                else // Si no es letra ni numero
                {
                    if (resultado.Length > 0 && resultado[resultado.Length - 1] != ' ') // Verificar que no hay espacio al final
                    {
                        resultado = resultado + ' '; // Agregar un espacio como separador
                    }
                }
            }

            return resultado; // Devolver el texto sin puntuacion
        }

        private string EliminarStopwords(string texto)
        {
            if (texto == null || texto == "") return "";                // Verificar si el texto esta vacio

            string[] palabras = texto.Split(' ');                       // Separar el texto en palabras usando espacios
            string resultado = "";                                      // Variable para almacenar el texto sin stopwords

            for (int i = 0; i < palabras.Length; i++)                    // Recorrer cada palabra
            {
                string palabra = palabras[i];                               // Obtener la palabra en la posicion i
                if (palabra != "" && !StopWords.IsStopword(palabra))    // Verificar que no sea vacia ni stopword
                {
                    if (resultado != "")                    // Si ya hay texto en el resultado
                    {
                        resultado = resultado + " "; // Agregar un espacio antes de la palabra
                    }
                    resultado = resultado + palabra; // Agregar la palabra valida
                }
            }

            return resultado; // Devolver el texto sin stopwords
        }
    }
}