using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RestauranteAPI.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int NumeroMesa { get; set; }
        public bool Disponivel { get; set; }
        public List<Item> Itens { get; set; }
        public DateTime DataHoraAbertura { get; set; }
        public DateTime DataHoraFecho { get; set; }
        public decimal PrecoTotal { get; set; }
        public string Estado { get; set; } // Em Preparação, Servido
    }
}
