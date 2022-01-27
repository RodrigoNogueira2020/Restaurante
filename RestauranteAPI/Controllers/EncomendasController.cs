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
                    Estafeta = encomenda.Estafeta.Nome,
                };

                encomendas.Add(pop);
            }

            return Ok(encomendas);
        }

        // encomenda/1
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterEncomenda(int id)
        {
            Encomenda e = await _context.Encomenda.SingleOrDefaultAsync(e => e.Id == id);

            if (e == null)
                return NotFound();

            return Ok(e);
        }
    }


}
