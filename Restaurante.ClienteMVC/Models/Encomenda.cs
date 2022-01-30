using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurante.ClienteMVC.Models
{
    public class Encomenda
    {
        public int Id { get; set; }
        public virtual List<Item> Itens { get; set; }
        public DateTime DataHoraAbertura { get; set; }
        public DateTime DataHoraFecho { get; set; }
        public decimal PrecoTotal { get; set; }
        public string Estado { get; set; }
        public string Morada { get; set; }
        public int EstafetaId { get; set; }
        public virtual Estafeta Estafeta { get; set; }
    }
}
