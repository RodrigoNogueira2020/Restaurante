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
            IQueryable<Pedido> pedidos = _context.Pedido;

            if (parametros.Id != null)
                pedidos = pedidos.Where(p => p.Id == parametros.Id);

            if (parametros.NumeroMesa != null)
                pedidos = pedidos.Where(p => p.NumeroMesa == parametros.NumeroMesa);

            if (parametros.Disponivel != null)
                pedidos = pedidos.Where(p => p.Disponivel == parametros.Disponivel);

            if (parametros.DataHoraAbertura != null)
                pedidos = pedidos.Where(p => p.DataHoraAbertura == parametros.DataHoraAbertura);

            if (parametros.DataHoraFecho != null)
                pedidos = pedidos.Where(p => p.DataHoraFecho == parametros.DataHoraFecho);

            if (parametros.PrecoTotal < 0 && parametros.PrecoTotal != null)
                pedidos = pedidos.Where(p => p.PrecoTotal == parametros.PrecoTotal);

            if (!string.IsNullOrWhiteSpace(parametros.Estado))
                pedidos = pedidos.Where(p => p.Estado.ToLower().Equals(parametros.Estado.ToLower().Trim()));

            pedidos = pedidos.Skip(parametros.Tamanho * (parametros.Pagina - 1)).Take(parametros.Tamanho);

            if (pedidos == null)
                return NotFound();

            List<PedidoDto> produtos = new();

            foreach (Pedido pedido in pedidos.Include(i => i.Itens).ToList())
            {

                List<ItemDto> pop0 = _context.Item.Where(i => i.PedidoId == pedido.Id)
                    .Include(i => i.Pedido)
                    .Select(l => new ItemDto()
                    {
                        Id = l.Id,
                        EncomendaId = l.EncomendaId,
                        PedidoId = l.PedidoId,
                        ProdutoId = l.ProdutoId,
                        ProdutoNome = l.Produto.Nome,
                        Quantidade = l.Quantidade,
                    }).ToList();

                PedidoDto pop = new PedidoDto()
                {
                    Id = pedido.Id,
                    NumeroMesa = pedido.NumeroMesa,
                    Disponivel = pedido.Disponivel,
                    DataHoraAbertura = pedido.DataHoraAbertura,
                    DataHoraFecho = pedido.DataHoraFecho,
                    PrecoTotal = pedido.PrecoTotal,
                    Estado = pedido.Estado,
                    Itens = pop0
                };
                produtos.Add(pop);
            }
            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public IActionResult ObterPedido(int id)
        {
            Pedido p;
            try
            {
                p = _context.Pedido.Where(p => p.Id == id).Single();
            }
            catch (System.InvalidOperationException e)
            {
                return NotFound();
            }

            List<ItemDto> pop0 = _context.Item.Where(i => i.PedidoId == p.Id)
                    .Include(i => i.Pedido)
                    .Select(l => new ItemDto()
                    {
                        Id = l.Id,
                        EncomendaId = l.EncomendaId,
                        PedidoId = l.PedidoId,
                        ProdutoId = l.ProdutoId,
                        ProdutoNome = l.Produto.Nome,
                        Quantidade = l.Quantidade,
                    }).ToList();

            PedidoDto pop = new()
            {
                Id = p.Id,
                NumeroMesa = p.NumeroMesa,
                Disponivel = p.Disponivel,
                DataHoraAbertura = p.DataHoraAbertura,
                DataHoraFecho = p.DataHoraFecho,
                Itens = pop0,
                PrecoTotal = p.PrecoTotal,
                Estado = p.Estado,
            };

            return Ok(pop);
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarProduto([FromBody] Pedido pedido)
        {
            if (!_context.Produto.Any(p => p.Id == pedido.Id))
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
            Pedido pedido = await _context.Pedido.SingleOrDefaultAsync(p => p.Id == id);
            if (pedido == null)
                return NotFound();

            _context.Pedido.Remove(pedido);

            await _context.SaveChangesAsync();
            return Ok(pedido);
        }
    }
}
