namespace Sc3Hosted.Shared.ViewModels
{
	public class LoginResult
	{
		public bool Successful { get; set; }
		public string Error { get; set; } = string.Empty;
		public string Token { get; set; } = string.Empty;
	}
}
