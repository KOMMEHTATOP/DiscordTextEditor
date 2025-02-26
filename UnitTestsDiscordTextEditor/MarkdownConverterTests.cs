using Xunit;
using DiscordTextEditor.Helpers;
using System.Diagnostics;

namespace UnitTestsDiscordTextEditor
{
    public class MarkdownConverterTests
    {
        #region BaseConvert
        [Fact]
        public void Convert_ShouldConvertBoldToMarkdown()
        {
            string html = "<b>Привет</b>";
            string expected = "**Привет**";
            string actual = ConvertHtmlToMarkdown.Convert(html);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Convert_ShouldConvertItalicToMarkdown()
        {
            string html = "<i>Привет</i>";
            string expected = "*Привет*";
            string actual = ConvertHtmlToMarkdown.Convert(html);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Convert_ShouldConvertUnderlineToMarkdown()
        {
            string html = "<u>Привет</u>";
            string expected = "__Привет__";
            string actual = ConvertHtmlToMarkdown.Convert(html);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Convert_ShouldConvertStrikeToMarkdown()
        {
            string html = "<s>Привет</s>";
            string expected = "~~Привет~~";
            string actual = ConvertHtmlToMarkdown.Convert(html);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Garbage
        [Fact]
        public void Delete_ShouldReplaceDiv()
        {
            string html = "<div>Привет</div>";
            string expected = "\nПривет";
            string actual = ConvertHtmlToMarkdown.Convert(html);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Delete_ShouldReplaceBr()
        {
            string html = "<br>Привет</br>";
            string expected = "\nПривет";
            string actual = ConvertHtmlToMarkdown.Convert(html);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Convert_ShouldReplaceNbspWithSpace()
        {
            string html = "Привет&nbsp;мир!";
            string expected = "Привет мир!";  // Ожидаем обычный пробел
            string actual = ConvertHtmlToMarkdown.Convert(html);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Combined Formatting

        [Fact]
        public void Convert_ShouldConvertBoldItalicToMarkdown()
        {
            string html = "<b><i>Привет</i></b>";
            string expected = "***Привет***"; // Жирный + курсив
            string actual = ConvertHtmlToMarkdown.Convert(html);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Convert_ShouldConvertUnderlineItalicToMarkdown()
        {
            string html = "<u><i>Привет</i></u>";
            string expected = "__*Привет*__"; // Подчёркнутый + курсив
            string actual = ConvertHtmlToMarkdown.Convert(html);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Convert_ShouldConvertUnderlineBoldToMarkdown()
        {
            string html = "<u><b>Привет</b></u>";
            string expected = "__**Привет**__"; // Подчёркнутый + жирный
            string actual = ConvertHtmlToMarkdown.Convert(html);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Convert_ShouldConvertUnderlineBoldItalicToMarkdown()
        {
            string html = "<u><b><i>Привет</i></b></u>";
            string expected = "__***Привет***__"; // Подчёркнутый + жирный + курсив
            string actual = ConvertHtmlToMarkdown.Convert(html);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Convert_ShouldConvertStrikethroughToMarkdown()
        {
            string html = "<s>Привет</s>";
            string expected = "~~Привет~~"; // Зачёркнутый
            string actual = ConvertHtmlToMarkdown.Convert(html);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Whitespace Handling

        [Fact]
        public void Convert_ShouldNotAddFormattingOnEmptyLines()
        {
            string html = "<b></b>\n<i></i>\nПривет";
            string expected = "\n\nПривет"; // Ожидаем просто пустые строки
            string actual = ConvertHtmlToMarkdown.Convert(html);
            Debug.WriteLine($"[{actual}]");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Convert_ShouldPreserveLineBreaks()
        {
            string html = "Первая строка<br>Вторая строка";
            string expected = "Первая строка\nВторая строка"; // <br> → \n
            string actual = ConvertHtmlToMarkdown.Convert(html);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Convert_ShouldPreserveParagraphBreaks()
        {
            string html = "<div>Первая строка</div><div>Вторая строка</div>";
            string expected = "\nПервая строка\nВторая строка"; // <div> → \n
            string actual = ConvertHtmlToMarkdown.Convert(html);

            Assert.Equal(expected, actual);
        }

        #endregion

        [Fact]
        public void Convert_ShouldHandleRepeatedFormatting()
        {
            string html = "<b>текст</b>";  // Уже жирный
            string expected = "**текст**"; // Ожидаем без двойного дублирования

            string firstPass = ConvertHtmlToMarkdown.Convert(html);
            string secondPass = ConvertHtmlToMarkdown.Convert(firstPass); // Еще раз форматируем

            Assert.Equal(expected, secondPass); // Должно остаться таким же
        }

        #region Repeated Formatting

        [Fact]
        public void Convert_ShouldHandleRepeatedBold()
        {
            string html = "<b><b>Привет</b></b>"; // Двойной жирный
            string expected = "**Привет**"; // Ожидаем один слой форматирования
            string actual = ConvertHtmlToMarkdown.Convert(html);
            Debug.WriteLine($"[{actual}]");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Convert_ShouldHandleRepeatedItalic()
        {
            string html = "<i><i>Привет</i></i>"; // Двойной курсив
            string expected = "*Привет*"; // Ожидаем один слой форматирования
            string actual = ConvertHtmlToMarkdown.Convert(html);
            Debug.WriteLine($"[{actual}]");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Convert_ShouldHandleRepeatedUnderline()
        {
            string html = "<u><u>Привет</u></u>"; // Двойное подчёркивание
            string expected = "__Привет__"; // Ожидаем один слой форматирования
            string actual = ConvertHtmlToMarkdown.Convert(html);
            Debug.WriteLine($"[{actual}]");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Convert_ShouldHandleRepeatedStrikethrough()
        {
            string html = "<s><s>Привет</s></s>"; // Двойное зачёркивание
            string expected = "~~Привет~~"; // Ожидаем один слой форматирования
            string actual = ConvertHtmlToMarkdown.Convert(html);
            Debug.WriteLine($"[{actual}]");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Convert_ShouldHandleMixedRepeatedFormatting()
        {
            string html = "<b><i><b>Привет</b></i></b>"; // Жирный + курсив + повторный жирный
            string expected = "***Привет***"; // Ожидаем жирный + курсив
            string actual = ConvertHtmlToMarkdown.Convert(html);
            Debug.WriteLine($"[{actual}]");

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}