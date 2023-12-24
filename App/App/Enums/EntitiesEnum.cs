using System;
using System.ComponentModel;

namespace App.Enums
{
	public enum EntitiesEnum
	{
        [Description("Account")]
        Account,

		[Description("Student")]
		Student,

		[Description("Course")]
		Course,

        [Description("Notes")]
        Notes,

        [Description("Employee")]
        Employee,

        [Description("Admin")]
        Admin
	}
}

