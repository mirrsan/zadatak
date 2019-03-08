using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZadatakNeki.Models;

namespace ZadatakNeki.Repository
{
    public class OsobaUredjajRepository : IRepository<OsobaUredjaj>
    {
        private readonly ToDoContext _context;

        public OsobaUredjajRepository(ToDoContext context)
        {
            _context = context;
        }

        public List<OsobaUredjaj> DajSveEntitete()
        {
            return _context.OsobaUredjaj.Include(o => o.Osoba)
                                        .Include(u => u.Uredjaj)
                                        .Include(k => k.Osoba.Kancelarija).ToList();
        }

        public OsobaUredjaj EntitetPoId(long id)
        {
            foreach (var osobaUredjaj in DajSveEntitete())
            {
                if (osobaUredjaj.Id == id)
                {
                    return osobaUredjaj;
                }
            }

            return null;
        }

        public OsobaUredjaj SacuvajEntitet(OsobaUredjaj entitet)
        {
            _context.OsobaUredjaj.Add(entitet);
            _context.SaveChanges();
            return entitet;
        }
    }
}
