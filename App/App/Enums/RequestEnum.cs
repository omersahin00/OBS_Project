using System;
using System.ComponentModel;

namespace App.Enums
{
	public enum RequestEnum
	{
        [Description("Get")]
        Get,

        [Description("Post")]
        Post,

        [Description("LoginEmployee")]
        LoginEmployee,

        [Description("LoginStudent")]
        LoginStudent
    }
}

