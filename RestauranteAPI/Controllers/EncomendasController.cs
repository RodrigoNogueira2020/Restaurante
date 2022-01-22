using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestauranteAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestauranteAPI.Controllers
{
    [ApiController]
    [Route("Encomenda")]
    public class EncomendasController : ControllerBase
    {
        private readonly RestauranteContext _context;

        public EncomendasController(RestauranteContext context)
        {
            if (context == null)
                throw new ArgumentNullException();

            _context = context;

            _context.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodasEncomendas()
        {
            List<Encomenda> p = await _context.Encomenda.ToListAsync();

            if (p == null)
                return NotFound();

            return Ok(p);
        }

        // encomenda/1
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterEncomenda(int id)
        {
            Encomenda e = await _context.Encomenda.SingleOrDefaultAsync(e => e.Id == id);

            if (e == null)
                return NotFound();

            return Ok(e);
        }
    }
}
