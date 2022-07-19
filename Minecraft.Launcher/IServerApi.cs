using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Minecraft.Application.Modification.Dto;
using Refit;

namespace Minecraft.Launcher;

public interface IServerApi
{
    [Get("/modifications")]
    Task<List<ModificationDto>> GetModifications();
    
    [Get("/modifications/download/all")]
    Task<Stream> DownloadAll();
}