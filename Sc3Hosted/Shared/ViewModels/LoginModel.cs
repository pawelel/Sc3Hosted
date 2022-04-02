using System.ComponentModel.DataAnnotations;

namespace Sc3Hosted.Shared.ViewModels
{
	public class LoginModel
	{
		[Required]
		public string Email { get; set; } = string.Empty;

		[Required]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
	}
}
