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
    [Route("Produto")]
    public class ProdutosController : ControllerBase
    {
        private readonly RestauranteContext _context;

        public ProdutosController(RestauranteContext context)
        {
            if (context == null)
                throw new ArgumentNullException();

            _context = context;

            _context.Database.EnsureCreated();
        }


        [HttpGet]
        public async Task<IActionResult> ObterTodosProdutos([FromQuery] ParametrosProduto parametros)
        {
            IQueryable<Produto> produtosDB = _context.Produto;

            if (parametros.Id != null)
                produtosDB = produtosDB.Where(p => p.Id == parametros.Id);

            if (!string.IsNullOrWhiteSpace(parametros.Nome))
                produtosDB = produtosDB.Where(p => p.Nome.ToLower().Equals(parametros.Nome.ToLower().Trim()));

            if (parametros.Preco != null)
                produtosDB = produtosDB.Where(p => p.Preco == parametros.Preco);

            if (parametros.Iva != null)
                produtosDB = produtosDB.Where(p => p.Iva == parametros.Iva);

            produtosDB = produtosDB.Skip(parametros.Tamanho * (parametros.Pagina - 1))
                 .Take(parametros.Tamanho);

            if (produtosDB == null)
                return NotFound();

            return Ok(await produtosDB.ToListAsync());
        }


        [HttpPost]
        public async Task<IActionResult> AdicionarProduto([FromBody] Produto produto)
        {

            // Verifica se já existe um produto, baseado no nome
            if(_context.Produto.Any(p => p.Nome.ToLower() == produto.Nome.Trim().ToLower()))
                return Conflict();

            _context.Produto.Add(produto);
            await _context.SaveChangesAsync();
            return CreatedAtAction("ObterTodosProdutos", new ParametrosProduto { Id = produto.Id }, produto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarProduto([FromRoute] int id, [FromBody] Produto produto)
        {
            //List<Produto> p = _context.Produto.ToList();
            //p.Add(produto);

            //Se o nome do produto for mudado para um nome já existente,
            //é - lhe impedido de duplicar o nome

            //if (_context.Produto.Any(p => p.Nome.ToLower() == produto.Nome.Trim().ToLower()))
            //    return Conflict();

            if (id != produto.Id)
                return BadRequest();

            _context.Entry(produto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.Produto.Find(id) == null)
                    return NotFound();

                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ApagarProduto(int id)
        {
            Produto produto = await _context.Produto.SingleOrDefaultAsync(p => p.Id == id);
            if (produto == null)
                return NotFound();

            _context.Produto.Remove(produto);

            await _context.SaveChangesAsync();
            return Ok(produto);
        }


        // Pagina 19 - file:///C:/Users/Ricardo/Desktop/CTESP/PIS-Programa%C3%A7%C3%A3o-e-Integra%C3%A7%C3%A3o-de-Servi%C3%A7os/LAB05_2/Lab%2005%20-%20ASP.NET%20Core%20RESTful%20Web%20API%20-%20Partes%201,%202,%203%20e%204.pdf

    }
}
