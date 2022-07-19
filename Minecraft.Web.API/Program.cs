using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using Minecraft.Application.Modification;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IModificationAppService, ModificationAppService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Eyespert.Server v1"));
}

app.MapGet("/modifications", (IModificationAppService modificationAppService) => modificationAppService.GetListAsync());
app.MapGet("/modifications/download/one/{name}",  async (IModificationAppService modificationAppService, string name) =>
{
    var stream = await modificationAppService.Download(name);
    return Results.File(stream);
});

app.MapGet("/modifications/download/all",  async (IModificationAppService modificationAppService) =>
{
    var stream = await modificationAppService.DownloadAll();
    return Results.File(stream, "application/zip", "mods.zip");
});

app.Run();