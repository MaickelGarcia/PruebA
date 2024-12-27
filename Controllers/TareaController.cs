using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EPARCIAL.Data;
using EPARCIAL.Models;

namespace EPARCIAL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TareaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TareaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Tarea
        [HttpGet]
        public async Task<IActionResult> GetTareas()
        {
            try
            {
                var tareas = await _context.Tareas
                                           .Where(t => t.FlagActivo)
                                           .Select(t => new
                                           {
                                               t.ID,
                                               t.Nombre,
                                               t.Descripcion,
                                               t.FechaLimite,
                                               t.Estado,
                                               t.Prioridad,
                                               t.FlagActivo,
                                               t.ProyectoID  // Solo el ProyectoID
                                           })
                                           .ToListAsync();

                return Ok(new { message = "Tareas obtenidas", status = "success", data = tareas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las tareas", status = "error", error = ex.Message });
            }
        }

        // GET: api/Tarea/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTarea(int id)
        {
            try
            {
                var tarea = await _context.Tareas
                                          .Where(t => t.ID == id && t.FlagActivo)
                                          .Select(t => new
                                          {
                                              t.ID,
                                              t.Nombre,
                                              t.Descripcion,
                                              t.FechaLimite,
                                              t.Estado,
                                              t.Prioridad,
                                              t.FlagActivo,
                                              t.ProyectoID  // Solo el ProyectoID
                                          })
                                          .FirstOrDefaultAsync();

                if (tarea == null)
                    return NotFound(new { message = "Tarea no encontrada", status = "error" });

                return Ok(new { message = "Tarea obtenida", status = "success", data = tarea });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la tarea", status = "error", error = ex.Message });
            }
        }

        // POST: api/Tarea
        [HttpPost]
        public async Task<IActionResult> CreateTarea([FromBody] Tarea tarea)
        {
            try
            {
                // Validar que el proyecto exista y esté activo
                var proyecto = await _context.Proyectos.FindAsync(tarea.ProyectoID);
                if (proyecto == null || !proyecto.FlagActivo)
                    return BadRequest(new { message = "El proyecto asociado no existe o no está activo", status = "error" });

                // Estado predeterminado si no se proporciona
                tarea.Estado ??= "Pendiente";

                _context.Tareas.Add(tarea);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Tarea creada exitosamente", status = "success", data = tarea });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear la tarea", status = "error", error = ex.Message });
            }
        }

        // PUT: api/Tarea/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTarea(int id, [FromBody] Tarea tarea)
        {
            try
            {
                var existingTarea = await _context.Tareas.FindAsync(id);

                if (existingTarea == null || !existingTarea.FlagActivo)
                    return NotFound(new { message = "Tarea no encontrada", status = "error" });

                // Validar que el proyecto asociado exista y esté activo
                var proyecto = await _context.Proyectos.FindAsync(tarea.ProyectoID);
                if (proyecto == null || !proyecto.FlagActivo)
                    return BadRequest(new { message = "El proyecto asociado no existe o no está activo", status = "error" });

                // Actualizar valores
                existingTarea.Nombre = tarea.Nombre;
                existingTarea.Descripcion = tarea.Descripcion;
                existingTarea.FechaLimite = tarea.FechaLimite;
                existingTarea.Estado = tarea.Estado ?? "Pendiente";
                existingTarea.Prioridad = tarea.Prioridad;
                existingTarea.ProyectoID = tarea.ProyectoID;  // Solo actualizamos el ProyectoID

                await _context.SaveChangesAsync();

                return Ok(new { message = "Tarea actualizada exitosamente", status = "success", data = existingTarea });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar la tarea", status = "error", error = ex.Message });
            }
        }

        // DELETE: api/Tarea/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarea(int id)
        {
            try
            {
                var tarea = await _context.Tareas.FindAsync(id);

                if (tarea == null || !tarea.FlagActivo)
                    return NotFound(new { message = "Tarea no encontrada", status = "error" });

                // Desactivamos en lugar de eliminar físicamente
                tarea.FlagActivo = false;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Tarea eliminada exitosamente", status = "success" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar la tarea", status = "error", error = ex.Message });
            }
        }
    }
}
