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
        public async Task<IActionResult> ObterTodosPedidos([FromQuery] ParametrosPedido parametros)
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

            List<PedidoVerbose> produtos = new();

            foreach (Pedido pedido in pedidos.Include(i => i.Itens).ToList())
            {

                List<ItemVerbose> pop0 = _context.Item.Where(i => i.PedidoId == pedido.Id)
                    .Include(i => i.Pedido)
                    .Select(l => new ItemVerbose()
                    {
                        Id = l.Id,
                        EncomendaId = l.EncomendaId,
                        PedidoId = l.PedidoId,
                        ProdutoId = l.ProdutoId,
                        ProdutoNome = l.Produto.Nome,
                        Quantidade = l.Quantidade,
                    }).ToList();

                PedidoVerbose pop = new PedidoVerbose()
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
        public async Task<IActionResult> ObterPedido(int id)
        {
            Pedido i = await _context.Pedido.SingleOrDefaultAsync(i => i.Id == id);

            if (i == null)
                return NotFound();

            return Ok(i);
        }
    }
}
