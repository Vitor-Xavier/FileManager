using FileManager.Context;
using FileManager.DependencyInjection;
using FileManager.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigureServices.RegisterServices(builder.Services, builder.Configuration);
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureAuthentication(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FileManagerContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseProblemDetailsExceptionHandler();
app.MapControllers().RequireAuthorization();

app.Run();
