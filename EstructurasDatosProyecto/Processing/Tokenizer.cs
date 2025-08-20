
namespace EstructurasDatosProyecto.Processing
{
    public class Tokenizer
    {
        public Tokenizer()
        {
        }

        public string[] TokenizarTexto(string texto)
        {
            if (texto == null || texto == "") // Verificar si el texto esta vacio
            {
                return new string[0]; // Devolver arreglo vacio
            }

            string[] palabras = texto.Split(' '); // Dividir por espacios
            int contador = 0; // Contador para palabras validas

            // Contar cuantas palabras no estan vacias
            for (int i = 0; i < palabras.Length; i++)
            {
                string palabra = LimpiarPalabra(palabras[i]); // Limpiar espacios
                if (palabra != "") // Si la palabra no esta vacia
                {
                    contador++; // Aumentar contador
                }
            }

            // Crear arreglo del tamano exacto
            string[] resultado = new string[contador];
            int indice = 0; // Indice para el arreglo resultado

            // Llenar el arreglo resultado con palabras validas
            for (int i = 0; i < palabras.Length; i++)
            {
                string palabra = LimpiarPalabra(palabras[i]); // Limpiar espacios
                if (palabra != "") // Si la palabra no esta vacia
                {
                    resultado[indice] = palabra; // Agregar palabra al resultado
                    indice++; // Avanzar indice
                }
            }

            return resultado; // Devolver arreglo con las palabras
        }

        private string LimpiarPalabra(string palabra)
        {
            if (palabra == null || palabra == "") return ""; // Si esta vacia devolver vacio

            int inicio = 0; // Posicion inicial
            int fin = palabra.Length - 1; // Posicion final

            // Buscar primer caracter que no sea espacio desde el inicio
            while (inicio <= fin && palabra[inicio] == ' ')
            {
                inicio++; // Avanzar posicion inicial
            }

            // Buscar primer caracter que no sea espacio desde el final
            while (fin >= inicio && palabra[fin] == ' ')
            {
                fin--; // Retroceder posicion final
            }

            // Si todos eran espacios
            if (inicio > fin) return "";

            // Construir la palabra sin espacios al inicio y final
            string resultado = "";
            for (int i = inicio; i <= fin; i++)
            {
                resultado = resultado + palabra[i]; // Agregar cada caracter
            }

            return resultado; // Devolver palabra limpia
        }
    }
}