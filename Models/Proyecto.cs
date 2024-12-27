using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EPARCIAL.Models
{
    public class Proyecto
    {
        [Key]
        [JsonIgnore]
        public int ID { get; set; }
        public string Nombre { get; set; } // Máximo 100 caracteres
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool FlagActivo { get; set; }
        public string Estado { get; set; } // Valores permitidos: Pendiente, En Progreso, Completada

    }

}
