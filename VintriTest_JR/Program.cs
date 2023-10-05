using VintriTest_JR.ActionFilters;
using VintriTest_JR.Models;
using VintriTest_JR.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<UsernameValidationFilterAttribute>();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddTransient<IApiService,ApiService>();
builder.Services.Configure<Config>(
builder.Configuration.GetSection("Config"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
