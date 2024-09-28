using System.Text.Json.Nodes;

namespace BlueTreadHiringAssessment.Classes
{
    public static class Extensions
    {
        public static String CleanSpecialChars(this String s)
        {
            return s.Replace("'","`").Replace("\"","`");
        }

        public static string GetLinkByName(this JsonNode data, string name)
        {
            try
            {
                var urlString = data[name][0]["url"].GetValue<string>();
                if (urlString != null)
                {
                    return urlString;
                }
            }
            catch (Exception ex)
            {
                return "#";
            }
            return "#";
        }
    }
}
