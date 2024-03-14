using Microsoft.EntityFrameworkCore;
using RegisterOfContracts.Domain.Abstract;
using RegisterOfContracts.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterOfContracts.Domain.DAL
{
    public class EFContractRepository : IContractRepository
    {
        private readonly DataBaseContext _db;

        public EFContractRepository(DataBaseContext db)
        {
            _db = db;
        }

        public Entity.Contract GetContract(int id) 
        {
            return _db.Contracts.Include(x => x.Attachments).FirstOrDefault(c => c.id == id);
        }

        public IQueryable<Entity.Contract> GetAllContracts() 
        {
            return _db.Contracts.Include(x => x.Attachments).AsQueryable();
        }
    }
}
