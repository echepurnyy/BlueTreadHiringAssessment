using System.Text.Json.Nodes;

namespace BlueTreadHiringAssessment.Classes
{
    public static class Extensions
    {
        /*Removes HTML-sensitive characters from a string*/
        public static String CleanSpecialChars(this String s)
        {
            return s.Replace("'","`").Replace("\"","`").Replace("&", " and ").Replace("?", "");
        }

        /*retrieves an external site link out of their many 
         *nested containers, using the name (youtube, twitter etc.) as the key.*/
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
