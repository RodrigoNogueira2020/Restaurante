using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RestauranteAPI.Models
{
    public class Encomenda
    {
        [JsonIgnore]
        public int Id { get; set; }


        public virtual List<Item> Itens { get; set; }


        public DateTime DataHoraAbertura { get; set; }
        public DateTime DataHoraFecho { get; set; }
        public decimal PrecoTotal { get; set; }
        public string Estado { get; set; } // Aberto(?), Em Preparação, Servido
        
        // Por questões de segurança, não se deve mostrar né?
        // [JsonIgnore]
        public string Morada { get; set; }


        public int EstafetaId { get; set; }
        [JsonIgnore]
        public virtual Estafeta Estafeta { get; set; }
    }
}
