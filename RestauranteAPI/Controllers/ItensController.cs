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
            IQueryable<Pedido> PedidoDB = _context.Pedido;

            if (parametros.Id != null)
                ItemBD = ItemBD.Where(i => i.Id == parametros.Id);

            if (parametros.EncomendaId != null)
                ItemBD = ItemBD.Where(i => i.EncomendaId.Equals(parametros.EncomendaId));

            if (parametros.PedidoId != null)
                ItemBD = ItemBD.Where(i => i.PedidoId.Equals(parametros.PedidoId));

            if (parametros.ProdutoId != null)
                ItemBD = ItemBD.Where(i => i.Produto.Id == parametros.ProdutoId);

            if (!string.IsNullOrWhiteSpace(parametros.ProdutoNome))
                ItemBD = ItemBD.Where(i => i.Produto.Nome.ToLower().Contains(parametros.ProdutoNome.ToLower().Trim()));

            if (parametros.Quantidade != null)
                ItemBD = ItemBD.Where(i => i.Quantidade == parametros.Quantidade);

            ItemBD = ItemBD.Skip(parametros.Tamanho * (parametros.Pagina - 1))
                 .Take(parametros.Tamanho);

            if (ItemBD == null)
                return NotFound();

            List<ItemDto> itensDto = new();

            foreach (Item item in ItemBD.Include(i => i.Produto).ToList())
            {
                ItemDto itemDto = new ItemDto()
                {
                    Id = item.Id,
                    EncomendaId = item.EncomendaId,
                    PedidoId = item.PedidoId,
                    ProdutoId = item.ProdutoId,
                    ProdutoNome = item.Produto.Nome,
                    Quantidade = item.Quantidade,
                };
                itensDto.Add(itemDto);
            }

            return Ok(itensDto);
        }

        [HttpGet("{id}")]
        public IActionResult ObterItens(int id)
        {
            Item itemAObter;
            try
            {
                itemAObter = _context.Item.Where(e => e.Id == id).Include(e => e.Produto).Single();
            }
            catch (System.InvalidOperationException e)
            {
                return NotFound();
            }

            ItemDto itemDto = new()
            {
                Id = itemAObter.Id,
                EncomendaId = itemAObter.EncomendaId,
                PedidoId = itemAObter.PedidoId,
                ProdutoId = itemAObter.ProdutoId,
                ProdutoNome = itemAObter.Produto.Nome,
                Quantidade = itemAObter.Quantidade
            };

            return Ok(itemDto);
        }

        [HttpGet("{ItemId:int}/Produto/")]
        public async Task<IActionResult> ObterProdutoDoItem(int ItemId)
        {
            Item item = await _context.Item.Include(i => i.Produto).SingleOrDefaultAsync(p => p.Id == ItemId);
            if (item == null)
                return NotFound();

            Produto produto = item.Produto;

            return Ok(produto);
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarItem([FromBody] Item item)
        {

            //Verifica se o produto existe
            if (!_context.Produto.Any(p => p.Id == item.ProdutoId) 
                || (!_context.Pedido.Any(p => p.Id == item.PedidoId)
                && !_context.Encomenda.Any(p => p.Id == item.EncomendaId)))
                return NotFound();

            else if (item.Quantidade <= 0)
                return Conflict();

            _context.Item.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction("ObterItens", new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarItem([FromRoute] int id, [FromBody] Item Item)
        {
            if (id != Item.Id)
                return BadRequest();

            _context.Entry(Item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.Item.Find(id) == null)
                    return NotFound();

                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ApagarProduto(int id)
        {
            Item item = await _context.Item.SingleOrDefaultAsync(p => p.Id == id);
            if (item == null)
                return NotFound();

            _context.Item.Remove(item);

            await _context.SaveChangesAsync();
            return Ok(item);
        }


    }
}
