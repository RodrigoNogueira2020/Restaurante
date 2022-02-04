using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestauranteAPI.QueryModels
{
    public class ParametrosItem : Parametros
    {
        public int? Id { get; set; }
        public int? EncomendaId { get; set; }
        public int? PedidoId { get; set; }
        public int? ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        public int? Quantidade { get; set; }
    }
}
