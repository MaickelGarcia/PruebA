using System.ComponentModel.DataAnnotations;

namespace EPARCIAL.Models
{
    public class Tarea
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaLimite { get; set; }
        public string Estado { get; set; }
        public string Prioridad { get; set; }
        public bool FlagActivo { get; set; } = true;
        public int ProyectoID { get; set; } // Relación directa al Proyecto
    }

}
