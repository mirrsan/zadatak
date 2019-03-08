using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZadatakNeki.Models;

namespace ZadatakNeki.Repository
{
    public class UredjajRepository : IRepository<Uredjaj>
    {
        private readonly ToDoContext _context;

        public UredjajRepository(ToDoContext context)
        {
            _context = context;
        }

        public List<Uredjaj> DajSveEntitete()
        {
            return _context.Uredjaji.ToList();
        }

        public Uredjaj EntitetPoId(long id)
        {
            return _context.Uredjaji.Find(id);
        }

        public Uredjaj SacuvajEntitet(Uredjaj entitet)
        {
            _context.Uredjaji.Add(entitet);
            _context.SaveChanges();
            return entitet;
        }
    }
}
