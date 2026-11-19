using System.Text.Json.Serialization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common;
using RoomHub.Api.Domain;
using RoomHub.Api.Features.Bookings.CancelBooking;
using RoomHub.Api.Features.Bookings.CreateBooking;
using RoomHub.Api.Features.Bookings.GetBooking;
using RoomHub.Api.Features.Rooms.CreateRoom;
using RoomHub.Api.Features.Rooms.GetRoom;
using RoomHub.Api.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Unexpected errors (not business errors) become ProblemDetails too.
builder.Services.AddProblemDetails();

// Enums appear in JSON as text.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// "Now" comes from TimeProvider (R5, R11).
builder.Services.AddSingleton(TimeProvider.System);

// Persistence: EF Core + SQLite. A relative database path is resolved against the
// project folder, so the same file is reused after every restart.
var sqlite = new SqliteConnectionStringBuilder(
    builder.Configuration.GetConnectionString("RoomHub") ?? "Data Source=roomhub.db");
if (!Path.IsPathRooted(sqlite.DataSource))
    sqlite.DataSource = Path.Combine(builder.Environment.ContentRootPath, sqlite.DataSource);

builder.Services.AddDbContext<RoomHubDbContext>(options => options.UseSqlite(sqlite.ConnectionString));
builder.Services.AddScoped<IRoomRepository, EfRoomRepository>();
builder.Services.AddScoped<IBookingRepository, EfBookingRepository>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

// One handler per feature slice.
builder.Services.AddScoped<ICommandHandler<CreateRoomCommand, CreateRoomResponse>, CreateRoomHandler>();
builder.Services.AddScoped<IQueryHandler<GetRoomQuery, GetRoomResponse>, GetRoomHandler>();
builder.Services.AddScoped<ICommandHandler<CreateBookingCommand, CreateBookingResponse>, CreateBookingHandler>();
builder.Services.AddScoped<IQueryHandler<GetBookingQuery, GetBookingResponse>, GetBookingHandler>();
builder.Services.AddScoped<ICommandHandler<CancelBookingCommand>, CancelBookingHandler>();

var app = builder.Build();

// Create the database and tables on first start; existing data is kept.
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<RoomHubDbContext>().Database.EnsureCreated();
}

// Malformed JSON / unreadable body -> 400 ProblemDetails (not 500); anything else unexpected -> 500.
app.UseExceptionHandler(new ExceptionHandlerOptions
{
    StatusCodeSelector = ex => ex is BadHttpRequestException bad
        ? bad.StatusCode
        : StatusCodes.Status500InternalServerError
});

app.MapGet("/", () => "RoomHub API is running.");

app.MapCreateRoom();
app.MapGetRoom();
app.MapCreateBooking();
app.MapGetBooking();
app.MapCancelBooking();

app.Run();
