using System;
using System.ComponentModel;

namespace App.Enums
{
	public enum MethodEnum
	{
        [Description("WithID")]
        WithID,

        [Description("WithNumber")]
        WithNumber,

        [Description("Create")]
        Create,

        [Description("Update")]
        Update,

        [Description("All")]
        All,

        [Description("AllWithNumber")]
        AllWithNumber,

        [Description("AllStudentWithID")]
        AllStudentWithID,

        [Description("AllStudentsWithEmployeeNumber")]
        AllStudentsWithEmployeeNumber,

        [Description("CourseWithNumber")]
        CourseWithNumber,

        [Description("CourseRegistration")]
        CourseRegistration,

        [Description("CheckNumber")]
        CheckNumber
    }
}

