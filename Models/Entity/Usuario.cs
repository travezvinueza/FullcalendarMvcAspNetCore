using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Fullcalendar.Enum;
using Microsoft.AspNetCore.Identity;

namespace Fullcalendar.Models.Entity
{
    public class Usuario : IdentityUser
    {
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public string? ProfilePicture { get; set; }

        public TipoUsuario TipoUsuario { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();
    }
}

