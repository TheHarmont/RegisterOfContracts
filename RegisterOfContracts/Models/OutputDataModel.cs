using RegisterOfContracts.Domain.Entity;

namespace RegisterOfContracts.Models
{

    public class OutputDataModel
    {
        public object? pathPattern { get; set; }

        public Contract contracts { get; set; }

        public OutputDataModel(string webRootPath, string folderName)
        {
            pathPattern = Path.Combine(webRootPath, folderName);
        }
    }
}
