using System.IO.Compression;
using System.Net.Http.Headers;
using System.Net.Mime;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Minecraft.Application.Modification.Dto;

namespace Minecraft.Application.Modification;

public class ModificationAppService : IModificationAppService
{
    public ModificationAppService()
    {
        
    }

    private const string FolderMods = @"C:\Users\User\AppData\Roaming\.minecraft\versions\1\mods";
    
    public Task<List<ModificationDto>> GetListAsync()
    {
        var mods = Directory.GetFiles(FolderMods);
        var mappedMods = mods
            .Select(m => new ModificationDto(Path.GetFileName(m), m))
            .ToList();
        return Task.FromResult(mappedMods);
    }

    public async Task<Stream> Download(string name)
    {
        var mods = await GetListAsync();
        var mod = mods.FirstOrDefault(m => m.Name == name);
        return File.OpenRead(mod!.Link);
    }

    public async Task<Stream> DownloadAll()
    {
        const string fileName = "tmp_mods.zip";

        if (File.Exists(Path.Combine(FolderMods, fileName)))
        {
            return new FileStream(Path.Combine(FolderMods, fileName), FileMode.Create);
        }
        
        var modifications = await GetListAsync();

        var outputMemStream = new FileStream(Path.Combine(FolderMods, fileName), FileMode.Create);
        var zipStream = new ZipOutputStream(outputMemStream);
 
        zipStream.SetLevel(9);
 
        foreach (var modification in modifications)
        {
            var fi = new FileInfo(modification.Link);
 
            var entryName = ZipEntry.CleanName(fi.Name);
            var newEntry = new ZipEntry(entryName);
            newEntry.DateTime = fi.LastWriteTime;
            newEntry.Size = fi.Length;
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