using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestauranteAPI.Models;
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
        public async Task<IActionResult> ObterTodosPedidos()
        {
            List<Pedido> p = await _context.Pedido.ToListAsync();

            if (p == null)
                return NotFound();

            return Ok(p);
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
