using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;

namespace microSerUsuarios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstudiantesController : ControllerBase
    {
        [HttpGet]
        public async Task<List<Estudiante>> Get()
        {
            PlataformaEducativaOnlineDbContext context = new PlataformaEducativaOnlineDbContext();
            List<Estudiante> estudiantes = context.Estudiantes.ToList();
            return estudiantes;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Estudiante>> Get(int id)
        {
            PlataformaEducativaOnlineDbContext context = new PlataformaEducativaOnlineDbContext();
            Estudiante estudiante = await context.Estudiantes.FindAsync(id);

            if (estudiante == null)
            {
                return NotFound($"No existe el estudiante con id: {id}");
            }

            return estudiante;
        }

        [HttpGet("reporte")]
        public async Task<List<ReporteEstudiante>> ObtenerReportesEstudiantes()
        {
            PlataformaEducativaOnlineDbContext context = new PlataformaEducativaOnlineDbContext();

            var reportesEstudiantes = new List<ReporteEstudiante>();

            foreach (var estudiante in context.Estudiantes)
            {
                var reporteEstudiante = new ReporteEstudiante
                {
                    Estudiante = estudiante,
                    CursosAprobados = context.Calificaciones
                        .Count(cal => cal.CalEstudianteId == estudiante.EstId && cal.CalNota >= 7),
                    CursosTotales = context.Calificaciones
                        .Where(cal => cal.CalEstudianteId == estudiante.EstId)
                        .Select(cal => cal.CalCursoId)
                        .Distinct()
                        .Count()
                };

                reportesEstudiantes.Add(reporteEstudiante);
            }

            return reportesEstudiantes;
        }

        [HttpPost]
        public async Task<ActionResult<Estudiante>> CrearEstudiante([FromBody] Estudiante nuevoEstudiante)
        {
            try
            {
                using (var context = new PlataformaEducativaOnlineDbContext())
                {
                    context.Estudiantes.Add(nuevoEstudiante);
                    await context.SaveChangesAsync();
                }

                return CreatedAtAction(nameof(Get), new { id = nuevoEstudiante.EstId }, nuevoEstudiante);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear el estudiante: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarEstudiante(int id, [FromBody] Estudiante estudiante)
        {
            try
            {
                if (id != estudiante.EstId)
                {
                    return BadRequest($"No coincide el id del estudiante con el id de la URL");
                }

                using (var context = new PlataformaEducativaOnlineDbContext())
                {
                    context.Entry(estudiante).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    return Ok($"Estudiante con id: {id} actualizado");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar el estudiante: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarEstudiante(int id)
        {
            try
            {
                using (var context = new PlataformaEducativaOnlineDbContext())
                {
                    var estudiante = await context.Estudiantes.FindAsync(id);

                    if (estudiante == null)
                    {
                        return NotFound($"No existe el estudiante con id: {id}");
                    }

                    context.Estudiantes.Remove(estudiante);
                    await context.SaveChangesAsync();
                    return Ok($"Estudiante con id: {id} eliminado");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar el estudiante: {ex.Message}");
            }
        }

    }
}
