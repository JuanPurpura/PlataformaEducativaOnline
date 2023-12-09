using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ReporteEstudiante
    {
        public Estudiante Estudiante { get; set; }
        public int CursosAprobados { get; set; }
        public int CursosTotales { get; set; }
    }
}
