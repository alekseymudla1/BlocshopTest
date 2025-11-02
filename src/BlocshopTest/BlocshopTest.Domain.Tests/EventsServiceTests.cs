using BlocshopTest.Domain.Events.Models;
using BlocshopTest.Domain.Events.Services;
using BlocshopTest.Domain.Holds.Models;
using BlocshopTest.Domain.Holds.Services;
using BlocshopTest.Domain.Reservations.Models;
using BlocshopTest.Domain.Shared.Models;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace BlocshopTest.Domain.Tests;

public class EventsServiceTests
{
    private Mock<IEventsRepository> _eventsRepositoryMock;
    private Mock<IHoldsCache> _holdsCacheMock;
    private IOptions<PageSettings> _pageSettingsOptions;
    private EventsService _service;
    
    public EventsServiceTests()
    {
        _eventsRepositoryMock = new Mock<IEventsRepository>();
        _holdsCacheMock = new Mock<IHoldsCache>();
        _pageSettingsOptions = Options.Create(new PageSettings { DefaultPage = 1, DefaultPageSize = 10 });

        _service = new EventsService(_eventsRepositoryMock.Object, _holdsCacheMock.Object, _pageSettingsOptions);

    }

    [Test]
    public async Task GetEventsPage_Should_CallRepositoryWithFilter_AndReturnPage()
    {
        // Arrange
        var expectedPage = new Page<Event> { Items = new List<Event> { new Event { Name = "Concert" } } };
        _eventsRepositoryMock
            .Setup(r => r.GetFilteredPageAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Event, bool>>>(), 1, 10))
            .ReturnsAsync(expectedPage);

        // Act
        var result = await _service.GetEventsPage("Concert", null, null);

        // Assert
        result.Should().BeEquivalentTo(expectedPage);
        _eventsRepositoryMock.Verify(r => r.GetFilteredPageAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Event, bool>>>(), 1, 10), Times.Once);
    }

    [Test]
    public async Task GetEventById_Should_ReturnEventWithHolds()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var holds = new List<Hold> { new Hold { Id = Guid.NewGuid(), EventId = eventId } };
        var eventEntity = new Event { Id = eventId, Holds = new List<Hold>() };

        _eventsRepositoryMock.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(eventEntity);
        _holdsCacheMock.Setup(c => c.GetHoldsByEventId(eventId)).ReturnsAsync(holds);

        // Act
        var result = await _service.GetEventById(eventId);

        // Assert
        result.Holds.Should().BeEquivalentTo(holds);
    }

    [Test]
    public async Task CreateEvent_Should_AddEventToRepository()
    {
        // Arrange
        var newEvent = new Event { Id = Guid.NewGuid(), Name = "New Event" };

        // Act
        var result = await _service.CreateEvent(newEvent);

        // Assert
        result.Should().Be(newEvent);
        _eventsRepositoryMock.Verify(r => r.AddAsync(newEvent), Times.Once);
    }

    [Test]
    public async Task ConfirmRegistration_Should_ReturnEventNotFound_IfEventMissing()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        _eventsRepositoryMock.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync((Event)null);

        // Act
        var result = await _service.ConfirmRegistration(eventId, Guid.NewGuid());

        // Assert
        result.Result.Should().Be(ConfirmationResult.EventNotFound);
    }

    [Test]
    public async Task ConfirmRegistration_Should_ReturnHoldNotFound_IfHoldMissing()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var evt = new Event { Id = eventId, Reservations = new List<Reservation>() };
        _eventsRepositoryMock.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(evt);
        _holdsCacheMock.Setup(c => c.GetHoldById(It.IsAny<Guid>())).ReturnsAsync((Hold)null);

        // Act
        var result = await _service.ConfirmRegistration(eventId, Guid.NewGuid());

        // Assert
        result.Result.Should().Be(ConfirmationResult.HoldNotFound);
    }

    [Test]
    public async Task ConfirmRegistration_Should_Succeed_AndDeleteHold()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var holdId = Guid.NewGuid();
        var hold = new Hold { Id = holdId, EventId = eventId, Seats = 5 };
        var evt = new Event { Id = eventId, Reservations = new List<Reservation>() };

        _eventsRepositoryMock.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(evt);
        _holdsCacheMock.Setup(c => c.GetHoldById(holdId)).ReturnsAsync(hold);

        // Act
        var result = await _service.ConfirmRegistration(eventId, holdId);

        // Assert
        result.Result.Should().Be(ConfirmationResult.Success);
        evt.Reservations.Should().ContainSingle(r => r.HoldId == holdId && r.Seats == hold.Seats);
        _eventsRepositoryMock.Verify(r => r.UpdateAsync(evt), Times.Once);
        _holdsCacheMock.Verify(c => c.DeleteHold(holdId), Times.Once);
    }
}

