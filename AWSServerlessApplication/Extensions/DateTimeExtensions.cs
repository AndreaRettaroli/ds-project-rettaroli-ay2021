using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerlessApplication.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime TruncateToSecondStart(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }
        public static DateTime Cest(this DateTime dateTime)
        {
            TimeZoneInfo romeTimeZoneInfo;
            try
            {
                romeTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
            }
            catch
            {
                romeTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Europe/Rome");
            }
            return TimeZoneInfo.ConvertTime(dateTime, romeTimeZoneInfo);
        }
        public static DateTime CestNow => DateTime.UtcNow.Cest();
    }
}
