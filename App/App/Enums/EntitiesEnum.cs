using System;
using System.ComponentModel;

namespace App.Enums
{
	public enum EntitiesEnum
	{
		[Description("Student")]
		Student,

		[Description("Course")]
		Course,

        [Description("Notes")]
        Notes,

        [Description("Employee")]
        Employee
	}
}

