using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurante.ClienteMVC.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        [Display(Name = "Número da mesa")]
        public int NumeroMesa { get; set; }
        [Display(Name = "Disponível")]
        public bool Disponivel { get; set; }
        public List<Item> Itens { get; set; }
        [Display(Name = "Data e hora de abertura")]
        public DateTime DataHoraAbertura { get; set; }
        [Display(Name = "Data e hora de fecho")]
        public DateTime DataHoraFecho { get; set; }
        [Display(Name = "Preço")]
        public decimal PrecoTotal { get; set; }
        public string Estado { get; set; }
    }
}
