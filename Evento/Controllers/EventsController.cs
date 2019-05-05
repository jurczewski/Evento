using Evento.Infrastructure.Commands.CreateEvent;
using Evento.Infrastructure.Commands.Events;
using Evento.Infrastructure.DTO;
using Evento.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Evento.Api.Controllers
{
    public class EventsController : ApiControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IMemoryCache _cache;
        public EventsController(IEventService eventService, IMemoryCache cache)
        {
            _eventService = eventService;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string name)
        {
            var events = _cache.Get<IEnumerable<EventDTO>>("events");
            if(events == null)
            {
                Console.WriteLine("Fetching from service.");
                events = await _eventService.BrowseAsync(name);
                _cache.Set("events", events, TimeSpan.FromMinutes(1));
            }
            else
            {
                Console.WriteLine("Fetching from cache.");
            }            

            return Json(events);
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> Get(Guid eventId)
        {
            var @event = await _eventService.GetAsync(eventId);
            if (@event == null)
            {
                return NotFound();
            }

            return Json(@event);
        }

        [HttpPost]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<IActionResult> Post([FromBody]CreateEvent command)
        {
            command.EventId = Guid.NewGuid();
            await _eventService.CreateAsync(command.EventId, command.Name, command.Description, command.StartDate, command.EndDate);
            await _eventService.AddTicketsAsync(command.EventId, command.Tickets, command.Price);

            return Created($"/event/{command.EventId}", null);
        }

        [HttpPut("{eventId}")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<IActionResult> Put(Guid eventId, [FromBody]UpdateEvent command)
        {
            await _eventService.UpdateAsync(eventId, command.Name, command.Description);

            return NoContent();
        }

        [HttpDelete("{eventId}")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<IActionResult> Delete(Guid eventId)
        {
            await _eventService.DeleteAsync(eventId);

            return NoContent();
        }
    }
}
