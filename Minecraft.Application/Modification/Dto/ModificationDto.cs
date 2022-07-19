namespace Minecraft.Application.Modification.Dto;

public class ModificationDto
{
    public ModificationDto(string name, string link)
    {
        Name = name;
        Link = link;
    }

    public string Name { get; set; }
    public string Link { get; set; }
}