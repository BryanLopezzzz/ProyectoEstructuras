using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDatosProyecto.Models
{
    internal class ResultadoSimilitud
    {
        public int DocumentoId { get; set; }
        public double Similitud { get; set; }

        public override string ToString()
        {
            return $"Documento {DocumentoId}: Similitud = {Similitud:F4}";
        }
    }
}
