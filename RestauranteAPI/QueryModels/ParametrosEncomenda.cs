using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestauranteAPI.QueryModels
{
    public class ParametrosEncomenda : Parametros
    {
        public int? Id { get; set; }
        public DateTime? DataHoraAbertura { get; set; }
        public DateTime? DataHoraFecho { get; set; }
        public decimal? PrecoTotal { get; set; }
        public string Estado { get; set; }
        public string Morada { get; set; }
        public int? EstafetaId { get; set; }
        public string EstafetaNome { get; set; }
    }
}
