using System;
using System.Globalization;

namespace BF2Statistics.Web.Bf2Stats
{
    /// <summary>
    /// This object acts as a base object for all Models used in the Bf2Stats views
    /// </summary>
    public abstract class BF2PageModel
    {
        /// <summary>
        /// Indicates the specific culture formating for dates
        /// </summary>
        public static readonly CultureInfo SpecificCulture = CultureInfo.CreateSpecificCulture("en-US");

        /// <summary>
        /// Contains the title of the page
        /// </summary>
        public string Title = Program.Config.BF2S_Title;

        /// <summary>
        /// Provides the Root url to the bf2stats pages
        /// </summary>
        public string Root = String.Empty;

        /// <summary>
        /// Gets or Sets the value in the Search bar
        /// </summary>
        public string SearchBarValue = String.Empty;

        public BF2PageModel(HttpClient Client)
        {
            this.Root = "http://" + Client.Request.Url.DnsSafeHost + "/bf2stats";
        }

        /// <summary>
        /// Formats an integer timestamp to a timespan format that was used in BF2sClone
        /// </summary>
        /// <param name="Time"></param>
        /// <returns></returns>
        public string FormatNumber(object Time)
        {
            return String.Format(SpecificCulture, "{0:n0}", Double.Parse(Time.ToString()));
        }

        /// <summary>
        /// Formats an integer timestamp to a timespan format that was used in BF2sClone
        /// </summary>
        /// <param name="Time"></param>
        /// <returns></returns>
        public string FormatTime(double Time)
        {
            TimeSpan Span = TimeSpan.FromSeconds(Time);

            // We don't want the Timespan rounding up the fraction of total hours
            // so we will make our own method for calculation...
            double hours = (Span.Days * 24) + Span.Hours;
            return String.Format(SpecificCulture, "{0:00}:{1:00}:{2:00}", hours, Span.Minutes, Span.Seconds);
        }

        /// <summary>
        /// Takes a timestamp and converts it to a data format that was used in BF2sClone
        /// </summary>
        /// <param name="Time"></param>
        /// <returns></returns>
        public string FormatDate(int Time)
        {
            DateTime T = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (T.AddSeconds(Time)).ToString("yyyy-MM-dd HH:mm:ss", SpecificCulture);
        }

        /// <summary>
        /// Returns a string in the <see cref="contents"/> variable if the <see cref="condition"/> is met
        /// </summary>
        /// <param name="contents">The contents to be returned if the condition is met</param>
        /// <param name="condition">a bool indicating whether the condition is met</param>
        /// <returns></returns>
        public string WriteIf(string contents, bool condition)
        {
            return (condition) ? contents : String.Empty;
        }

        /// <summary>
        /// Returns a string based on the value of the <see cref="condition"/> variable
        /// </summary>
        /// <param name="condition">a bool indicating whether the condition is met</param>
        /// <param name="trueValue">The value to return if the condition is met</param>
        /// <param name="falseValue">The value to return if the condition is NOT met</param>
        /// <returns></returns>
        public string WriteIfElse(bool condition, string trueValue, string falseValue)
        {
            return (condition) ? trueValue : falseValue;
        }

        /// <summary>
        /// Writes the plain contents to the template
        /// </summary>
        /// <param name="contents">The contents to be written</param>
        /// <returns></returns>
        public string Write(string contents)
        {
            return contents;
        }
    }
}
