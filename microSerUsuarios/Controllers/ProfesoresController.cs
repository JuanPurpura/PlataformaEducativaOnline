using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;

namespace microSerUsuarios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfesoresController : ControllerBase
    {
        [HttpGet]
        public async Task<List<Profesore>> Get()
        {
            PlataformaEducativaOnlineDbContext context = new PlataformaEducativaOnlineDbContext();
            List<Profesore> profesores = context.Profesores.ToList();
            return profesores;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Profesore>> Get(int id)
        {
            PlataformaEducativaOnlineDbContext context = new PlataformaEducativaOnlineDbContext();
            Profesore profesor = await context.Profesores.FindAsync(id);

            if (profesor == null)
            {
                return NotFound($"No existe el profesor con id: {id}");
            }

            return profesor;
        }

        [HttpGet("reporte")]
        public async Task<List<ProfesorConEstudiantes>> ObtenerProfesoresConEstudiantes()
        {
            PlataformaEducativaOnlineDbContext context = new PlataformaEducativaOnlineDbContext();

            var profesoresConEstudiantes = new List<ProfesorConEstudiantes>();

            foreach (var profesor in context.Profesores)
            {
                var estudiantesDelProfesor = context.Calificaciones
                    .Where(calificacion => calificacion.CalCurso.CurProfesorId == profesor.ProId)
                    .Select(calificacion => calificacion.CalEstudiante)
                    .Distinct()
                    .ToList();

                var profesorConEstudiantes = new ProfesorConEstudiantes
                {
                    Profesor = profesor,
                    Estudiantes = estudiantesDelProfesor
                };

                profesoresConEstudiantes.Add(profesorConEstudiantes);
            }

            return profesoresConEstudiantes;
        }

        [HttpPost]
        public async Task<ActionResult<Profesore>> CrearProfesor([FromBody] Profesore nuevoProfesor)
        {
            try
            {
                // Puedes validar o procesar los datos según tus necesidades antes de agregarlos a la base de datos.
                // Aquí simplemente se agrega el nuevo profesor al contexto y se guarda en la base de datos.

                using (var context = new PlataformaEducativaOnlineDbContext())
                {
                    context.Profesores.Add(nuevoProfesor);
                    await context.SaveChangesAsync();
                }

                return CreatedAtAction(nameof(Get), new { id = nuevoProfesor.ProId }, nuevoProfesor);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear el profesor: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarProfesor(int id, [FromBody] Profesore profesor)
        {
            try
            {
                if (id != profesor.ProId)
                {
                    return BadRequest($"No coincide el id del profesor con el id de la URL");
                }

                using (var context = new PlataformaEducativaOnlineDbContext())
                {
                    context.Entry(profesor).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    return Ok($"Profesor con id: {id} actualizado");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar el profesor: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarProfesor(int id)
        {
            try
            {
                using (var context = new PlataformaEducativaOnlineDbContext())
                {
                    var profesor = await context.Profesores.FindAsync(id);

                    if (profesor == null)
                    {
                        return NotFound($"No existe el profesor con id: {id}");
                    }

                    context.Profesores.Remove(profesor);
                    await context.SaveChangesAsync();
                    return Ok($"Profesor con id: {id} eliminado");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar el profesor: {ex.Message}");
            }
        }
    }
}
