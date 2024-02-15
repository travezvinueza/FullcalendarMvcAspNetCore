using Fullcalendar.Data;
using Fullcalendar.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Fullcalendar.Service.Impl
{
    public class CalendarEventService : ICalendarEventService
    {
        private readonly DatabaseContext _context;

        public CalendarEventService(DatabaseContext context)
        {
            _context = context;
        }

        public void Add(CalendarEvent calendarEvent)
        {
            try
            {
                calendarEvent.DateCreated = DateTime.Now;
                calendarEvent.DateModified = DateTime.Now;
                _context.CalendarEvents.Add(calendarEvent);
                _context.SaveChanges();
                Console.WriteLine("Evento agregado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar evento: {ex.Message}");

                throw;
            }
        }

        public void Delete(int id)
        {
            var eventDelete = _context.CalendarEvents.Find(id);
            if (eventDelete != null)
            {
                _context.CalendarEvents.Remove(eventDelete);
                _context.SaveChanges();
            }
        }

        public IEnumerable<CalendarEvent> GetAll()
        {
            return _context.CalendarEvents
            .Include(e => e.Usuario)
            .ToList();
        }

        public CalendarEvent GetById(int id)
        {
            return _context.CalendarEvents.Find(id)!;
        }

        public CalendarEvent GetByIdAndUsuarioId(int id, string usuarioId)
        {
            return _context.CalendarEvents.FirstOrDefault(e => e.Id == id && e.UsuarioId == usuarioId);
        }

        public async Task<List<CalendarEvent>> GetCalendarEventsAsync()
        {
            return await _context.CalendarEvents.Include(ce => ce.Usuario).ToListAsync();
        }

        public void Update(CalendarEvent calendarEvent)
        {
            calendarEvent.DateModified = DateTime.Now;
            _context.CalendarEvents.Update(calendarEvent);
            _context.SaveChanges();
        }
    }
}