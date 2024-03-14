using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RegisterOfContracts.Domain.Abstract;
using RegisterOfContracts.Models;
using System.Diagnostics;

namespace RegisterOfContracts.Controllers
{
    public class ContractController : Controller
    {
        private readonly ILogger<ContractController> _logger;
        private readonly IContractRepository _contractRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContractController(ILogger<ContractController> logger, IContractRepository contractRepository,  IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _contractRepository = contractRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var customerData = new OutputDataModel(_webHostEnvironment.WebRootPath, "template");
            return View(customerData);
        }

        /// <summary>
        /// Возвращает контракт по id
        /// </summary>
        public IActionResult GetContractByID(int id)
        {
            return Json(new
            {
                data = _contractRepository.GetContract(id)
            });
        }

        /// <summary>
        /// POST-Запрос. Получение всех контрактов
        /// </summary>
        /// <param name="hiddenColumns">Содержит скрытые столбцы. Необходим для фильтрации</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetContractAjax(List<bool> hiddenColumns)
        {
            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var firstData = _contractRepository.GetAllContracts();

            #region Фильтрация
            #region Глобальный поиск
            var searchValue = Request.Form["search[value]"].FirstOrDefault().ToLower();
            if (!string.IsNullOrEmpty(searchValue))
            {
                firstData = firstData.Where(col =>
                                                !hiddenColumns[0] && col.number.Contains(searchValue) ||
                                                !hiddenColumns[1] && col.signDate.Date.ToString().Contains(searchValue) ||
                                                !hiddenColumns[2] && col.shortName.Contains(searchValue) ||
                                                !hiddenColumns[3] && col.purchaseCode.Contains(searchValue) ||
                                                !hiddenColumns[4] && col.contractSubject.Contains(searchValue) ||
                                                !hiddenColumns[5] && col.contactEMail.Contains(searchValue) ||
                                                !hiddenColumns[6] && col.INN.Contains(searchValue) ||
                                                !hiddenColumns[7] && col.KPP.Contains(searchValue) ||
                                                !hiddenColumns[8] && col.regNum.Contains(searchValue) ||
                                                !hiddenColumns[9] && col.address.Contains(searchValue));
            }
            #endregion

            #region Фильтрация по столбцам
            Dictionary<int, string> searchCols = new Dictionary<int, string>(13);
            for (int i = 0; i < 11; ++i)
                searchCols.Add(i, Request.Form["columns[" + i + "][search][value]"].FirstOrDefault());

            foreach (var searchCol in searchCols)
            {
                var val = searchCol.Value.ToLower();
                if (!string.IsNullOrEmpty(searchCol.Value) && !searchCol.Value.IsNullOrEmpty())
                {
                    switch (searchCol.Key)
                    {
                        case 0:
                            firstData = firstData.Where(x => x.id.ToString().Contains(val));
                            break;
                        case 1:
                            firstData = firstData.Where(x => x.number.Contains(val));
                            break;
                        case 2:
                            firstData = firstData.Where(x => x.signDate.Date.ToString().Contains(val));
                            break;
                        case 3:
                            firstData = firstData.Where(x => x.shortName.Contains(val));
                            break;
                        case 4:
                            firstData = firstData.Where(x => x.purchaseCode.Contains(val));
                            break;
                        case 5:
                            firstData = firstData.Where(x => x.contractSubject.Contains(val));
                            break;
                        case 6:
                            firstData = firstData.Where(x => x.contactEMail.Contains(val));
                            break;
                        case 7:
                            firstData = firstData.Where(x => x.INN.Contains(val));
                            break;
                        case 8:
                            firstData = firstData.Where(x => x.KPP.Contains(val));
                            break;
                        case 9:
                            firstData = firstData.Where(x => x.regNum.Contains(val));
                            break;
                        case 10:
                            firstData = firstData.Where(x => x.address.Contains(val));
                            break;
                    }
                }
            }
            #endregion
            #endregion

            #region Сортировка
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                if (sortColumnDirection == "desc")
                {
                    switch (sortColumn)
                    {
                        case "id":
                            firstData = firstData.OrderBy(x => x.id);
                            break;
                        case "number":
                            firstData = firstData.OrderBy(x => x.number);
                            break;
                        case "signDate":
                            firstData = firstData.OrderBy(x => x.signDate);
                            break;
                        case "shortName":
                            firstData = firstData.OrderBy(x => x.shortName);
                            break;
                        case "purchaseCode":
                            firstData = firstData.OrderBy(x => x.purchaseCode);
                            break;
                        case "contractSubject":
                            firstData = firstData.OrderBy(x => x.contractSubject);
                            break;
                        case "contactEMail":
                            firstData = firstData.OrderBy(x => x.contactEMail);
                            break;
                        case "INN":
                            firstData = firstData.OrderBy(x => x.INN);
                            break;
                        case "KPP":
                            firstData = firstData.OrderBy(x => x.KPP);
                            break;
                        case "regNum":
                            firstData = firstData.OrderBy(x => x.regNum);
                            break;
                        case "address":
                            firstData = firstData.OrderBy(x => x.address);
                            break;
                    }
                }
                else if (sortColumnDirection == "asc")
                {
                    switch (sortColumn)
                    {
                        case "id":
                            firstData = firstData.OrderByDescending(x => x.id);
                            break;
                        case "number":
                            firstData = firstData.OrderByDescending(x => x.number);
                            break;
                        case "signDate":
                            firstData = firstData.OrderByDescending(x => x.signDate);
                            break;
                        case "shortName":
                            firstData = firstData.OrderByDescending(x => x.shortName);
                            break;
                        case "purchaseCode":
                            firstData = firstData.OrderByDescending(x => x.purchaseCode);
                            break;
                        case "contractSubject":
                            firstData = firstData.OrderByDescending(x => x.contractSubject);
                            break;
                        case "contactEMail":
                            firstData = firstData.OrderByDescending(x => x.contactEMail);
                            break;
                        case "INN":
                            firstData = firstData.OrderByDescending(x => x.INN);
                            break;
                        case "KPP":
                            firstData = firstData.OrderByDescending(x => x.KPP);
                            break;
                        case "regNum":
                            firstData = firstData.OrderByDescending(x => x.regNum);
                            break;
                        case "address":
                            firstData = firstData.OrderByDescending(x => x.address);
                            break;
                    }
                }
            }
            #endregion

            recordsTotal = firstData.Count();
            if (pageSize == -1)
                pageSize = recordsTotal;

            var customerData = firstData
                .Skip(skip)
                .Take(pageSize)
                .Where(x => x != null)
                .ToList()
                .Select(x => new
                {
                    id = x.id,
                    number = x.number,
                    signDate = x.signDate,
                    shortName = x.shortName,
                    purchaseCode = x.purchaseCode,
                    contractSubject = x.contractSubject,
                    contactEMail = x.contactEMail,
                    INN = x.INN,
                    KPP = x.KPP,
                    regNum = x.regNum,
                    address = x.address
                });

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = customerData });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}