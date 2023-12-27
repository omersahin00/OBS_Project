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


        public string GetDisplayVariable(int AuthorityLavel)
		{
            ClaimsPrincipal? currentUser = _httpContextAccessor.HttpContext?.User;
            string? claimRole = currentUser?.FindFirst(ClaimTypes.Role)?.Value;
            RoleEnum roleEnum;

            if (currentUser == null || claimRole == null)
                roleEnum = RoleEnum.Default;
            else
                roleEnum = GetRoleEnum(claimRole);


            if (AuthorityLavel <= ((int)roleEnum))
                return "block";
            else
                return "none";
        }


        public RoleEnum GetRoleEnum(string claimRole)
        {
            if (claimRole == RoleEnum.Admin.ToString()) return RoleEnum.Admin;
            else if (claimRole == RoleEnum.Employee.ToString()) return RoleEnum.Employee;
            else if (claimRole == RoleEnum.Student.ToString()) return RoleEnum.Student;
            else if (claimRole == RoleEnum.Default.ToString()) return RoleEnum.Default;
            else return RoleEnum.Null;
        }
    }
}

