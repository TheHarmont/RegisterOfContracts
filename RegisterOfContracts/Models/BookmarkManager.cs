namespace RegisterOfContracts.Models
{
    public class BookmarkItem
    {
        /// <summary>
        /// Название в шаблонах
        /// </summary>
        public string? bmName { get; set; }

        /// <summary>
        /// Фактическое название закладки в БД
        /// </summary>
        public string? bmValue { get; set; }

        public BookmarkItem(string bmname, string bmvalue) 
        {
            bmName = bmname;
            bmValue = bmvalue;
        }
    }

    /// <summary>
    /// Класс, содержащий список закладок для каждого из контрактов
    /// Лучше реализацию не придумал
    /// </summary>
    internal class BookmarkManager
    {
        private Dictionary<string, List<BookmarkItem>> bookmarkTemplates;

        public BookmarkManager()
        {
            //При появлении нового контракта, добавляем сюда необходимую строку
            bookmarkTemplates = new Dictionary<string, List<BookmarkItem>>
            {
                #region Решение комиссии о списании.docx
                ["Решение комиссии о списании.docx"] =
                    new List<BookmarkItem>()
                    {
                    new BookmarkItem("Дата", ""),
                    new BookmarkItem("НомерКонтракта1","number"),
                    new BookmarkItem("ДатаЗаключения1", "signDate"),
                    new BookmarkItem("НаименованиеПоставщика", "shortName"),
                    new BookmarkItem("МестоположениеПоставщика", "mailingAddress"),
                    new BookmarkItem("ИННПоставщика","INN"),
                    new BookmarkItem("НомерКонтракта2","number"),
                    new BookmarkItem("ДатаЗаключения2","signDate"),
                    new BookmarkItem("ПредметКонтракта","contractSubject"),
                    new BookmarkItem("КодЗакупки","purchaseCode"),
                    },
                #endregion

                #region Требование об уплате неустойки.docx
                ["Требование об уплате неустойки.docx"] =
                    new List<BookmarkItem>()
                    {
                    new BookmarkItem("ПолноеИмяПоставщика","fullName"),
                    new BookmarkItem("МестоположениеПоставщика","mailingAddress"),
                    new BookmarkItem("АдресЭлПочты","contactEMail"),
                    new BookmarkItem("НомерКонтракта1","number"),
                    new BookmarkItem("ДатаЗаключения","signDate"),
                    new BookmarkItem("НаименованиеПоставщика","shortName"),
                    new BookmarkItem("ПредметКонтракта","contractSubject"),
                    new BookmarkItem("НомерКонтракта2","number"),
                    },
                #endregion

                #region Приказ о списании.docx
                ["Приказ о списании.docx"] =
                    new List<BookmarkItem>()
                    {
                    new BookmarkItem("Дата",""),
                    new BookmarkItem("НаименованиеПоставщика1","shortName"),
                    new BookmarkItem("НаименованиеПоставщика2","shortName"),
                    new BookmarkItem("МестоположениеПоставщика","mailingAddress"),
                    new BookmarkItem("ИННПоставщика","INN"),
                    new BookmarkItem("НомерКонтракта","number"),
                    new BookmarkItem("ДатаЗаключения","signDate"),
                    new BookmarkItem("ПредметКонтракта","contractSubject"),
                    new BookmarkItem("КодЗакупки","purchaseCode"),
                    },
                #endregion

                #region Уведомление о списании.docx
                ["Уведомление о списании.docx"] =
                    new List<BookmarkItem>()
                    {
                    new BookmarkItem("ПолноеИмяПоставщика", "fullName"),
                    new BookmarkItem("ИННПоставщика","INN"),
                    new BookmarkItem("КПППоставщика","KPP"),
                    new BookmarkItem("НаименованиеОПФ","singularName"),
                    new BookmarkItem("МестоположениеПоставщика","mailingAddress"),
                    new BookmarkItem("НомерКонтракта","number") ,
                    new BookmarkItem("ДатаЗаключения","signDate"),
                    new BookmarkItem("НомерВРеестре","regNum"),
                    }
                #endregion
            };
        }

        public List<BookmarkItem> GetBookmarksFromPattern(string templateName)
        {
            if (bookmarkTemplates.ContainsKey(templateName))
            {
                return bookmarkTemplates[templateName];
            }

            throw new ArgumentException("Неверное название шаблона.");
        }
    }
}
