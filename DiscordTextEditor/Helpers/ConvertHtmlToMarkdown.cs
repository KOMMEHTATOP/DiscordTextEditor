using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordTextEditor.Helpers
{
    public class ConvertHtmlToMarkdown
    {
        public static string Convert(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            // Преобразуем <b> и <strong> в **
            html = html.Replace("<b>", "**").Replace("</b>", "**");
            html = html.Replace("<strong>", "**").Replace("</strong>", "**");

            return html;
        }
    }
}
