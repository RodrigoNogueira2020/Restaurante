using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurante.ClienteMVC.Models
{
    public class Item
    {
        public int Id { get; set; }
        public int EncomendaId { get; set; }
        public virtual Encomenda Encomenda { get; set; }
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }
        public int ProdutoId { get; set; }
        public virtual Produto Produto { get; set; }
        public int Quantidade { get; set; }
    }
}
