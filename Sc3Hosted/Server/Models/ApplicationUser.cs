﻿using Microsoft.AspNetCore.Identity;

namespace Sc3Hosted.Server.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string CustomClaim { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
