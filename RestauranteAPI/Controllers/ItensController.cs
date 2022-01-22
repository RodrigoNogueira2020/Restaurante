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
        public async Task<IActionResult> ObterTodosItens()
        {
            List<Item> p = await _context.Item.ToListAsync();

            if (p == null)
                return NotFound();

            return Ok(p);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterItem(int id)
        {
            Item i = await _context.Item.SingleOrDefaultAsync(i => i.Id == id);

            if (i == null)
                return NotFound();

            return Ok(i);
        }
    }
}
