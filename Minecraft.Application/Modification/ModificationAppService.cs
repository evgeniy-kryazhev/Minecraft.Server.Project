using System.IO.Compression;
using System.Net.Http.Headers;
using System.Net.Mime;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Minecraft.Application.Modification.Dto;
using Minecraft.Domain.Shared;

namespace Minecraft.Application.Modification;

public class ModificationAppService : IModificationAppService
{
    public ModificationAppService(ModificationsConfiguration modificationsConfiguration)
    {
        _modificationsConfiguration = modificationsConfiguration;
    }

    private readonly ModificationsConfiguration _modificationsConfiguration;
    public Task<List<ModificationDto>> GetListAsync()
    {
        if (_modificationsConfiguration?.FolderMods is null)
            return Task.FromResult(Enumerable.Empty<ModificationDto>().ToList());

        var mods = Directory.GetFiles(_modificationsConfiguration.FolderMods);
        var mappedMods = mods
            .Select(m => new ModificationDto(Path.GetFileName(m), m))
            .ToList();
        return Task.FromResult(mappedMods);
    }

    public async Task<Stream?> Download(string name)
    {
        var mods = await GetListAsync();
        var mod = mods.FirstOrDefault(m => m.Name == name);
        return mod != null ? File.OpenRead(mod.Link) : null;
    }

    public async Task<Stream?> DownloadAll()
    {
        if(_modificationsConfiguration?.FolderMods is null)
            return null;
        
        var modifications = await GetListAsync();

        var outputMemStream = new MemoryStream();
        var zipStream = new ZipOutputStream(outputMemStream);
 
        zipStream.SetLevel(9);
 
        foreach (var modification in modifications)
        {
            var fi = new FileInfo(modification.Link);
 
            var entryName = ZipEntry.CleanName(fi.Name);
            var newEntry = new ZipEntry(entryName)
            {
                DateTime = fi.LastWriteTime,
                Size = fi.Length
            };
            zipStream.PutNextEntry(newEntry);
 
            var buffer = new byte[4096];
            await using (var streamReader = System.IO.File.OpenRead(fi.FullName))
            {
                StreamUtils.Copy(streamReader, zipStream, buffer);
            }
            zipStream.CloseEntry();
        }
        zipStream.IsStreamOwner = false;
        zipStream.Close();
     
        outputMemStream.Position = 0;
        
        return outputMemStream;
    }
}