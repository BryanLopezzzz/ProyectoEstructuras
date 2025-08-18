using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDatosProyecto.Calculos
{
    public class Vector
    {
        private double[] elementos;                      // Arreglo que contiene los elementos del vector
        private int dimension;                          // Número de elementos en el vector

        // Constructor por defecto - crea vector vacío
        public Vector()
        {
            elementos = new double[0];                  // Inicializar arreglo vacío
            dimension = 0;                              // Dimensión 0
        }

        // Constructor con dimensión específica - crea vector de ceros
        public Vector(int dimension)
        {
            this.dimension = dimension;                 // Asignar la dimensión recibida
            elementos = new double[dimension];          // Crear arreglo con la dimensión especificada
            for (int i = 0; i < dimension; i++)         // Inicializar todos los elementos
            {
                elementos[i] = 0.0;                     // Establecer cada elemento en 0.0
            }
        }

        // Constructor con arreglo de valores - crea vector con valores iniciales
        public Vector(double[] valoresIniciales)
        {
            dimension = valoresIniciales.Length;        // La dimensión es el tamaño del arreglo
            elementos = new double[dimension];          // Crear arreglo con la dimensión correcta
            for (int i = 0; i < dimension; i++)         // Copiar cada elemento del arreglo inicial
            {
                elementos[i] = valoresIniciales[i];     // Asignar valor del arreglo original
            }
        }

        // Establece el valor de un elemento en una posición específica
        public void EstablecerElemento(int indice, double valor)
        {
            if (indice >= 0 && indice < dimension)      // Validar que el índice esté en rango
            {
                elementos[indice] = valor;              // Asignar el valor en la posición indicada
            }
        }

        // Obtiene el valor de un elemento en una posición específica
        public double ObtenerElemento(int indice)
        {
            if (indice >= 0 && indice < dimension)      // Validar que el índice esté en rango
            {
                return elementos[indice];               // Retornar el valor en la posición
            }
            return 0.0;                                 // Si índice inválido, retornar 0
        }

        // Obtiene la dimensión (número de elementos) del vector
        public int ObtenerDimension()
        {
            return dimension;                           // Retornar la dimensión actual
        }

        // Sobrecarga del operador * para producto punto entre dos vectores
        // Implementa A * B = a1*b1 + a2*b2 + ... + an*bn
        public static double operator *(Vector vectorA, Vector vectorB)
        {
            if (vectorA.dimension != vectorB.dimension) // Verificar que ambos vectores tengan la misma dimensión
            {
                return 0.0;                             // Si dimensiones diferentes, retornar 0
            }

            double productoPunto = 0.0;                 // Inicializar acumulador del producto punto
            for (int i = 0; i < vectorA.dimension; i++) // Recorrer todos los elementos
            {
                double multiplicacion = vectorA.elementos[i] * vectorB.elementos[i]; // Multiplicar elementos correspondientes
                productoPunto = productoPunto + multiplicacion; // Sumar al acumulador
            }

            return productoPunto;                       // Retornar la suma total (producto punto)
        }

        // Calcula la magnitud (norma) del vector usando ||A|| = sqrt(a1² + a2² + ... + an²)
        // Implementación manual de la raíz cuadrada sin librerías externas
        public double CalcularMagnitud()
        {
            double sumaCuadrados = 0.0;                 // Inicializar suma de cuadrados

            for (int i = 0; i < dimension; i++)         // Recorrer todos los elementos
            {
                double cuadrado = elementos[i] * elementos[i]; // Calcular el cuadrado del elemento
                sumaCuadrados = sumaCuadrados + cuadrado; // Sumar al acumulador
            }

            return Math.Sqrt(sumaCuadrados); // Calcular raíz cuadrada del resultado
        }


        // Suma dos vectores elemento por elemento: C = A + B
        public static Vector operator +(Vector vectorA, Vector vectorB)
        {
            if (vectorA.dimension != vectorB.dimension) // Verificar dimensiones iguales
            {
                return new Vector();                    // Si diferentes, retornar vector vacío
            }

            Vector resultado = new Vector(vectorA.dimension); // Crear vector resultado

            for (int i = 0; i < vectorA.dimension; i++) // Sumar cada elemento correspondiente
            {
                double suma = vectorA.elementos[i] + vectorB.elementos[i]; // Sumar elementos
                resultado.EstablecerElemento(i, suma);  // Establecer resultado en posición i
            }

            return resultado;                           // Retornar vector suma
        }

        // Resta dos vectores elemento por elemento: C = A - B
        public static Vector operator -(Vector vectorA, Vector vectorB)
        {
            if (vectorA.dimension != vectorB.dimension) // Verificar dimensiones iguales
            {
                return new Vector();                    // Si diferentes, retornar vector vacío
            }

            Vector resultado = new Vector(vectorA.dimension); // Crear vector resultado

            for (int i = 0; i < vectorA.dimension; i++) // Restar cada elemento correspondiente
            {
                double resta = vectorA.elementos[i] - vectorB.elementos[i]; // Restar elementos
                resultado.EstablecerElemento(i, resta); // Establecer resultado en posición i
            }

            return resultado;                           // Retornar vector diferencia
        }

        // Multiplica el vector por un escalar: B = k * A
        public static Vector operator *(double escalar, Vector vector)
        {
            Vector resultado = new Vector(vector.dimension); // Crear vector resultado

            for (int i = 0; i < vector.dimension; i++)  // Multiplicar cada elemento por el escalar
            {
                double multiplicacion = escalar * vector.elementos[i]; // Multiplicar elemento por escalar
                resultado.EstablecerElemento(i, multiplicacion); // Establecer resultado
            }

            return resultado;                           // Retornar vector escalado
        }

        // Verifica si el vector está vacío (dimensión 0)
        public bool EstaVacio()
        {
            return dimension == 0;                      // Retornar true si dimensión es 0
        }

        // Verifica si todos los elementos del vector son cero
        public bool EsCero()
        {
            for (int i = 0; i < dimension; i++)         // Recorrer todos los elementos
            {
                if (elementos[i] != 0.0)                // Si algún elemento no es cero
                {
                    return false;                       // El vector no es cero
                }
            }
            return true;                                // Todos los elementos son cero
        }

        // Crea una copia exacta del vector actual
        public Vector Clonar()
        {
            Vector copia = new Vector(dimension);       // Crear nuevo vector con misma dimensión
            for (int i = 0; i < dimension; i++)         // Copiar cada elemento
            {
                copia.elementos[i] = elementos[i];      // Asignar valor del elemento original
            }
            return copia;                               // Retornar copia del vector
        }

        // Representación en cadena del vector para depuración
        public override string ToString()
        {
            string resultado = "[";                     // Comenzar con corchete
            for (int i = 0; i < dimension; i++)         // Agregar cada elemento
            {
                resultado += elementos[i].ToString("F3"); // Agregar elemento con 3 decimales
                if (i < dimension - 1)                  // Si no es el último elemento
                {
                    resultado += ", ";                  // Agregar coma y espacio
                }
            }
            resultado += "]";                           // Cerrar con corchete
            return resultado;                           // Retornar representación completa
        }
    }
}
