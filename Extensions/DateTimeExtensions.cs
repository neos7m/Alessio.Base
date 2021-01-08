using System;
using System.Collections.Generic;
using System.Text;

namespace Alessio.Base.Extensions
{
	public static class DateTimeExtensions
	{
        public static DateTime GetMonthBegin(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime GetMonthEnd(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }

        public static bool IsMonthEnd(this DateTime date)
        {
            return date.Day == DateTime.DaysInMonth(date.Year, date.Month);
        }
    }
}
