using System;
using System.Collections.Generic;

namespace Model;

public partial class Estudiante
{
    public int EstId { get; set; }

    public string EstNombre { get; set; } = null!;

    public string EstCorreo { get; set; } = null!;

    public virtual ICollection<Calificacione> Calificaciones { get; set; } = new List<Calificacione>();
}
