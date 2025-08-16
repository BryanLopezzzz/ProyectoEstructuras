using EstructurasDatosProyecto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDatosProyecto.Calculos
{
    internal class SimilitudCoseno
    {
        public static double CalcularSimilitud(Vector vectorA, Vector vectorB)
        {
            if (vectorA.Longitud != vectorB.Longitud)
                throw new ArgumentException("Los vectores deben tener la misma longitud");

            double productoPunto = vectorA.ProductoPunto(vectorB);
            double magnitudA = vectorA.Magnitud();
            double magnitudB = vectorB.Magnitud();

        
          
            return productoPunto / (magnitudA * magnitudB);
        }

        /// Calcula varias similitudes coseno entre una consulta y una lista de documentos
        
        public static ListaDobleCircular<ResultadoSimilitud> CalcularSimilitudes(
            Vector consulta,
            ListaDobleCircular<Vector> documentos,
            ListaDobleCircular<int> idsDocumentos = null)
        {
            var resultados = new ListaDobleCircular<ResultadoSimilitud>();

            int indice = 0;
            documentos.Recorrer(vectorDoc =>
            {
                double similitud = CalcularSimilitud(consulta, vectorDoc);

                // Obtener el ID del documento correspondiente
                int docId = indice;
                if (idsDocumentos != null)
                {
                    int contador = 0;
                    idsDocumentos.Recorrer(id =>
                    {
                        if (contador == indice)
                            docId = id;
                        contador++;
                    });
                }

                resultados.Insertar(new ResultadoSimilitud
                {
                    DocumentoId = docId,
                    Similitud = similitud
                });

                indice++;
            });

            // Ordenar por similitud descendente 
            return OrdenarPorSimilitud(resultados);
        }

       
        /// Ordena los resultados por similitud descendente
        /// Implementación de ordenamiento burbuja para ListaDobleCircular
      
        private static ListaDobleCircular<ResultadoSimilitud> OrdenarPorSimilitud(ListaDobleCircular<ResultadoSimilitud> resultados)
        {
            if (resultados.EstaVacia()) return resultados;

            int n = resultados.Count();

            // Convertir a array temporal para ordenar
            ResultadoSimilitud[] array = new ResultadoSimilitud[n];
            int indice = 0;
            resultados.Recorrer(resultado =>
            {
                array[indice] = resultado;
                indice++;
            });

            // Ordenamiento burbuja
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (array[j].Similitud < array[j + 1].Similitud)
                    {
                        // Intercambiar
                        ResultadoSimilitud temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                    }
                }
            }

            // Crear nueva lista ordenada
            var resultadosOrdenados = new ListaDobleCircular<ResultadoSimilitud>();
            for (int i = 0; i < n; i++)
            {
                resultadosOrdenados.Insertar(array[i]);
            }

            return resultadosOrdenados;
        }


    }
}
