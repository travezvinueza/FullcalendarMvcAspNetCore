using AspNetCoreHero.ToastNotification.Abstractions;
using Fullcalendar.Data;
using Fullcalendar.Models;
using Fullcalendar.Models.Entity;
using Fullcalendar.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Fullcalendar.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        protected readonly IFileService _fileService;
        private readonly INotyfService _notifyService;
        private readonly DatabaseContext _context;
        private readonly UserManager<Usuario> _userManager;


        public AdminController(
            DatabaseContext context,
            IUsuarioService usuarioService,
            INotyfService notifyService,
                UserManager<Usuario> userManager,

            IFileService fileService)
        {
            _context = context;
            _usuarioService = usuarioService;
            _notifyService = notifyService;
            _fileService = fileService;
            _userManager = userManager;


        }

        public IActionResult Display()
        {
            ViewBag.CantidadEventos = _context.CalendarEvents.Count();
            ViewBag.CantidadUsuarios = _context.Users.Count();
            ViewBag.CantidadRoles = _context.Roles.Count();


            return View();
        }

        public IActionResult ResumenEvento()
        {
            DateTime FechaInicio = DateTime.Now;
            FechaInicio = FechaInicio.AddDays(-5);

            var Lista = _context.Users
                .Select(usuario => new
                {
                    UsuarioId = usuario.Id,
                    UsuarioNombre = usuario.UserName,
                    CantidadEventos = usuario.CalendarEvents.Count(evento => evento.Start >= FechaInicio)
                })
                .ToList();

            return Json(Lista);
        }

        public async Task<IActionResult> Lista()
        {
            var usuarios = await _usuarioService.GetAll();
            return View(usuarios);
        }

          [HttpGet]
        public IActionResult AddUser()
        {
            var model = new UsuarioViewModel
            {
                AvailableRoles = _context.Roles.Select(r => r.Name).ToList()!
            };

            return View(model);
        }


        //agrega y guarda el usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(UsuarioViewModel newUser)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(newUser);
                }

                if (newUser.ImageFile != null)
                {
                    var imageResult = _fileService.SaveImage(newUser.ImageFile);

                    if (imageResult.Item1 != 1)
                    {
                        ModelState.AddModelError("", imageResult.Item2);
                        return View(newUser);
                    }

                    newUser.ProfilePicture = imageResult.Item2;
                }

                var user = new Usuario
                {
                    ProfilePicture = newUser.ProfilePicture,
                    UserName = newUser.UserName,
                    Email = newUser.Email,
                    PhoneNumber = newUser.PhoneNumber,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                };

                var result = await _userManager.CreateAsync(user, newUser.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, newUser.TipoUsuario.ToString());
                    return RedirectToAction("Display");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(newUser);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error inesperado: " + ex.Message);
                return View(newUser);
            }
        }





        [HttpGet]
        public async Task<IActionResult> EliminarUsuario(string username)
        {
            try
            {
                var result = await _usuarioService.EliminarUsuario(username);

                if (result.Succeeded)
                {
                    _notifyService.Success("Usuario eliminado correctamente.");
                }
                else
                {
                    _notifyService.Error("Error al eliminar el usuario.");
                }
            }
            catch (Exception)
            {
                _notifyService.Error("Ocurrió un error al procesar la solicitud.");
            }

            return RedirectToAction("Lista");
        }


        public IActionResult Roles()
        {
            var roles = _context.Roles.ToList();
            return View(roles);
        }

    }
}