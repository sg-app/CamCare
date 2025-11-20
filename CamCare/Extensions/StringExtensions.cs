namespace CamCare.Extensions
{
    public static class StringExtensions
    {
        public static string Trim(this string str, int length)
        {
            if (string.IsNullOrEmpty(str) || length <= 0)
                return str;

            if (str.Length <= length)
                return str;

            return str.Substring(0, length) + "...";
        }
    }
}
