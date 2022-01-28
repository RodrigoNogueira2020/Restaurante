using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestauranteAPI.Models;
using RestauranteAPI.QueryModels;
using System;
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
        public async Task<IActionResult> ObterTodosEstafetas([FromQuery] ParametrosEstafeta parametros)
        {

            IQueryable<Estafeta> EstafetasDB = _context.Estafeta;

            if (parametros.Id != null)
                EstafetasDB = EstafetasDB.Where(p => p.Id == parametros.Id);

            if (!string.IsNullOrWhiteSpace(parametros.Nome))
                EstafetasDB = EstafetasDB.Where(p => p.Nome.ToLower().Equals(parametros.Nome.ToLower().Trim()));

            if (parametros.Disponivel != null)
                EstafetasDB = EstafetasDB.Where(p => p.Disponivel == parametros.Disponivel);

            EstafetasDB = EstafetasDB.Skip(parametros.Tamanho * (parametros.Pagina - 1))
                 .Take(parametros.Tamanho);

            if (EstafetasDB == null)
                return NotFound();

            return Ok(await EstafetasDB.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterEstafeta(int id)
        {
            Estafeta estafeta = await _context.Estafeta.SingleOrDefaultAsync(e => e.Id == id);

            if(estafeta == null)
                return NotFound();

            return Ok(estafeta);
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarEstafeta([FromBody] Estafeta estafeta)
        {
            if(_context.Estafeta.Any(p => p.Id == estafeta.Id))
                return Conflict();

            _context.Estafeta.Add(estafeta);
            await _context.SaveChangesAsync();
            return CreatedAtAction("ObterTodosEstafetas", new ParametrosEstafeta { Id = estafeta.Id }, estafeta);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarEstafeta([FromRoute] int id, [FromBody] Estafeta estafeta)
        {
            if (id != estafeta.Id)
                return BadRequest();

            if(!estafeta.Disponivel)
                estafeta.Disponivel = true;

            _context.Entry(estafeta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.Estafeta.Find(id) == null)
                    return NotFound();

                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ApagarEstafeta(int id)
        {
            Estafeta estafeta = await _context.Estafeta.SingleOrDefaultAsync(p => p.Id == id);
            if (estafeta == null)
                return NotFound();

            _context.Estafeta.Remove(estafeta);

            await _context.SaveChangesAsync();
            return Ok(estafeta);
        }
    }
}
