using System;
using System.Globalization;

public static class ExtensionMethods
{
    /// <summary>
    /// Offsets to move the day of the year on a week, allowing
    /// for the current year Jan 1st day of week, and the Sun/Mon 
    /// week start difference between ISO 8601 and Microsoft
    /// </summary>
    private static int[] moveByDays = { 6, 7, 8, 9, 10, 4, 5 };
    /// <summary>
    /// Get the Week number of the year
    /// (In the range 1..53)
    /// This conforms to ISO 8601 specification for week number.
    /// </summary>
    /// <param name="date"></param>
    /// <returns>Week of year</returns>
    public static int WeekOfYear(this DateTime date)
    {
        DateTime startOfYear = new DateTime(date.Year, 1, 1);
        DateTime endOfYear = new DateTime(date.Year, 12, 31);
        // ISO 8601 weeks start with Monday 
        // The first week of a year includes the first Thursday 
        // This means that Jan 1st could be in week 51, 52, or 53 of the previous year...
        int numberDays = date.Subtract(startOfYear).Days +
                        moveByDays[(int)startOfYear.DayOfWeek];
        int weekNumber = numberDays / 7;
        switch (weekNumber)
        {
            case 0:
                // Before start of first week of this year - in last week of previous year
                weekNumber = WeekOfYear(startOfYear.AddDays(-1));
                break;
            case 53:
                // In first week of next year.
                if (endOfYear.DayOfWeek < DayOfWeek.Thursday)
                {
                    weekNumber = 1;
                }
                break;
        }
        return weekNumber;
    }

    public static DateTime GetFirstDayOfWeek(this DateTime date)
    {
        var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

        while (date.DayOfWeek != firstDayOfWeek)
        {
            date = date.AddDays(-1);
        }

        return date;
    }
    
    public static int BusinessDaysUntil(this DateTime firstDay, DateTime lastDay, params DateTime[] bankHolidays)
    {
        firstDay = firstDay.Date;
        lastDay = lastDay.Date;
        if (firstDay > lastDay)
            throw new ArgumentException("Incorrect last day " + lastDay);

        TimeSpan span = lastDay - firstDay;
        int businessDays = span.Days + 1;
        int fullWeekCount = businessDays / 7;
        // find out if there are weekends during the time exceedng the full weeks
        if (businessDays > fullWeekCount * 7)
        {
            // we are here to find out if there is a 1-day or 2-days weekend
            // in the time interval remaining after subtracting the complete weeks
            int firstDayOfWeek = (int)firstDay.DayOfWeek;
            int lastDayOfWeek = (int)lastDay.DayOfWeek;
            if (lastDayOfWeek < firstDayOfWeek)
                lastDayOfWeek += 7;
            if (firstDayOfWeek <= 6)
            {
                if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                    businessDays -= 2;
                else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                    businessDays -= 1;
            }
            else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                businessDays -= 1;
        }

        // subtract the weekends during the full weeks in the interval
        businessDays -= fullWeekCount + fullWeekCount;

        // subtract the number of bank holidays during the time interval
        foreach (DateTime bankHoliday in bankHolidays)
        {
            DateTime bh = bankHoliday.Date;
            if (firstDay <= bh && bh <= lastDay)
                --businessDays;
        }

        return businessDays;
    }

}
