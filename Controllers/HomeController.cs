using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Fullcalendar.Models;
using Fullcalendar.Data;
using Fullcalendar.Service;
using Fullcalendar.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace Fullcalendar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<Usuario> _userManager;
        private readonly DatabaseContext _context;
        private readonly INotyfService _notifyService;
        private readonly ICalendarEventService _calendarEventService;


        public HomeController(
            ILogger<HomeController> logger,
            UserManager<Usuario> userManager,
            DatabaseContext context,
            ICalendarEventService calendarEventService,
            INotyfService notifyService)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _calendarEventService = calendarEventService;
            _notifyService = notifyService;
        }

        [AllowAnonymous]
        public new IActionResult Unauthorized()
        {
            ViewBag.Message = TempData["UnauthorizedMessage"]?.ToString();

            if (!string.IsNullOrEmpty(ViewBag.Message))
            {
                _notifyService.Warning("Acceso no autorizado: " + ViewBag.Message);
            }

            return View();
        }



        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetEvents()
        {
            var events = _calendarEventService.GetAll();
            foreach (var e in events)
            {
                Console.WriteLine($"Evento {e.Id}, Usuario: {e.Usuario?.Email}");
            }

            // Mapear los eventos a un formato que incluya datos del usuario
            var getEventos = events.Select(e => new
            {
                id = e.Id,
                title = $"<img src='/Uploads/{e.Usuario?.ProfilePicture}' alt='Profile Picture' style='width: 45px; height: 55px; border-radius: 40%;' /> {e.Title}",
                description = e.Description,
                start = e.Start,
                end = e.End,
                color = e.Color,
                allDay = e.AllDay,
                usuarioId = e.UsuarioId,
                user = e.Usuario?.UserName,
                email = e.Usuario?.Email,
                phoneNumber = e.Usuario?.PhoneNumber,

            }).ToArray();

            return Json(getEventos);
        }

        [HttpGet]
        public IActionResult Create(string eventDate)
        {
            Console.WriteLine("EventDate received: " + eventDate);
            ViewBag.EventDate = eventDate;

            var data = new CalendarEvent()
            {

            };
            return View(data);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CalendarEvent calendarEvent)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                using (var dbContext = _context)
                {

                    var nuevoEvento = new CalendarEvent
                    {
                        UsuarioId = currentUser!.Id,
                        Title = calendarEvent.Title,
                        Start = calendarEvent.Start,
                        End = calendarEvent.End,
                        Color = calendarEvent.Color,
                        Description = calendarEvent.Description,
                        AllDay = calendarEvent.AllDay,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,

                    };

                    dbContext.CalendarEvents.Add(nuevoEvento);
                    await dbContext.SaveChangesAsync();

                    _notifyService.Success("Evento creado correctamente");

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al crear el evento: {ex.Message}");
                _notifyService.Error("¡Hubo un error al crear el evento!");
                return RedirectToAction("Index");
            }

        }


        // Acción para mostrar el formulario de edición
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditAsync(string usuarioId, int id)
        {

            var eventToEdit = _calendarEventService.GetByIdAndUsuarioId(id, usuarioId);

            if (eventToEdit == null)
            {
                return NotFound();
            }
            // Verifica si el usuario actual es el propietario del evento
            var currentUser = await _userManager.GetUserAsync(User);
            if (eventToEdit.UsuarioId != currentUser!.Id)
            {
                TempData["UnauthorizedMessage"] = "No estás autorizado para ver este evento.";
                return RedirectToAction("Unauthorized");
            }

            return View(eventToEdit);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditAsync(CalendarEvent editedEvent)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (editedEvent.UsuarioId != currentUser!.Id)
                {
                    TempData["UnauthorizedMessage"] = "No estás autorizado para ver este evento.";
                    return RedirectToAction("Unauthorized");
                }

                _calendarEventService.Update(editedEvent);

                return RedirectToAction("Index");
            }
            return View(editedEvent);

        }

        // Metodo para eliminar
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteAsync(string usuarioId, int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var eventToDelete = _calendarEventService.GetByIdAndUsuarioId(id, usuarioId);

            if (currentUser == null || (eventToDelete.UsuarioId != currentUser.Id && !await _userManager.IsInRoleAsync(currentUser, "Admin, User")))
            {
                TempData["UnauthorizedMessage"] = "No estás autorizado para ver este evento.";
                _notifyService.Warning("No estás autorizado para ver este evento.");
                return RedirectToAction("Unauthorized");
            }

            _calendarEventService.Delete(eventToDelete.Id);
            _notifyService.Success("Evento eliminado exitosamente.");

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Eventos()
        {
            var calendarEvents = await _calendarEventService.GetCalendarEventsAsync();
            return View(calendarEvents);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}