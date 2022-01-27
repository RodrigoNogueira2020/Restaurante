using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RestauranteAPI.Models
{
    public class ItemVerbose
    {
        public int Id { get; set; }
     
        // Encomenda tem Itens
        public int EncomendaId { get; set; }

        // Pedido tem Itens
        public int PedidoId { get; set; }

        // Colunas do Item
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        public int Quantidade { get; set; }
    }
}
