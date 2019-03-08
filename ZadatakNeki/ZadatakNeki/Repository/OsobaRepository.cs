using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZadatakNeki.Models;

namespace ZadatakNeki.Repository
{
    public class OsobaRepository : IRepository<Osoba>
    {
        private readonly ToDoContext _context;

        public OsobaRepository(ToDoContext context)
        {
            _context = context;
        }

        public List<Osoba> DajSveEntitete()
        {
            return _context.Osobe.Include(p => p.Kancelarija).ToList();
        }

        public Osoba EntitetPoId(long id)
        {
            foreach (var osoba in DajSveEntitete())
            {
                if (osoba.Id == id)
                {
                    return osoba;
                }
            }
            return null;
        }

        public Osoba SacuvajEntitet(Osoba entitet)
        {
            _context.Osobe.Add(entitet);
            _context.SaveChanges();
            return entitet;
        }
    }
}
