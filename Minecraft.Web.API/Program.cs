using Minecraft.Application.Modification;
using Minecraft.Domain.Shared;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration.GetSection(nameof(ModificationsConfiguration));
builder.Services.AddTransient(x => config.Get<ModificationsConfiguration>());

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
    return stream != null ? Results.File(stream, "application/octet-stream", name) : Results.NotFound($"Mod {name} not found");
});

app.MapGet("/modifications/download/all",  async (IModificationAppService modificationAppService) =>
{
    var stream = await modificationAppService.DownloadAll();
    return stream != null ? Results.File(stream, "application/zip", "mods.zip") : Results.NotFound($"Mods not found");
});

app.Run();