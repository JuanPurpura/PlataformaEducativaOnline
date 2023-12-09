using System;
using System.Collections.Generic;

namespace Model;

public partial class Calificacione
{
    public int CalId { get; set; }

    public int? CalEstudianteId { get; set; }

    public int? CalNota { get; set; }

    public int? CalCursoId { get; set; }

    public virtual Curso? CalCurso { get; set; }

    public virtual Estudiante? CalEstudiante { get; set; }
}
