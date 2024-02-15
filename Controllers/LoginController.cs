using Fullcalendar.Data;
using Fullcalendar.Enum;
using Fullcalendar.Models;
using Fullcalendar.Models.Entity;
using Fullcalendar.Service;
using Microsoft.AspNetCore.Mvc;

namespace Fullcalendar.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly DatabaseContext _context;
        protected readonly IFileService _fileService;

        public LoginController(
            IUsuarioService usuarioService,
            DatabaseContext context,
            IFileService fileService)
        {
            _usuarioService = usuarioService;
            _context = context;
            _fileService = fileService;
        }

        public IActionResult IniciarSesion()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Display", "Admin");
                }
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _usuarioService.IniciarSesion(model);
                if (result.Succeeded)
                {
                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("Display", "Admin");
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            }
            return View(model);
        }

        public IActionResult Registro()
        {
            UsuarioViewModel model = new()
            {
                Id = Guid.Empty.ToString(),
                TipoUsuario = TipoUsuario.User,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(UsuarioViewModel model, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError("Imagen", "Por favor, seleccione una imagen.");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var imageResult = _fileService.SaveImage(imageFile);

                    if (imageResult.Item1 == 1)
                    {
                        model.ProfilePicture = imageResult.Item2;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Error al guardar la imagen: {imageResult.Item2}");
                        return View(model);
                    }
                }

                Usuario usuario = await _usuarioService.CrearUsuario(model);
                if (usuario == null)
                {
                    ModelState.AddModelError(string.Empty, "Este correo ya está siendo usado.");
                    return View(model);
                }
                LoginViewModel loginViewModel = new()
                {
                    UserName = model.UserName,
                    Password = model.Password,
                    RememberMe = true,

                };
                var result = await _usuarioService.IniciarSesion(loginViewModel);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await _usuarioService.CerrarSesion();
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> EditarUsuario()
        {
            Console.WriteLine("Iniciando acción EditarUsuario");

            Usuario usuario = await _usuarioService.ObtenerUsuario(User.Identity.Name);
            Console.WriteLine($"Usuario autenticado: {User.Identity.IsAuthenticated}");

            if (usuario == null)
            {
                Console.WriteLine("Usuario no encontrado. Devolviendo NotFound().");

                return NotFound();
            }

            Usuario model = new()
            {
                ProfilePicture = usuario.ProfilePicture,
                UserName = usuario.UserName,
                Email = usuario.Email,
                PhoneNumber = usuario.PhoneNumber,
                Id = usuario.Id,
            };

            Console.WriteLine("Usuario encontrado. Mostrando la vista con el modelo.");

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(Usuario model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var imageResult = _fileService.SaveImage(imageFile);

                    if (imageResult.Item1 == 1)
                    {
                        model.ProfilePicture = imageResult.Item2;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Error al guardar la imagen: {imageResult.Item2}");
                        return View(model);
                    }
                }
                else
                {
                    // Si no se carga una nueva imagen, mantener la misma imagen actual
                    Usuario usuarioActual = await _usuarioService.ObtenerUsuario(User.Identity.Name);
                    model.ProfilePicture = usuarioActual.ProfilePicture;
                }

                Usuario usuario = await _usuarioService.ObtenerUsuario(User.Identity.Name);
                usuario.UserName = model.UserName;
                usuario.PhoneNumber = model.PhoneNumber;
                usuario.Email = model.Email;
                usuario.ProfilePicture = model.ProfilePicture;

                await _usuarioService.ActualizarUsuario(usuario);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }


    }
}