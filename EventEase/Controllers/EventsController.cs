using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    public class EventsController : Controller
    {
        // The code was adapted from "MVC, Entity Framework & SQL Azure" by Adeol Adisa
        private readonly ApplicationDbContext _context;
        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? eventTypeId, int? venueId, DateTime? startDate, DateTime? endDate)
        {
            // Load Event Types and Venues for dropdowns
            ViewBag.EventTypes = new SelectList(_context.EventTypes, "EventTypeId", "EventTypeName");
            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "VenueName");


            var eventsQuery = _context.Events
                .Include(i => i.Venues)
                .Include(e => e.EventType)
                .AsQueryable();

            // Apply filters if provided
            if (eventTypeId.HasValue)
            {
                eventsQuery = eventsQuery.Where(e => e.EventTypeId == eventTypeId);
            }

            if (venueId.HasValue)
            {
                eventsQuery = eventsQuery.Where(e => e.VenueId == venueId);
            }

            if (startDate.HasValue)
            {
                eventsQuery = eventsQuery.Where(e => e.EventDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                eventsQuery = eventsQuery.Where(e => e.EventDate <= endDate.Value);
            }

            // Execute query
            var events = await eventsQuery.ToListAsync();

            return View(events);
        }

        public IActionResult Create()
        {
            ViewBag.Venues = _context.Venues.ToList();
            ViewBag.EventTypes = _context.EventTypes.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Events events)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(events);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "An error occurred while creating the event.";
                    // Re-display the form with the current model and venues
                    ViewBag.Venues = _context.Venues.ToList();
                    return View(events);
                }
            }

            // Model validation failed
            TempData["ErrorMessage"] = "Please correct the errors and try again.";
            ViewBag.Venues = _context.Venues.ToList();
            ViewBag.EventTypes = _context.EventTypes.ToList();
            return View(events);
        }


        public async Task<IActionResult> Delete(int? eventId)
        {
            if (eventId == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events
                .Include(e => e.Venues) // Include related venue data
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int eventId)
        {
            var eventItem = await _context.Events
                .Include(e => e.Bookings)  // Include bookings to check for dependencies
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventItem == null)
            {
                return NotFound();
            }

            // Check if the event has any bookings
            if (eventItem.Bookings.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete event. It has existing bookings.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Events.Remove(eventItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event deleted successfully!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the event.";
            }

            return RedirectToAction(nameof(Index));
        }


        private bool EventExists(int eventId)
        {
            return _context.Events.Any(e => e.EventId == eventId);
        }

        public async Task<IActionResult> Edit(int? eventId)
        {
            if (eventId == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                return NotFound();
            }

            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "VenueName", eventItem.VenueId);
            ViewBag.EventTypes = new SelectList(_context.EventTypes, "EventTypeId", "EventTypeName", eventItem.EventTypeId); // ✅ Add this

            return View(eventItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int eventId, Events eventItem)
        {
            if (eventId != eventItem.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventItem);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(eventItem.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "A concurrency error occurred.";
                        throw;
                    }
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "An error occurred while updating the event.";
                }
            }

            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "VenueName", eventItem.VenueId);
            ViewBag.EventTypes = new SelectList(_context.EventTypes, "EventTypeId", "EventTypeName", eventItem.EventTypeId); // ✅ Add this

            return View(eventItem);
        }


        public async Task<IActionResult> Details(int? eventId)
        {
            if (eventId == null)
            {
                return NotFound();
            }

            var events = await _context.Events
                .Include(f => f.Venues)   // Include related venue data
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(b => b.EventId == eventId);

            if (events == null)
            {
                return NotFound();
            }

            return View(events);  // Return the booking details to the view
        }


    }
}
