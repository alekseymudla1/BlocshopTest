using BlocshopTest.Domain.Holds.Models;
using BlocshopTest.Domain.Holds.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace BlocshopTest.Domain.Tests;

[TestFixture]
public class HoldsServiceTests
{
    private Mock<IHoldsCache> _holdsCacheMock;
    private IOptions<HoldingSettings> _holdingOptions;
    private HoldsService _service;

    [SetUp]
    public void SetUp()
    {
        _holdsCacheMock = new Mock<IHoldsCache>();
        _holdingOptions = Options.Create(new HoldingSettings
        {
            ExpirationInterval = 15
        });

        _service = new HoldsService(_holdsCacheMock.Object, _holdingOptions);
    }

    [Test]
    public async Task CreateHold_Should_CreateHoldWithCorrectData_And_SaveToCache()
    {
        // Arrange
        var createHold = new CreateHold
        {
            IdempotencyKey = "some-key",
            EventId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Seats = 3
        };

        Hold savedHold = null!;
        _holdsCacheMock
            .Setup(c => c.SaveHold(It.IsAny<Hold>()))
            .ReturnsAsync((Hold h) =>
            {
                savedHold = h;
                return h;
            });

        // Act
        var result = await _service.CreateHold(createHold);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.EventId.Should().Be(createHold.EventId);
        result.CustomerId.Should().Be(createHold.CustomerId);
        result.Seats.Should().Be(createHold.Seats);
        result.IdempotencyKey.Should().Be(createHold.IdempotencyKey);
        result.ExpiresAt.Should().BeAfter(DateTimeOffset.UtcNow);

        _holdsCacheMock.Verify(c => c.SaveHold(It.IsAny<Hold>()), Times.Once);
    }

    [Test]
    public async Task GetHoldByIdempotencyKey_Should_ReturnHold_IfSameEventAndCustomer()
    {
        // Arrange
        var key = "some-key";
        var eventId = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        var existingHold = new Hold
        {
            Id = Guid.NewGuid(),
            IdempotencyKey = key,
            EventId = eventId,
            CustomerId = customerId,
            Seats = 2
        };

        var createHold = new CreateHold
        {
            IdempotencyKey = key,
            EventId = eventId,
            CustomerId = customerId,
            Seats = 2
        };

        _holdsCacheMock
            .Setup(c => c.GetHoldByIdempotencyKey(key))
            .ReturnsAsync(existingHold);

        // Act
        var result = await _service.GetHoldByIdempotencyKey(createHold);

        // Assert
        result.Should().BeEquivalentTo(existingHold);
    }

    [Test]
    public async Task GetHoldByIdempotencyKey_Should_ReturnNull_IfNotFound()
    {
        // Arrange
        var createHold = new CreateHold { IdempotencyKey = "some-key" };
        _holdsCacheMock
            .Setup(c => c.GetHoldByIdempotencyKey(It.IsAny<string>()))
            .ReturnsAsync((Hold)null!);

        // Act
        var result = await _service.GetHoldByIdempotencyKey(createHold);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetHoldByIdempotencyKey_Should_ReturnNull_IfEventOrCustomerMismatch()
    {
        // Arrange
        var key = "some-key";
        var createHold = new CreateHold
        {
            IdempotencyKey = key,
            EventId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Seats = 4
        };

        var cachedHold = new Hold
        {
            IdempotencyKey = key,
            EventId = Guid.NewGuid(), 
            CustomerId = createHold.CustomerId
        };

        _holdsCacheMock
            .Setup(c => c.GetHoldByIdempotencyKey(key))
            .ReturnsAsync(cachedHold);

        // Act
        var result = await _service.GetHoldByIdempotencyKey(createHold);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task DeleteHold_Should_CallCacheDelete()
    {
        // Arrange
        var holdId = Guid.NewGuid();

        // Act
        await _service.DeleteHold(holdId);

        // Assert
        _holdsCacheMock.Verify(c => c.DeleteHold(holdId), Times.Once);
    }
}