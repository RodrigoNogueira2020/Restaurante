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

        //[HttpGet]
        //public async Task<IActionResult> ObterTodosProdutos()
        //{
        //    List<Produto> p = await _context.Produto.ToListAsync();

        //    if (p == null)
        //        return NotFound();

        //    return Ok(p);
        //}

        [HttpGet]
        public async Task<IActionResult> ObterTodosProdutos([FromQuery] ParametrosProduto parametros)
        {
            IQueryable<Produto> p = _context.Produto;

            if (parametros.Id != null)
                p = p.Where(p => p.Id == parametros.Id);

            if (!string.IsNullOrWhiteSpace(parametros.Nome))
                p = p.Where(p => p.Nome.ToLower().Contains(parametros.Nome.ToLower().Trim()));

            if (parametros.Preco != null)
                p = p.Where(p => p.Preco == parametros.Preco);

            if (parametros.Iva != null)
                p = p.Where(p => p.Iva == parametros.Iva);

            p = p.Skip(parametros.Tamanho * (parametros.Pagina - 1))
                 .Take(parametros.Tamanho);

            if (p == null)
                return NotFound();

            return Ok(await p.ToListAsync());
        }

        // produto/1
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterProduto(int id)
        {
            Produto p = await _context.Produto.SingleOrDefaultAsync(p => p.Id == id);

            if(p == null)
                return NotFound();

            return Ok(p);
        }

        // Pagina 12 - file:///C:/Users/Ricardo/Desktop/CTESP/PIS-Programa%C3%A7%C3%A3o-e-Integra%C3%A7%C3%A3o-de-Servi%C3%A7os/LAB05_2/Lab%2005%20-%20ASP.NET%20Core%20RESTful%20Web%20API%20-%20Partes%201,%202,%203%20e%204.pdf

    }
}
