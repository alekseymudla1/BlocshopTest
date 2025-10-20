using AutoMapper;
using BlocshopTest.Domain.Events.Models;
using BlocshopTest.Domain.Events.Services;
using BlocshopTest.Web.Models.Events;
using Microsoft.AspNetCore.Mvc;

namespace BlocshopTest.Web.Controllers;


[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly IEventsService _eventsService;
    private readonly IMapper _mapper;
    public EventsController(IEventsService eventsService,
        IMapper mapper)
    {
        _eventsService = eventsService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetEventsPage([FromQuery] string search, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var events = await _eventsService.GetEventsPage(search, page, pageSize);
        var eventsDto = _mapper.Map<IEnumerable<EventSimpleDto>>(events);
        return Ok(eventsDto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEventById([FromRoute] Guid id)
    {
        var eventEntity = await _eventsService.GetEventById(id);
        if (eventEntity == null)
        {
            return NotFound();
        }
        var eventDto = _mapper.Map<EventDto>(eventEntity);
        return Ok(eventDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto eventCreateDto)
    {
        var eventEntity = _mapper.Map<Event>(eventCreateDto);
        var createdEvent = await _eventsService.CreateEvent(eventEntity);
        var createdEventDto = _mapper.Map<EventSimpleDto>(createdEvent);
        return CreatedAtAction(nameof(GetEventById), new { id = createdEventDto.Id }, createdEventDto);

    }
}
