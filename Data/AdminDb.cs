using Fullcalendar.Enum;
using Fullcalendar.Models.Entity;
using Fullcalendar.Service;


namespace Fullcalendar.Data
{
    public class AdminDb
    {
        private readonly DatabaseContext _context;
        private readonly IUsuarioService _usuarioService;

        public AdminDb(DatabaseContext context, IUsuarioService usuarioService)
        {
            _context = context;
            _usuarioService = usuarioService;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await VerificarRolesAsync();
            await VerificarUsuariosAsync("Ricardo", "ricardo@gmail.com", "0999997536", TipoUsuario.Admin);
        }


        private async Task<Usuario> VerificarUsuariosAsync(string username, string email, string telefono, TipoUsuario tipoUsuario)
        {
            Usuario usuario = await _usuarioService.ObtenerUsuario(email);
            if (usuario == null)
            {
                usuario = new Usuario
                {
                    UserName = username,
                    Email = email,
                    PhoneNumber = telefono,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    TipoUsuario = tipoUsuario,
                };
                await _usuarioService.AdminCrearUsuario(usuario, "123456");
                await _usuarioService.AsignarRol(usuario, tipoUsuario.ToString());
            }
            else
            {
                if (!await _usuarioService.UsuarioEnRol(usuario, tipoUsuario.ToString()))
                {
                    await _usuarioService.AsignarRol(usuario, tipoUsuario.ToString());
                }

                usuario.UserName = username;
                usuario.Email =email;
                usuario.PhoneNumber = telefono;

                await _context.SaveChangesAsync();
            }
            return usuario;
        }

        private async Task VerificarRolesAsync()
        {
            await _usuarioService.VerificarRol(TipoUsuario.Admin.ToString());
            await _usuarioService.VerificarRol(TipoUsuario.User.ToString());
            await _usuarioService.VerificarRol(TipoUsuario.Cliente.ToString());
        }

    }
}
