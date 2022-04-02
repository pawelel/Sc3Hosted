namespace Sc3Hosted.Shared.Dtos
{
	public class RegisterResultDto
	{
		public bool Successful { get; init; }
		public IEnumerable<string>? Errors { get; init; }
	}
}
