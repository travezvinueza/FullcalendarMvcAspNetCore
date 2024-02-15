using System.ComponentModel.DataAnnotations;

namespace Fullcalendar.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El usuario es requerido.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida.")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}