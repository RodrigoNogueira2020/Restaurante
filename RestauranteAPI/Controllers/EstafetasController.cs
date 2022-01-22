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
    [Route("Estafeta")]
    public class EstafetasController : ControllerBase
    {
        private readonly RestauranteContext _context;

        public EstafetasController(RestauranteContext context)
        {
            if (context == null)
                throw new ArgumentNullException();

            _context = context;

            _context.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodosEstafetas()
        {
            List<Estafeta> p = await _context.Estafeta.ToListAsync();

            if (p == null)
                return NotFound();

            return Ok(p);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterEstafeta(int id)
        {
            Estafeta e = await _context.Estafeta.SingleOrDefaultAsync(e => e.Id == id);

            if (e == null)
                return NotFound();

            return Ok(e);
        }
    }
}
