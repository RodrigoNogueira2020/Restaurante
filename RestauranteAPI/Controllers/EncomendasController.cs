using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestauranteAPI.Models;
using RestauranteAPI.QueryModels;
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
        public IActionResult ObterTodasEncomendas([FromQuery] ParametrosEncomenda parametros)
        {
            IQueryable<Encomenda> encomendaDB = _context.Encomenda;

            if (parametros.Id != null)
                encomendaDB = encomendaDB.Where(e => e.Id == parametros.Id);

            if (parametros.DataHoraAbertura != null)
                encomendaDB = encomendaDB.Where(e => e.DataHoraAbertura.Equals(parametros.DataHoraAbertura));


            encomendaDB = encomendaDB.Skip(parametros.Tamanho * (parametros.Pagina - 1))
                 .Take(parametros.Tamanho);

            if (encomendaDB == null)
                return NotFound();

            List<EncomendaVerbose> encomendas = new();

            foreach (Encomenda encomenda in encomendaDB.Include(i => i.Estafeta).ToList())
            {
                List<ItemVerbose> pop0 = _context.Item.Where(i => i.EncomendaId == encomenda.Id)
                    .Include(i => i.Produto)
                    .Select(l => new ItemVerbose()
                    {
                        Id = l.Id,
                        EncomendaId = l.EncomendaId,
                        PedidoId = l.PedidoId,
                        ProdutoId = l.ProdutoId,
                        ProdutoNome = l.Produto.Nome,
                        Quantidade = l.Quantidade,
                    }).ToList();

                EncomendaVerbose pop = new()
                {
                    Id = encomenda.Id,
                    //Itens = encomenda.Itens,
                    Itens = pop0,
                    DataHoraAbertura = encomenda.DataHoraAbertura,
                    DataHoraFecho = encomenda.DataHoraFecho,
                    PrecoTotal = encomenda.PrecoTotal,
                    Estado = encomenda.Estado,
                    Morada = encomenda.Morada,
                    EstafetaId = encomenda.EstafetaId,
                    EstafetaNome = encomenda.Estafeta.Nome,
                };

                encomendas.Add(pop);
            }

            return Ok(encomendas);
        }

        // encomenda/1
        [HttpGet("{id}")]
        public IActionResult ObterEncomenda(int id)
        {
            Encomenda p;
            try
            {
                p = _context.Encomenda.Where(p => p.Id == id).Single();
            }
            catch (System.InvalidOperationException e)
            {
                return NotFound();
            }

            List<ItemVerbose> pop0 = _context.Item.Where(i => i.EncomendaId == p.Id)
                    .Include(i => i.Pedido).Include(i => i.Produto)
                    .Select(l => new ItemVerbose()
                    {
                        Id = l.Id,
                        EncomendaId = l.EncomendaId,
                        PedidoId = l.PedidoId,
                        ProdutoId = l.ProdutoId,
                        ProdutoNome = l.Produto.Nome,
                        Quantidade = l.Quantidade,
                    }).ToList();

            Estafeta estafeta = _context.Estafeta.Where(i => i.Id == p.EstafetaId).FirstOrDefault();

            EncomendaVerbose pop = new()
            {
                Id = p.Id,
                Itens = pop0,
                DataHoraAbertura = p.DataHoraAbertura,
                DataHoraFecho = p.DataHoraFecho,
                PrecoTotal = p.PrecoTotal,
                Estado = p.Estado,
                Morada = p.Morada,
                EstafetaId = p.EstafetaId,
                EstafetaNome = p.Estafeta.Nome,
            };

            return Ok(pop);
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarProduto([FromBody] Encomenda encomenda)
        {
            if (!_context.Produto.Any(p => p.Id == encomenda.Id))
                return NotFound();

            _context.Encomenda.Add(encomenda);
            await _context.SaveChangesAsync();
            return CreatedAtAction("ObterTodasEncomendas", new { id = encomenda.Id }, encomenda);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarEncomendas([FromRoute] int id, [FromBody] Encomenda encomenda)
        {
            if (id != encomenda.Id)
                return BadRequest();

            _context.Entry(encomenda).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.Encomenda.Find(id) == null)
                    return NotFound();

                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ApagarEncomenda(int id)
        {
            Encomenda encomenda = await _context.Encomenda.SingleOrDefaultAsync(p => p.Id == id);
            if (encomenda == null)
                return NotFound();

            _context.Encomenda.Remove(encomenda);

            await _context.SaveChangesAsync();
            return Ok(encomenda);
        }
    }


}
