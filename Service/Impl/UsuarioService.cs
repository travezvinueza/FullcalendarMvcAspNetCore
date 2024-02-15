using Fullcalendar.Data;
using Fullcalendar.Models;
using Fullcalendar.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Fullcalendar.Service.Impl
{
    public class UsuarioService : IUsuarioService
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Usuario> _signInManager;

        public UsuarioService(DatabaseContext context, UserManager<Usuario> userManager,
            RoleManager<IdentityRole> roleManager, SignInManager<Usuario> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task AsignarRol(Usuario usuario, string nombreRol)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario), "El usuario no puede ser nulo.");
            }

            await _userManager.AddToRoleAsync(usuario, nombreRol);
        }

        public async Task<bool> UsuarioEnRol(Usuario usuario, string nombreRol)
        {
            return await _userManager.IsInRoleAsync(usuario, nombreRol);
        }

        public async Task VerificarRol(string nombreRol)
        {
            bool roleExiste = await _roleManager.RoleExistsAsync(nombreRol);
            if (!roleExiste)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = nombreRol
                });
            }

        }

        public async Task<IdentityResult> AdminCrearUsuario(Usuario usuario, string password)
        {
            return await _userManager.CreateAsync(usuario, password);
        }

        public async Task CerrarSesion()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<Usuario> CrearUsuario(UsuarioViewModel model)
        {
            Usuario usuario = new Usuario
            {
                ProfilePicture = model.ProfilePicture,
                UserName = model.UserName,
                Email = model.Email ?? throw new ArgumentNullException(nameof(model.Email), "El correo electrónico no puede ser nulo."),
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                TipoUsuario = model.TipoUsuario
            };

            IdentityResult result = await _userManager.CreateAsync(usuario, model.Password);

            if (result != IdentityResult.Success)
            {
                throw new InvalidOperationException($"Error al crear el usuario: {string.Join(", ", result.Errors)}");
            }

            Usuario nuevoUsuario = await ObtenerUsuario(usuario.Email);

            if (nuevoUsuario == null)
            {
                throw new ArgumentNullException(nameof(nuevoUsuario), "El nuevo usuario no puede ser nulo.");
            }

            await AsignarRol(nuevoUsuario, usuario.TipoUsuario.ToString());

            return nuevoUsuario;
        }

        public async Task<SignInResult> IniciarSesion(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(
            model.UserName,
            model.Password,
            model.RememberMe,
            true);
        }

        public async Task<IdentityResult> ActualizarUsuario(Usuario usuario)
        {
            return await _userManager.UpdateAsync(usuario);
        }

        public async Task<Usuario> ObtenerUsuario(string username)
        {
            var usuario = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            return usuario;
        }

        public async Task<Usuario> ObtenerUsuario(Guid userId)
        {
            return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId.ToString());
        }

        //trae una lista de todos los usuarios en la vista admin
        public async Task<List<Usuario>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

    }
}
