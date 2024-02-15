using Fullcalendar.Data;
using Fullcalendar.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fullcalendar.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly DatabaseContext _context;

        public AdminController(
            DatabaseContext context,
            IUsuarioService usuarioService)
        {
            _context = context;
            _usuarioService = usuarioService;
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

        public IActionResult Roles()
        {
            var roles = _context.Roles.ToList();
            return View(roles);
        }

    }
}