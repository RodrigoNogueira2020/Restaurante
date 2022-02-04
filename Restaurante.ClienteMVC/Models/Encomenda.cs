using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurante.ClienteMVC.Models
{
    public class Encomenda
    {
        public int Id { get; set; }
        public virtual List<Item> Itens { get; set; }
        [Display(Name = "Data e hora de abertura")]
        public DateTime DataHoraAbertura { get; set; }
        [Display(Name = "Data e hora de fecho")]
        public DateTime DataHoraFecho { get; set; }
        [Display(Name = "Preço")]
        public decimal PrecoTotal { get; set; }
        public string Estado { get; set; }
        public string Morada { get; set; }
        [Display(Name = "ID do estafeta")]
        public int EstafetaId { get; set; }
        public virtual Estafeta Estafeta { get; set; }
    }
}
