using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZadatakNeki.Models;

namespace ZadatakNeki.Repository
{
    public class KancelarijaRepository : IRepository<Kancelarija>
    {
        private readonly ToDoContext _context;

        public KancelarijaRepository(ToDoContext context)
        {
            _context = context;
        }

        public List<Kancelarija> DajSveEntitete()
        {
            return _context.Kancelarije.ToList();
        }

        public Kancelarija EntitetPoId(long id)
        {
            return _context.Kancelarije.Find(id);
        }

        public Kancelarija SacuvajEntitet(Kancelarija entitet)
        {
            _context.Kancelarije.Add(entitet);
            _context.SaveChanges();
            return entitet;
        }
    }
}
