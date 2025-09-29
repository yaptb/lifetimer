using System;

namespace LifeTimer.Helpers
{
    public static class DateTimeFormatHelper
    {
        public static string FormatTargetDateTime(DateTime dateTime)
        {
            return $"Target: {dateTime:MMM dd, yyyy HH:mm:ss}";
        }

        public static string FormatTimeRemaining(TimeSpan timeSpan, bool daysOnly = false, bool showSeconds = true)
        {
            if (timeSpan.TotalMilliseconds <= 0)
                return "Timer has expired";

            if (daysOnly)
            {
                return $"{Math.Floor(timeSpan.TotalDays):F0} days";
            }

            var parts = new System.Collections.Generic.List<string>();

            if (timeSpan.Days > 0)
                parts.Add($"{timeSpan.Days}d");

            if (timeSpan.Hours > 0)
                parts.Add($"{timeSpan.Hours}h");

            if (timeSpan.Minutes > 0)
                parts.Add($"{timeSpan.Minutes}m");

            if (showSeconds && timeSpan.Seconds > 0)
                parts.Add($"{timeSpan.Seconds}s");

            return parts.Count > 0 ? string.Join(" ", parts) : "0";
        }

        /// <summary>
        /// Formats a timespan for countdown display (time remaining until target)
        /// </summary>
        /// <param name="targetDateTime">Target date/time</param>
        /// <param name="daysOnly">Show only days count</param>
        /// <param name="showHours">Include hours in display</param>
        /// <param name="showMinutes">Include minutes in display</param>
        /// <param name="showSeconds">Include seconds in display</param>
        /// <returns>Formatted countdown string</returns>
        public static string FormatCountdown(DateTime targetDateTime, bool daysOnly = false, bool showHours = true, bool showMinutes = true, bool showSeconds = true)
        {
            var timeRemaining = targetDateTime - DateTime.Now;

            if (timeRemaining.TotalMilliseconds <= 0)
            {
                return "Timer expired";
            }

            return FormatTimeSpanNew(timeRemaining, daysOnly, showHours, showMinutes, showSeconds);
        }

        /// <summary>
        /// Formats a timespan for countup display (time elapsed since target)
        /// </summary>
        /// <param name="targetDateTime">Target date/time</param>
        /// <param name="daysOnly">Show only days count</param>
        /// <param name="showHours">Include hours in display</param>
        /// <param name="showMinutes">Include minutes in display</param>
        /// <param name="showSeconds">Include seconds in display</param>
        /// <returns>Formatted countup string</returns>
        public static string FormatCountup(DateTime targetDateTime, bool daysOnly = false, bool showHours = true, bool showMinutes = true, bool showSeconds = true)
        {
            var timeElapsed = DateTime.Now - targetDateTime;

            if (timeElapsed.TotalMilliseconds <= 0)
            {
                return "Not started";
            }

            return FormatTimeSpanNew(timeElapsed, daysOnly, showHours, showMinutes, showSeconds);
        }

        /// <summary>
        /// Formats a timespan with hierarchical display options
        /// </summary>
        /// <param name="timeSpan">TimeSpan to format</param>
        /// <param name="daysOnly">Show days only format (Days:Hours:Minutes:Seconds)</param>
        /// <param name="showHours">Include hours in display</param>
        /// <param name="showMinutes">Include minutes in display</param>
        /// <param name="showSeconds">Include seconds in display</param>
        /// <returns>Formatted timespan string</returns>
        public static string FormatTimeSpanNew(TimeSpan timeSpan, bool daysOnly = false, bool showHours = true, bool showMinutes = true, bool showSeconds = true)
        {
            var totalDays = (int)Math.Floor(timeSpan.TotalDays);
            var hours = timeSpan.Hours;
            var minutes = timeSpan.Minutes;
            var seconds = timeSpan.Seconds;

            if (daysOnly)
            {
                // Format: Days:Hours:Minutes:Seconds (based on enabled options)
                var parts = new System.Collections.Generic.List<string> { totalDays.ToString() };

                if (showHours)
                {
                    parts.Add($"{hours:D2}");

                    if (showMinutes)
                    {
                        parts.Add($"{minutes:D2}");

                        if (showSeconds)
                        {
                            parts.Add($"{seconds:D2}");
                        }
                    }
                }

                return string.Join(":", parts);
            }
            else
            {
                // Format: Years:Days:Hours:Minutes:Seconds (based on enabled options)
                var years = totalDays / 365;
                var days = totalDays % 365;

                var parts = new System.Collections.Generic.List<string> { years.ToString(), days.ToString() };

                if (showHours)
                {
                    parts.Add($"{hours:D2}");

                    if (showMinutes)
                    {
                        parts.Add($"{minutes:D2}");

                        if (showSeconds)
                        {
                            parts.Add($"{seconds:D2}");
                        }
                    }
                }

                return string.Join(":", parts);
            }
        }

        /// <summary>
        /// Formats a timespan with appropriate units (legacy method for backwards compatibility)
        /// </summary>
        /// <param name="timeSpan">TimeSpan to format</param>
        /// <param name="showSeconds">Include seconds in display</param>
        /// <returns>Formatted timespan string</returns>
        public static string FormatTimeSpan(TimeSpan timeSpan, bool showSeconds = true)
        {
            var parts = new System.Collections.Generic.List<string>();

            if (timeSpan.Days > 0)
                parts.Add($"{timeSpan.Days}d");

            if (timeSpan.Hours > 0)
                parts.Add($"{timeSpan.Hours}h");

            if (timeSpan.Minutes > 0)
                parts.Add($"{timeSpan.Minutes}m");

            if (showSeconds && timeSpan.Seconds > 0)
                parts.Add($"{timeSpan.Seconds}s");

            // If no parts and not showing seconds, show minutes
            if (parts.Count == 0 && !showSeconds)
            {
                parts.Add($"{timeSpan.Minutes}m");
            }

            // If still no parts, show "0m" or "0s"
            if (parts.Count == 0)
            {
                parts.Add(showSeconds ? "0s" : "0m");
            }

            return string.Join(" ", parts);
        }

        public static string FormatSimpleDateTime(DateTime dateTime)
        {
            return dateTime.ToString("MMM dd, yyyy HH:mm:ss");
        }

        public static string FormatShortDateTime(DateTime dateTime)
        {
            return dateTime.ToString("MMM dd, yyyy");
        }
    }
}