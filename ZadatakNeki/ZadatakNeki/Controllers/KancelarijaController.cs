using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ZadatakNeki.DTO;
using ZadatakNeki.Models;
using ZadatakNeki.Repository;

namespace ZadatakNeki.Controllers
{
    [Route("api/[controller]/[action]")]
    public class KancelarijaController : BaseController<Kancelarija, KancelarijaDTO>
    {
        private readonly ToDoContext _context;
        private readonly IMapper _mapper;
        private readonly KancelarijaRepository _repository;

        public KancelarijaController(ToDoContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
            _repository = new KancelarijaRepository(_context);
        }

        // akcija koja vraca sve kancelarije
        [HttpGet]
        public IActionResult SveKancelarije()
        {
            var sve = _repository.DajSveEntitete();
            List<KancelarijaDTO> sveDto = new List<KancelarijaDTO>();

            foreach (var kancelarija in sve)
            {
                sveDto.Add(_mapper.Map<KancelarijaDTO>(kancelarija));
            }

            return Ok(sveDto);
        }

        // akcija koja vraca samo entitet koji ima dati ID
        [HttpGet("{id}:int")]
        public IActionResult PoId(long id)
        {
            var kancelarija = _repository.EntitetPoId(id);
            if (kancelarija == null)
            {
                return NotFound("Evo ne nadjo nista sa tim ID-em.");
            }

            return Ok(_mapper.Map<KancelarijaDTO>(kancelarija));
        }

        // akcija koja upisuje novi entitet u bazu
        [HttpPost]
        public IActionResult Upisivanje(KancelarijaDTO kancelarijaNova)
        {
            if (kancelarijaNova == null)
            {
                return BadRequest("Nisi nesto fino upisao.");
            }
            Kancelarija kancelarija = _repository.SacuvajEntitet(_mapper.Map<Kancelarija>(kancelarijaNova));
            return Ok(_mapper.Map<KancelarijaDTO>(kancelarija));
        }

        // akcija koja menja postojeci entitet koji ima dati ID
        [HttpPut("{id}:int")]
        public IActionResult IzmenaPodataka(long id, KancelarijaDTO kancelarija)
        {
            return base.Izmena(id, kancelarija);
        }

        // akcija koja brise entitet koji ima dati ID
        [HttpDelete("{id}")]
        public IActionResult Brisanje(long id)
        {
            Kancelarija kancelarija = _repository.EntitetPoId(id);

            if (kancelarija == null)
            {
                return NotFound();
            }

            if (kancelarija.Opis.Equals("kantina"))
            {
                return BadRequest("Necu obrisat kantinu.");
            }

            List<Osoba> beziIzKancelarije = (from nn in _context.Osobe
                                             where nn.Kancelarija == kancelarija
                                             select nn).ToList();

            for (int i = 0; i <= beziIzKancelarije.Count(); i++)
            {
                Osoba osoba = beziIzKancelarije[i];

                osoba.Kancelarija = new Kancelarija() { Opis = "kantina" };

                var mozdaIma = (from nn in _context.Kancelarije
                                where nn.Opis == osoba.Kancelarija.Opis
                                select nn).FirstOrDefault();

                if (mozdaIma != null)
                {
                    osoba.Kancelarija = mozdaIma;
                }

                _context.SaveChanges();
            }

            _context.Kancelarije.Remove(kancelarija);
            _context.SaveChanges();

            return Ok("Kancelarija je izbrisana iz baze podataka.");
        }

        // akcija koja pretrazuje entitet po nazivu kancelarije
        [HttpGet("{opis}")]
        public IActionResult PretragaPoNazivu(string opis)
        {
            var kancelarija = from n in _context.Kancelarije where n.Opis == opis select new { Naziv = n.Opis };
            if (kancelarija == null)
            {
                return NotFound();
            }
            return Ok(kancelarija.ToList());
        }

        // ispis svih osoba u kancelariji
        [HttpGet("{kancelarija}")]
        public IActionResult PretragaSveOsobeIzKancelarije(string kancelarija)
        {
            var sveOsobeIzKancelarije = from nn in _context.Osobe
                                        where nn.Kancelarija.Opis == kancelarija
                                        select new { Osoba = nn.Ime.ToUpper() + " " + nn.Prezime.ToUpper() };

            return Ok(sveOsobeIzKancelarije);
        }
    }
}