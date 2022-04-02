namespace Sc3Hosted.Shared.ViewModels
{
	public class RegisterResult
	{
		public bool Successful { get; init; }
		public IEnumerable<string>? Errors { get; init; }
	}
}
