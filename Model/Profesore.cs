using System;
using System.Collections.Generic;

namespace Model;

public partial class Profesore
{
    public int ProId { get; set; }

    public string ProNombre { get; set; } = null!;

    public string ProCorreo { get; set; } = null!;

    public virtual ICollection<Curso> Cursos { get; set; } = new List<Curso>();
}
