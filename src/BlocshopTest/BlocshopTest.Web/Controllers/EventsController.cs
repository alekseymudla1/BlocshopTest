using AutoMapper;
using BlocshopTest.Domain.Events.Models;
using BlocshopTest.Domain.Events.Services;
using BlocshopTest.Domain.Holds.Models;
using BlocshopTest.Domain.Holds.Services;
using BlocshopTest.Web.Models.Events;
using BlocshopTest.Web.Models.Holds;
using BlocshopTest.Web.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlocshopTest.Web.Controllers;


[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly IEventsService _eventsService;
    private readonly IHoldsService _holdsService;
    private readonly IMapper _mapper;
    public EventsController(IEventsService eventsService,
        IHoldsService holdsService,
        IMapper mapper)
    {
        _eventsService = eventsService;
        _holdsService = holdsService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetEventsPage([FromQuery] string search, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var events = await _eventsService.GetEventsPage(search, page, pageSize);
        var eventsDto = _mapper.Map<PageDto<EventSimpleDto>>(events);
        return Ok(eventsDto);
    }

    [HttpGet("{id:guid}")]
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

    [HttpPost("{id:guid}/holds")]
    public async Task<IActionResult> CreateHold([FromRoute] Guid id, [FromBody] CreateHoldDto createHoldDto, [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
    {
        var createHold = new CreateHold
        {
            EventId = id,
            IdempotencyKey = idempotencyKey,
            CustomerId = createHoldDto.CustomerId,
            Seats = createHoldDto.Seats
        };
        var existingHold = await _holdsService.GetHoldByIdempotencyKey(createHold);
        if (existingHold != null)
        {
            var idempotentResult = _mapper.Map<CreatedHoldDto>(existingHold);
            return Ok(idempotentResult);
        }
        var eventEntity = await _eventsService.GetEventById(id);
        if (eventEntity.AvailableSeats >= createHold.Seats)
        {
            var createdHold = await _holdsService.CreateHold(createHold);
            var result = _mapper.Map<CreatedHoldDto>(createdHold);
            return Ok(result);
        }

        return BadRequest($"Only {eventEntity.AvailableSeats} seats are available");
    }

    [HttpPost("{id:guid}/holds/{holdId:guid}/confirm")]
    public async Task<IActionResult> ConfirmHold([FromRoute] Guid id, [FromRoute] Guid holdId)
    {
        var confirmationResult = await _eventsService.ConfirmRegistration(id, holdId);
        return confirmationResult.Result switch
        {
            ConfirmationResult.Success => Ok(confirmationResult),
            ConfirmationResult.EventNotFound => NotFound("Event not found"),
            ConfirmationResult.HoldNotFound => NotFound("Hold not found"),
            _ => BadRequest()
        };
    }

    [HttpDelete("{id:guid}/holds/{holdId:guid}")]
    public async Task<IActionResult> CancelHold([FromRoute] Guid id, [FromRoute] Guid holdId)
    {
        await _holdsService.DeleteHold(holdId);
        return Ok();
    }
}
