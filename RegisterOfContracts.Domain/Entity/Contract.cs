using ContractParser.Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RegisterOfContracts.Domain.Entity
{
    public class Contract
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Номер контракта")]
        public string number { get; set; }

        [Display(Name = "Дата заключения")]
        public DateTime signDate { get; set; }

        [Display(Name = "Наименование поставщика")]
        public string? shortName { get; set; }

        [Display(Name = "Наименование ОПФ")]
        public string? singularName { get; set; }

        [Display(Name = "Местоположение поставщика")]
        public string? mailingAddress { get; set; }

        [Display(Name = "Полное имя поставщика")]
        public string? fullName { get; set; }

        [Display(Name = "Код закупки")]
        public string? purchaseCode { get; set; }

        [Display(Name = "Предмет контракта")]
        public string contractSubject { get; set; }

        [Display(Name = "EMail постащика")]
        public string? contactEMail { get; set; }

        [Display(Name = "ИНН поставщика")]
        public string? INN { get; set; }

        [Display(Name = "КПП поставщика")]
        public string? KPP { get; set; }

        [Display(Name = "Номер в реестре")]
        public string? regNum { get; set; }

        public virtual ICollection<Attachment>? Attachments { get; set; }

        [Display(Name = "Юр адрес поставщика")]
        public string? address { get; set; }

        [Display(Name = "Имя контрагента")]
        public string? counterpartyName { get; set; }

        [Display(Name = "href")]
        public string href { get; set; }
        public string sourceHash { get; set; }
        public long sourceSize { get; set; }
        public DateTime uploadDate { get; set; }

        public Contract()
        {
            Attachments = new List<Attachment>();
        }
    }
}
