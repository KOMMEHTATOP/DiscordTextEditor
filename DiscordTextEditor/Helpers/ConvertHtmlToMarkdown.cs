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

            // Преобразуем базовые стили в Markdown
            html = html.Replace("<b>", "**").Replace("</b>", "**");
            html = html.Replace("<strong>", "**").Replace("</strong>", "**");
            html = html.Replace("<i>", "*").Replace("</i>", "*");
            html = html.Replace("<em>", "*").Replace("</em>", "*");
            html = html.Replace("<u>", "__").Replace("</u>", "__");
            html = html.Replace("<s>", "~~").Replace("</s>", "~~");

            // Убираем ненужные HTML-символы
            html = html.Replace("&nbsp;", " ");

            // Обрабатываем переносы строк
            html = html.Replace("<br>", "\n").Replace("</br>", "");
            html = html.Replace("<div>", "\n").Replace("</div>", "");

            // Удаляем форматирование на пустых строках
            var lines = html.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                // Убираем все символы Markdown и проверяем, осталась ли строка пустой
                string cleanLine = lines[i]
                    .Replace("**", "")
                    .Replace("*", "")
                    .Replace("__", "")
                    .Replace("~~", "");
                if (string.IsNullOrWhiteSpace(cleanLine))
                    lines[i] = ""; // Если после удаления форматирования строка пуста, очищаем её
            }
            html = string.Join("\n", lines);

            return html;
        }
    }
}
