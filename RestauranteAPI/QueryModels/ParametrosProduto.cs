using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestauranteAPI.QueryModels
{
    public class ParametrosProduto : Parametros
    {
        public int? Id { get; set; }
        public string Nome { get; set; }
        public decimal? Preco { get; set; }
        public double? Iva { get; set; }
    }
}
