using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEHv1.Services.Utils
{
	public static class Conversions
	{

		public static double toUnixTimestamp(this DateTime dateTime)
		{
			return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
						 new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
		}

	}
}
