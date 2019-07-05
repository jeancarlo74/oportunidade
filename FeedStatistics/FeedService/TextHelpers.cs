using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FeedServices
{
    public static class TextHelpers
    {
        public static string RemoveHtmlTags(string htmlText)
        {
            Regex rx = new Regex("<[^>]*>");

            return rx.Replace(htmlText, "");
        }
    }
}
