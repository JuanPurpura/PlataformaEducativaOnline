using System;
using System.Collections.Generic;

namespace Model;

public partial class Curso
{
    public int CurId { get; set; }

    public string? CurNombre { get; set; }

    public string? CurDescripcion { get; set; }

    public int? CurProfesorId { get; set; }

    public virtual ICollection<Calificacione> Calificaciones { get; set; } = new List<Calificacione>();

    public virtual Profesore? CurProfesor { get; set; }

}
