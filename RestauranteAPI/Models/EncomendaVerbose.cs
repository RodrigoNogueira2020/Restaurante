using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestauranteAPI.Models
{
    public class EncomendaVerbose
    {
        public int Id { get; set; }
        public List<ItemVerbose> Itens { get; set; }
        public DateTime DataHoraAbertura { get; set; }
        public DateTime DataHoraFecho { get; set; }
        public decimal PrecoTotal { get; set; }
        public string Estado { get; set; } // Aberto(?), Em Preparação, Servido
        public string Morada { get; set; }
        public int EstafetaId { get; set; }
        public string EstafetaNome { get; set; }
    }
}
