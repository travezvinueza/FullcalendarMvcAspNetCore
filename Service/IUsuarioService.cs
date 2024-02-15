using Fullcalendar.Models;
using Fullcalendar.Models.Entity;
using Microsoft.AspNetCore.Identity;


namespace Fullcalendar.Service
{
    public interface IUsuarioService
    {
        Task<IdentityResult> AdminCrearUsuario(Usuario usuario, string password);
        Task<Usuario> ObtenerUsuario(string username);
        Task VerificarRol(string nombreRol);
        Task AsignarRol(Usuario usuario, string nombreRol);
        Task<bool> UsuarioEnRol(Usuario usuario, string nombreRol);
        Task<SignInResult> IniciarSesion(LoginViewModel model);
        Task CerrarSesion();
        Task<Usuario> CrearUsuario(UsuarioViewModel model);
        Task<IdentityResult> ActualizarUsuario(Usuario usuario);
        Task<Usuario> ObtenerUsuario(Guid userId);

        Task<List<Usuario>> GetAll(); //trae una lista

    }

}
