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
    public class OsobaUredjajController : BaseController<OsobaUredjaj, OsobaUredjajDTO>
    {
        private readonly ToDoContext _context;
        private readonly IMapper _mapper;
        private readonly OsobaUredjajRepository _repository;

        public OsobaUredjajController(ToDoContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
            _repository = new OsobaUredjajRepository(_context);
        }

        // svi entiteti iz bazu
        [HttpGet]
        public IActionResult Sve()
        {
            var sve = _repository.DajSveEntitete();
            List<OsobaUredjajDTO> sveDto = new List<OsobaUredjajDTO>();

            foreach (var osobaUredjaj in sve)
            {
                sveDto.Add(_mapper.Map<OsobaUredjajDTO>(osobaUredjaj));
            }

            return Ok(sveDto);
        }

        // akcija vraca entitet koji ima dati ID
        [HttpGet("{id}")]
        public IActionResult PoId(long id)
        {
            var osobaUredjaj = _repository.EntitetPoId(id);

            if (osobaUredjaj == null)
            {
                return NotFound("Nema sa tim ID-em nista.");
            }

            return Ok(_mapper.Map<OsobaUredjajDTO>(osobaUredjaj));
        }

        // Upisivanje novog entiteta u bazy
        [HttpPost]
        public IActionResult DodatiNovi(OsobaUredjajDTO ouNovi)
        {
            if (ouNovi == null)
            {
                return Ok("aha ne moze to tako e");
            }

            OsobaUredjaj osobaUredjaj = _mapper.Map<OsobaUredjaj>(ouNovi);

            // ako osoba vec postoji
            Osoba osoba = (from nn in _context.Osobe
                           where nn.Ime == osobaUredjaj.Osoba.Ime && nn.Prezime == osobaUredjaj.Osoba.Prezime
                           select nn).FirstOrDefault();
            if (osoba != null)
            {
                osobaUredjaj.Osoba = osoba;
            }

            // ako kancelarija vec postoji
            if (osoba == null)
            {
                var kancelarija = (from nn in _context.Kancelarije
                    where nn.Opis == ouNovi.Osoba.Kancelarija.Opis
                    select nn).FirstOrDefault();
                if (kancelarija != null)
                {
                    osobaUredjaj.Osoba.Kancelarija = kancelarija;
                }
            }

            // ako uredjaj vec postoji
            Uredjaj uredjaj = (from nn in _context.Uredjaji
                              where nn.Naziv == osobaUredjaj.Uredjaj.Naziv
                              select nn).FirstOrDefault();
            if (uredjaj != null)
            {
                osobaUredjaj.Uredjaj = uredjaj;
            }

            _repository.SacuvajEntitet(osobaUredjaj);

            return Ok("Sacuvano je ;)");
        }
        
        // akcija koja menja postojeci entitet koji ima dati ID
        [HttpPut("{id}")]
        public IActionResult MenjanjeEntiteta(long id, OsobaUredjajDTO novi)
        {
            OsobaUredjaj osobaUredjaj = _context.OsobaUredjaj.Find(id);

            Osoba osoba = _mapper.Map<Osoba>(novi.Osoba);
            Uredjaj uredjaj = _mapper.Map<Uredjaj>(novi.Uredjaj);
           
            // ako osobe ima u bazi
            var osoba2 = (from nn in _context.Osobe
                          where nn.Ime == osoba.Ime && nn.Prezime == osoba.Prezime
                          select nn).FirstOrDefault();
            if (osoba2 != null)
            {
                osobaUredjaj.Osoba = osoba2;
            }
            else { osobaUredjaj.Osoba = osoba; }
          
            // ako kancelarije ima u bazi
            if (osoba2 == null)
            {
                var kancelarija = (from nn in _context.Kancelarije
                                   where nn.Opis == novi.Osoba.Kancelarija.Opis
                                   select nn).FirstOrDefault();
                if (kancelarija != null)
                {
                    osobaUredjaj.Osoba.Kancelarija = kancelarija;
                }
                else
                {
                    osoba.Kancelarija = _mapper.Map<Kancelarija>(novi.Osoba.Kancelarija);
                }
            }

            // ako uredjaj ima u bazi
            Uredjaj uredjaj2 = (from nn in _context.Uredjaji
                                where nn.Naziv == novi.Uredjaj.Naziv
                                select nn).FirstOrDefault();
            if (uredjaj2 != null)
            {
                osobaUredjaj.Uredjaj = uredjaj2;
            }
            else
            {
                osobaUredjaj.Uredjaj = uredjaj;
            }

            osobaUredjaj.PocetakKoriscenja = novi.PocetakKoriscenja;
            osobaUredjaj.KrajKoriscenja = novi.KrajKoriscenja;
            _context.SaveChanges();

            return Ok("Zamenjemo.");
        }

        //akcija koja brise entitet iz baze
        [HttpDelete("{id}")]
        public IActionResult BrisanjeEntiteta(long id)
        {
            return base.Brisanje(id);
        }

        // akcija koja vraca entitete koji imaju pocetakKoriscenja veci i jednak od unetog
        [HttpGet("pocetak")]
        public IActionResult PretragaPoPocetku(DateTime pocetak)
        {
            var niz = from nn in _context.OsobaUredjaj
                where nn.PocetakKoriscenja >= pocetak
                select new
                {
                    DatimOd = nn.PocetakKoriscenja,
                    DatumDo = nn.KrajKoriscenja,
                    ImeOsobe = nn.Osoba.Ime,
                    PrezimeOsobe = nn.Osoba.Prezime,
                    Uredjaj = nn.Uredjaj.Naziv
                };

            return Ok(niz.ToList());
        }

        // akcija koja vraca entitet koji ima dat pocetak i kraj koriscenja
        [HttpGet("{pocetak}/{kraj}")]
        public IActionResult PretragaPocetakKraj(DateTime pocetak, DateTime kraj)
        {
            var niz = from nn in _context.OsobaUredjaj
                where nn.PocetakKoriscenja == pocetak && nn.KrajKoriscenja == kraj
                select new
                {
                    DatimOd = nn.PocetakKoriscenja,
                    DatumDo = nn.KrajKoriscenja,
                    ImeOsobe = nn.Osoba.Ime,
                    PrezimeOsobe = nn.Osoba.Prezime,
                    Uredjaj = nn.Uredjaj.Naziv
                };

            return Ok(niz.ToList());
        }
    }
}