using EPARCIAL.Data;
using EPARCIAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EPARCIAL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProyectosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProyectosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProyectos()
        {
            try
            {
                var proyectos = await _context.Proyectos
                                              .Where(p => p.FlagActivo)
                                              .Select(p => new
                                              {
                                                  p.ID,  // Incluye el ID
                                                  p.Nombre,
                                                  p.Descripcion,
                                                  p.FechaInicio,
                                                  p.FechaFin,
                                                  p.Estado
                                              })
                                              .ToListAsync();

                return Ok(new { message = "Proyectos obtenidos", status = "success", data = proyectos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los proyectos", status = "error", error = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProyecto(int id)
        {
            try
            {
                var proyecto = await _context.Proyectos.FindAsync(id);
                if (proyecto == null || !proyecto.FlagActivo)
                    return NotFound(new { message = "Proyecto no encontrado", status = "fail" });

                return Ok(new { message = "Proyecto obtenido", status = "success", data = proyecto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el proyecto", status = "error", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProyecto([FromBody] Proyecto proyecto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Datos inválidos", status = "fail" });

            // Validación: Nombre único y longitud
            if (proyecto.Nombre.Length > 100)
                return BadRequest(new { message = "El nombre del proyecto no debe exceder los 100 caracteres", status = "fail" });

            var nombreExistente = await _context.Proyectos.AnyAsync(p => p.Nombre == proyecto.Nombre && p.FlagActivo);
            if (nombreExistente)
                return BadRequest(new { message = "El nombre del proyecto ya existe", status = "fail" });

            // Validación: Fechas coherentes
            if (proyecto.FechaInicio > proyecto.FechaFin)
                return BadRequest(new { message = "La fecha de inicio no puede ser posterior a la fecha de fin", status = "fail" });

            try
            {
                _context.Proyectos.Add(proyecto);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProyecto),
                                       new { id = proyecto.ID },
                                       new { message = "Proyecto creado", status = "success", data = proyecto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el proyecto", status = "error", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProyecto(int id, [FromBody] Proyecto proyecto)
        {
            try
            {
                // Buscar el proyecto existente por ID
                var existingProyecto = await _context.Proyectos.FindAsync(id);
                if (existingProyecto == null)
                {
                    return NotFound(new { message = "Proyecto no encontrado", status = "fail" });
                }

                // Validar el modelo recibido
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Datos inválidos", status = "fail", errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
                }

                // Validar que el nombre no exceda 100 caracteres
                if (proyecto.Nombre.Length > 100)
                {
                    return BadRequest(new { message = "El nombre no debe exceder los 100 caracteres.", status = "fail" });
                }

                // Validar fechas coherentes
                if (proyecto.FechaInicio > proyecto.FechaFin)
                {
                    return BadRequest(new { message = "La fecha de inicio no puede ser posterior a la fecha de fin.", status = "fail" });
                }

                // Verificar si ya existe otro proyecto con el mismo nombre (único)
                var proyectoDuplicado = await _context.Proyectos
                                                      .Where(p => p.ID != id && p.Nombre == proyecto.Nombre)
                                                      .FirstOrDefaultAsync();
                if (proyectoDuplicado != null)
                {
                    return BadRequest(new { message = "Ya existe un proyecto con el mismo nombre.", status = "fail" });
                }

                // Actualizar los campos permitidos
                existingProyecto.Nombre = proyecto.Nombre;
                existingProyecto.Descripcion = proyecto.Descripcion;
                existingProyecto.FechaInicio = proyecto.FechaInicio;
                existingProyecto.FechaFin = proyecto.FechaFin;
                existingProyecto.FlagActivo = proyecto.FlagActivo;
                existingProyecto.Estado = proyecto.Estado;

                // Guardar los cambios
                await _context.SaveChangesAsync();

                return Ok(new { message = "Proyecto actualizado", status = "success", data = existingProyecto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el proyecto", status = "error", error = ex.Message });
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProyecto(int id)
        {
            var proyecto = await _context.Proyectos.FindAsync(id);
            if (proyecto == null || !proyecto.FlagActivo)
                return NotFound(new { message = "Proyecto no encontrado", status = "fail" });

            try
            {
                proyecto.FlagActivo = false;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Proyecto eliminado (soft-delete)", status = "success", data = proyecto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el proyecto", status = "error", error = ex.Message });
            }
        }
    }
}
