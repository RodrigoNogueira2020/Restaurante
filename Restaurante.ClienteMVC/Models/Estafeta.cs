using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurante.ClienteMVC.Models
{
    public class Estafeta
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        [Display(Name = "Disponível")]
        public bool Disponivel { get; set; }
        public virtual List<Encomenda> Encomendas { get; set; }
    }
}
