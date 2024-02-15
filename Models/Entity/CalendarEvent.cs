using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fullcalendar.Models.Entity
{
    public class CalendarEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }   
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Color { get; set; } = string.Empty;
        public Boolean AllDay { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; } 
        public string? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
  
    }
}

