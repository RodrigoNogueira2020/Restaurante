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
    [Route("Item")]
    public class ItensController : ControllerBase
    {
        private readonly RestauranteContext _context;

        public ItensController(RestauranteContext context)
        {
            if (context == null)
                throw new ArgumentNullException();

            _context = context;

            _context.Database.EnsureCreated();
        }

        [HttpGet]
        public IActionResult ObterTodosItens([FromQuery] ParametrosItem parametros)
        {
            IQueryable<Item> ItemBD = _context.Item;

            if (parametros.Id != null)
                ItemBD = ItemBD.Where(i => i.Id == parametros.Id);

            if (parametros.ProdutoId != null)
                ItemBD = ItemBD.Where(i => i.Produto.Id.Equals(parametros.ProdutoId));
            
            if (!string.IsNullOrWhiteSpace(parametros.NomeProduto))
                ItemBD = ItemBD.Where(i => i.Produto.Nome.ToLower() == parametros.NomeProduto.ToLower().Trim());

            if (parametros.Quantidade != null)
                ItemBD = ItemBD.Where(i => i.Quantidade == parametros.Quantidade);

            ItemBD = ItemBD.Skip(parametros.Tamanho * (parametros.Pagina - 1))
                 .Take(parametros.Tamanho);

            if (ItemBD == null)
                return NotFound();

            List<ItemVerbose> produtos = new();

            foreach (Item produto in ItemBD.Include(i => i.Produto).ToList())
            {
                ItemVerbose pop = new ItemVerbose()
                {
                    Id = produto.Id,
                    EncomendaId = produto.EncomendaId,
                    PedidoId = produto.PedidoId,
                    ProdutoId = produto.ProdutoId,
                    ProdutoNome = produto.Produto.Nome,
                    Quantidade = produto.Quantidade,
                };
                produtos.Add(pop);
            }

            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterItens(int id)
        {
            Item e = await _context.Item.SingleOrDefaultAsync(e => e.Id == id);

            if (e == null)
                return NotFound();

            return Ok(e);
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarItem([FromBody] Item item)
        {

            //Verifica se o produto existe
            if (!_context.Produto.Any(p => p.Id == item.ProdutoId))
                return NotFound();

            else if (item.Quantidade <= 0)
                return Conflict();

            _context.Item.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction("ObterItens", new { id = item.Id }, item);
        }

    }
}
