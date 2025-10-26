using BlocshopTest.Cache;
using BlocshopTest.Domain;
using BlocshopTest.Domain.Holds.Models;
using BlocshopTest.Domain.Shared.Models;
using BlocshopTest.EF;
using BlocshopTest.EF.DataSeed;
using BlocshopTest.Web;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<PageSettings>(builder.Configuration.GetSection("PageSettings"));
builder.Services.Configure<HoldingSettings>(builder.Configuration.GetSection("HoldingSettings"));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis");
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddDomainServices(builder.Configuration);
builder.Services.AddEFServices(builder.Configuration);
builder.Services.AddCacheServices(builder.Configuration);
builder.Services.AddAutoMapper(typeof(WebAutoMapperProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
        await EventsDataSeed.Seed(dbContext);
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
