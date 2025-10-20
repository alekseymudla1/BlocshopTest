using BlocshopTest.Domain.Events.Models;
using BlocshopTest.Domain.Events.Services;
using BlocshopTest.Domain.Shared.Models;
using BlocshopTest.Domain.Tests.TestData;
using Microsoft.Extensions.Options;
using Moq;
using System.Linq.Expressions;

namespace BlocshopTest.Domain.Tests;

public class EventsServiceTests
{
    private readonly IEventsRepository _testRepository;
    private readonly EventsService _eventsService;

    private readonly PageSettings _pageSettings = new PageSettings
    {
        DefaultPage = 1,
        DefaultPageSize = 20
    };

    public EventsServiceTests()
    {
        _testRepository = new TestEventsRepository();
        _eventsService = new EventsService(_testRepository, Options.Create(_pageSettings));
    }

    [Test]
    [TestCase(null, null, null, 6)]
    [TestCase("test", null, null, 3)]
    [TestCase(null, 1, 3, 3)]
    [TestCase(null, 2, 10, 0)]
    [TestCase("not existing", 1, 10, 0)]
    public async Task GetEventsPage_CallsRepositoryWithCorrectParameters(string? search, int? page, int? pageSize, int resultCount)
    {
        // Arrange

        // Act
        var events = await _eventsService.GetEventsPage(search, page, pageSize);

        // Assert
        Assert.That(resultCount == events.Count());
    }
}
