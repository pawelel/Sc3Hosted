namespace Sc3Hosted.Shared.Dtos;

public class SituationCreateDto
{
public string Name { get; set; }

    public SituationCreateDto(string name)
    {
        Name = name;
    }
}
