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

            if (parametros.DataHoraFecho != null)
                encomendaDB = encomendaDB.Where(e => e.DataHoraFecho.Equals(parametros.DataHoraFecho));
            
            if (parametros.PrecoTotal != null)
                encomendaDB = encomendaDB.Where(e => e.PrecoTotal.Equals(parametros.PrecoTotal));

            if (parametros.Estado != null)
                encomendaDB = encomendaDB.Where(e => e.Estado.Equals(parametros.Estado));

            if (parametros.Morada != null)
                encomendaDB = encomendaDB.Where(e => e.Morada.Equals(parametros.Morada));

            if (parametros.EstafetaId != null)
                encomendaDB = encomendaDB.Where(e => e.EstafetaId.Equals(parametros.EstafetaId));
            
            if (parametros.EstafetaNome != null)
                encomendaDB = encomendaDB.Where(e => 
                e.Estafeta.Nome.ToLower()
                .Contains(parametros.EstafetaNome.Trim().ToLower()));

            encomendaDB = encomendaDB.Skip(parametros.Tamanho * (parametros.Pagina - 1))
                 .Take(parametros.Tamanho);

            if (encomendaDB == null)
                return NotFound();

            List<EncomendaDto> encomendas = new();

            foreach (Encomenda encomenda in encomendaDB.Include(i => i.Estafeta).ToList())
            {
                List<ItemDto> itemDto = _context.Item.Where(i => i.EncomendaId == encomenda.Id)
                    .Include(i => i.Produto)
                    .Select(l => new ItemDto()
                    {
                        Id = l.Id,
                        EncomendaId = l.EncomendaId,
                        PedidoId = l.PedidoId,
                        ProdutoId = l.ProdutoId,
                        ProdutoNome = l.Produto.Nome,
                        Quantidade = l.Quantidade,
                    }).ToList();

                EncomendaDto encomendaDto = new()
                {
                    Id = encomenda.Id,
                    Itens = itemDto,
                    DataHoraAbertura = encomenda.DataHoraAbertura,
                    DataHoraFecho = encomenda.DataHoraFecho,
                    PrecoTotal = encomenda.PrecoTotal,
                    Estado = encomenda.Estado,
                    Morada = encomenda.Morada,
                    EstafetaId = encomenda.EstafetaId,
                    EstafetaNome = encomenda.Estafeta.Nome,
                };

                encomendas.Add(encomendaDto);
            }

            return Ok(encomendas);
        }

        [HttpGet("{encomendaId:int}/Item/")]
        public IActionResult ObterEncomendas(int encomendaId)
        {
            if (!_context.Encomenda.Any(p => p.Id == encomendaId))
                return NotFound();

            List<ItemDto> itensDto = new();

            DbSet<Item> ItemDB = _context.Item;

            foreach (Item i in ItemDB.Include(i => i.Produto).ToList())
                itensDto = ItemDB.Where(i => i.EncomendaId == encomendaId)
                    .Include(i => i.Produto)
                    .Select(l => new ItemDto()
                    {
                        Id = l.Id,
                        EncomendaId = l.EncomendaId,
                        PedidoId = l.PedidoId,
                        ProdutoId = l.ProdutoId,
                        ProdutoNome = l.Produto.Nome,
                        Quantidade = l.Quantidade,
                    }).ToList();

            return Ok(itensDto);
        }

        [HttpGet("{encomendaId:int}/Item/{itemId:int}")]
        public async Task<IActionResult> ObterEncomendas(int encomendaId, int itemId)
        {
            Encomenda encomenda = await _context.Encomenda.Include(e => e.Itens).SingleOrDefaultAsync(e => e.Id == encomendaId);
            if (encomenda == null || !encomenda.Itens.Any(i => i.Id == itemId))
                return NotFound();

            Item item = await _context.Item.Include(e => e.Produto).SingleOrDefaultAsync(p => p.Id == itemId);
            if (item == null)
                return NotFound();

            ItemDto itemDto = new()
            {
                Id = item.Id,
                EncomendaId = item.EncomendaId,
                PedidoId = item.PedidoId,
                ProdutoId = item.ProdutoId,
                ProdutoNome = item.Produto.Nome,
                Quantidade = item.Quantidade,
            };

            if (!encomenda.Itens.Any(e => e.Id == item.Id))
                return NotFound();

            return Ok(itemDto);
        }

        // encomenda/1
        [HttpGet("{id}")]
        public IActionResult ObterEncomenda(int id)
        {
            Encomenda encomendaAObter;
            try
            {
                encomendaAObter = _context.Encomenda.Where(e => e.Id == id).Single();
            }
            catch (System.InvalidOperationException e)
            {
                return NotFound();
            }

            List<ItemDto> itensDto = _context.Item.Where(i => i.EncomendaId == encomendaAObter.Id)
                    .Include(i => i.Pedido).Include(i => i.Produto)
                    .Select(itemDto => new ItemDto()
                    {
                        Id = itemDto.Id,
                        EncomendaId = itemDto.EncomendaId,
                        PedidoId = itemDto.PedidoId,
                        ProdutoId = itemDto.ProdutoId,
                        ProdutoNome = itemDto.Produto.Nome,
                        Quantidade = itemDto.Quantidade,
                    }).ToList();

            Estafeta estafeta = _context.Estafeta.Where(e => e.Id == encomendaAObter.EstafetaId).FirstOrDefault();

            EncomendaDto encomendaDto = new()
            {
                Id = encomendaAObter.Id,
                Itens = itensDto,
                DataHoraAbertura = encomendaAObter.DataHoraAbertura,
                DataHoraFecho = encomendaAObter.DataHoraFecho,
                PrecoTotal = encomendaAObter.PrecoTotal,
                Estado = encomendaAObter.Estado,
                Morada = encomendaAObter.Morada,
                EstafetaId = encomendaAObter.EstafetaId,
                EstafetaNome = encomendaAObter.Estafeta.Nome,
            };

            return Ok(encomendaDto);
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarEncomenda([FromBody] Encomenda encomenda)
        {
            Encomenda encomendaDB = _context.Encomenda.Where(e => e.Id == encomenda.Id).Single();

            encomendaDB.Estafeta.Disponivel = false;

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

        [HttpPut("FecharEncomenda/{id}")]
        public async Task<IActionResult> FecharEncomenda([FromRoute] int id)
        {
            if (!_context.Encomenda.Any(e => e.Id == id))
                return BadRequest();

            Encomenda encomendaDB = _context.Encomenda.Include(e => e.Estafeta).Where(e => e.Id == id).SingleOrDefault();

            encomendaDB.Estafeta.Disponivel = false;
            encomendaDB.Estado = "Fechada";

            _context.Entry(encomendaDB).State = EntityState.Modified;

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

            List<Item> itens = _context.Item.Where(i => i.EncomendaId == encomenda.Id).ToList();

            foreach (var item in itens)
                _context.Item.Remove(item);

            if (encomenda == null)
                return NotFound();

            _context.Encomenda.Remove(encomenda);

            await _context.SaveChangesAsync();
            return Ok(encomenda);
        }
    }


}
