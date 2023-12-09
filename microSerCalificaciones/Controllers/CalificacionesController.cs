using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace microSerCalificaciones.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalificacionesController : ControllerBase
    {
        [HttpGet]
        public async Task<List<Calificacione>> Get()
        {
            PlataformaEducativaOnlineDbContext context = new PlataformaEducativaOnlineDbContext();

            var calificaciones = context.Calificaciones
                .Include(cal => cal.CalCurso)
                .Include(cal => cal.CalEstudiante)
                .Select(cal => new Calificacione
                {
                    CalId = cal.CalId,
                    CalEstudianteId = cal.CalEstudianteId,
                    CalNota = cal.CalNota,
                    CalCursoId = cal.CalCursoId,
                    CalEstudiante = new Estudiante
                    {
                        EstId = cal.CalEstudiante.EstId,
                        EstNombre = cal.CalEstudiante.EstNombre
                    },
                    CalCurso = new Curso
                    {   
                        CurId = cal.CalCurso.CurId,
                        CurProfesorId = cal.CalCurso.CurProfesorId,
                        CurNombre = cal.CalCurso.CurNombre
                    }
                })
                .ToList();

            return calificaciones;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Calificacione>> Get(int id)
        {
            PlataformaEducativaOnlineDbContext context = new PlataformaEducativaOnlineDbContext();
            Calificacione calificacion = await context.Calificaciones.FindAsync(id);

            if (calificacion == null)
            {
                return NotFound($"No existe la calificación con id: {id}");
            }

            return calificacion;
        }

        [HttpGet]
        [Route("reporte")]
        public async Task<IEnumerable<CalificacionesPorProfesor>> CalificacionesPorProfesor()
        {
            PlataformaEducativaOnlineDbContext context = new PlataformaEducativaOnlineDbContext();

            var calificacionesPorProfesor = context.Profesores.Select(profesor => new CalificacionesPorProfesor
            {
                Profesor = profesor,
                CantidadCalificaciones = context.Calificaciones.Count(cal => cal.CalCurso.CurProfesorId == profesor.ProId)
            });

            return calificacionesPorProfesor;
        }

        [HttpPost]
        public async Task<ActionResult<Calificacione>> CrearCalificacion([FromBody] Calificacione nuevaCalificacion)
        {
            try
            {
                using (var context = new PlataformaEducativaOnlineDbContext())
                {
                    context.Calificaciones.Add(nuevaCalificacion);
                    await context.SaveChangesAsync();
                }

                return CreatedAtAction(nameof(Get), new { id = nuevaCalificacion.CalId }, nuevaCalificacion);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear la calificación: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarCalificacion(int id, [FromBody] Calificacione calificacion)
        {
            try
            {
                if (id != calificacion.CalId)
                {
                    return BadRequest($"No coincide el id de la calificación con el id de la URL");
                }

                using (var context = new PlataformaEducativaOnlineDbContext())
                {
                    context.Entry(calificacion).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    return Ok($"Calificación con id: {id} actualizada");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar la calificación: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarCalificacion(int id)
        {
            try
            {
                using (var context = new PlataformaEducativaOnlineDbContext())
                {
                    var calificacion = await context.Calificaciones.FindAsync(id);

                    if (calificacion == null)
                    {
                        return NotFound($"No existe la calificación con id: {id}");
                    }

                    context.Calificaciones.Remove(calificacion);
                    await context.SaveChangesAsync();
                    return Ok($"Calificación con id: {id} eliminada");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar la calificación: {ex.Message}");
            }
        }

        //para saber si cursos tiene dependencias
        [HttpGet]
        [Route("curso/{id}")]
        public async Task<ActionResult<Calificacione>> GetCurso(int id)
        {
            try
            {
                PlataformaEducativaOnlineDbContext context = new PlataformaEducativaOnlineDbContext();

                // Verifica si hay calificaciones asociadas al curso
                var tieneDependencias = await context.Calificaciones.AnyAsync(cal => cal.CalCursoId == id);

                return Ok(tieneDependencias);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al verificar dependencias en curso: {ex.Message}");
            }
        }

        //para saber si estudiantes tiene dependencias
        [HttpGet]
        [Route("estudiante/{id}")]
        public async Task<ActionResult<Calificacione>> GetEstudiante(int id)
        {
            try
            {
                PlataformaEducativaOnlineDbContext context = new PlataformaEducativaOnlineDbContext();

                // Verifica si hay calificaciones asociadas al estudiante
                var tieneDependencias = context.Calificaciones.AnyAsync(cal => cal.CalEstudianteId == id);

                return Ok(tieneDependencias.Result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al verificar dependencias en estudiante: {ex.Message}");
            }
        }



    }
}
