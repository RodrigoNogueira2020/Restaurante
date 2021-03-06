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
    [Route("Pedido")]
    public class PedidosController : ControllerBase
    {
        private readonly RestauranteContext _context;

        public PedidosController(RestauranteContext context)
        {
            if (context == null)
                throw new ArgumentNullException();

            _context = context;

            _context.Database.EnsureCreated();
        }

        [HttpGet]
        public IActionResult ObterTodosPedidos([FromQuery] ParametrosPedido parametros)
        {
            IQueryable<Pedido> pedidosDB = _context.Pedido;

            if (parametros.Id != null)
                pedidosDB = pedidosDB.Where(p => p.Id == parametros.Id);

            if (parametros.NumeroMesa != null)
                pedidosDB = pedidosDB.Where(p => p.NumeroMesa == parametros.NumeroMesa);

            if (parametros.Disponivel != null)
                pedidosDB = pedidosDB.Where(p => p.Disponivel == parametros.Disponivel);

            if (parametros.DataHoraAbertura != null)
                pedidosDB = pedidosDB.Where(p => p.DataHoraAbertura == parametros.DataHoraAbertura);

            if (parametros.DataHoraFecho != null)
                pedidosDB = pedidosDB.Where(p => p.DataHoraFecho == parametros.DataHoraFecho);

            if (parametros.PrecoTotal < 0 && parametros.PrecoTotal != null)
                pedidosDB = pedidosDB.Where(p => p.PrecoTotal == parametros.PrecoTotal);

            if (!string.IsNullOrWhiteSpace(parametros.Estado))
                pedidosDB = pedidosDB.Where(p => p.Estado.ToLower().Equals(parametros.Estado.ToLower().Trim()));

            pedidosDB = pedidosDB.Skip(parametros.Tamanho * (parametros.Pagina - 1)).Take(parametros.Tamanho);

            if (pedidosDB == null)
                return NotFound();

            List<PedidoDto> produtos = new();

            foreach (Pedido pedido in pedidosDB.Include(i => i.Itens).ToList())
            {

                // Item com nome do produto incluido
                List<ItemDto> itemDto = _context.Item.Where(i => i.PedidoId == pedido.Id)
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

                // Pedido com a lista de itens incluida
                PedidoDto pedidoDto = new()
                {
                    Id = pedido.Id,
                    NumeroMesa = pedido.NumeroMesa,
                    Disponivel = pedido.Disponivel,
                    DataHoraAbertura = pedido.DataHoraAbertura,
                    DataHoraFecho = pedido.DataHoraFecho,
                    PrecoTotal = pedido.PrecoTotal,
                    Estado = pedido.Estado,
                    Itens = itemDto
                };
                produtos.Add(pedidoDto);
            }
            return Ok(produtos);
        }

        [HttpGet("{pedidoId:int}/Item/")]
        public IActionResult ObterEncomendas(int pedidoId)
        {
            if (!_context.Pedido.Any(p => p.Id == pedidoId))
                return NotFound();

            List<ItemDto> itensDto = new();

            DbSet<Item> ItemDB = _context.Item;

            foreach (Item i in ItemDB.Include(i => i.Produto).ToList())
                itensDto = ItemDB.Where(i => i.PedidoId == pedidoId)
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

        [HttpGet("{pedidoId:int}/Item/{itemId:int}")]
        public async Task<IActionResult> ObterEncomendas(int pedidoId, int itemId)
        {
            Pedido pedido = await _context.Pedido.Include(e => e.Itens).SingleOrDefaultAsync(p => p.Id == pedidoId);
            if (pedido == null || !pedido.Itens.Any(i => i.Id == itemId))
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

            if (!pedido.Itens.Any(e => e.Id == item.Id))
                return NotFound();

            return Ok(itemDto);
        }


        [HttpGet("{id}")]
        public IActionResult ObterPedido(int id)
        {
            Pedido pedidoAObter;
            try
            {
                pedidoAObter = _context.Pedido.Where(p => p.Id == id).Single();
            }
            catch (System.InvalidOperationException e)
            {
                return NotFound();
            }

            // Item com nome do produto incluido
            List<ItemDto> itensDto = _context.Item.Where(i => i.PedidoId == pedidoAObter.Id)
                    .Include(i => i.Pedido)
                    .Select(itemDto => new ItemDto()
                    {
                        Id = itemDto.Id,
                        EncomendaId = itemDto.EncomendaId,
                        PedidoId = itemDto.PedidoId,
                        ProdutoId = itemDto.ProdutoId,
                        ProdutoNome = itemDto.Produto.Nome,
                        Quantidade = itemDto.Quantidade,
                    }).ToList();

            // Pedido com a lista de itens incluida
            PedidoDto pedidoDto = new()
            {
                Id = pedidoAObter.Id,
                NumeroMesa = pedidoAObter.NumeroMesa,
                Disponivel = pedidoAObter.Disponivel,
                DataHoraAbertura = pedidoAObter.DataHoraAbertura,
                DataHoraFecho = pedidoAObter.DataHoraFecho,
                Itens = itensDto,
                PrecoTotal = pedidoAObter.PrecoTotal,
                Estado = pedidoAObter.Estado,
            };

            return Ok(pedidoDto);
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarPedido([FromBody] Pedido pedido)
        {
            if (!_context.Pedido.Any(p => p.Id == pedido.Id))
                return NotFound();

            _context.Pedido.Add(pedido);
            await _context.SaveChangesAsync();
            return CreatedAtAction("ObterTodosPedidos", new { id = pedido.Id }, pedido);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarPedido([FromRoute] int id, [FromBody] Pedido pedido)
        {
            if (id != pedido.Id)
                return BadRequest();

            _context.Entry(pedido).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.Pedido.Find(id) == null)
                    return NotFound();

                throw;
            }
            return NoContent();
        }

        [HttpPut("FecharPedido/{id}")]
        public async Task<IActionResult> FecharPedido([FromRoute] int id)
        {
            if (!_context.Pedido.Any(e => e.Id == id))
                return BadRequest();

            Pedido pedidoDB = _context.Pedido.Where(e => e.Id == id).SingleOrDefault();

            pedidoDB.Estado = "Servido";

            _context.Entry(pedidoDB).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.Pedido.Find(id) == null)
                    return NotFound();

                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ApagarPedido(int id)
        {
            Pedido pedidoAApagar = _context.Pedido.Where(p => p.Id == id).Single();

            List<Item> itens = _context.Item.Where(i => i.PedidoId == pedidoAApagar.Id).ToList();

            foreach (var item in itens)
                _context.Item.Remove(item);


            if (pedidoAApagar == null)
                return NotFound();
                
            _context.Pedido.Remove(pedidoAApagar);

            await _context.SaveChangesAsync();
            return Ok(pedidoAApagar);
        }
    }
}
