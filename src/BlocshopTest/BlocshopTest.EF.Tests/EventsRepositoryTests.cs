using BlocshopTest.Domain.Events.Models;
using BlocshopTest.EF.Repositories.Events;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BlocshopTest.EF.Tests;

[TestFixture]
public class EventsRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> _dbContextOptions;
    private EventsRepository _repository;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Port=5433;Database=ticket_reservation_service_tests;Username=myuser;Password=root")
            .Options;

        var context = new ApplicationDbContext(_dbContextOptions);
        context.Database.EnsureCreated();
        context.Database.MigrateAsync();
    }

    [SetUp]
    public void Setup()
    {
        var context = new ApplicationDbContext(_dbContextOptions);
        _repository = new EventsRepository(context);
    }

    [TearDown]
    public async Task TearDown()
    {
        var context = new ApplicationDbContext(_dbContextOptions);
        context.Set<Event>().RemoveRange(context.Set<Event>());
        await context.SaveChangesAsync();
        context.Dispose();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        var context = new ApplicationDbContext(_dbContextOptions);
        context.Database.EnsureDeleted();
        context.Dispose();
    }

    [Test]
    public async Task AddAsync_ShouldAddEntityAndSetCreatedAt()
    {
        // Arrange
        var entity = new Event
        {
            Id = Guid.NewGuid(),
            Name = "Test Event"
        };

        // Act
        await _repository.AddAsync(entity);
        var result = await _repository.GetByIdAsync(entity.Id);

        // Assert
        result.Should().NotBeNull();
        result.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(3));
        result.Name.Should().Be("Test Event");
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateEntityAndSetUpdatedAt()
    {
        // Arrange
        var entity = new Event { Id = Guid.NewGuid(), Name = "Old Name" };
        await _repository.AddAsync(entity);

        // Act
        entity.Name = "New Name";
        await _repository.UpdateAsync(entity);
        var result = await _repository.GetByIdAsync(entity.Id);

        // Assert
        result.Name.Should().Be("New Name");
        result.UpdatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(3));
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        var entity = new Event { Id = Guid.NewGuid(), Name = "ToDelete" };
        await _repository.AddAsync(entity);

        // Act
        await _repository.DeleteAsync(entity);
        var result = await _repository.GetByIdAsync(entity.Id);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        await _repository.AddAsync(new Event { Id = Guid.NewGuid(), Name = "E1" });
        await _repository.AddAsync(new Event { Id = Guid.NewGuid(), Name = "E2" });

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Test]
    public async Task GetFilteredPageAsync_ShouldReturnPagedResults()
    {
        // Arrange
        for (int i = 0; i < 30; i++)
        {
            await _repository.AddAsync(new Event
            {
                Id = Guid.NewGuid(),
                Name = $"Event{i}"
            });
        }

        // Act
        var page = await _repository.GetFilteredPageAsync(e => e.Name.Contains("Event"), page: 2, pageSize: 10);

        // Assert
        page.Items.Should().HaveCount(10);
        page.PageNo.Should().Be(2);
        page.PageSize.Should().Be(10);
        page.TotalItems.Should().Be(30);
        page.TotalPages.Should().Be(3);
    }

    [Test]
    public async Task GetFilteredPageAsync_ShouldClampPageSizeAndPageNumber()
    {
        // Arrange
        for (int i = 0; i < 200; i++)
        {
            await _repository.AddAsync(new Event
            {
                Id = Guid.NewGuid(),
                Name = $"Event{i}"
            });
        }

        // Act
        var page = await _repository.GetFilteredPageAsync(e => e.Name.Contains("Event"), page: -1, pageSize: 999);

        // Assert
        page.PageSize.Should().Be(100);
        page.PageNo.Should().Be(1);
    }
}