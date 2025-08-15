using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDatosProyecto.Calculos
{
    public class Vector
    {
        private double[] elementos;

        public int Longitud => elementos.Length;
        // constructores
        public Vector(int tamaño)
        {
            elementos = new double[tamaño];
        }

        public Vector(double[] valores)
        {
            elementos = new double[valores.Length];
            Array.Copy(valores, elementos, valores.Length);
        }

        // Indexador para acceder a los elementos
        public double this[int indice]
        {
            get => elementos[indice];
            set => elementos[indice] = value;
        }

        // Sobrecarga del operador * 
        public static Vector operator *(Vector v1, Vector v2)
        {
            if (v1.Longitud != v2.Longitud)
                throw new ArgumentException("Los vectores deben tener la misma longitud");

            Vector resultado = new Vector(v1.Longitud);
            for (int i = 0; i < v1.Longitud; i++)
            {
                resultado[i] = v1[i] * v2[i];
            }
            return resultado;
        }

        // Producto punto 
        public double ProductoPunto(Vector otro)
        {
            if (this.Longitud != otro.Longitud)
                throw new ArgumentException("Los vectores deben tener la misma longitud");

            double suma = 0;
            for (int i = 0; i < this.Longitud; i++)
            {
                suma += this[i] * otro[i];
            }
            return suma;
        }

        // Magnitud (norma) del vector
        public double Magnitud()
        {
            double sumaDeCuadrados = 0;
            for (int i = 0; i < elementos.Length; i++)
            {
                sumaDeCuadrados += elementos[i] * elementos[i];
            }
            return System.Math.Sqrt(sumaDeCuadrados);
        }

        // Normalizar el vector
        public Vector Normalizar()
        {
            double magnitud = this.Magnitud();
            if (magnitud == 0) return new Vector(this.Longitud);

            Vector normalizado = new Vector(this.Longitud);
            for (int i = 0; i < this.Longitud; i++)
            {
                normalizado[i] = this[i] / magnitud;
            }
            return normalizado;
        }

        // Convertir a array
        public double[] ObtenerElementos()
        {
            double[] copia = new double[elementos.Length];
            Array.Copy(elementos, copia, elementos.Length);
            return copia;
        }

        public override string ToString()
        {
            return "[" + string.Join(", ", elementos.Select(x => x.ToString("F3"))) + "]";
        }
    }

}
