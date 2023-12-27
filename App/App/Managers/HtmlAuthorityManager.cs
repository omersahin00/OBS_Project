using System;
using App.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace App.Managers
{
	public class HtmlAuthorityManager
	{
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HtmlAuthorityManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public string GetDisplayVariable(int RequiredAuthority)
		{
            // Daha sonra yazılacak:
            // Daha sonra yazılacak:
            // Daha sonra yazılacak:

            ClaimsPrincipal? currentUser = _httpContextAccessor.HttpContext?.User;
            if (currentUser == null) return "";

            string? claimEmail = currentUser.FindFirst(ClaimTypes.Email)?.Value;
            if (claimEmail == null) return "";

            


            return "";
		}

    }
}

