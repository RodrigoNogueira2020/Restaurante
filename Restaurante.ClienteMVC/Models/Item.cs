using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurante.ClienteMVC.Models
{
    public class Item
    {
        public int Id { get; set; }
        [Display(Name = "ID da encomenda")]
        public int EncomendaId { get; set; }
        public virtual Encomenda Encomenda { get; set; }
        [Display(Name = "ID do pedido")]
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }
        [Display(Name = "ID do produto")]
        public int ProdutoId { get; set; }
        public virtual Produto Produto { get; set; }
        public int Quantidade { get; set; }
    }
}
