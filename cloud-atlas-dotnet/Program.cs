using cloud_atlas_dotnet.Data_Access;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "cloud-atlas-dotnet.xml"));
});

var postgreSqlContainer = new PostgreSqlBuilder().Build();
await postgreSqlContainer.StartAsync();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connection = postgreSqlContainer.GetConnectionString();
    options.UseNpgsql(connection);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
