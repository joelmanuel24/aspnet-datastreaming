using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader());
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/weatherforecast", async (HttpContext context, [FromBody]MyModel model) =>
{
    var g = context.Features.Get<IHttpResponseBodyFeature>();
    g.DisableBuffering();

    context.Response.StatusCode = StatusCodes.Status206PartialContent;
    context.Response.ContentType = "text/plain; charset=utf-8";

    await g.StartAsync();

    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(model.Input);

    foreach (char chr in System.Convert.ToBase64String(plainTextBytes))
    {
        await g.Writer.WriteAsync(new ReadOnlyMemory<byte>(System.Text.Encoding.UTF8.GetBytes(chr.ToString())));
        await g.Writer.FlushAsync();

        await Task.Delay(Random.Shared.Next(1, 6) * 1000);
    }

    await g.CompleteAsync();
})
.WithName("GetWeatherForecast")
.WithOpenApi();



app.Run();

class MyModel
{
    public string Input { get; set; }
}