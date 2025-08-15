using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDatosProyecto.Models
{
    public class Vocabulario // Clase para manejar el vocabulario de terminos del indice invertido
    {
        private ListaDobleCircular<string> terminos;

        public Vocabulario()
        {
            terminos = new ListaDobleCircular<string>();
        }

        public void AgregarTermino(string termino)
        {
            // Solo agregar si no existe
            string existe = terminos.Buscar(t => t == termino);
            if (existe == null)
            {
                terminos.Insertar(termino);
            }
        }

        public int ObtenerIndice(string termino)
        {
            int indice = -1;
            int contador = 0;

            terminos.Recorrer(t =>
            {
                if (t == termino && indice == -1)
                {
                    indice = contador;
                }
                contador++;
            });

            return indice;
        }

        public string ObtenerTermino(int indice)
        {
            string resultado = null;
            int contador = 0;

            terminos.Recorrer(t =>
            {
                if (contador == indice)
                {
                    resultado = t;
                }
                contador++;
            });

            return resultado;
        }

        public int Count()
        {
            return terminos.Count();
        }

        public ListaDobleCircular<string> ObtenerTerminos()
        {
            return terminos;
        }

        public bool EstaVacio()
        {
            return terminos.EstaVacia();
        }
    }
}
