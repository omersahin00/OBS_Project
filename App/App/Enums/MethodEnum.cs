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

        [Description("CheckNumber")]
        CheckNumber
    }
}

