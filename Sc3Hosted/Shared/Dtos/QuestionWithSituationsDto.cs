namespace Sc3Hosted.Shared.Dtos;
public class QuestionWithSituationsDto
{
    public int QuestionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual List<SituationDto> Situations { get; set; } = new();
}
