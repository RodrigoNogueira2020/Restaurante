using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RestauranteAPI.Models
{
    public class Produto
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public double Iva { get; set; }
        [JsonIgnore]
        public virtual Item Item { get; set; }


    }
}
