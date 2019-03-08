using System;
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
    public class OsobaController : Controller
    {
        private readonly ToDoContext _context;
        private readonly IMapper _mapper;
        private readonly OsobaRepository _repository;

        public OsobaController(ToDoContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
            _repository = new OsobaRepository(_context);
        }

        //metod koji vraca sve osobe
        [HttpGet]
        public IActionResult SveOsobe()
        {
           var svi = _repository.DajSveEntitete();
           List<OsobaDTO> sviDto = new List<OsobaDTO>();

           foreach (var osoba in svi)
           {
               sviDto.Add(_mapper.Map<OsobaDTO>(osoba));
           }
           return Ok(sviDto);
        }

        //akcija koja vraca samo entitet koji ima dati ID
        [HttpGet("{id}")]
        public IActionResult OsobaPoId(long id)
        {
            var osoba = _repository.EntitetPoId(id);

            if (osoba == null)
            {
                return NotFound("Nisam nasao nista sa tim ID.");
            }
            return Ok(_mapper.Map<OsobaDTO>(osoba));
        }

        //akcija koja upisuje novi entitet u bazu
        [HttpPost]
        public IActionResult UpisivanjeOsobe(OsobaDTO osobaInfo)
        {
            if(osobaInfo == null)
            {
                return BadRequest("Niste upisali podatke da valja!");
            }
            Osoba osoba = _mapper.Map<Osoba>(osobaInfo);

            var kancelarija = (from nn in _context.Kancelarije
                               where nn.Opis == osobaInfo.Kancelarija.Opis
                               select nn).FirstOrDefault();

            if (kancelarija != null)
            {
                osoba.Kancelarija = kancelarija;
            }

            _repository.SacuvajEntitet(osoba);
            return Ok("Osoba je sacuvana u bazi.");
        }

        // akcija koja menja postojeci entitet koji ima dati ID
        [HttpPut("{id}")]
        public IActionResult IzmenaOsobe(long id, OsobaDTO noviInfo)
        {
            Osoba stariInfo = _repository.EntitetPoId(id);

            stariInfo.Ime = noviInfo.Ime;
            stariInfo.Prezime = noviInfo.Prezime;

            var kancelarija = (from nn in _context.Kancelarije
                               where nn.Opis == noviInfo.Kancelarija.Opis
                               select nn).FirstOrDefault();

            if (kancelarija != null)
            {
                stariInfo.Kancelarija = kancelarija;
            }
            else
            {
                stariInfo.Kancelarija = new Kancelarija() { Opis = noviInfo.Kancelarija.Opis };
            }

            _context.SaveChanges();
            
            return Ok("Sacuvane su izmene.");
        }

        // akcija koja brise entitet koji ima dati ID
        [HttpDelete("{id}")]
        public IActionResult BrisanjeOsobe(long id)
        {
            Osoba osoba = _repository.EntitetPoId(id);

            if (osoba == null)
            {
                return NotFound();
            }

            var mozda = (from nn in _context.OsobaUredjaj
                where nn.Osoba == osoba
                select new {Osoba = nn.Osoba.Ime, Uredjaj = nn.Uredjaj.Naziv}).FirstOrDefault();

            if (mozda != null)
            {
                return BadRequest("Osoba nije izbrisana iz baze zato sto je vec koristila/koristi neki od uredjaja.");
            }
            
            _context.Osobe.Remove(osoba);
            _context.SaveChanges();

            return Ok("Osoba je izbrisana iz baze podataka.");
        }

        // akcija koja pretrazuje entitet po imenu osobe
        [HttpGet("{ime}")]
        public IActionResult PretragaPoImenuOsobe(string ime)
        {
            var osoba = from n in _context.Osobe
                where n.Ime == ime
                select new OsobaDTO {Ime = n.Ime,
                                     Prezime = n.Prezime,
                                     Kancelarija = _mapper.Map<KancelarijaDTO>(n.Kancelarija)};
            if (osoba == null)
            {
                return NotFound();
            }
            return Ok(osoba.ToList());
        }

        // akcija koja pretrazuje entitete po nazivu kancelarije
        [HttpGet("{opisKancelarije}")]
        public IActionResult PretragaPoOpisuKancelarije(string opisKancelarije)
        {
            var osoba = from n in _context.Osobe
                where n.Kancelarija.Opis == opisKancelarije
                select new OsobaDTO {Ime = n.Ime,
                                     Prezime = n.Prezime,
                                     Kancelarija = _mapper.Map<KancelarijaDTO>(n.Kancelarija) };
            if (osoba == null)
            {
                return NotFound();
            }
            return Ok(osoba.ToList());
        }

        // akcija koja pretrazuje entitet po datumu od/do i vraca uredjaja i osobu 
        [HttpGet("{odDatum}/{doDatum}")]
        public IActionResult PretragaPoDatumuOdDo(DateTime odDatum, DateTime doDatum)
        {
            var bzz = from n in _context.OsobaUredjaj
                      where n.PocetakKoriscenja == odDatum && n.KrajKoriscenja == doDatum
                      select new { Osoba = n.Osoba.Ime, Uredjaj = n.Uredjaj.Naziv };

            return Ok(bzz.ToList());
        }
    }
}