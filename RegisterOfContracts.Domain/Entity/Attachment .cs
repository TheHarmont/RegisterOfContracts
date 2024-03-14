using RegisterOfContracts.Domain.Entity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ContractParser.Domain.Entity
{
    public class Attachment
    {
        [Key]
        public int id { get; set; }
        public int contractId { get; set; } 
        public string url { get; set; } 
        public string fileName { get; set; }

        [JsonIgnore]
        public virtual Contract Contract { get; set; } 
    }
}
