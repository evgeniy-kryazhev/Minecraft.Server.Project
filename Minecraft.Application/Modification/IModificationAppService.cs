using Minecraft.Application.Modification.Dto;

namespace Minecraft.Application.Modification;

public interface IModificationAppService
{
    Task<List<ModificationDto>> GetListAsync();
    Task<Stream?> Download(string name);
    Task<Stream?> DownloadAll();
}