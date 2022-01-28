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
