using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace microSerCursos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CursosController : ControllerBase
    {
        [HttpGet]
        public async Task<List<Curso>> Get()
        {
            PlataformaEducativaOnlineDbContext context = new PlataformaEducativaOnlineDbContext();

            List<Curso> cursos = context.Cursos
                .Include(curso => curso.CurProfesor)
                .Select(curso => new Curso
                {
                    CurId = curso.CurId,
                    CurNombre = curso.CurNombre,
                    CurDescripcion = curso.CurDescripcion,
                    CurProfesorId = curso.CurProfesorId,
                    CurProfesor = new Profesore
                    {
                        ProId = curso.CurProfesor.ProId,
                        ProNombre = curso.CurProfesor.ProNombre,
                        ProCorreo = curso.CurProfesor.ProCorreo
                    }
                })
                .ToList();

            return cursos;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Curso>> Get(int id)
        {
            PlataformaEducativaOnlineDbContext context = new PlataformaEducativaOnlineDbContext();
            Curso curso = await context.Cursos.FindAsync(id);

            if (curso == null)
            {
                return NotFound($"No existe el curso con id: {id}");
            }

            return curso;
        }

        [HttpGet("reporte")]
        public async Task<IEnumerable<CursoConEstudiantes>> CursosConEstudiantes()
        {
            PlataformaEducativaOnlineDbContext context = new PlataformaEducativaOnlineDbContext();

            var cursosConEstudiantes = context.Cursos.Select(curso => new CursoConEstudiantes
            {
                CursoId = curso.CurId,
                NombreCurso = curso.CurNombre,
                CantidadEstudiantes = context.Calificaciones
                    .Where(cal => cal.CalCursoId == curso.CurId)
                    .Select(cal => cal.CalEstudianteId)
                    .Distinct()
                    .Count()
            }).ToList();

            return cursosConEstudiantes;
        }

        [HttpPost]
        public async Task<ActionResult<Curso>> CrearCurso([FromBody] Curso nuevoCurso)
        {
            try
            {
                using (var context = new PlataformaEducativaOnlineDbContext())
                {
                    context.Cursos.Add(nuevoCurso);
                    await context.SaveChangesAsync();
                }

                return CreatedAtAction(nameof(Get), new { id = nuevoCurso.CurId }, nuevoCurso);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear el curso: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarCurso(int id, [FromBody] Curso curso)
        {
            try
            {
                if (id != curso.CurId)
                {
                    return BadRequest($"No coincide el id del curso con el id de la URL");
                }

                using (var context = new PlataformaEducativaOnlineDbContext())
                {
                    context.Entry(curso).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    return Ok($"Curso con id: {id} actualizado");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar el curso: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarCurso(int id)
        {
            try
            {
                using (var context = new PlataformaEducativaOnlineDbContext())
                {
                    var curso = await context.Cursos.FindAsync(id);

                    if (curso == null)
                    {
                        return NotFound($"No existe el curso con id: {id}");
                    }

                    context.Cursos.Remove(curso);
                    await context.SaveChangesAsync();
                    return Ok($"Curso con id: {id} eliminado");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar el curso: {ex.Message}");
            }
        }

        //para saber si profesores tiene dependencias
        [HttpGet]
        [Route("profesor/{id}")]
        public async Task<ActionResult<Curso>> GetProfesor(int id)
        {
            try
            {
                PlataformaEducativaOnlineDbContext context = new PlataformaEducativaOnlineDbContext();

                // Verifica si hay cursos asociados al profesor
                var tieneDependencias = await context.Cursos.AnyAsync(cur => cur.CurProfesorId == id);

                return Ok(tieneDependencias);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al verificar dependencias en profesor: {ex.Message}");
            }
        }
    }

}
