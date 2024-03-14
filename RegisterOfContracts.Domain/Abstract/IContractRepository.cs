using RegisterOfContracts.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterOfContracts.Domain.Abstract
{
    public interface IContractRepository
    {
        public Contract GetContract(int id);
        public IQueryable<Contract> GetAllContracts();
    }
}
