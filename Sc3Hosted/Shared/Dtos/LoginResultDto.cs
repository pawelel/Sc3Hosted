namespace Sc3Hosted.Shared.Dtos;
public class LoginResultDto
{
    public bool Successful { get; set; }
    public string Error { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
