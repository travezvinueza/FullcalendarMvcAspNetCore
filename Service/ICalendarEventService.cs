using Fullcalendar.Models.Entity;

namespace Fullcalendar.Service
{
    public interface ICalendarEventService
    {
        IEnumerable<CalendarEvent> GetAll();
        CalendarEvent GetById(int id);
        void Add(CalendarEvent calendarEvent);
        void Update(CalendarEvent calendarEvent);
        CalendarEvent GetByIdAndUsuarioId(int id, string usuarioId);
        void Delete(int id);
        Task<List<CalendarEvent>> GetCalendarEventsAsync();
  
    }
}