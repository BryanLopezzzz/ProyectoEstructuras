using EstructurasDatosProyecto.Models;
using System;
using EstructurasDatosProyecto.Calculos;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDatosProyecto.Indexing
{
    // esta es la calculadora para crear los documentos
    public class CalculadoraTfIdf
    {
        private InvertedIndex indice;
        private int totalDocumentos;

        public CalculadoraTfIdf(InvertedIndex indice, int totalDocumentos)
        {
            this.indice = indice;
            this.totalDocumentos = totalDocumentos;
        }

        // Calcula IDF de un termino
        public double CalcularIdf(string termino)
        {
            TermInfo termInfo = indice.BuscarTermino(termino);
            if (termInfo != null)
            {
                int df = termInfo.Df;
                return System.Math.Log10((double)totalDocumentos / df);
            }
            return 0.0;
        }

        // Crea vector para un documento
        public Vector CrearVectorDocumento(int docId, ListaDobleCircular<string> todosLosTerminos)
        {
            int cantidadTerminos = todosLosTerminos.Count();
            Vector vector = new Vector(cantidadTerminos);

            int posicion = 0;
            todosLosTerminos.Recorrer(termino =>
            {
                double tf = 0;

                TermInfo termInfo = indice.BuscarTermino(termino);
                if (termInfo != null)
                {
                    // buscar posting de este documento
                    for (int j = 0; j < termInfo.Postings.Count; j++)
                    {
                        if (termInfo.Postings[j].DocId == docId)
                        {
                            tf = termInfo.Postings[j].Tf;
                            break;
                        }
                    }
                }

                double idf = CalcularIdf(termino);
                vector[posicion] = tf * idf;
                posicion++;
            });

            return vector;
        }

        // Crea vector para consulta
        public Vector CrearVectorConsulta(string[] terminosConsulta, ListaDobleCircular<string> todosLosTerminos)
        {
            int cantidadTerminos = todosLosTerminos.Count();
            Vector vector = new Vector(cantidadTerminos);

            int posicion = 0;
            todosLosTerminos.Recorrer(termino =>
            {
                int tf = 0;

                // contar cuantas veces aparece en la consulta
                for (int j = 0; j < terminosConsulta.Length; j++)
                {
                    if (terminosConsulta[j] == termino)
                    {
                        tf++;
                    }
                }

                double idf = CalcularIdf(termino);
                vector[posicion] = tf * idf;
                posicion++;
            });

            return vector;
        }

        // Obtiene todos los terminos del indice en una ListaDobleCircular
        public ListaDobleCircular<string> ObtenerTodosLosTerminos()
        {
            return indice.ObtenerTodosLosTerminos();
        }

        // Busca un termino en la lista y devuelve su posicion
        public int BuscarPosicionTermino(string termino, ListaDobleCircular<string> todosLosTerminos)
        {
            int posicion = 0;
            int encontrado = -1;

            todosLosTerminos.Recorrer(t =>
            {
                if (t == termino && encontrado == -1)
                {
                    encontrado = posicion;
                }
                posicion++;
            });

            return encontrado;
        }

        // Obtiene un termino por su posicion
        public string ObtenerTerminoPorPosicion(int posicion, ListaDobleCircular<string> todosLosTerminos)
        {
            string resultado = null;
            int contador = 0;

            todosLosTerminos.Recorrer(termino =>
            {
                if (contador == posicion)
                {
                    resultado = termino;
                }
                contador++;
            });

            return resultado;
        }
    }
}

