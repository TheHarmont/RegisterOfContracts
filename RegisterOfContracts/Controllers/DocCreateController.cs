using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using OpenXmlPowerTools;
using RegisterOfContracts.Domain.Abstract;
using RegisterOfContracts.Domain.Entity;
using RegisterOfContracts.Models;
using System.Collections;
using System.Xml.Linq;

namespace RegisterOfContracts.Controllers
{
    public class DocCreateController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IContractRepository _contractRepository;

        public DocCreateController(IWebHostEnvironment webHostEnvironment, IContractRepository contractRepository)
        {
            _webHostEnvironment = webHostEnvironment;
            _contractRepository = contractRepository;
        }

        #region Получить список закладка - значение
        /// <summary>
        /// Возвращает список закладка-значение
        /// </summary>
        public List<BookmarkItem> GetBookmarksValue(int contractId, List<BookmarkItem> bookmarksList)
        {
            var contract = _contractRepository.GetContract(contractId);

            var duplicates = new List<string>();

            foreach (var bookmark in bookmarksList)
            {
                if (!duplicates.Contains(bookmark.bmName))
                {
                    if (bookmark.bmName == "Дата") //Уникальная закладка, которая содержит текущую дату
                        bookmark.bmValue = DateTime.Today.ToShortDateString();
                    else if (bookmark.bmName.Contains("ДатаЗаключения")) 
                        bookmark.bmValue = Convert.ToDateTime(typeof(Contract).GetProperty(bookmark.bmValue)?.GetValue(contract)).ToString("dd.MM.yyyy");
                    else
                        bookmark.bmValue = typeof(Contract).GetProperty(bookmark.bmValue)?.GetValue(contract)?.ToString(); //Получаю элемент объекта contract, в зависисмости от названия bookmark.bmValue  

                    duplicates.Add(bookmark.bmName);
                }
            }

            return bookmarksList;
        }
        #endregion

        #region Генерация Документа
        /// <summary>
        /// На основе шаблона и соответствующего списка закладок, генерирует документ в виде массива байт
        /// </summary>
        private byte[] GenerateDocument(string templatePath, List<BookmarkItem> bmValueList)
        {
            using (FileStream fs = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                using (var generatedStream = new MemoryStream())
                {
                    fs.CopyTo(generatedStream);
                    generatedStream.Position = 0;

                    using (var doc = WordprocessingDocument.Open(generatedStream, true))
                    {
                        var bookmarkMap = doc.MainDocumentPart.Document.Body.Descendants<BookmarkStart>().ToDictionary(b => b.Name, b => b);

                        foreach (var bookmark in bmValueList)
                        {
                            if (bookmarkMap.TryGetValue(bookmark.bmName, out var bookmarkStart))
                            {

                                // Находим соответствующий конец закладки
                                var bookmarkEnd = bookmarkStart.NextSibling<BookmarkEnd>();

                                var runs = bookmarkStart.Parent.Descendants<Run>().ToList();

                                // Получаем форматирование оригинальной закладки
                                var originalFormatting = runs.SelectMany(run => run.Descendants<RunProperties>()).FirstOrDefault();

                                // Удаляем все элементы между закладкой (кроме закладки самой)
                                var currentNode = bookmarkStart.NextSibling();
                                while (currentNode != bookmarkEnd)
                                {
                                    var nextNode = currentNode.NextSibling();
                                    currentNode.Remove();
                                    currentNode = nextNode;
                                }

                                // Создаем новый элемент Run с текстом из bmValue
                                var run = new Run(new Text(bookmark.bmValue));

                                // Применяем форматирование оригинальной закладки к новому Run
                                if (originalFormatting != null)
                                {
                                    run.PrependChild(originalFormatting.CloneNode(true));
                                }

                                // Вставляем новый элемент Run после закладки
                                bookmarkEnd.InsertAfterSelf(run);

                                // Удаляем начало и конец закладок
                                bookmarkStart.Remove();
                                bookmarkEnd.Remove();
                            }
                        }

                        // Сохраняем изменения в потоке
                        doc.Save();
                        generatedStream.Position = 0;
                    }
                    // Возвращаем поток байтов
                    return generatedStream.ToArray();
                }
            }
        }
        #endregion

        #region Формирование HTML на основе docx
        /// <summary>
        /// Формирует html на основе массива байт документа
        /// </summary>
        public XElement GetHtmlFromDocx(byte[] documentByte)
        {
            XElement htmlContent;
            using (MemoryStream mem = new MemoryStream())
            {
                mem.Write(documentByte, 0, documentByte.Length);
                using (WordprocessingDocument doc = WordprocessingDocument.Open(mem, true))
                {
                    HtmlConverterSettings settings = new HtmlConverterSettings();
                    htmlContent = HtmlConverter.ConvertToHtml(doc, settings);
                }
            }
            return htmlContent;
        }
        #endregion

        /// <summary>
        /// Формируем документ в виде набора байт и html объекта.
        /// Набор байт нужен для скачивания
        /// html объект необходим для отображения на странице
        /// Также возвращаем имя файла,которое совпадает с названием шаблона
        /// </summary>
        [HttpPost]
        public IActionResult Index(int contractId, string selectedPattern)
        {
            //Пудь до шаблонов
            var templatePath = Path.Combine(_webHostEnvironment.WebRootPath, "template", selectedPattern);

            var bookmarkManager = new BookmarkManager();

            //Получить список закладка-значение
            var bmValueList = GetBookmarksValue(contractId, bookmarkManager.GetBookmarksFromPattern(selectedPattern));

            // Генерация документа
            byte[] documentBytes = GenerateDocument(templatePath, bmValueList);

            //Получение объекта html
            var htmlContent = GetHtmlFromDocx(documentBytes);

            return Json(new { fileData = Convert.ToBase64String(documentBytes), fileContent = htmlContent.ToString(), fileName = selectedPattern });
        }
    }
}

