using System;
using System.ComponentModel;

namespace App.Enums
{
	public enum UrlTypeEnum
	{
        [Description("/api")]
        api,

        [Description("https://sql-server")] // Daha sonra tekrar düzenlencek!
        sql
	}
}

