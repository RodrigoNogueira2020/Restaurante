using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RestauranteAPI.Models
{
    public class Item
    {
        public int Id { get; set; }

        // Encomenda tem Itens
        public int EncomendaId { get; set; }
        [JsonIgnore]
        public virtual Encomenda Encomenda { get; set; }

        // Pedido tem Itens
        public int PedidoId { get; set; }
        [JsonIgnore]
        public Pedido Pedido { get; set; }

        // Colunas do Item
        public int ProdutoId { get; set; }

        [JsonIgnore]
        public virtual Produto Produto { get; set; }

        public int Quantidade { get; set; }
    }
}
